using Abp.Domain.Entities;

namespace IBS.VoucherWarehouse.Features.Dto;

public sealed record class GetTenantFeaturesInputDto : IMayHaveTenant
{
    public int? TenantId { get; set; }
}
