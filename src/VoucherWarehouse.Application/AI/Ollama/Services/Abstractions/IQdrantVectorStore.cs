using IBS.VoucherWarehouse.AI.Ollama.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IQdrantVectorStore : ITransientDependency
{
    Task EnsureCollectionExistsAsync(CancellationToken cancellationToken = default);
    Task UpsertChunksAsync(IReadOnlyCollection<DocumentChunkRecordDto> chunks, CancellationToken cancellationToken = default);
    Task DeleteByFilePathAsync(string filePath, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentSearchHitDto>> SearchAsync(float[] embedding, int topK, CancellationToken cancellationToken = default);
}
