namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;



public class ReceiveCreditNoteECFInputDto: ReceiveSalesEcfInputDto
{
    public InformacionReferencia informacionReferencia { get; set; }
}

public class InformacionReferencia
{
    public string nCFModificado { get; set; }
    public string rNCOtroContribuyente { get; set; }
    [StringLength(10)]
    public string fechaNCFModificado { get; set; }
    public int codigoModificacion { get; set; }
    [StringLength(90)]
    public string razonModificacion { get; set; }
}
