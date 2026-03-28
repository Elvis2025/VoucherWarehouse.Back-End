using IBS.VoucherWarehouse.Extensions;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.EntityFrameworkCore.Seed.Host.VoucherWarehouse.TaxVoucherTypes;

public class HostTaxVoucherTypesCreator
{
    private readonly VoucherWarehouseDbContext context;

    public HostTaxVoucherTypesCreator(VoucherWarehouseDbContext context) => this.context = context;
    

    public void Create()
    {
        CreateHostTaxVoucherTypes();
    }

    private void CreateHostTaxVoucherTypes()
    {
        context.TaxVouchersTypes.AddOrSetValuesRange(DefaultTaxVoucherTypes.TaxVouchersTypes,x => x.Code, context);
        context.SaveChanges();
    }

}
