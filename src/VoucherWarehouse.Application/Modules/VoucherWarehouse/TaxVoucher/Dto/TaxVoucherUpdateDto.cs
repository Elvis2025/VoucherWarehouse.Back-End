using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;

public sealed record class TaxVoucherUpdateDto : BaseCreateOrUpdateEntityDto<int>
{
    public string Comment { get; set; }
    public int InitialSequence { get; set; }
    public int CurrentSequence { get; set; }
    public int FinalSequence { get; set; }
    public int RegisteredQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int MinimumToAlert { get; set; }
    public DateTime ExpeditionDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    [Required]
    public int TaxVoucherTypeId { get; set; }
}
