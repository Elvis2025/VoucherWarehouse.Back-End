using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class EcfVoucherWarehouseOutputDto : BaseEntityDto<int>
{
    public string DgiiResponseMessage { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoAgravadoTotal { get; set; }
    public decimal ITBIS1 { get; set; }
    public decimal ITBIS2 { get; set; }
    public decimal ITBIS3 { get; set; }
    public string NombreComercial { get; set; }
    public string RNCComprador { get; set; }
    public string RNCEmisor { get; set; }
    public string RazonSocialComprador { get; set; }
    public string RazonSocialEmisor { get; set; }
    public string DireccionComprador { get; set; }
    public string DireccionEmisor { get; set; }
    public string CorreoComprador { get; set; }
    public string CorreoEmisor { get; set; }
    public decimal TotalITBIS { get; set; }
    public string ENCF { get; set; }
    public string TipoECF { get; set; }
    public string TipoCuentaPago { get; set; }
    public int TipoPago { get; set; }
    public string DgiiQrCodeUrl { get; set; }
    public string DgiiReceivedDate { get; set; }
    public string DgiiResponseCode { get; set; }
    public string DgiiSecurityCode { get; set; }
    public string DgiiSignatureDate { get; set; }
    public string DgiiUsedSequence { get; set; }
    public string Status { get; set; }
    public DateTime FechaEmision { get; set; }
    public string StatusFomatted => string.Equals(Status, "000", StringComparison.CurrentCultureIgnoreCase) ? "Aceptado" : "Rechazado";
}
