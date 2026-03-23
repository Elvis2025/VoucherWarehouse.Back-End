using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;

public sealed record class TaxVoucherTypesOutputDto : BaseEntityDto<int>
{
    [StringLength(5)]
    [Required]
    public string Code { get; set; }
    [StringLength(100)]
    [Required]
    public string Description { get; set; }
    public string CodeAndDescription => Code + " - " + Description;
    public int TaxVoucherLenght { get; set; }
    public string Format { get; set; }
}
