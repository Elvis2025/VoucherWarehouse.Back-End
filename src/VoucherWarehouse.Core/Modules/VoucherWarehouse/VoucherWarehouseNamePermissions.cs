namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse;

public static class VoucherWarehouseNamePermissions
{
    public const string Module = nameof(VoucherWarehouse);

    public static class EcfApiAuthentication
    {
        public const string Default = $"{Module}.{nameof(EcfApiAuthentication)}";
        public const string Create = $"{Default}{PermissionCRUD.Create}";
        public const string Read = $"{Default}{PermissionCRUD.Read}";
        public const string Update = $"{Default}{PermissionCRUD.Update}";
        public const string Delete = $"{Default}{PermissionCRUD.Delete}";
    }

    public static class TaxVouchers
    {
        public const string Default = $"{Module}.{nameof(TaxVouchers)}";
        public const string Create = $"{Default}{PermissionCRUD.Create}";
        public const string Read = $"{Default}{PermissionCRUD.Read}";
        public const string Update = $"{Default}{PermissionCRUD.Update}";
        public const string Delete = $"{Default}{PermissionCRUD.Delete}";
    }

    public static class TaxVouchersTypes
    {
        public const string Default = $"{Module}.{nameof(TaxVouchersTypes)}";
        public const string Create  = $"{Default}{PermissionCRUD.Create}";
        public const string Read    = $"{Default}{PermissionCRUD.Read}";
        public const string Update  = $"{Default}{PermissionCRUD.Update}";
        public const string Delete  = $"{Default}{PermissionCRUD.Delete}";
    }

    public static class  EcfVoucherWarehouse
    {
        public const string Default = $"{Module}.{nameof(EcfVoucherWarehouse)}";
        public const string Create = $"{Default}{PermissionCRUD.Create}";
        public const string Read = $"{Default}{PermissionCRUD.Read}";
        public const string Update = $"{Default}{PermissionCRUD.Update}";
        public const string Delete = $"{Default}{PermissionCRUD.Delete}";
    }

    public static class  EcfQueries
    {
        public const string Default = $"{Module}.{nameof(EcfQueries)}";
        public const string Read = $"{Default}{PermissionCRUD.Read}";
    }


}
