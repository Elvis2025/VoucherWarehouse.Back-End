using Abp.Authorization;
using System;
using VoucherWarehouse.Authorization.Abstractions;
using static VoucherWarehouse.Modules.CoreSystem.CoreSystemNamePermissions;

namespace VoucherWarehouse.Modules.CoreSystem;

public sealed class CoreSystemPermissions : PermissionBase
{
    private static readonly Lazy<CoreSystemPermissions> _instance =
       new Lazy<CoreSystemPermissions>(() => new CoreSystemPermissions());

    public static CoreSystemPermissions Instance => _instance.Value;

    private CoreSystemPermissions() { }



    public override void Set(IPermissionDefinitionContext context)
    {

        var coreSystem = context.CreatePermission(GroupName, L("CoreSystem"));

        var auditLogs = coreSystem.CreateChildPermission(AuditLogs.Default, L("AuditLogs"));
        auditLogs.CreateChildPermission(AuditLogs.Error, L("AuditLogsError"));
        auditLogs.CreateChildPermission(AuditLogs.ServiceName, L("AuditLogsServiceName"));
        auditLogs.CreateChildPermission(AuditLogs.ShowDetails, L("AuditLogsShowDetails"));
        auditLogs.CreateChildPermission(AuditLogs.Dashboard, L("AuditLogsDashboard"));


    }
}
