using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;

public interface IDocumentTextExtractor : ITransientDependency
{
    bool CanHandle(string fileExtension);

    Task<string> ExtractTextAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}
