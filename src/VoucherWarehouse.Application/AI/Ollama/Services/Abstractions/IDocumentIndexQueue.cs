using IBS.VoucherWarehouse.AI.Ollama.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IDocumentIndexQueue : ITransientDependency
{
    ValueTask EnqueueAsync(DocumentIndexQueueItemDto item, CancellationToken cancellationToken = default);
    ValueTask<DocumentIndexQueueItemDto> DequeueAsync(CancellationToken cancellationToken = default);
}
