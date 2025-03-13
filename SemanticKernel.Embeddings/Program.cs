using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Milvus;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using OllamaSharp;

// No Out of the box connecor for Milvus yet, so we need to manually configure it
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var milvusStore = new MilvusMemoryStore("localhost", 19530);
var memoryBuilder = new MemoryBuilder();
memoryBuilder
    .WithMemoryStore(milvusStore)
    .WithTextEmbeddingGeneration(new OllamaApiClient(new Uri("http://localhost:11434"), "all-minilm").AsTextEmbeddingGenerationService());

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


var builder = Kernel
                .CreateBuilder()
                //.AddOllamaTextEmbeddingGeneration("all-minilm", new Uri("http://localhost:11434"))
                .AddOllamaTextGeneration("phi3", new Uri("http://localhost:11434"));


var kernel = builder.Build();
var memory = memoryBuilder.Build();

var instructions = await File.ReadAllTextAsync("Plugin/instructions.txt");
var csvData = await File.ReadAllTextAsync("Plugin/data.csv");
var prompt = await File.ReadAllTextAsync("Plugin/prompt.txt");

var instructionsLines = instructions.Split('\n', StringSplitOptions.RemoveEmptyEntries);
var dataCsvLines = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);


int idx = 0;
foreach (var line in instructionsLines)
{
    await memory.SaveInformationAsync("instructions", line, $"{idx++}");
    //kernel.Data.Add($"instructions-{idx++}", line);
}

foreach (var line in dataCsvLines)
{
    await memory.SaveInformationAsync("data", line, $"{idx++}");
}



Console.WriteLine("Embeddings stored in Milvus!");

var sqlPlugin = kernel.CreateFunctionFromPrompt(prompt);


var response = await sqlPlugin.InvokeAsync(kernel, new KernelArguments { ["instructions"] = instructions, ["csvData"] = csvData });

Console.WriteLine("Generated SQL:");
Console.WriteLine(response);

