using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Embeddings;
using SemanticKernel.Embeddings;
using System.Linq;

var builder = Kernel
                .CreateBuilder()
                .AddOllamaTextEmbeddingGeneration("all-minilm", new Uri("http://localhost:11434"))
                .AddOllamaTextGeneration("phi3", new Uri("http://localhost:11434"))
                .AddInMemoryVectorStore();
                

var kernel = builder.Build();
var memory = kernel.GetRequiredService<InMemoryVectorStore>();
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var embedding = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var instructions = await File.ReadAllTextAsync("Plugin/instructions.txt");
var data = await File.ReadAllTextAsync("Plugin/data.csv");
var prompt = await File.ReadAllTextAsync("Plugin/prompt.txt");

var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
var collection = memory.GetCollection<string, Data<string>>("Data");
await collection.CreateCollectionIfNotExistsAsync();

int idx = 0;
foreach (var line in lines)
{
    await collection.UpsertAsync(new Data<string>
    {
        Category = "data",
        Key = $"{idx++}",
        Text = line,
        TextEmbedding = await embedding.GenerateEmbeddingAsync(line)
    });
}   

var query = "Generate a query for the Sales workspace";
var queryEmbedding = await embedding.GenerateEmbeddingAsync(query);

var search = await collection.VectorizedSearchAsync(queryEmbedding, new VectorSearchOptions { Top = 1 });
var results = await search.Results.AsAsyncEnumerable().ToListAsync();
var csvData = results?.First()?.Record?.Text;

var sqlPlugin = kernel.CreateFunctionFromPrompt(prompt);
var response = await sqlPlugin.InvokeAsync(kernel, new KernelArguments { ["instructions"] = instructions, ["csvData"] = csvData});

Console.WriteLine("Generated SQL:");
Console.WriteLine(response);
