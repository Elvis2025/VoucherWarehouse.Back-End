using HtmlAgilityPack;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class HtmlDocumentTextExtractor : IDocumentTextExtractor, ITransientDependency
{
    public bool CanHandle(string fileExtension)
        => ".html".Equals(fileExtension, StringComparison.OrdinalIgnoreCase)
        || ".htm".Equals(fileExtension, StringComparison.OrdinalIgnoreCase);

    public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var html = await File.ReadAllTextAsync(filePath, cancellationToken);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
    }
}