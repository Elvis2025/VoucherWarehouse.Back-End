using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VoucherWarehouse.TenantBranding.Dto;

namespace VoucherWarehouse.TenantBranding;

public interface ITenantBrandingAppService : IApplicationService
{
    Task<TenantBrandingDto> GetCurrentTenantBrandingLogoByTenantId(int tenantId);
    Task<TenantBrandingDto> GetCurrentTenantLogoAsync();
    Task RemoveCurrentTenantLogoAsync();
    Task<TenantBrandingDto> UploadLogoAsync([FromForm] UploadTenantLogoRequestDto input);
}
