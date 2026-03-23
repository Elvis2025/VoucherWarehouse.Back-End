using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Hosting;
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

public sealed class FolderBootstrapIndexerHostedService : BackgroundService
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
            Directory.CreateDirectory(root);

        var files = Directory.EnumerateFiles(
            root,
            "*.*",
            _options.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (IsRegistryFile(file))
                continue;

            await _queue.EnqueueAsync(new DocumentIndexQueueItemDto
            {
                FilePath = file,
                Reason = "StartupScan"
            }, stoppingToken);
        }

        _logger.LogInformation("Escaneo inicial completado para carpeta: {Root}", root);
    }

    private bool IsRegistryFile(string path)
        => Path.GetFullPath(path)
            .Equals(Path.GetFullPath(_options.RegistryFilePath), StringComparison.OrdinalIgnoreCase);
}
