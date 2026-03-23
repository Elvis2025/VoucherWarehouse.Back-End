using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IOllamaChatClient : ITransientDependency
{
    Task WarmupAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateAnswerAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}
