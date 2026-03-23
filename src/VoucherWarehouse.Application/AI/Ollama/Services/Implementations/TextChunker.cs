using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class TextChunker : ITextChunker, ITransientDependency
{
    public IReadOnlyList<string> Chunk(string text, int chunkSize, int overlap)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<string>();

        text = Normalize(text);

        var chunks = new List<string>();
        var start = 0;

        while (start < text.Length)
        {
            var length = Math.Min(chunkSize, text.Length - start);
            var slice = text.Substring(start, length);

            if (start + length < text.Length)
            {
                var breakIndex = slice.LastIndexOfAny(new[] { '.', '!', '?', '\n', ';', ':' });
                if (breakIndex > Math.Min(250, slice.Length / 3))
                {
                    slice = slice[..(breakIndex + 1)];
                }
            }

            slice = slice.Trim();

            if (!string.IsNullOrWhiteSpace(slice))
                chunks.Add(slice);

            start += Math.Max(1, slice.Length - overlap);
        }

        return chunks;
    }

    private static string Normalize(string input)
    {
        var normalized = input
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Replace("\t", " ");

        while (normalized.Contains("\n\n\n", StringComparison.Ordinal))
            normalized = normalized.Replace("\n\n\n", "\n\n", StringComparison.Ordinal);

        while (normalized.Contains("  ", StringComparison.Ordinal))
            normalized = normalized.Replace("  ", " ", StringComparison.Ordinal);

        return normalized.Trim();
    }
}
