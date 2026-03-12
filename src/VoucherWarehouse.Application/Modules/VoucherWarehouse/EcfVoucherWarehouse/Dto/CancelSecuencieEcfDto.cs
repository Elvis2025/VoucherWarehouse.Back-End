namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class CancelSecuencieEcfDto
{
    public string EcfType { get; set; }
    public string  SequenceEcfFrom { get; set; }
    public string SequenceEcfTo { get; set; }
    public int SequenceQuantity { get; set; }
}
