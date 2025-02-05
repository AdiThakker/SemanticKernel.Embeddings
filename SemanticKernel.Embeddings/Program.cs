using Microsoft.SemanticKernel;

var builder = Kernel
                .CreateBuilder()
                .AddOllamaTextEmbeddingGeneration("all-minilm", new Uri("http://localhost:11434"))
                .AddOllamaTextGeneration("phi3", new Uri("http://localhost:11434"));

var kernel = builder.Build();

var instructions = await File.ReadAllTextAsync("Plugin/instructions.txt");
var csvData = await File.ReadAllTextAsync("Plugin/data.csv");
var prompt = await File.ReadAllTextAsync("Plugin/prompt.txt");

var sqlPlugin = kernel.CreateFunctionFromPrompt(prompt);


var response = await sqlPlugin.InvokeAsync(kernel, new KernelArguments { ["instructions"] = instructions, ["csvData"] = csvData });

Console.WriteLine("Generated SQL:");
Console.WriteLine(response);

