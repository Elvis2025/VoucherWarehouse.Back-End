using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.MultiTenancy;

[Table("TenantBrandings")]
public class TenantBranding : FullAuditedEntity<long>
{
    public const int MaxLogoPathLength = 500;
    public const int MaxLogoFileNameLength = 260;
    public const int MaxLogoContentTypeLength = 100;

    public int TenantId { get; set; }

    [Required]
    [StringLength(MaxLogoPathLength)]
    public string LogoPath { get; set; }

    [StringLength(MaxLogoFileNameLength)]
    public string LogoFileName { get; set; }

    [StringLength(MaxLogoContentTypeLength)]
    public string LogoContentType { get; set; }

    [StringLength(MaxLogoContentTypeLength)]
    public string CompanyName { get; set; }

    [StringLength(MaxLogoContentTypeLength)]
    public string CompanyType { get; set; }

    [StringLength(MaxLogoContentTypeLength)]
    public string CompanyDescription { get; set; }

    public long? LogoSize { get; set; }
}