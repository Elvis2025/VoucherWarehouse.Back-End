using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class AiWarmupHostedService : IHostedService
{
    private readonly IOllamaChatClient _chatClient;
    private readonly IOllamaEmbeddingClient _embeddingClient;
    private readonly IQdrantVectorStore _vectorStore;
    private readonly ILogger<AiWarmupHostedService> _logger;

    public AiWarmupHostedService(
        IOllamaChatClient chatClient,
        IOllamaEmbeddingClient embeddingClient,
        IQdrantVectorStore vectorStore,
        ILogger<AiWarmupHostedService> logger)
    {
        _chatClient = chatClient;
        _embeddingClient = embeddingClient;
        _vectorStore = vectorStore;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando warmup de IA...");

        await _vectorStore.EnsureCollectionExistsAsync(cancellationToken);
        await _embeddingClient.WarmupAsync(cancellationToken);
        await _chatClient.WarmupAsync(cancellationToken);

        _logger.LogInformation("Warmup completado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
