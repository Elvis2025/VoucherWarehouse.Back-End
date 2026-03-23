using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Helpers;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class DocumentIndexingService : IDocumentIndexingService, ITransientDependency
{
    private readonly IDocumentTextExtractorResolver _resolver;
    private readonly IFileHashService _fileHashService;
    private readonly ITextChunker _chunker;
    private readonly IOllamaEmbeddingClient _embeddingClient;
    private readonly IQdrantVectorStore _vectorStore;
    private readonly IDocumentRegistryStore _registryStore;
    private readonly AiDocumentIndexingOptionsDto _options;
    private readonly ILogger<DocumentIndexingService> _logger;

    public DocumentIndexingService(
        IDocumentTextExtractorResolver resolver,
        IFileHashService fileHashService,
        ITextChunker chunker,
        IOllamaEmbeddingClient embeddingClient,
        IQdrantVectorStore vectorStore,
        IDocumentRegistryStore registryStore,
        IOptions<AiDocumentIndexingOptionsDto> options,
        ILogger<DocumentIndexingService> logger)
    {
        _resolver = resolver;
        _fileHashService = fileHashService;
        _chunker = chunker;
        _embeddingClient = embeddingClient;
        _vectorStore = vectorStore;
        _registryStore = registryStore;
        _options = options.Value;
        _logger = logger;
    }

    public async Task IndexFileAsync(string filePath, string reason, CancellationToken cancellationToken = default)
    {
        filePath = Path.GetFullPath(filePath);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("No se puede indexar porque el archivo no existe: {FilePath}", filePath);
            return;
        }

        var extension = Path.GetExtension(filePath);

        try
        {
            await WaitUntilFileIsReadyAsync(filePath, cancellationToken);

            var fileInfo = new FileInfo(filePath);
            var sha256 = await _fileHashService.ComputeSha256Async(filePath, cancellationToken);
            var existing = await _registryStore.GetAsync(filePath, cancellationToken);

            if (existing is not null &&
                existing.Sha256 == sha256 &&
                existing.LastWriteTimeUtc == fileInfo.LastWriteTimeUtc &&
                existing.Status == "Indexed")
            {
                _logger.LogInformation("Archivo sin cambios. Se omite reindexación: {FilePath}", filePath);
                return;
            }

            var extractor = _resolver.Resolve(filePath);
            var extractedText = await extractor.ExtractTextAsync(filePath, cancellationToken);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                await _registryStore.UpsertAsync(new IndexedDocumentRegistryItemDto
                {
                    FilePath = filePath,
                    FileName = fileInfo.Name,
                    Extension = extension,
                    FileSizeBytes = fileInfo.Length,
                    LastWriteTimeUtc = fileInfo.LastWriteTimeUtcUtc(),
                    Sha256 = sha256,
                    Status = "Empty",
                    LastIndexedAtUtc = DateTime.UtcNow,
                    LastSeenAtUtc = DateTime.UtcNow,
                    ChunkCount = 0,
                    DocumentGroupId = existing?.DocumentGroupId ?? Guid.NewGuid(),
                    LastError = null
                }, cancellationToken);

                await _vectorStore.DeleteByFilePathAsync(filePath, cancellationToken);
                return;
            }

            var chunks = _chunker.Chunk(extractedText, _options.ChunkSize, _options.ChunkOverlap);
            var documentGroupId = existing?.DocumentGroupId ?? Guid.NewGuid();

            var preparedChunks = new List<DocumentChunkRecordDto>(chunks.Count);

            for (var i = 0; i < chunks.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var embedding = await _embeddingClient.GenerateEmbeddingAsync(chunks[i], cancellationToken);

                preparedChunks.Add(new DocumentChunkRecordDto
                {
                    ChunkId = Guid.NewGuid(),
                    DocumentGroupId = documentGroupId,
                    FilePath = filePath,
                    FileName = fileInfo.Name,
                    Extension = extension,
                    ChunkIndex = i,
                    Text = chunks[i],
                    Embedding = embedding,
                    Sha256 = sha256,
                    LastWriteTimeUtc = fileInfo.LastWriteTimeUtcUtc()
                });
            }

            await _vectorStore.DeleteByFilePathAsync(filePath, cancellationToken);
            await _vectorStore.UpsertChunksAsync(preparedChunks, cancellationToken);

            await _registryStore.UpsertAsync(new IndexedDocumentRegistryItemDto
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                Extension = extension,
                FileSizeBytes = fileInfo.Length,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtcUtc(),
                Sha256 = sha256,
                Status = "Indexed",
                LastIndexedAtUtc = DateTime.UtcNow,
                LastSeenAtUtc = DateTime.UtcNow,
                ChunkCount = preparedChunks.Count,
                DocumentGroupId = documentGroupId,
                LastError = null
            }, cancellationToken);

            _logger.LogInformation("Archivo indexado correctamente: {FilePath} | Chunks: {ChunkCount} | Motivo: {Reason}",
                filePath, preparedChunks.Count, reason);
        }
        catch (NotSupportedException ex)
        {
            await SaveErrorStateAsync(filePath, extension, ex, cancellationToken, "Unsupported");
            _logger.LogWarning(ex, "Formato no soportado: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            await SaveErrorStateAsync(filePath, extension, ex, cancellationToken, "Error");
            _logger.LogError(ex, "Error indexando archivo: {FilePath}", filePath);
        }
    }

    public async Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        filePath = Path.GetFullPath(filePath);

        await _vectorStore.DeleteByFilePathAsync(filePath, cancellationToken);
        await _registryStore.DeleteAsync(filePath, cancellationToken);

        _logger.LogInformation("Archivo eliminado del índice: {FilePath}", filePath);
    }

    private async Task SaveErrorStateAsync(
        string filePath,
        string extension,
        Exception ex,
        CancellationToken cancellationToken,
        string status)
    {
        var fileExists = File.Exists(filePath);
        var fileInfo = fileExists ? new FileInfo(filePath) : null;
        var existing = await _registryStore.GetAsync(filePath, cancellationToken);

        await _registryStore.UpsertAsync(new IndexedDocumentRegistryItemDto
        {
            FilePath = filePath,
            FileName = fileInfo?.Name ?? Path.GetFileName(filePath),
            Extension = extension,
            FileSizeBytes = fileInfo?.Length ?? 0,
            LastWriteTimeUtc = fileInfo?.LastWriteTimeUtcUtc() ?? DateTime.UtcNow,
            Sha256 = existing?.Sha256 ?? string.Empty,
            Status = status,
            LastIndexedAtUtc = DateTime.UtcNow,
            LastSeenAtUtc = DateTime.UtcNow,
            ChunkCount = existing?.ChunkCount ?? 0,
            DocumentGroupId = existing?.DocumentGroupId ?? Guid.NewGuid(),
            LastError = ex.Message
        }, cancellationToken);
    }

    private async Task WaitUntilFileIsReadyAsync(string filePath, CancellationToken cancellationToken)
    {
        await Task.Delay(_options.StableFileWaitMilliseconds, cancellationToken);

        for (var i = 0; i < _options.MaxFileOpenRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var stream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete);

                if (stream.Length >= 0)
                    return;
            }
            catch
            {
                // retry
            }

            await Task.Delay(_options.FileOpenRetryDelayMilliseconds, cancellationToken);
        }

        throw new IOException($"No se pudo abrir el archivo para indexación: {filePath}");
    }
}

