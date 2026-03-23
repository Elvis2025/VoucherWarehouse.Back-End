using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services;

public class OllamaAppService : VoucherWarehouseAppServiceBase, IOllamaAppService
{
    private readonly IRagQueryService ragQueryService;

    public OllamaAppService(IRagQueryService ragQueryService)
    {
        this.ragQueryService = ragQueryService;
    }

    public async Task<RagAskResponseDto> Ask([FromBody] RagAskRequestDto request, CancellationToken cancellationToken)
    {
        var result = await ragQueryService.AskAsync(request, cancellationToken);
        return result;
    }
}
