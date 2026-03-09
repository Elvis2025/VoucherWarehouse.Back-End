using Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace VoucherWarehouse.Authorization.Accounts.Dto;

public class IsTenantAvailableInput
{
    [Required]
    [StringLength(AbpTenantBase.MaxTenancyNameLength)]
    public string TenancyName { get; set; }
}
