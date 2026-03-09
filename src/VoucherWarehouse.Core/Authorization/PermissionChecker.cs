using Abp.Authorization;
using VoucherWarehouse.Authorization.Roles;
using VoucherWarehouse.Authorization.Users;

namespace VoucherWarehouse.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
