using Microsoft.Extensions.VectorData;

namespace SemanticKernel.Embeddings;

/// <summary>
/// QDrant-optimized data model for workspace information
/// Designed for production use with proper vector dimensions and indexing
/// </summary>
public sealed class QdrantWorkspaceData
{
    [VectorStoreRecordKey]
    public required string Id { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public required string Category { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public required string WorkspaceName { get; set; }

    [VectorStoreRecordData]
    public required string Content { get; set; }

    [VectorStoreRecordData]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [VectorStoreRecordVector(384)]
    public ReadOnlyMemory<float> ContentVector { get; set; }
}