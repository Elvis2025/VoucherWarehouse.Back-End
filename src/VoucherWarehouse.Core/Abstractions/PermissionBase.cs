using Abp.Authorization;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoucherWarehouse;

namespace VoucherWarehouse.Authorization.Abstractions;

public abstract class PermissionBase
{
    public abstract void Set(IPermissionDefinitionContext context);
    public ILocalizableString L(string name)
    {
        return new LocalizableString(name, VoucherWarehouseConsts.LocalizationSourceName);
    }
}
