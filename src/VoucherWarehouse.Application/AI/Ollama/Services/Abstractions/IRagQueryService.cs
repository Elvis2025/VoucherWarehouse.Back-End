using IBS.VoucherWarehouse.AI.Ollama.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IRagQueryService : ITransientDependency
{
    Task<RagAskResponseDto> AskAsync(RagAskRequestDto request, CancellationToken cancellationToken = default);
}
