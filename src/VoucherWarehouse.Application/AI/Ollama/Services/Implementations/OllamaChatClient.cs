using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class OllamaChatClient : IOllamaChatClient, ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptionsDto _options;

    public OllamaChatClient(HttpClient httpClient, IOptions<OllamaOptionsDto> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task WarmupAsync(CancellationToken cancellationToken = default)
    {
        _ = await GenerateAnswerAsync(
            "Eres un asistente útil.",
            "Responde únicamente: listo",
            cancellationToken);
    }

    public async Task<string> GenerateAnswerAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = _options.ChatModel,
            stream = false,
            options = new
            {
                // Menor contexto = más velocidad
                num_ctx = Math.Min(_options.ChatContextWindow, 4096),

                // Limitar salida = evita respuestas eternas
                num_predict = 300,

                // Más bajo = más estable y rápido para tareas RAG
                temperature = 0.2,

                // Evita variaciones innecesarias
                top_p = 0.9
            },
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            }
        };

        using var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Ollama Chat falló. " +
                $"Status: {(int)response.StatusCode} {response.StatusCode}. " +
                $"Response: {raw}");
        }

        var payload = JsonSerializer.Deserialize<ChatResponse>(raw);

        return payload?.Message?.Content?.Trim()
               ?? throw new InvalidOperationException("Ollama no devolvió contenido.");
    }

    private sealed class ChatResponse
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }
    }

    private sealed class ChatMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}