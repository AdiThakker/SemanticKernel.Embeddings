using Microsoft.Extensions.VectorData;

namespace SemanticKernel.Embeddings;

internal sealed class Artifacts<TKey>
{
    [VectorStoreRecordKey]
    public required TKey Key { get; set; }

    [VectorStoreRecordData]
    public required string Category { get; set; }

    [VectorStoreRecordData]
    public required string Text { get; set; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> TextEmbedding { get; set; }
}
