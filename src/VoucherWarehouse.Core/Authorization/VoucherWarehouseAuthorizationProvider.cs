using Abp.Authorization;
using VoucherWarehouse.Authorization.Roles;
using VoucherWarehouse.Authorization.Users;
using VoucherWarehouse.Modules.CoreSystem;
using VoucherWarehouse.MultiTenancy;

namespace VoucherWarehouse.Authorization;

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
