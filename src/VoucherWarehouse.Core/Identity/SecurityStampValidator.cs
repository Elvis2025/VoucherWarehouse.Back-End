using Abp.Authorization;
using Abp.Domain.Uow;
using IBS.VoucherWarehouse.Authorization.Roles;
using IBS.VoucherWarehouse.Authorization.Users;
using IBS.VoucherWarehouse.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IBS.VoucherWarehouse.Identity;

public class SecurityStampValidator : AbpSecurityStampValidator<Tenant, Role, User>
{
    public SecurityStampValidator(
        IOptions<SecurityStampValidatorOptions> options,
        SignInManager signInManager,
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager)
        : base(options, signInManager, loggerFactory, unitOfWorkManager)
    {
    }
}
