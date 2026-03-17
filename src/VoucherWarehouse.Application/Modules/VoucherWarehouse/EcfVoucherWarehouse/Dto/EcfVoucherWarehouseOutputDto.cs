using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

public sealed record class EcfVoucherWarehouseOutputDto : BaseEntityDto<int>
{
    public string Status { get; set; }
}
