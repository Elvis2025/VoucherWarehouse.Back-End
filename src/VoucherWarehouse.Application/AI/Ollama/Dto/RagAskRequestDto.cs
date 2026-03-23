using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class RagAskRequestDto
{
    public string Question { get; set; } = default!;
    public int? TopK { get; set; }
}


