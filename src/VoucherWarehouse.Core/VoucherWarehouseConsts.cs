using IBS.VoucherWarehouse.Debugging;

namespace IBS.VoucherWarehouse;

public class VoucherWarehouseConsts
{
    public const string LocalizationSourceName = "VoucherWarehouse";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "3105608d742f431d97bacdb1af8f4805";
}
