using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class AiDocumentIndexingOptionsDto
{
    public const string SectionName = "AiDocumentIndexing";

    public string RootFolderPath { get; set; } = @"C:\IBSAI-Rag-Documents";
    public string RegistryFilePath { get; set; } = @"C:\IBSAI-Rag-Documents\.ibsai-registry.json";
    public int ChunkSize { get; set; } = 1000;
    public int ChunkOverlap { get; set; } = 150;
    public int TopK { get; set; } = 5;
    public double MinScoreThreshold { get; set; } = 0.70;
    public int MaxContextChunks { get; set; } = 3;
    public int StableFileWaitMilliseconds { get; set; } = 1500;
    public int MaxFileOpenRetries { get; set; } = 10;
    public int FileOpenRetryDelayMilliseconds { get; set; } = 500;
    public bool IncludeSubdirectories { get; set; } = true;
}
