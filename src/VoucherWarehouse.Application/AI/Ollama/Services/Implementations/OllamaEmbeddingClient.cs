using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class OllamaEmbeddingClient : IOllamaEmbeddingClient, ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptionsDto _options;

    public OllamaEmbeddingClient(HttpClient httpClient, IOptions<OllamaOptionsDto> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task WarmupAsync(CancellationToken cancellationToken = default)
    {
        _ = await GenerateEmbeddingAsync("warmup", cancellationToken);
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = _options.EmbeddingModel,
            input = text
        };

        using var response = await _httpClient.PostAsJsonAsync("/api/embed", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<EmbedResponse>(cancellationToken: cancellationToken);

        if (payload?.Embeddings is null || payload.Embeddings.Count == 0)
            throw new InvalidOperationException("Ollama no devolvió embeddings.");

        return payload.Embeddings[0].ToArray();
    }

    private sealed class EmbedResponse
    {
        [JsonPropertyName("embeddings")]
        public List<List<float>> Embeddings { get; set; } = new();
    }
}
