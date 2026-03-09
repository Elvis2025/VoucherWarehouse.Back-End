using Abp.MultiTenancy;
using VoucherWarehouse.Authorization.Users;

namespace VoucherWarehouse.MultiTenancy;

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
