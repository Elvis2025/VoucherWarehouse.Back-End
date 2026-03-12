using Abp.Domain.Entities;

namespace VoucherWarehouse.Features.Dto;

public sealed record class GetTenantFeaturesInputDto : IMayHaveTenant
{
    public int? TenantId { get; set; }
}
