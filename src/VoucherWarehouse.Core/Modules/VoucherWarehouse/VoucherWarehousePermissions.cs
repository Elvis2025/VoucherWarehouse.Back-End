using Abp.Authorization;
using IBS.VoucherWarehouse.Authorization.Abstractions;
using System;
using static IBS.VoucherWarehouse.Modules.VoucherWarehouse.VoucherWarehouseNamePermissions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse;

public class VoucherWarehousePermissions : PermissionBase
{
    private static readonly Lazy<VoucherWarehousePermissions> instance = 
        new Lazy<VoucherWarehousePermissions>(() => new());

    public static readonly VoucherWarehousePermissions Instance = instance.Value;

    public override void Set(IPermissionDefinitionContext context)
    {
        var voucherWarehouse = context.CreatePermission(Module, L("VoucherWarehouse"));

        var ecfVoucherWarehouse = voucherWarehouse.CreateChildPermission(EcfVoucherWarehouse.Default, L("EcfApiAuthentication"));
            ecfVoucherWarehouse.CreateChildPermission(EcfVoucherWarehouse.Create, L("Create"));
            ecfVoucherWarehouse.CreateChildPermission(EcfVoucherWarehouse.Read, L("Read"));
            ecfVoucherWarehouse.CreateChildPermission(EcfVoucherWarehouse.Update, L("Update"));
            ecfVoucherWarehouse.CreateChildPermission(EcfVoucherWarehouse.Delete, L("Delete"));

        var ecfApiAuthentication = voucherWarehouse.CreateChildPermission(EcfApiAuthentication.Default, L("EcfApiAuthentication"));
            ecfApiAuthentication.CreateChildPermission(EcfApiAuthentication.Create, L("Create"));
            ecfApiAuthentication.CreateChildPermission(EcfApiAuthentication.Read, L("Read"));
            ecfApiAuthentication.CreateChildPermission(EcfApiAuthentication.Update, L("Update"));
            ecfApiAuthentication.CreateChildPermission(EcfApiAuthentication.Delete, L("Delete"));

        var taxVouchers = voucherWarehouse.CreateChildPermission(TaxVouchers.Default, L("TaxVouchers"));
            taxVouchers.CreateChildPermission(TaxVouchers.Create, L("Create"));
            taxVouchers.CreateChildPermission(TaxVouchers.Read, L("Read"));
            taxVouchers.CreateChildPermission(TaxVouchers.Update, L("Update"));
            taxVouchers.CreateChildPermission(TaxVouchers.Delete, L("Delete"));

        var taxVoucherTypes = voucherWarehouse.CreateChildPermission(TaxVouchersTypes.Default, L("TaxVoucherTypes"));
            taxVoucherTypes.CreateChildPermission(TaxVouchersTypes.Create, L("Create"));
            taxVoucherTypes.CreateChildPermission(TaxVouchersTypes.Read, L("Read"));
            taxVoucherTypes.CreateChildPermission(TaxVouchersTypes.Update, L("Update"));
            taxVoucherTypes.CreateChildPermission(TaxVouchersTypes.Delete, L("Delete"));


    }
}
