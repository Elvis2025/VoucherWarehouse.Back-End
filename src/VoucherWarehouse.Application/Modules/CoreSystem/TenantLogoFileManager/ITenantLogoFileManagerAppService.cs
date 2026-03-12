using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using IBS.VoucherWarehouse.Modules.CoreSystem.TenantLogoFileManager.Dto;

namespace IBS.VoucherWarehouse.Modules.CoreSystem.TenantLogoFileManager;

public interface ITenantLogoFileManagerAppService
{
    Task DeleteAsync(string relativePath);
    Task<TenantLogoStoredFileDto> SaveAsync(int tenantId, IFormFile file);
}
