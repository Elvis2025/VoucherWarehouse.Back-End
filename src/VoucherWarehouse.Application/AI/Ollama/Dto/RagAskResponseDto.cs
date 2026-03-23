using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Dto;

public sealed record class RagAskResponseDto
{
    public string Answer { get; set; } = default!;
    public List<RagAskSourceDto> Sources { get; set; } = new();
}
