using IBS.VoucherWarehouse.AI.Ollama.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IDocumentRegistryStore : ISingletonDependency
{
    Task<IReadOnlyDictionary<string, IndexedDocumentRegistryItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IndexedDocumentRegistryItemDto?> GetAsync(string filePath, CancellationToken cancellationToken = default);
    Task UpsertAsync(IndexedDocumentRegistryItemDto item, CancellationToken cancellationToken = default);
    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}
