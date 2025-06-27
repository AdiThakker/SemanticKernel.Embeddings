using Microsoft.Extensions.VectorData;

namespace SemanticKernel.Embeddings;

public sealed class Data<TKey>
{
    [VectorStoreRecordKey]
    public required TKey Key { get; set; }

    [VectorStoreRecordData]
    public required string Category { get; set; }

    [VectorStoreRecordData]
    public required string Text { get; set; }

    [VectorStoreRecordVector(384)]
    public ReadOnlyMemory<float> TextEmbedding { get; set; }
}
