namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;


class ReceiveSaleResumeInputDto
{
    public int printFormat { get; set; }
    public bool sendPrintedFile { get; set; }
    public EncabezadoResume encabezado { get; set; }
}

public class EncabezadoResume
{
    public IdDocResume idDoc { get; set; }
    [Required]
    public EmisorResume emisor { get; set; }
    public CompradorResume comprador { get; set; }
    [Required]
    public TotalesResume totales { get; set; }
}

public class IdDocResume
{
    [Required]
    [StringLength(2)]
    public string tipoeCF { get; set; }
    [Required]
    [StringLength(13)]
    public string eNCF { get; set; }
    [StringLength(2)]
    public string tipoIngresos { get; set; }
    [Required]
    public int tipoPago { get; set; }
    [StringLength(10)]
    public string fechaLimitePago { get; set; }
    public List<TablaFormasPago> tablaFormasPago { get; set; }
    public string tipoCuentaPago { get; set; }
}

public class EmisorResume
{
    [Required]
    public string rNCEmisor { get; set; }
    [Required]
    [StringLength(150)]
    public string razonSocialEmisor { get; set; }
    [Required]
    [StringLength(10)]
    public string fechaEmision { get; set; }
}

public class CompradorResume : Comprador
{
    //[Required]
    public string identificadorExtranjero { get; set; }
}

public class TotalesResume
{
    public decimal? montoGravadoI1 { get; set; }
    public decimal? montoGravadoI2 { get; set; }
    public decimal? montoGravadoI3 { get; set; }
    public decimal? montoGravadoTotal { get; set; }
    public decimal montoExento { get; set; }
    public decimal? totalITBIS1 { get; set; }
    public decimal? totalITBIS2 { get; set; }
    public decimal? totalITBIS3 { get; set; }
    public decimal? totalITBIS { get; set; }
    public decimal? montoImpuestoAdicional { get; set; }
    public List<ImpuestosAdicional> impuestosAdicionales { get; set; }
    public decimal montoNoFacturable { get; set; }
    [Required]
    public decimal montoTotal { get; set; }
    public decimal montoPeriodo { get; set; }
}
