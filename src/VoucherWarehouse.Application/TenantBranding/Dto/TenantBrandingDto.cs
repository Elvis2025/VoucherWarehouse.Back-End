namespace VoucherWarehouse.TenantBranding.Dto;

public sealed record class TenantBrandingDto
{
    public int TenantId { get; set; }
    public string LogoPath { get; set; }
    public string LogoFileName { get; set; }
    public string LogoContentType { get; set; }
    public string CompanyName { get; set; }
    public string CompanyType { get; set; }
    public string CompanyDescription { get; set; }
    public long? LogoSize { get; set; }
    public string FullLogoUrl { get; set; }
}
