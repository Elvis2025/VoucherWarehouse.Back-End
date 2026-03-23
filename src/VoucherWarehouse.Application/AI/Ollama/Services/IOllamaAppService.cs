using IBS.VoucherWarehouse.AI.Ollama.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services;

public interface IOllamaAppService : IApplicationService
{
    Task<RagAskResponseDto> Ask([FromBody] RagAskRequestDto request, CancellationToken cancellationToken);
}
