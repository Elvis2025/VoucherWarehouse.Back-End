namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class CommercialApprovalEcfInputDto
{
    public DetalleAprobacionComercial detalleAprobacionComercial { get; set; }
}

public sealed record class DetalleAprobacionComercial
{
    [Required]
    public string rNCEmisor { get; set; }
    [Required]
    [StringLength(13)]
    public string eNCF { get; set; }
    [Required]
    [StringLength(10)]
    public string fechaEmision { get; set; }
    [Required]
    public decimal montoTotal { get; set; }
    [Required]
    [StringLength(13)]
    public string rNCComprador { get; set; }
    [Required]
    public int Estado { get; set; }
    public string detalleMotivoRechazo { get; set; }
    [Required]
    [StringLength(19)]
    public string fechaHoraAprobacionComercial { get; set; }
}
