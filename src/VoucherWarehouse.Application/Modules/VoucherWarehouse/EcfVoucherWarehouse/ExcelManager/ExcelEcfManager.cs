using ClosedXML.Excel;
using IBS.VoucherWarehouse.Common.Constants;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.ExcelManager;

public static class ExcelEcfManager
{
    private const string ExcelExtensionXlsx = ".xlsx";
    private const string ExcelExtensionXls = ".xls";
    private const string CsvExtension = ".csv";
    private const string TextExtension = ".txt";

    private static readonly Regex IndexedColumnRegex =
        new(@"^(?<name>.+)\[(?<index>\d+)\]$", RegexOptions.Compiled);

    public static async Task ImportAsync(Stream fileStream, string fileName, Action<List<DgiiExcelImportDto>> action)
    {
        try
        {
            if (fileStream == null)
            {
                throw new UserFriendlyException("No file was received.");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new UserFriendlyException("The file name is invalid.");
            }

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            if (!IsSupportedExtension(extension))
            {
                throw new UserFriendlyException("Only Excel (.xlsx, .xls), CSV (.csv), and TXT (.txt) files are supported.");
            }

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var result = extension switch
            {
                ExcelExtensionXlsx or ExcelExtensionXls => ReadExcel(memoryStream),
                CsvExtension => ReadCsv(memoryStream),
                TextExtension => ReadText(memoryStream),
                _ => throw new UserFriendlyException("Unsupported file format.")
            };

            action?.Invoke(result);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }
        
    }

    private static bool IsSupportedExtension(string extension)
    {
        return extension is FileExtension.Xlsx
            or FileExtension.Xls
            or FileExtension.Csv
            or FileExtension.Txt;
    }

    #region Excel

    private static List<DgiiExcelImportDto> ReadExcel(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        const int headerRowNumber = 1;
        const int dataStartRowNumber = 2;

        var lastColumnUsed = worksheet.LastColumnUsed();
        var lastRowUsed = worksheet.LastRowUsed();

        if (lastColumnUsed == null || lastRowUsed == null)
        {
            return new List<DgiiExcelImportDto>();
        }

        var lastColumnNumber = lastColumnUsed.ColumnNumber();
        var lastRowNumber = lastRowUsed.RowNumber();

        var headers = BuildExcelHeadersMap(worksheet, headerRowNumber, lastColumnNumber);
        var result = new List<DgiiExcelImportDto>();

        for (int rowNumber = dataStartRowNumber; rowNumber <= lastRowNumber; rowNumber++)
        {
            if (IsExcelRowEmpty(worksheet, rowNumber, lastColumnNumber))
            {
                continue;
            }

            var dto = MapExcelRowToDto(worksheet, rowNumber, headers);
            result.Add(dto);
        }

        return result;
    }

    private static Dictionary<int, string> BuildExcelHeadersMap(IXLWorksheet worksheet, int headerRowNumber, int lastColumnNumber)
    {
        var headers = new Dictionary<int, string>(lastColumnNumber);

        for (int columnNumber = 1; columnNumber <= lastColumnNumber; columnNumber++)
        {
            headers[columnNumber] = worksheet.Cell(headerRowNumber, columnNumber).GetValue<string>()?.Trim() ?? string.Empty;
        }

        return headers;
    }

    private static DgiiExcelImportDto MapExcelRowToDto(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers)
    {
        var dto = new DgiiExcelImportDto
        {
            TipoeCF = GetExcelIntValue(worksheet, rowNumber, headers, "TipoeCF") ?? 0,
            IndicadorMontoGravado = GetExcelIntValue(worksheet, rowNumber, headers, "IndicadorMontoGravado"),
            TipoIngresos = GetExcelStringValue(worksheet, rowNumber, headers, "TipoIngresos"),
            TipoPago = GetExcelIntValue(worksheet, rowNumber, headers, "TipoPago"),

            RNCEmisor = GetExcelStringValue(worksheet, rowNumber, headers, "RNCEmisor"),
            RazonSocialEmisor = GetExcelStringValue(worksheet, rowNumber, headers, "RazonSocialEmisor"),
            NombreComercial = GetExcelStringValue(worksheet, rowNumber, headers, "NombreComercial"),
            DireccionEmisor = GetExcelStringValue(worksheet, rowNumber, headers, "DireccionEmisor"),
            Municipio = GetExcelStringValue(worksheet, rowNumber, headers, "Municipio"),
            Provincia = GetExcelStringValue(worksheet, rowNumber, headers, "Provincia"),
            CorreoEmisor = GetExcelStringValue(worksheet, rowNumber, headers, "CorreoEmisor"),
            WebSite = GetExcelStringValue(worksheet, rowNumber, headers, "WebSite"),
            CodigoVendedor = GetExcelStringValue(worksheet, rowNumber, headers, "CodigoVendedor"),

            NumeroFacturaInterna = GetExcelStringValue(worksheet, rowNumber, headers, "NumeroFacturaInterna"),
            NumeroPedidoInterno = GetExcelStringValue(worksheet, rowNumber, headers, "NumeroPedidoInterno"),
            ZonaVenta = GetExcelStringValue(worksheet, rowNumber, headers, "ZonaVenta"),
            FechaEmision = GetExcelStringValue(worksheet, rowNumber, headers, "FechaEmision"),

            RNCComprador = GetExcelStringValue(worksheet, rowNumber, headers, "RNCComprador"),
            RazonSocialComprador = GetExcelStringValue(worksheet, rowNumber, headers, "RazonSocialComprador"),
            ContactoComprador = GetExcelStringValue(worksheet, rowNumber, headers, "ContactoComprador"),
            CorreoComprador = GetExcelStringValue(worksheet, rowNumber, headers, "CorreoComprador"),
            DireccionComprador = GetExcelStringValue(worksheet, rowNumber, headers, "DireccionComprador"),
            MunicipioComprador = GetExcelStringValue(worksheet, rowNumber, headers, "MunicipioComprador"),
            ProvinciaComprador = GetExcelStringValue(worksheet, rowNumber, headers, "ProvinciaComprador"),

            FechaEntrega = GetExcelStringValue(worksheet, rowNumber, headers, "FechaEntrega"),
            FechaOrdenCompra = GetExcelStringValue(worksheet, rowNumber, headers, "FechaOrdenCompra"),
            NumeroOrdenCompra = GetExcelStringValue(worksheet, rowNumber, headers, "NumeroOrdenCompra"),
            CodigoInternoComprador = GetExcelStringValue(worksheet, rowNumber, headers, "CodigoInternoComprador"),
            NumeroContenedor = GetExcelStringValue(worksheet, rowNumber, headers, "NumeroContenedor"),
            NumeroReferencia = GetExcelStringValue(worksheet, rowNumber, headers, "NumeroReferencia"),

            MontoGravadoTotal = GetExcelDecimalValue(worksheet, rowNumber, headers, "MontoGravadoTotal"),
            MontoGravadoI1 = GetExcelDecimalValue(worksheet, rowNumber, headers, "MontoGravadoI1"),
            ITBIS1 = GetExcelDecimalValue(worksheet, rowNumber, headers, "ITBIS1"),
            TotalITBIS = GetExcelDecimalValue(worksheet, rowNumber, headers, "TotalITBIS"),
            TotalITBIS1 = GetExcelDecimalValue(worksheet, rowNumber, headers, "TotalITBIS1"),
            MontoTotal = GetExcelDecimalValue(worksheet, rowNumber, headers, "MontoTotal")
        };

        dto.TelefonosEmisor = GetExcelIndexedStringValues(worksheet, rowNumber, headers, "TelefonoEmisor");
        dto.FormasPago = GetExcelPaymentMethods(worksheet, rowNumber, headers);
        dto.Items = GetExcelItems(worksheet, rowNumber, headers);

        return dto;
    }

    private static string? GetExcelStringValue(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string headerName)
    {
        var columnNumber = FindHeaderColumn(headers, headerName);
        return columnNumber == 0
            ? null
            : NormalizeString(worksheet.Cell(rowNumber, columnNumber).GetFormattedString());
    }

    private static int? GetExcelIntValue(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string headerName)
    {
        var rawValue = GetExcelStringValue(worksheet, rowNumber, headers, headerName);
        return ParseInt(rawValue);
    }

    private static decimal? GetExcelDecimalValue(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string headerName)
    {
        var rawValue = GetExcelStringValue(worksheet, rowNumber, headers, headerName);
        return ParseDecimal(rawValue);
    }

    private static string? GetExcelIndexedStringValue(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string baseName, int index)
    {
        var indexedHeader = $"{baseName}[{index}]";
        var columnNumber = FindHeaderColumn(headers, indexedHeader);

        return columnNumber == 0
            ? null
            : NormalizeString(worksheet.Cell(rowNumber, columnNumber).GetFormattedString());
    }

    private static int? GetExcelIndexedIntValue(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string baseName, int index)
    {
        var rawValue = GetExcelIndexedStringValue(worksheet, rowNumber, headers, baseName, index);
        return ParseInt(rawValue);
    }

    private static decimal? GetExcelIndexedDecimalValue(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string baseName, int index)
    {
        var rawValue = GetExcelIndexedStringValue(worksheet, rowNumber, headers, baseName, index);
        return ParseDecimal(rawValue);
    }

    private static List<string> GetExcelIndexedStringValues(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers, string baseName)
    {
        var result = new List<string>();

        foreach (var index in GetIndexedColumnNumbers(headers, baseName).OrderBy(x => x))
        {
            var value = GetExcelIndexedStringValue(worksheet, rowNumber, headers, baseName, index);
            if (!string.IsNullOrWhiteSpace(value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static List<DgiiExcelFormaPagoDto> GetExcelPaymentMethods(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers)
    {
        var result = new List<DgiiExcelFormaPagoDto>();

        var indexes = GetIndexedColumnNumbers(headers, "FormaPago")
            .Union(GetIndexedColumnNumbers(headers, "MontoPago"))
            .Distinct()
            .OrderBy(x => x);

        foreach (var index in indexes)
        {
            var paymentType = GetExcelIndexedIntValue(worksheet, rowNumber, headers, "FormaPago", index);
            var paymentAmount = GetExcelIndexedDecimalValue(worksheet, rowNumber, headers, "MontoPago", index);

            if (paymentType == null && paymentAmount == null)
            {
                continue;
            }

            result.Add(new DgiiExcelFormaPagoDto
            {
                Numero = index,
                FormaPago = paymentType,
                MontoPago = paymentAmount
            });
        }

        return result;
    }

    private static List<DgiiExcelDetalleDto> GetExcelItems(IXLWorksheet worksheet, int rowNumber, Dictionary<int, string> headers)
    {
        var result = new List<DgiiExcelDetalleDto>();

        var indexes = GetIndexedColumnNumbers(headers, "NumeroLinea")
            .Union(GetIndexedColumnNumbers(headers, "IndicadorFacturacion"))
            .Union(GetIndexedColumnNumbers(headers, "NombreItem"))
            .Union(GetIndexedColumnNumbers(headers, "IndicadorBienoServicio"))
            .Union(GetIndexedColumnNumbers(headers, "CantidadItem"))
            .Union(GetIndexedColumnNumbers(headers, "UnidadMedida"))
            .Union(GetIndexedColumnNumbers(headers, "PrecioUnitarioItem"))
            .Union(GetIndexedColumnNumbers(headers, "MontoItem"))
            .Distinct()
            .OrderBy(x => x);

        foreach (var index in indexes)
        {
            var item = new DgiiExcelDetalleDto
            {
                Numero = index,
                NumeroLinea = GetExcelIndexedIntValue(worksheet, rowNumber, headers, "NumeroLinea", index),
                IndicadorFacturacion = GetExcelIndexedIntValue(worksheet, rowNumber, headers, "IndicadorFacturacion", index),
                NombreItem = GetExcelIndexedStringValue(worksheet, rowNumber, headers, "NombreItem", index),
                IndicadorBienoServicio = GetExcelIndexedIntValue(worksheet, rowNumber, headers, "IndicadorBienoServicio", index),
                CantidadItem = GetExcelIndexedDecimalValue(worksheet, rowNumber, headers, "CantidadItem", index),
                UnidadMedida = GetExcelIndexedIntValue(worksheet, rowNumber, headers, "UnidadMedida", index),
                PrecioUnitarioItem = GetExcelIndexedDecimalValue(worksheet, rowNumber, headers, "PrecioUnitarioItem", index),
                MontoItem = GetExcelIndexedDecimalValue(worksheet, rowNumber, headers, "MontoItem", index)
            };

            if (IsEmptyItem(item))
            {
                continue;
            }

            result.Add(item);
        }

        return result;
    }

    private static bool IsExcelRowEmpty(IXLWorksheet worksheet, int rowNumber, int lastColumnNumber)
    {
        for (int columnNumber = 1; columnNumber <= lastColumnNumber; columnNumber++)
        {
            if (!string.IsNullOrWhiteSpace(worksheet.Cell(rowNumber, columnNumber).GetFormattedString()))
            {
                return false;
            }
        }

        return true;
    }

    #endregion


    private static List<DgiiExcelImportDto> ReadCsv(Stream stream)
    {
        return ReadDelimited(stream, FileDelimiter.Pipe);
    }

    private static List<DgiiExcelImportDto> ReadText(Stream stream)
    {
        return ReadDelimited(stream, FileDelimiter.Pipe);
    }


    #region Delimited files


    private static List<DgiiExcelImportDto> ReadDelimited(Stream stream, char delimiter)
    {
        var result = new List<DgiiExcelImportDto>();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);

        var headerLine = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            return result;
        }

        var headerValues = SplitDelimitedLine(headerLine, delimiter);
        var headers = BuildDelimitedHeadersMap(headerValues);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = SplitDelimitedLine(line, delimiter);

            if (IsDelimitedRowEmpty(values))
            {
                continue;
            }

            var dto = MapDelimitedRowToDto(values, headers);
            result.Add(dto);
        }

        return result;
    }

    private static Dictionary<int, string> BuildDelimitedHeadersMap(IReadOnlyList<string> headerValues)
    {
        var headers = new Dictionary<int, string>(headerValues.Count);

        for (int index = 0; index < headerValues.Count; index++)
        {
            headers[index] = NormalizeString(headerValues[index]) ?? string.Empty;
        }

        return headers;
    }

    private static DgiiExcelImportDto MapDelimitedRowToDto(IReadOnlyList<string> values, Dictionary<int, string> headers)
    {
        var dto = new DgiiExcelImportDto
        {
            TipoeCF = GetDelimitedIntValue(values, headers, "TipoeCF") ?? 0,
            IndicadorMontoGravado = GetDelimitedIntValue(values, headers, "IndicadorMontoGravado"),
            TipoIngresos = GetDelimitedStringValue(values, headers, "TipoIngresos"),
            TipoPago = GetDelimitedIntValue(values, headers, "TipoPago"),

            RNCEmisor = GetDelimitedStringValue(values, headers, "RNCEmisor"),
            RazonSocialEmisor = GetDelimitedStringValue(values, headers, "RazonSocialEmisor"),
            NombreComercial = GetDelimitedStringValue(values, headers, "NombreComercial"),
            DireccionEmisor = GetDelimitedStringValue(values, headers, "DireccionEmisor"),
            Municipio = GetDelimitedStringValue(values, headers, "Municipio"),
            Provincia = GetDelimitedStringValue(values, headers, "Provincia"),
            CorreoEmisor = GetDelimitedStringValue(values, headers, "CorreoEmisor"),
            WebSite = GetDelimitedStringValue(values, headers, "WebSite"),
            CodigoVendedor = GetDelimitedStringValue(values, headers, "CodigoVendedor"),

            NumeroFacturaInterna = GetDelimitedStringValue(values, headers, "NumeroFacturaInterna"),
            NumeroPedidoInterno = GetDelimitedStringValue(values, headers, "NumeroPedidoInterno"),
            ZonaVenta = GetDelimitedStringValue(values, headers, "ZonaVenta"),
            FechaEmision = GetDelimitedStringValue(values, headers, "FechaEmision"),

            RNCComprador = GetDelimitedStringValue(values, headers, "RNCComprador"),
            RazonSocialComprador = GetDelimitedStringValue(values, headers, "RazonSocialComprador"),
            ContactoComprador = GetDelimitedStringValue(values, headers, "ContactoComprador"),
            CorreoComprador = GetDelimitedStringValue(values, headers, "CorreoComprador"),
            DireccionComprador = GetDelimitedStringValue(values, headers, "DireccionComprador"),
            MunicipioComprador = GetDelimitedStringValue(values, headers, "MunicipioComprador"),
            ProvinciaComprador = GetDelimitedStringValue(values, headers, "ProvinciaComprador"),

            FechaEntrega = GetDelimitedStringValue(values, headers, "FechaEntrega"),
            FechaOrdenCompra = GetDelimitedStringValue(values, headers, "FechaOrdenCompra"),
            NumeroOrdenCompra = GetDelimitedStringValue(values, headers, "NumeroOrdenCompra"),
            CodigoInternoComprador = GetDelimitedStringValue(values, headers, "CodigoInternoComprador"),
            NumeroContenedor = GetDelimitedStringValue(values, headers, "NumeroContenedor"),
            NumeroReferencia = GetDelimitedStringValue(values, headers, "NumeroReferencia"),

            MontoGravadoTotal = GetDelimitedDecimalValue(values, headers, "MontoGravadoTotal"),
            MontoGravadoI1 = GetDelimitedDecimalValue(values, headers, "MontoGravadoI1"),
            ITBIS1 = GetDelimitedDecimalValue(values, headers, "ITBIS1"),
            TotalITBIS = GetDelimitedDecimalValue(values, headers, "TotalITBIS"),
            TotalITBIS1 = GetDelimitedDecimalValue(values, headers, "TotalITBIS1"),
            MontoTotal = GetDelimitedDecimalValue(values, headers, "MontoTotal")
        };

        dto.TelefonosEmisor = GetDelimitedIndexedStringValues(values, headers, "TelefonoEmisor");
        dto.FormasPago = GetDelimitedPaymentMethods(values, headers);
        dto.Items = GetDelimitedItems(values, headers);

        return dto;
    }

    private static string GetDelimitedStringValue(IReadOnlyList<string> values, Dictionary<int, string> headers, string headerName)
    {
        var columnIndex = FindHeaderColumn(headers, headerName);

        return columnIndex >= 0 && columnIndex < values.Count
            ? NormalizeString(values[columnIndex])
            : null;
    }

    private static int? GetDelimitedIntValue(IReadOnlyList<string> values, Dictionary<int, string> headers, string headerName)
    {
        var rawValue = GetDelimitedStringValue(values, headers, headerName);
        return ParseInt(rawValue);
    }

    private static decimal? GetDelimitedDecimalValue(IReadOnlyList<string> values, Dictionary<int, string> headers, string headerName)
    {
        var rawValue = GetDelimitedStringValue(values, headers, headerName);
        return ParseDecimal(rawValue);
    }

    private static string? GetDelimitedIndexedStringValue(IReadOnlyList<string> values, Dictionary<int, string> headers, string baseName, int index)
    {
        var indexedHeader = $"{baseName}[{index}]";
        var columnIndex = FindHeaderColumn(headers, indexedHeader);

        return columnIndex >= 0 && columnIndex < values.Count
            ? NormalizeString(values[columnIndex])
            : null;
    }

    private static int? GetDelimitedIndexedIntValue(IReadOnlyList<string> values, Dictionary<int, string> headers, string baseName, int index)
    {
        var rawValue = GetDelimitedIndexedStringValue(values, headers, baseName, index);
        return ParseInt(rawValue);
    }

    private static decimal? GetDelimitedIndexedDecimalValue(IReadOnlyList<string> values, Dictionary<int, string> headers, string baseName, int index)
    {
        var rawValue = GetDelimitedIndexedStringValue(values, headers, baseName, index);
        return ParseDecimal(rawValue);
    }

    private static List<string> GetDelimitedIndexedStringValues(IReadOnlyList<string> values, Dictionary<int, string> headers, string baseName)
    {
        var result = new List<string>();

        foreach (var index in GetIndexedColumnNumbers(headers, baseName).OrderBy(x => x))
        {
            var value = GetDelimitedIndexedStringValue(values, headers, baseName, index);
            if (!string.IsNullOrWhiteSpace(value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static List<DgiiExcelFormaPagoDto> GetDelimitedPaymentMethods(IReadOnlyList<string> values, Dictionary<int, string> headers)
    {
        var result = new List<DgiiExcelFormaPagoDto>();

        var indexes = GetIndexedColumnNumbers(headers, "FormaPago")
            .Union(GetIndexedColumnNumbers(headers, "MontoPago"))
            .Distinct()
            .OrderBy(x => x);

        foreach (var index in indexes)
        {
            var paymentType = GetDelimitedIndexedIntValue(values, headers, "FormaPago", index);
            var paymentAmount = GetDelimitedIndexedDecimalValue(values, headers, "MontoPago", index);

            if (paymentType == null && paymentAmount == null)
            {
                continue;
            }

            result.Add(new DgiiExcelFormaPagoDto
            {
                Numero = index,
                FormaPago = paymentType,
                MontoPago = paymentAmount
            });
        }

        return result;
    }

    private static List<DgiiExcelDetalleDto> GetDelimitedItems(IReadOnlyList<string> values, Dictionary<int, string> headers)
    {
        var result = new List<DgiiExcelDetalleDto>();

        var indexes = GetIndexedColumnNumbers(headers, "NumeroLinea")
            .Union(GetIndexedColumnNumbers(headers, "IndicadorFacturacion"))
            .Union(GetIndexedColumnNumbers(headers, "NombreItem"))
            .Union(GetIndexedColumnNumbers(headers, "IndicadorBienoServicio"))
            .Union(GetIndexedColumnNumbers(headers, "CantidadItem"))
            .Union(GetIndexedColumnNumbers(headers, "UnidadMedida"))
            .Union(GetIndexedColumnNumbers(headers, "PrecioUnitarioItem"))
            .Union(GetIndexedColumnNumbers(headers, "MontoItem"))
            .Distinct()
            .OrderBy(x => x);

        foreach (var index in indexes)
        {
            var item = new DgiiExcelDetalleDto
            {
                Numero = index,
                NumeroLinea = GetDelimitedIndexedIntValue(values, headers, "NumeroLinea", index),
                IndicadorFacturacion = GetDelimitedIndexedIntValue(values, headers, "IndicadorFacturacion", index),
                NombreItem = GetDelimitedIndexedStringValue(values, headers, "NombreItem", index),
                IndicadorBienoServicio = GetDelimitedIndexedIntValue(values, headers, "IndicadorBienoServicio", index),
                CantidadItem = GetDelimitedIndexedDecimalValue(values, headers, "CantidadItem", index),
                UnidadMedida = GetDelimitedIndexedIntValue(values, headers, "UnidadMedida", index),
                PrecioUnitarioItem = GetDelimitedIndexedDecimalValue(values, headers, "PrecioUnitarioItem", index),
                MontoItem = GetDelimitedIndexedDecimalValue(values, headers, "MontoItem", index)
            };

            if (IsEmptyItem(item))
            {
                continue;
            }

            result.Add(item);
        }

        return result;
    }

    private static bool IsDelimitedRowEmpty(IReadOnlyList<string> values)
    {
        return values == null || values.All(string.IsNullOrWhiteSpace);
    }

    private static List<string> SplitDelimitedLine(string line, char delimiter)
    {
        var result = new List<string>();
        if (line == null)
        {
            return result;
        }

        var builder = new StringBuilder(line.Length);
        var isInsideQuotes = false;

        for (int index = 0; index < line.Length; index++)
        {
            var currentChar = line[index];

            if (currentChar == '"')
            {
                if (isInsideQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    builder.Append('"');
                    index++;
                }
                else
                {
                    isInsideQuotes = !isInsideQuotes;
                }

                continue;
            }

            if (currentChar == delimiter && !isInsideQuotes)
            {
                result.Add(builder.ToString().Trim());
                builder.Clear();
                continue;
            }

            builder.Append(currentChar);
        }

        result.Add(builder.ToString().Trim());

        return result;
    }

    #endregion

    #region Shared helpers

    private static int FindHeaderColumn(Dictionary<int, string> headers, string headerName)
    {
        foreach (var header in headers)
        {
            if (string.Equals(header.Value, headerName, StringComparison.OrdinalIgnoreCase))
            {
                return header.Key;
            }
        }

        return headers.Keys.Any(k => k == 0) ? -1 : 0;
    }

    private static IEnumerable<int> GetIndexedColumnNumbers(Dictionary<int, string> headers, string baseName)
    {
        foreach (var header in headers)
        {
            if (string.IsNullOrWhiteSpace(header.Value))
            {
                continue;
            }

            var match = IndexedColumnRegex.Match(header.Value);
            if (!match.Success)
            {
                continue;
            }

            var currentBaseName = match.Groups["name"].Value.Trim();
            if (!string.Equals(currentBaseName, baseName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (int.TryParse(match.Groups["index"].Value, out var index))
            {
                yield return index;
            }
        }
    }

    private static bool IsEmptyItem(DgiiExcelDetalleDto item)
    {
        return string.IsNullOrWhiteSpace(item.NombreItem)
            && item.CantidadItem == null
            && item.PrecioUnitarioItem == null
            && item.MontoItem == null;
    }

    private static string NormalizeString(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static int? ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            ? number
            : null;
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        value = value.Trim();

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariantValue))
        {
            return invariantValue;
        }

        if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("es-DO"), out var dominicanValue))
        {
            return dominicanValue;
        }

        var normalizedValue = value.Replace(",", ".");
        if (decimal.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var normalizedDecimal))
        {
            return normalizedDecimal;
        }

        return null;
    }

    #endregion
}