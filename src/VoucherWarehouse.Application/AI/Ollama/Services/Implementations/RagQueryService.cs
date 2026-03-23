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
        if (string.IsNullOrWhiteSpace(request.Question))
            throw new ArgumentException("La pregunta es obligatoria.", nameof(request.Question));

        var embedding = await _embeddingClient.GenerateEmbeddingAsync(request.Question, cancellationToken);
        var topK = request.TopK.GetValueOrDefault(_options.TopK);

        var hits = await _vectorStore.SearchAsync(embedding, topK, cancellationToken);

        var context = new StringBuilder();
        foreach (var hit in hits.OrderByDescending(x => x.Score))
        {
            context.AppendLine($"[Archivo: {hit.FileName} | Chunk: {hit.ChunkIndex} | Score: {hit.Score:F4}]");
            context.AppendLine(hit.Text);
            context.AppendLine("-----");
        }

        var systemPrompt = """
Eres IBS AI, un asistente empresarial.
Debes responder únicamente usando el contexto documental recuperado.
Si el contexto no contiene la respuesta, dilo claramente.
No inventes datos.
Cuando puedas, resume con claridad y precisión.
""";

        var userPrompt = $"""
Contexto recuperado:
{context}

Pregunta:
{request.Question}
""";

        var answer = await _chatClient.GenerateAnswerAsync(systemPrompt, userPrompt, cancellationToken);

        return new RagAskResponseDto
        {
            Answer = answer,
            Sources = hits
                .OrderByDescending(x => x.Score)
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
}
