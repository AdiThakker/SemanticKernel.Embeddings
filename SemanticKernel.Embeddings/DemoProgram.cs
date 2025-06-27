using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using SemanticKernel.Embeddings;
using System.Linq;

// Demo version without Ollama dependency
try
{
    Console.WriteLine("Starting SQLite Vector Store Demo (without Ollama)...");

    // Initialize SQLite vector store
    Console.WriteLine("Initializing SQLite vector store...");
    using var dbContext = new VectorDbContext();
    var vectorStore = new SqliteVectorStore(dbContext);
    await vectorStore.InitializeAsync();

    Console.WriteLine("Reading sample data...");
    var sampleData = new[]
    {
        "Sales data for January 2024: Revenue $100,000, Units sold: 500",
        "Marketing campaign results: CTR 3.5%, Conversion rate 2.1%",
        "Customer satisfaction survey: Average rating 4.2/5"
    };

    Console.WriteLine("Processing and storing embeddings (with mock embeddings)...");
    
    int idx = 0;
    foreach (var line in sampleData)
    {
        Console.WriteLine($"Processing line {idx + 1}/{sampleData.Length}: {line.Substring(0, Math.Min(50, line.Length))}...");
        
        // Create mock embeddings for demo purposes
        var mockEmbedding = GenerateMockEmbedding(line);
        
        await vectorStore.UpsertAsync(new Data<string>
        {
            Category = "data",
            Key = $"{idx++}",
            Text = line,
            TextEmbedding = mockEmbedding
        });
    }   

    Console.WriteLine("Performing vector search...");
    var query = "Sales information";
    var queryEmbedding = GenerateMockEmbedding(query);

    var searchResults = await vectorStore.VectorizedSearchAsync(queryEmbedding, 3);
    
    Console.WriteLine($"Found {searchResults.Count} results:");
    foreach (var result in searchResults)
    {
        Console.WriteLine($"Score: {result.Score:F4} - {result.Record.Text}");
    }

    Console.WriteLine("\nSQLite Vector Store Demo completed successfully!");
    Console.WriteLine($"Database file created at: vectors.db");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

// Helper method to generate mock embeddings for demo
static ReadOnlyMemory<float> GenerateMockEmbedding(string text)
{
    // Simple hash-based mock embedding for demo purposes
    var hash = text.GetHashCode();
    var random = new Random(hash);
    var embedding = new float[384]; // Smaller dimension for demo
    
    for (int i = 0; i < embedding.Length; i++)
    {
        embedding[i] = (float)(random.NextDouble() * 2 - 1); // Random values between -1 and 1
    }
    
    // Normalize the vector
    var magnitude = Math.Sqrt(embedding.Sum(x => x * x));
    for (int i = 0; i < embedding.Length; i++)
    {
        embedding[i] /= (float)magnitude;
    }
    
    return new ReadOnlyMemory<float>(embedding);
}
