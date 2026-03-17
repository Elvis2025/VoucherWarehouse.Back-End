namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse;

public static class VoucherWarehouseNamePermissions
{
    public const string Module = nameof(VoucherWarehouse);

    public static class EcfApiAuthentication
    {
        public const string Default = $"{Module}.{nameof(Models.EcfApiAuthentication)}";
        public const string Create = $"{Default}.{PermissionCRUD.Create}";
        public const string Read = $"{Default}.{PermissionCRUD.Read}";
        public const string Update = $"{Default}.{PermissionCRUD.Update}";
        public const string Delete = $"{Default}.{PermissionCRUD.Delete}";
    }

    public static class TaxVouchers
    {
        public const string Default = $"{Module}.{nameof(Models.TaxVouchers)}";
        public const string Create = $"{Default}.{PermissionCRUD.Create}";
        public const string Read = $"{Default}.{PermissionCRUD.Read}";
        public const string Update = $"{Default}.{PermissionCRUD.Update}";
        public const string Delete = $"{Default}.{PermissionCRUD.Delete}";
    }

    public static class TaxVouchersTypes
    {
        public const string Default = $"{Module}.{nameof(Models.TaxVouchersTypes)}";
        public const string Create = $"{Default}.{PermissionCRUD.Create}";
        public const string Read = $"{Default}.{PermissionCRUD.Read}";
        public const string Update = $"{Default}.{PermissionCRUD.Update}";
        public const string Delete = $"{Default}.{PermissionCRUD.Delete}";
    }

    public static class  EcfVoucherWarehouse
    {
        public const string Default = $"{Module}.{nameof(Models.EcfVoucherWarehouse)}";
        public const string Create = $"{Default}.{PermissionCRUD.Create}";
        public const string Read = $"{Default}.{PermissionCRUD.Read}";
        public const string Update = $"{Default}.{PermissionCRUD.Update}";
        public const string Delete = $"{Default}.{PermissionCRUD.Delete}";
    }


}
