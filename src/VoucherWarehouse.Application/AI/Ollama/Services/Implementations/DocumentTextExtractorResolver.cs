using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class DocumentTextExtractorResolver : IDocumentTextExtractorResolver, ITransientDependency
{
    private readonly IReadOnlyCollection<IDocumentTextExtractor> _extractors;

    public DocumentTextExtractorResolver(IEnumerable<IDocumentTextExtractor> extractors)
    {
        _extractors = extractors.ToArray();
    }

    public IDocumentTextExtractor Resolve(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        var extractor = _extractors.FirstOrDefault(x => x.CanHandle(extension));
        if (extractor is null)
            throw new NotSupportedException($"No existe extractor para el archivo '{filePath}'.");

        return extractor;
    }
}
