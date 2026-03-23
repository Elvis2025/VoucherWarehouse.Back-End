using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class DocumentRegistryStore : IDocumentRegistryStore, ISingletonDependency
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private readonly string _registryPath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public DocumentRegistryStore(IOptions<AiDocumentIndexingOptionsDto> options)
    {
        _registryPath = options.Value.RegistryFilePath;
    }

    public async Task<IReadOnlyDictionary<string, IndexedDocumentRegistryItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            return await ReadCoreAsync(cancellationToken);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task<IndexedDocumentRegistryItemDto?> GetAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            var data = await ReadCoreAsync(cancellationToken);
            return data.TryGetValue(Normalize(filePath), out var item) ? item : null;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task UpsertAsync(IndexedDocumentRegistryItemDto item, CancellationToken cancellationToken = default)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            var data = await ReadCoreAsync(cancellationToken);
            data[Normalize(item.FilePath)] = item;
            await WriteCoreAsync(data, cancellationToken);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            var data = await ReadCoreAsync(cancellationToken);
            data.Remove(Normalize(filePath));
            await WriteCoreAsync(data, cancellationToken);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async Task<Dictionary<string, IndexedDocumentRegistryItemDto>> ReadCoreAsync(CancellationToken cancellationToken)
    {
        var folder = Path.GetDirectoryName(_registryPath);
        if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!File.Exists(_registryPath))
            return new Dictionary<string, IndexedDocumentRegistryItemDto>(StringComparer.OrdinalIgnoreCase);

        await using var stream = new FileStream(_registryPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, IndexedDocumentRegistryItemDto>>(stream, _jsonOptions, cancellationToken);

        return data ?? new Dictionary<string, IndexedDocumentRegistryItemDto>(StringComparer.OrdinalIgnoreCase);
    }

    private async Task WriteCoreAsync(Dictionary<string, IndexedDocumentRegistryItemDto> data, CancellationToken cancellationToken)
    {
        var tempPath = _registryPath + ".tmp";

        await using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await JsonSerializer.SerializeAsync(stream, data, _jsonOptions, cancellationToken);
        }

        File.Copy(tempPath, _registryPath, overwrite: true);
        File.Delete(tempPath);
    }

    private static string Normalize(string path) => Path.GetFullPath(path).Trim();
}