namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public class DgiiExcelImportDto
{
    // Cabecera
    public string CasoPrueba { get; set; }
    public int TipoeCF { get; set; }
    public string ENCF { get; set; }
    public string FechaVencimientoSecuencia { get; set; }
    public int? IndicadorMontoGravado { get; set; }
    public string TipoIngresos { get; set; }
    public int? TipoPago { get; set; }

    // Emisor
    public string RNCEmisor { get; set; }
    public string RazonSocialEmisor { get; set; }
    public string NombreComercial { get; set; }
    public string DireccionEmisor { get; set; }
    public string Municipio { get; set; }
    public string Provincia { get; set; }
    public List<string> TelefonosEmisor { get; set; } = null;
    public string CorreoEmisor { get; set; }
    public string WebSite { get; set; }
    public string CodigoVendedor { get; set; }

    // Documento comercial
    public string NumeroFacturaInterna { get; set; }
    public string NumeroPedidoInterno { get; set; }
    public string ZonaVenta { get; set; }
    public string FechaEmision { get; set; }

    // Comprador
    public string RNCComprador { get; set; }
    public string RazonSocialComprador { get; set; }
    public string ContactoComprador { get; set; }
    public string CorreoComprador { get; set; }
    public string DireccionComprador { get; set; }
    public string MunicipioComprador { get; set; }
    public string ProvinciaComprador { get; set; }

    // Referencias
    public string FechaEntrega { get; set; }
    public string FechaOrdenCompra { get; set; }
    public string NumeroOrdenCompra { get; set; }
    public string CodigoInternoComprador { get; set; }
    public string NumeroContenedor { get; set; }
    public string NumeroReferencia { get; set; }

    // Totales
    public decimal? MontoGravadoTotal { get; set; }
    public decimal? MontoGravadoI1 { get; set; }
    public decimal? ITBIS1 { get; set; }
    public decimal? TotalITBIS { get; set; }
    public decimal? TotalITBIS1 { get; set; }
    public decimal? MontoTotal { get; set; }

    // Grupos dinámicos
    public List<DgiiExcelFormaPagoDto> FormasPago { get; set; } = new();
    public List<DgiiExcelDetalleDto> Items { get; set; } = new();
    public EcfVoucherOutputDto Errors { get; set; }
}



public class DgiiExcelFormaPagoDto
{
    public int Numero { get; set; }
    public int? FormaPago { get; set; }
    public decimal? MontoPago { get; set; }
}

public class DgiiExcelDetalleDto
{
    public int Numero { get; set; }
    public int? NumeroLinea { get; set; }
    public int? IndicadorFacturacion { get; set; }
    public string NombreItem { get; set; }
    public int? IndicadorBienoServicio { get; set; }
    public decimal? CantidadItem { get; set; }
    public int? UnidadMedida { get; set; }
    public decimal? PrecioUnitarioItem { get; set; }
    public decimal? MontoItem { get; set; }
}