# Semantic Kernel RAG with Vector Storage

This project demonstrates how to build Retrieval Augmented Generation (RAG) applications using Microsoft Semantic Kernel with different vector storage backends.

## Features

### 🚀 InMemory Vector Store (Development)
- Perfect for development and demos
- No external dependencies required
- Fast setup and execution
- ⚠️ Data is lost when application restarts

### 🌟 QDrant Integration (Production)
- **Data Persistence**: Embeddings survive application restarts
- **Scalability**: Handle millions of vectors efficiently
- **Advanced Search**: Rich filtering and hybrid search capabilities
- **Production Ready**: Enterprise-grade clustering and monitoring
- **Minimal Code Changes**: Same Semantic Kernel abstractions

## Getting Started

### Prerequisites
- .NET 8.0 or later
- Ollama running locally with `all-minilm` and `phi3` models

### Running the Application

1. **Clone and build**:
   ```bash
   git clone <repository-url>
   cd SemanticKernel.Embeddings
   dotnet build
   dotnet run
   ```

2. **Choose your vector store**:
   - Option 1: InMemory store (no setup required)
   - Option 2: QDrant integration example

### QDrant Setup (Optional)

To experience the full QDrant integration:

1. **Start QDrant with Docker**:
   ```bash
   docker run -p 6333:6333 qdrant/qdrant
   ```

2. **Add QDrant packages** (when implementing actual integration):
   ```bash
   dotnet add package Microsoft.SemanticKernel.Connectors.Qdrant --prerelease
   ```

3. **Update the code** to use QDrant:
   ```csharp
   // Replace this:
   .AddInMemoryVectorStore()
   
   // With this:
   .AddQdrantVectorStore("http://localhost:6333")
   ```

## Project Structure

```
├── Program.cs                 # Main application with choice between implementations
├── QdrantExampleProgram.cs    # QDrant integration example and benefits
├── Data.cs                    # Vector data model for InMemory store
├── Plugin/
│   ├── data.csv              # Sample workspace data
│   ├── instructions.txt      # SQL generation instructions
│   └── prompt.txt            # LLM prompt template
└── README.md                 # This file
```

## How It Works

1. **Data Ingestion**: CSV data is loaded and converted to embeddings using Ollama's `all-minilm` model
2. **Vector Storage**: Embeddings are stored in either InMemory or QDrant vector store
3. **Semantic Search**: User queries are converted to embeddings and matched against stored vectors
4. **SQL Generation**: Retrieved context is used to generate SQL queries using Ollama's `phi3` model

## Migration from InMemory to QDrant

The beauty of Semantic Kernel's vector store abstraction is minimal code changes:

### Before (InMemory):
```csharp
var builder = Kernel
    .CreateBuilder()
    .AddOllamaTextEmbeddingGeneration("all-minilm", new Uri("http://localhost:11434"))
    .AddOllamaTextGeneration("phi3", new Uri("http://localhost:11434"))
    .AddInMemoryVectorStore(); // Ephemeral storage

var memory = kernel.GetRequiredService<InMemoryVectorStore>();
var collection = memory.GetCollection<string, Data<string>>("Data");
```

### After (QDrant):
```csharp
var builder = Kernel
    .CreateBuilder()
    .AddOllamaTextEmbeddingGeneration("all-minilm", new Uri("http://localhost:11434"))
    .AddOllamaTextGeneration("phi3", new Uri("http://localhost:11434"))
    .AddQdrantVectorStore("http://localhost:6333"); // Persistent storage

var vectorStore = kernel.GetRequiredService<IVectorStore>();
var collection = vectorStore.GetCollection<string, WorkspaceData>("workspace_data");
```

## Performance Comparison

| Feature | InMemory Store | QDrant |
|---------|----------------|---------|
| **Setup Time** | Instant | ~2 seconds (Docker) |
| **Data Persistence** | ❌ Session only | ✅ Persistent |
| **Search Speed** | ~1ms | ~5-10ms |
| **Memory Usage** | High (all in RAM) | Configurable |
| **Scalability** | Limited by RAM | Millions of vectors |
| **Production Ready** | ❌ Demos only | ✅ Enterprise grade |

## What's Next?

This implementation demonstrates the foundation for:
- **Hybrid Search**: Combining vector similarity with full-text search
- **Multi-modal RAG**: Images, documents, and text in the same workflow
- **Advanced Filtering**: Complex queries with QDrant's rich filtering capabilities
- **Performance Optimization**: Tuning QDrant for specific use cases

## Blog Post

This code accompanies the blog post: ["Semantic Kernel RAG with QDrant - (Post 2)"](https://adithakker.github.io/Semantic-Kernel-QDrant)

Happy coding! 🚀