using Abp.Authorization;
using Abp.Domain.Uow;
using IBS.VoucherWarehouse.Authorization.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IBS.VoucherWarehouse.Authorization.Users;

public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
{
    public UserClaimsPrincipalFactory(
        UserManager userManager,
        RoleManager roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        IUnitOfWorkManager unitOfWorkManager)
        : base(
              userManager,
              roleManager,
              optionsAccessor,
              unitOfWorkManager)
    {
    }
}
