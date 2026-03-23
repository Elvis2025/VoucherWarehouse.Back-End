using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementation;

public sealed class FolderBootstrapIndexerHostedService : BackgroundService, ITransientDependency
{
    private readonly IDocumentIndexQueue _queue;
    private readonly AiDocumentIndexingOptionsDto _options;
    private readonly ILogger<FolderBootstrapIndexerHostedService> _logger;

    public FolderBootstrapIndexerHostedService(
        IDocumentIndexQueue queue,
        IOptions<AiDocumentIndexingOptionsDto> options,
        ILogger<FolderBootstrapIndexerHostedService> logger)
    {
        _queue = queue;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var root = _options.RootFolderPath;

        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        var files = Directory.EnumerateFiles(
            root,
            "*.*",
            _options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (ShouldIgnoreFile(file))
            {
                continue;
            }

            await _queue.EnqueueAsync(new DocumentIndexQueueItemDto
            {
                FilePath = file,
                Reason = "StartupScan"
            }, stoppingToken);
        }

        _logger.LogInformation("Escaneo inicial de documentos completado en {RootFolderPath}", root);
    }

    private bool ShouldIgnoreFile(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);

        if (fullPath.Equals(Path.GetFullPath(_options.RegistryFilePath), StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fileName = Path.GetFileName(filePath)?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return true;
        }

        return fileName is "desktop.ini" or "thumbs.db"
            || fileName.StartsWith("~$")
            || fileName.EndsWith(".tmp");
    }
}