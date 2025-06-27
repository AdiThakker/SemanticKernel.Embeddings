using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SemanticKernel.Embeddings;

public class VectorDbContext : DbContext
{
    public DbSet<VectorRecord> VectorRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=vectors.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VectorRecord>(entity =>
        {
            entity.HasKey(e => e.Key);
            entity.Property(e => e.EmbeddingJson).HasColumnType("TEXT");
        });
    }
}

public class VectorRecord
{
    [Key]
    public string Key { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string EmbeddingJson { get; set; } = string.Empty;

    public ReadOnlyMemory<float> GetEmbedding()
    {
        if (string.IsNullOrEmpty(EmbeddingJson))
            return ReadOnlyMemory<float>.Empty;
        
        var values = JsonSerializer.Deserialize<float[]>(EmbeddingJson);
        return new ReadOnlyMemory<float>(values);
    }

    public void SetEmbedding(ReadOnlyMemory<float> embedding)
    {
        EmbeddingJson = JsonSerializer.Serialize(embedding.ToArray());
    }
}
