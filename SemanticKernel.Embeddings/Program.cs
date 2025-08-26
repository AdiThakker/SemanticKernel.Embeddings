using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Embeddings;
using SemanticKernel.Embeddings;
using System.Linq;

// 🚀 Semantic Kernel RAG Demo - InMemory vs QDrant Vector Storage
// This demo shows how to build RAG applications using Semantic Kernel
// with both InMemory (for development) and QDrant (for production) vector stores

Console.WriteLine("🚀 Semantic Kernel RAG Demo\n");
Console.WriteLine("Choose your vector storage backend:");
Console.WriteLine("1. InMemory Vector Store (development/demos)");
Console.WriteLine("2. QDrant Integration Example (production-ready)");
Console.Write("\nEnter your choice (1 or 2): ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        await RunInMemoryExample();
        break;
    case "2":
        await QdrantExampleProgram.RunExample();
        break;
    default:
        Console.WriteLine("Invalid choice. Running InMemory example by default...");
        await RunInMemoryExample();
        break;
}

static async Task RunInMemoryExample()
{
    Console.WriteLine("\n📂 Running InMemory Vector Store Example...");
    Console.WriteLine("⚠️  Note: Data will be lost when application restarts\n");
    
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

    Console.WriteLine("🔄 Ingesting data into InMemory store...");
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
    Console.WriteLine($"✅ Ingested {lines.Length} records into InMemory store\n");

    var query = "Generate a query for the Sales workspace";
    Console.WriteLine($"🔍 Searching for: '{query}'");
    
    var queryEmbedding = await embedding.GenerateEmbeddingAsync(query);
    var search = await collection.VectorizedSearchAsync(queryEmbedding, new VectorSearchOptions { Top = 1 });
    var results = await search.Results.AsAsyncEnumerable().ToListAsync();
    var csvData = results?.First()?.Record?.Text;

    Console.WriteLine($"📊 Found relevant data: {csvData}\n");

    var sqlPlugin = kernel.CreateFunctionFromPrompt(prompt);
    var response = await sqlPlugin.InvokeAsync(kernel, new KernelArguments { ["instructions"] = instructions, ["csvData"] = csvData });

    Console.WriteLine("🎯 Generated SQL:");
    Console.WriteLine(response);
    
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("📝 InMemory Vector Store Characteristics:");
    Console.WriteLine("✅ Fast setup - No external dependencies");
    Console.WriteLine("✅ Perfect for development and demos");
    Console.WriteLine("❌ Data lost on application restart");
    Console.WriteLine("❌ Limited by available RAM");
    Console.WriteLine("❌ Not suitable for production workloads");
}
