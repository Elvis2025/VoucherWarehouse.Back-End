using DocumentFormat.OpenXml.Packaging;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class DocxDocumentTextExtractor : IDocumentTextExtractor, ITransientDependency
{
    public bool CanHandle(string fileExtension) => ".docx".Equals(fileExtension, StringComparison.OrdinalIgnoreCase);

    public Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        using var document = WordprocessingDocument.Open(filePath, false);
        var body = document.MainDocumentPart?.Document?.Body;

        if (body is null)
            return Task.FromResult(string.Empty);

        foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            sb.Append(text.Text);
            sb.Append(' ');
        }

        return Task.FromResult(sb.ToString());
    }
}
