using Abp.Application.Features;
using Abp.Localization;

namespace IBS.VoucherWarehouse.Authorization.Abstractions;

public abstract class FeatureBase
{
    public abstract void Set(IFeatureDefinitionContext context);

    public ILocalizableString L(string name)
    {
        return new LocalizableString(name, VoucherWarehouseConsts.LocalizationSourceName);
    }
}
