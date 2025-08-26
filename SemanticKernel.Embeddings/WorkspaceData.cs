using Microsoft.Extensions.VectorData;

namespace SemanticKernel.Embeddings;

public sealed class WorkspaceData
{
    [VectorStoreRecordKey]
    public required string Id { get; set; }

    [VectorStoreRecordData]
    public required string Category { get; set; }

    [VectorStoreRecordData]
    public required string Content { get; set; }

    [VectorStoreRecordVector(384)] // all-minilm dimension
    public ReadOnlyMemory<float> ContentVector { get; set; }
}