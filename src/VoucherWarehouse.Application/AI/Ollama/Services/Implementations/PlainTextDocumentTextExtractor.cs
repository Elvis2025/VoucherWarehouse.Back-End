using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class PlainTextDocumentTextExtractor : IDocumentTextExtractor, ITransientDependency
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".md", ".log", ".csv", ".json", ".xml", ".yml", ".yaml", ".ini", ".config", ".cs", ".ts", ".js", ".html", ".htm"
    };

    public bool CanHandle(string fileExtension) => Extensions.Contains(fileExtension);

    public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(
            new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete),
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}
