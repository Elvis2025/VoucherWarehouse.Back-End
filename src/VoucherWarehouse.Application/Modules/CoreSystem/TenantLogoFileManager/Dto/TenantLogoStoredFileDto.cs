namespace VoucherWarehouse.Modules.CoreSystem.TenantLogoFileManager.Dto;

public sealed record class TenantLogoStoredFileDto
{
    public string RelativePath { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
}
