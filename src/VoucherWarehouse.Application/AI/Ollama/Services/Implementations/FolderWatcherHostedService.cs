using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class FolderWatcherHostedService : IHostedService, IDisposable, ITransientDependency
{
    private readonly IDocumentIndexQueue _queue;
    private readonly AiDocumentIndexingOptionsDto _options;
    private readonly ILogger<FolderWatcherHostedService> _logger;
    private FileSystemWatcher? _watcher;

    public FolderWatcherHostedService(
        IDocumentIndexQueue queue,
        IOptions<AiDocumentIndexingOptionsDto> options,
        ILogger<FolderWatcherHostedService> logger)
    {
        _queue = queue;
        _options = options.Value;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_options.RootFolderPath))
        {
            Directory.CreateDirectory(_options.RootFolderPath);
        }

        _watcher = new FileSystemWatcher(_options.RootFolderPath)
        {
            IncludeSubdirectories = _options.IncludeSubdirectories,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        _watcher.Created += OnCreatedOrChanged;
        _watcher.Changed += OnCreatedOrChanged;
        _watcher.Renamed += OnRenamed;
        _watcher.Deleted += OnDeleted;

        _logger.LogInformation("Watcher de documentos iniciado en {RootFolderPath}", _options.RootFolderPath);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        DisposeWatcher();
        return Task.CompletedTask;
    }

    private async void OnCreatedOrChanged(object sender, FileSystemEventArgs e)
    {
        if (ShouldIgnoreFile(e.FullPath))
        {
            return;
        }

        try
        {
            await _queue.EnqueueAsync(new DocumentIndexQueueItemDto
            {
                FilePath = e.FullPath,
                Reason = e.ChangeType.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando evento {ChangeType} para {FilePath}", e.ChangeType, e.FullPath);
        }
    }

    private async void OnRenamed(object sender, RenamedEventArgs e)
    {
        try
        {
            if (!ShouldIgnoreFile(e.OldFullPath))
            {
                await _queue.EnqueueAsync(new DocumentIndexQueueItemDto
                {
                    FilePath = e.OldFullPath,
                    Reason = "DeletedByRename"
                });
            }

            if (!ShouldIgnoreFile(e.FullPath))
            {
                await _queue.EnqueueAsync(new DocumentIndexQueueItemDto
                {
                    FilePath = e.FullPath,
                    Reason = "Renamed"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando renombrado de archivo {OldPath} -> {NewPath}", e.OldFullPath, e.FullPath);
        }
    }

    private async void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (ShouldIgnoreFile(e.FullPath))
        {
            return;
        }

        try
        {
            await _queue.EnqueueAsync(new DocumentIndexQueueItemDto
            {
                FilePath = e.FullPath,
                Reason = "Deleted"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando eliminación de archivo {FilePath}", e.FullPath);
        }
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

    private void DisposeWatcher()
    {
        if (_watcher is null)
        {
            return;
        }

        _watcher.EnableRaisingEvents = false;
        _watcher.Created -= OnCreatedOrChanged;
        _watcher.Changed -= OnCreatedOrChanged;
        _watcher.Renamed -= OnRenamed;
        _watcher.Deleted -= OnDeleted;
        _watcher.Dispose();
        _watcher = null;
    }

    public void Dispose()
    {
        DisposeWatcher();
    }
}