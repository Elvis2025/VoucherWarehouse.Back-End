using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class QdrantOptionsDto
{
    public const string SectionName = "Qdrant";

    public string BaseUrl { get; set; } = "http://localhost:6333";
    public string CollectionName { get; set; } = "ibs-ai-rag";
    public int VectorSize { get; set; } = 768;
    public string Distance { get; set; } = "Cosine";
}
