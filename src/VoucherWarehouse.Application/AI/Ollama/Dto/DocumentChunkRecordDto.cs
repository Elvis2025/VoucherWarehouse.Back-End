using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class DocumentChunkRecordDto
{
    public Guid ChunkId { get; set; }
    public Guid DocumentGroupId { get; set; }
    public string FilePath { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public int ChunkIndex { get; set; }
    public string Text { get; set; } = default!;
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public string Sha256 { get; set; } = default!;
    public DateTime LastWriteTimeUtc { get; set; }
}
