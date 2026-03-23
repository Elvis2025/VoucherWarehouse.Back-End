using IBS.VoucherWarehouse.AI.Ollama.Dto;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class DocumentIndexQueue : IDocumentIndexQueue , ITransientDependency
{
    private readonly Channel<DocumentIndexQueueItemDto> _channel;

    public DocumentIndexQueue()
    {
        var options = new BoundedChannelOptions(10_000)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<DocumentIndexQueueItemDto>(options);
    }

    public ValueTask EnqueueAsync(DocumentIndexQueueItemDto item, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(item, cancellationToken);

    public ValueTask<DocumentIndexQueueItemDto> DequeueAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAsync(cancellationToken);
}
