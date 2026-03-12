using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoucherWarehouse.Modules.CoreSystem;

public static class CoreSystemNamePermissions
{
    public const string GroupName = "CoreSystem";

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string Error = Default + ".Error";
        public const string ServiceName = Default + ".ServiceName";
        public const string Dashboard = Default + ".Dashboard";
        public const string ShowDetails = Default + ".ShowDetails";
    }




}