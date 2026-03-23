using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class DocumentIndexWorkerHostedService : BackgroundService
{
    private readonly IDocumentIndexQueue _queue;
    private readonly IDocumentIndexingService _indexingService;
    private readonly ILogger<DocumentIndexWorkerHostedService> _logger;

    private readonly ConcurrentDictionary<string, byte> _inProgress = new(StringComparer.OrdinalIgnoreCase);

    public DocumentIndexWorkerHostedService(
        IDocumentIndexQueue queue,
        IDocumentIndexingService indexingService,
        ILogger<DocumentIndexWorkerHostedService> logger)
    {
        _queue = queue;
        _indexingService = indexingService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de indexación iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var item = await _queue.DequeueAsync(stoppingToken);
            var fullPath = Path.GetFullPath(item.FilePath);

            if (!_inProgress.TryAdd(fullPath, 0))
                continue;

            _ = Task.Run(async () =>
            {
                try
                {
                    await ProcessAsync(fullPath, item.Reason, stoppingToken);
                }
                finally
                {
                    _inProgress.TryRemove(fullPath, out byte _);
                }
            }, stoppingToken);
        }
    }

    private async Task ProcessAsync(string filePath, string reason, CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                await _indexingService.DeleteFileAsync(filePath, cancellationToken);
                return;
            }

            await _indexingService.IndexFileAsync(filePath, reason, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando cola para archivo {FilePath}", filePath);
        }
    }
}
