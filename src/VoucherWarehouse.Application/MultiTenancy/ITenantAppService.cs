using Abp.Application.Services;
using VoucherWarehouse.MultiTenancy.Dto;

namespace VoucherWarehouse.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

