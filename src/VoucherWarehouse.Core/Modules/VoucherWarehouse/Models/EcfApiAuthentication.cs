using IBS.VoucherWarehouse.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Index(nameof(TenantId),IsUnique = true)]
[Table("EcfApiAuthentications")]

public class EcfApiAuthentication : BaseEntity<int>
{
    public string TenancyName { get; set; }
    public string UsernameOrEmailAddress { get; set; }
    public string Password { get; set; }
    public string AuthUrl { get; set; }
    public string BaseUrl { get; set; }

}
