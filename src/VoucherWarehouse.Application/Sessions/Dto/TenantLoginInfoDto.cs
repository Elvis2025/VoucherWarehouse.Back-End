using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using IBS.VoucherWarehouse.MultiTenancy;

namespace IBS.VoucherWarehouse.Sessions.Dto;

[AutoMapFrom(typeof(Tenant))]
public class TenantLoginInfoDto : EntityDto
{
    public string TenancyName { get; set; }

    public string Name { get; set; }
}
