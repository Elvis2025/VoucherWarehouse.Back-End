using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using VoucherWarehouse.Modules.CoreSystem.TenantLogoFileManager.Dto;

namespace VoucherWarehouse.Modules.CoreSystem.TenantLogoFileManager;

public interface ITenantLogoFileManagerAppService
{
    Task DeleteAsync(string relativePath);
    Task<TenantLogoStoredFileDto> SaveAsync(int tenantId, IFormFile file);
}
