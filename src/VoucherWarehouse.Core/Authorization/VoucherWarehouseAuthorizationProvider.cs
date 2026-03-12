using Abp.Authorization;
using IBS.VoucherWarehouse.Authorization.Roles;
using IBS.VoucherWarehouse.Authorization.Users;
using IBS.VoucherWarehouse.Modules.CoreSystem;
using IBS.VoucherWarehouse.MultiTenancy;

namespace IBS.VoucherWarehouse.Authorization;

public class VoucherWarehouseAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        TenantPermissions.Instance.Set(context);
        UsersPermissions.Instance.Set(context);
        RolePermissions.Instance.Set(context);
        CoreSystemPermissions.Instance.Set(context);
    }

   
}
