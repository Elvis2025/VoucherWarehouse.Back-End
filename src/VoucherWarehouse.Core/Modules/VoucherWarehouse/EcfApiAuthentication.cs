using IBS.VoucherWarehouse.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse;

[Index(nameof(TenantId),IsUnique = true)]

public class EcfApiAuthentication : BaseEntity<int>
{
    public string TenancyName { get; set; }
    public string UsernameOrEmailAddress { get; set; }
    public string Password { get; set; }
    public string AuthUrl { get; set; }
    public string BaseUrl { get; set; }

}
