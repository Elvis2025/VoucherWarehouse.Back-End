using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;

public sealed record class TaxVoucherCreateDto : IPassivable
{
    public bool IsActive { get; set; }
    public string Status { get; set; }
    [Required]
    public int TaxVoucherTypeId { get; set; }
}
