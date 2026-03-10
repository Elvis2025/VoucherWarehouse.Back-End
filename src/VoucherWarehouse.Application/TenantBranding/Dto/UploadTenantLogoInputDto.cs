using Microsoft.AspNetCore.Http;

namespace VoucherWarehouse.TenantBranding.Dto;

public sealed record class UploadTenantLogoInputDto
{
    public int TenantId { get; set; }
    public IFormFile File { get; set; }
    public string LogoFileName { get; set; }
    public string LogoContentType { get; set; }
    public string CompanyName { get; set; }
    public string CompanyType { get; set; }
    public string CompanyDescription { get; set; }
    public long? LogoSize { get; set; }
}
