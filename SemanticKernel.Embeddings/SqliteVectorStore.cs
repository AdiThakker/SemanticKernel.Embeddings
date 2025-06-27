using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.VectorData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticKernel.Embeddings;

public class SqliteVectorStore
{
    private readonly VectorDbContext _context;

    public SqliteVectorStore(VectorDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task UpsertAsync(Data<string> data)
    {
        var existing = await _context.VectorRecords.FirstOrDefaultAsync(x => x.Key == data.Key);
        
        if (existing != null)
        {
            existing.Category = data.Category;
            existing.Text = data.Text;
            existing.SetEmbedding(data.TextEmbedding);
        }
        else
        {
            var record = new VectorRecord
            {
                Key = data.Key,
                Category = data.Category,
                Text = data.Text
            };
            record.SetEmbedding(data.TextEmbedding);
            _context.VectorRecords.Add(record);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<VectorSearchResult<Data<string>>>> VectorizedSearchAsync(ReadOnlyMemory<float> queryEmbedding, int top = 10)
    {
        // For demonstration, we'll use a simple approach
        // In production, you'd want to use a proper vector similarity search
        var allRecords = await _context.VectorRecords.ToListAsync();
        
        var results = new List<VectorSearchResult<Data<string>>>();
        
        foreach (var record in allRecords)
        {
            var embedding = record.GetEmbedding();
            var similarity = CosineSimilarity(queryEmbedding.Span, embedding.Span);
            
            var data = new Data<string>
            {
                Key = record.Key,
                Category = record.Category,
                Text = record.Text,
                TextEmbedding = embedding
            };
            
            results.Add(new VectorSearchResult<Data<string>>(data, similarity));
        }
        
        return results.OrderByDescending(x => x.Score).Take(top).ToList();
    }

    private static double CosineSimilarity(ReadOnlySpan<float> vector1, ReadOnlySpan<float> vector2)
    {
        if (vector1.Length != vector2.Length)
            return 0;

        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += vector1[i] * vector1[i];
            magnitude2 += vector2[i] * vector2[i];
        }

        if (magnitude1 == 0 || magnitude2 == 0)
            return 0;

        return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
    }
}

public class VectorSearchResult<T>
{
    public T Record { get; set; }
    public double Score { get; set; }

    public VectorSearchResult(T record, double score)
    {
        Record = record;
        Score = score;
    }
}
