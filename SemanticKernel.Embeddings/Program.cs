using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using SemanticKernel.Embeddings;

var builder = Kernel
                .CreateBuilder()
                .AddOllamaTextEmbeddingGeneration("all-minilm", new Uri("http://localhost:11434"))
                .AddOllamaTextGeneration("phi3", new Uri("http://localhost:11434"))
                .AddInMemoryVectorStore();
                

var kernel = builder.Build();
var memory = kernel.GetRequiredService<InMemoryVectorStore>();
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var embedding = kernel.GetRequiredService<IEmbeddingGenerationService<string, float>>();
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var instructions = await File.ReadAllTextAsync("Plugin/instructions.txt");
var csvData = await File.ReadAllTextAsync("Plugin/data.csv");
var prompt = await File.ReadAllTextAsync("Plugin/prompt.txt");

var instructionsLines = instructions.Split('\n', StringSplitOptions.RemoveEmptyEntries);
var dataCsvLines = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);

var collection = memory.GetCollection<Guid, Artifacts<string>>("Artifacts");
await collection.CreateCollectionIfNotExistsAsync();

int idx = 0;
foreach (var line in instructionsLines)
{
   
}

foreach (var line in dataCsvLines)
{
   memoryStore.UpsertAsync
    await kernel.Memory.SaveInformationAsync(
        collectionName,
        text: line,
        id: $"data-{idx++}",
        description: "Workspace data line"
    );
}

Console.WriteLine("Embeddings stored in Milvus!");

var sqlPlugin = kernel.CreateFunctionFromPrompt(prompt);


var response = await sqlPlugin.InvokeAsync(kernel, new KernelArguments { ["instructions"] = instructions, ["csvData"] = csvData });

Console.WriteLine("Generated SQL:");
Console.WriteLine(response);

