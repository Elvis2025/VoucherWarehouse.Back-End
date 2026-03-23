using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class PdfDocumentTextExtractor : IDocumentTextExtractor, ITransientDependency
{
    public bool CanHandle(string fileExtension) => ".pdf".Equals(fileExtension, StringComparison.OrdinalIgnoreCase);

    public Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var text = new System.Text.StringBuilder();

        using var document = PdfDocument.Open(filePath);

        foreach (var page in document.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();
            text.AppendLine(page.Text);
        }

        return Task.FromResult(text.ToString());
    }
}
