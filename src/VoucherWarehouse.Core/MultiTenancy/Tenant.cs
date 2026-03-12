using Abp.MultiTenancy;
using IBS.VoucherWarehouse.Authorization.Users;

namespace IBS.VoucherWarehouse.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
}
