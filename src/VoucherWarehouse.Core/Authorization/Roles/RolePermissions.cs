using Abp.Authorization;
using IBS.VoucherWarehouse.Authorization.Abstractions;
using System;

namespace IBS.VoucherWarehouse.Authorization.Roles;

public class RolePermissions : PermissionBase
{
    private static readonly Lazy<RolePermissions> _instance =
      new Lazy<RolePermissions>(() => new RolePermissions());

    public static RolePermissions Instance => _instance.Value;

    private RolePermissions() { }


    public override void Set(IPermissionDefinitionContext context)
    {

        var roles = context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        roles.CreateChildPermission(PermissionNames.Pages_Roles_Create, L("RolesCreate"));
        roles.CreateChildPermission(PermissionNames.Pages_Roles_Edit, L("RolesEdit"));
        roles.CreateChildPermission(PermissionNames.Pages_Roles_Delete, L("RolesDelete"));
    }


}
