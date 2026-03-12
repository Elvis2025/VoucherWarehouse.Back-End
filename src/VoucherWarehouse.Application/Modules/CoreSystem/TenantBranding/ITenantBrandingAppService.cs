using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VoucherWarehouse.Modules.CoreSystem.TenantBranding.Dto;

namespace VoucherWarehouse.Modules.CoreSystem.TenantBranding;

public interface ITenantBrandingAppService : IApplicationService
{
    Task<TenantBrandingDto> GetCurrentTenantBrandingLogoByTenantIdAsync(int tenantId);
    Task<TenantBrandingDto> GetCurrentTenantLogoAsync();
    Task RemoveCurrentTenantLogoAsync();
    Task<TenantBrandingDto> UploadLogoAsync(UploadTenantLogoInputDto inputDto);
}
