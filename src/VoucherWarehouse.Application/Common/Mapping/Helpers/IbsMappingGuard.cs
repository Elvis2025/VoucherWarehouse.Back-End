namespace IBS.VoucherWarehouse.Common.Mapping.Helpers;

public static class IbsMappingGuard
{
    public static void AgainstNull(object? value, string paramName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}