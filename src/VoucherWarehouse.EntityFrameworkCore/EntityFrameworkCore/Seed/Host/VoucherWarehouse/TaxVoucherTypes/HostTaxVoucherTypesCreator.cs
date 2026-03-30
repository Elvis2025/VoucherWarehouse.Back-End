using Abp.MultiTenancy;
using IBS.VoucherWarehouse.Extensions;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes;

namespace IBS.VoucherWarehouse.EntityFrameworkCore.Seed.Host.VoucherWarehouse.TaxVoucherTypes;

public class HostTaxVoucherTypesCreator
{
    private readonly VoucherWarehouseDbContext context;
    private readonly AbpTenantBase tenantBase;

    public HostTaxVoucherTypesCreator(VoucherWarehouseDbContext context, AbpTenantBase tenantBase)
    {
        this.context = context;
        this.tenantBase = tenantBase;
    }

    public void Create()
    {
        CreateHostTaxVoucherTypes();
    }

    private void CreateHostTaxVoucherTypes()
    {
        context.TaxVouchersTypes.AddOrSetValuesRange(
            entities: DefaultTaxVoucherTypes.TaxVouchersTypes,
            tenant: tenantBase,
            filter: x => x.Code ,
            context: context);

        context.SaveChanges();
    }

}
