using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class IndexedDocumentRegistryItemDto
{
    public string FilePath { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long FileSizeBytes { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public string Sha256 { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public DateTime? LastIndexedAtUtc { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
    public int ChunkCount { get; set; }
    public string? LastError { get; set; }
    public Guid DocumentGroupId { get; set; }
}
