using Abp.Authorization;
using Abp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoucherWarehouse.Authorization;
using VoucherWarehouse.Authorization.Abstractions;

namespace VoucherWarehouse.MultiTenancy;

public sealed class TenantPermissions : PermissionBase
{
    private static readonly Lazy<TenantPermissions> _instance =
     new Lazy<TenantPermissions>(() => new TenantPermissions());

    public static TenantPermissions Instance => _instance.Value;

    private TenantPermissions() { }

    public override void Set(IPermissionDefinitionContext context)
    {
        var tenant = context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        tenant.CreateChildPermission(PermissionNames.Pages_Tenants_Edit, L("TenantEdit"), multiTenancySides: MultiTenancySides.Host);
        tenant.CreateChildPermission(PermissionNames.Pages_Tenants_Create, L("TenantCreate"), multiTenancySides: MultiTenancySides.Host);
        tenant.CreateChildPermission(PermissionNames.Pages_Tenants_Delete, L("TenantDelete"), multiTenancySides: MultiTenancySides.Host);
        tenant.CreateChildPermission(PermissionNames.Pages_Tenants_SetConnectionStrings, L("TenantsSetConnectionStrings"), multiTenancySides: MultiTenancySides.Host);
    }
}
