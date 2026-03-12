using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace IBS.VoucherWarehouse.Controllers;

public abstract class VoucherWarehouseControllerBase : AbpController
{
    protected VoucherWarehouseControllerBase()
    {
        LocalizationSourceName = VoucherWarehouseConsts.LocalizationSourceName;
    }

    protected void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }
}
