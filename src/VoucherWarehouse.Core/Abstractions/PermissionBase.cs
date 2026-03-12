using Abp.Authorization;
using Abp.Localization;

namespace IBS.VoucherWarehouse.Authorization.Abstractions;

public abstract class PermissionBase
{
    public abstract void Set(IPermissionDefinitionContext context);
    public ILocalizableString L(string name)
    {
        return new LocalizableString(name, VoucherWarehouseConsts.LocalizationSourceName);
    }
}
