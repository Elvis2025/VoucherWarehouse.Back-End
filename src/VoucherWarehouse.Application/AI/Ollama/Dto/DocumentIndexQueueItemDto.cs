using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class DocumentIndexQueueItemDto
{
    public string FilePath { get; set; } = default!;
    public string Reason { get; set; } = default!;
    public DateTime EnqueuedAtUtc { get; set; } = DateTime.UtcNow;
}
