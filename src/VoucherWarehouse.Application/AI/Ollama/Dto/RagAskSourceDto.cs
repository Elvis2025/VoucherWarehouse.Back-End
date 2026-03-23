using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class RagAskSourceDto
{
    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public int ChunkIndex { get; set; }
    public double Score { get; set; }
    public string Preview { get; set; } = default!;
}
