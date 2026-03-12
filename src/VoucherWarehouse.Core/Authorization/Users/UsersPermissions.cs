using Abp.Authorization;
using Abp.Localization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoucherWarehouse.Authorization;
using VoucherWarehouse.Authorization.Abstractions;

namespace VoucherWarehouse.Authorization.Users;

public sealed class UsersPermissions : PermissionBase
{

    private static readonly Lazy<UsersPermissions> _instance =
       new Lazy<UsersPermissions>(() => new UsersPermissions());

    public static UsersPermissions Instance => _instance.Value;

    private UsersPermissions() { }

    public override void Set(IPermissionDefinitionContext context)
    {
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        var users = context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        users.CreateChildPermission(PermissionNames.Pages_Users_Create, L("UsersCreate"));
        users.CreateChildPermission(PermissionNames.Pages_Users_Edit, L("UsersEdit"));
        users.CreateChildPermission(PermissionNames.Pages_Users_Delete, L("UsersDelete"));
        users.CreateChildPermission(PermissionNames.Pages_Users_AssignmentRole, L("UsersAssignmentRole"));
        users.CreateChildPermission(PermissionNames.Pages_Users_ChangePassword, L("UsersChangePassword"));
        users.CreateChildPermission(PermissionNames.Pages_Users_ResetPassword, L("UsersResetPassword"));

    }
}
