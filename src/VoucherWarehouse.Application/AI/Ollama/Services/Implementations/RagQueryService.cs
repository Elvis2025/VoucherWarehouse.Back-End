using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class RagQueryService : IRagQueryService, ITransientDependency
{
    private readonly IOllamaEmbeddingClient _embeddingClient;
    private readonly IQdrantVectorStore _vectorStore;
    private readonly IOllamaChatClient _chatClient;
    private readonly AiDocumentIndexingOptionsDto _options;

    public RagQueryService(
        IOllamaEmbeddingClient embeddingClient,
        IQdrantVectorStore vectorStore,
        IOllamaChatClient chatClient,
        IOptions<AiDocumentIndexingOptionsDto> options)
    {
        _embeddingClient = embeddingClient;
        _vectorStore = vectorStore;
        _chatClient = chatClient;
        _options = options.Value;
    }

    public async Task<RagAskResponseDto> AskAsync(RagAskRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ArgumentException("La pregunta es obligatoria.", nameof(request.Question));
        }

        var question = request.Question.Trim();

        var embedding = await _embeddingClient.GenerateEmbeddingAsync(question, cancellationToken);

        var topK = request.TopK.HasValue && request.TopK.Value > 0
            ? request.TopK.Value
            : _options.TopK;

        var hits = await _vectorStore.SearchAsync(embedding, topK, cancellationToken);

        var filteredHits = BuildContextHits(hits);

        if (filteredHits.Count == 0)
        {
            return new RagAskResponseDto
            {
                Answer = BuildOutOfScopeResponse(),
                Sources = new List<RagAskSourceDto>()
            };
        }

        var contextBuilder = new StringBuilder();

        foreach (var hit in filteredHits)
        {
            contextBuilder.AppendLine($"[Archivo: {hit.FileName} | Chunk: {hit.ChunkIndex} | Score: {hit.Score:F4}]");
            contextBuilder.AppendLine(hit.Text);
            contextBuilder.AppendLine("-----");
        }

        var systemPrompt = """
Eres IBS AI, un asistente profesional de IB Systems.

Reglas obligatorias:
1. Debes responder únicamente con base en la información contenida en la documentación recuperada.
2. Si la información no está claramente soportada por la documentación, no inventes ni completes con conocimiento general.
3. Si el contexto documental no contiene la respuesta suficiente, debes indicarlo de forma profesional y aclarar que estás limitado a responder preguntas relacionadas con la documentación y con IB Systems.
4. Si la respuesta sí está en la documentación, responde de forma clara, profesional, precisa y útil.
5. Puedes resumir, reorganizar y explicar mejor el contenido, pero sin alterar el sentido original.
6. No respondas como asistente general. Tu alcance está limitado a la documentación de IB Systems.
""";

        var userPrompt = $"""
Documentación recuperada:
{contextBuilder}

Pregunta del usuario:
{question}
""";

        var answer = await _chatClient.GenerateAnswerAsync(systemPrompt, userPrompt, cancellationToken);

        return new RagAskResponseDto
        {
            Answer = answer,
            Sources = filteredHits
                .Select(x => new RagAskSourceDto
                {
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    ChunkIndex = x.ChunkIndex,
                    Score = x.Score,
                    Preview = x.Text.Length > 240 ? x.Text[..240] + "..." : x.Text
                })
                .ToList()
        };
    }

    private List<DocumentSearchHitDto> BuildContextHits(IReadOnlyList<DocumentSearchHitDto> hits)
    {
        if (hits is null || hits.Count == 0)
        {
            return new List<DocumentSearchHitDto>();
        }

        var cleaned = hits
            .Where(x => x is not null)
            .Where(x => x.Score >= _options.MinScoreThreshold)
            .Where(x => !string.IsNullOrWhiteSpace(x.Text))
            .Where(x => x.Text.Trim().Length >= 30)
            .Where(x => !IsNoiseFile(x.FileName))
            .GroupBy(x => $"{x.FilePath}|{x.ChunkIndex}")
            .Select(g => g.First())
            .OrderByDescending(x => x.Score)
            .ToList();

        if (cleaned.Count == 0)
        {
            return new List<DocumentSearchHitDto>();
        }

        // Distribuir resultados entre varios documentos para no quedarse solo con uno
        var selected = new List<DocumentSearchHitDto>();
        var perDocumentCounter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var hit in cleaned)
        {
            var documentKey = hit.FilePath ?? string.Empty;

            if (!perDocumentCounter.ContainsKey(documentKey))
            {
                perDocumentCounter[documentKey] = 0;
            }

            if (perDocumentCounter[documentKey] >= _options.MaxChunksPerDocument)
            {
                continue;
            }

            selected.Add(hit);
            perDocumentCounter[documentKey]++;

            if (selected.Count >= _options.MaxContextChunks)
            {
                break;
            }
        }

        return selected
            .OrderByDescending(x => x.Score)
            .ToList();
    }

    private static string BuildOutOfScopeResponse()
    {
        return "Soy IBS AI, un asistente de IB Systems. Estoy limitado a responder preguntas y dudas relacionadas con la documentación disponible de IB Systems. No encontré información suficiente en la documentación indexada para responder esa consulta.";
    }

    private static bool IsNoiseFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return true;
        }

        var lower = fileName.Trim().ToLowerInvariant();

        return lower is "desktop.ini" or "thumbs.db"
            || lower.StartsWith("~$")
            || lower.EndsWith(".tmp");
    }
}