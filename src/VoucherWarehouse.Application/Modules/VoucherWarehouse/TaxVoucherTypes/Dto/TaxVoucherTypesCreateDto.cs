namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;

public sealed record class TaxVoucherTypesCreateDto : IEntityDto<int>, IPassivable
{
    [StringLength(5)]
    public string Code { get; set; }
    [StringLength(100)]
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int Id { get; set; }
}
