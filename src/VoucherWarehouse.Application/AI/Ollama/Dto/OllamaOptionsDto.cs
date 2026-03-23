using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class OllamaOptionsDto
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "llama3.1";
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
    public int ChatTimeoutMinutes { get; set; } = 5;
    public int EmbeddingTimeoutMinutes { get; set; } = 5;
    public int ChatContextWindow { get; set; } = 8192;
}
