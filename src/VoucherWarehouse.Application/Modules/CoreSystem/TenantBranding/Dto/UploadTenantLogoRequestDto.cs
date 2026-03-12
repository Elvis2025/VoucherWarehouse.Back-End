using Microsoft.AspNetCore.Http;

namespace IBS.VoucherWarehouse.Modules.CoreSystem.TenantBranding.Dto;

public sealed record class UploadTenantLogoRequestDto
{
    public IFormFile File { get; set; }

    public int TenantId { get; set; }

    public string CompanyDescription { get; set; }

    public string CompanyName { get; set; }

    public string CompanyType { get; set; }
}
