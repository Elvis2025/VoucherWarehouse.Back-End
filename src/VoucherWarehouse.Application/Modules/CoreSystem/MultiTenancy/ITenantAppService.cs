using Abp.Application.Services;

namespace IBS.VoucherWarehouse.Modules.CoreSystem.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

