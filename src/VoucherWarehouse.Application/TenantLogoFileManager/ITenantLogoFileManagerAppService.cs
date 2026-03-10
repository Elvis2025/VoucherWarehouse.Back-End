using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using VoucherWarehouse.TenantLogoFileManager.Dto;

namespace VoucherWarehouse.TenantLogoFileManager;

public interface ITenantLogoFileManagerAppService
{
    Task DeleteAsync(string relativePath);
    Task<TenantLogoStoredFileDto> SaveAsync(int tenantId, IFormFile file);
}
