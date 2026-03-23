using ClosedXML.Excel;
using IBS.VoucherWarehouse.AI.Ollama.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.AI.Ollama.Services.Implementations;

public sealed class XlsxDocumentTextExtractor : IDocumentTextExtractor, ITransientDependency
{
    public bool CanHandle(string fileExtension)
        => ".xlsx".Equals(fileExtension, StringComparison.OrdinalIgnoreCase)
        || ".xlsm".Equals(fileExtension, StringComparison.OrdinalIgnoreCase);

    public Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        using var workbook = new XLWorkbook(filePath);

        foreach (var worksheet in workbook.Worksheets)
        {
            cancellationToken.ThrowIfCancellationRequested();

            sb.AppendLine($"[Sheet: {worksheet.Name}]");

            foreach (var row in worksheet.RowsUsed())
            {
                var values = row.CellsUsed().Select(c => c.GetValue<string>().Trim());
                var line = string.Join(" | ", values.Where(v => !string.IsNullOrWhiteSpace(v)));

                if (!string.IsNullOrWhiteSpace(line))
                    sb.AppendLine(line);
            }

            sb.AppendLine();
        }

        return Task.FromResult(sb.ToString());
    }
}