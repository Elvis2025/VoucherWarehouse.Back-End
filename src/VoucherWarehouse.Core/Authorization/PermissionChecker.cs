using Abp.Authorization;
using IBS.VoucherWarehouse.Authorization.Roles;
using IBS.VoucherWarehouse.Authorization.Users;

namespace IBS.VoucherWarehouse.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
