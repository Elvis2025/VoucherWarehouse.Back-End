using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class QdrantVectorStore : IQdrantVectorStore, ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly QdrantOptionsDto _options;

    public QdrantVectorStore(HttpClient httpClient, IOptions<QdrantOptionsDto> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task EnsureCollectionExistsAsync(CancellationToken cancellationToken = default)
    {
        var collectionUrl = $"/collections/{_options.CollectionName}";

        using var existsResponse = await _httpClient.GetAsync(collectionUrl, cancellationToken);

        if (existsResponse.IsSuccessStatusCode)
        {
            return;
        }

        if (existsResponse.StatusCode != HttpStatusCode.NotFound)
        {
            var rawExistsError = await existsResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Error verificando la colección de Qdrant. " +
                $"Status: {(int)existsResponse.StatusCode} {existsResponse.StatusCode}. " +
                $"Response: {rawExistsError}");
        }

        var body = new
        {
            vectors = new
            {
                size = _options.VectorSize,
                distance = _options.Distance
            }
        };

        using var createResponse = await _httpClient.PutAsJsonAsync(
            collectionUrl,
            body,
            cancellationToken);

        if (createResponse.StatusCode == HttpStatusCode.Conflict)
        {
            // Si otro proceso la creó justo antes, no rompemos
            return;
        }

        if (!createResponse.IsSuccessStatusCode)
        {
            var rawCreateError = await createResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Error creando la colección de Qdrant. " +
                $"Status: {(int)createResponse.StatusCode} {createResponse.StatusCode}. " +
                $"Response: {rawCreateError}");
        }
    }

    public async Task UpsertChunksAsync(IReadOnlyCollection<DocumentChunkRecordDto> chunks, CancellationToken cancellationToken = default)
    {
        if (chunks.Count == 0)
            return;

        var points = chunks.Select(x => new
        {
            id = x.ChunkId.ToString(),
            vector = x.Embedding,
            payload = new
            {
                chunkId = x.ChunkId,
                documentGroupId = x.DocumentGroupId,
                filePath = x.FilePath,
                fileName = x.FileName,
                extension = x.Extension,
                chunkIndex = x.ChunkIndex,
                text = x.Text,
                sha256 = x.Sha256,
                lastWriteTimeUtc = x.LastWriteTimeUtc
            }
        });

        var body = new { points };

        using var response = await _httpClient.PutAsJsonAsync(
            $"/collections/{_options.CollectionName}/points",
            body,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteByFilePathAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var body = new
        {
            filter = new
            {
                must = new object[]
                {
                    new
                    {
                        key = "filePath",
                        match = new { value = filePath }
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/collections/{_options.CollectionName}/points/delete")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<DocumentSearchHitDto>> SearchAsync(float[] embedding, int topK, CancellationToken cancellationToken = default)
    {
        if (embedding is null || embedding.Length == 0)
        {
            throw new InvalidOperationException("El embedding de búsqueda está vacío.");
        }

        if (embedding.Length != _options.VectorSize)
        {
            throw new InvalidOperationException(
                $"Dimensión inválida del embedding. Esperado: {_options.VectorSize}, recibido: {embedding.Length}.");
        }

        var body = new
        {
            vector = embedding,
            limit = topK,
            with_payload = true
        };

        using var response = await _httpClient.PostAsJsonAsync(
            $"/collections/{_options.CollectionName}/points/search",
            body,
            cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Qdrant SearchAsync falló. " +
                $"Status: {(int)response.StatusCode} {response.StatusCode}. " +
                $"Collection: {_options.CollectionName}. " +
                $"VectorSizeConfig: {_options.VectorSize}. " +
                $"EmbeddingLength: {embedding.Length}. " +
                $"Response: {raw}");
        }

        using var json = JsonDocument.Parse(raw);

        var result = new List<DocumentSearchHitDto>();

        if (!json.RootElement.TryGetProperty("result", out var resultNode) || resultNode.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in resultNode.EnumerateArray())
        {
            var payload = item.GetProperty("payload");

            result.Add(new DocumentSearchHitDto
            {
                ChunkId = Guid.Parse(payload.GetProperty("chunkId").ToString()),
                DocumentGroupId = Guid.Parse(payload.GetProperty("documentGroupId").ToString()),
                FilePath = payload.GetProperty("filePath").GetString() ?? string.Empty,
                FileName = payload.GetProperty("fileName").GetString() ?? string.Empty,
                Extension = payload.GetProperty("extension").GetString() ?? string.Empty,
                ChunkIndex = payload.GetProperty("chunkIndex").GetInt32(),
                Text = payload.GetProperty("text").GetString() ?? string.Empty,
                Score = item.TryGetProperty("score", out var scoreProp) ? scoreProp.GetDouble() : 0
            });
        }

        return result;
    }
}
