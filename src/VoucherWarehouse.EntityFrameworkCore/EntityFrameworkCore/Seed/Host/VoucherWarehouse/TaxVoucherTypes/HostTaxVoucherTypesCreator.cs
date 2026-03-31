using Abp.MultiTenancy;
using IBS.VoucherWarehouse.Common.Enums;
using IBS.VoucherWarehouse.Extensions;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes;
using IBS.VoucherWarehouse.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IBS.VoucherWarehouse.EntityFrameworkCore.Seed.Host.VoucherWarehouse.TaxVoucherTypes;

public class HostTaxVoucherTypesCreator
{
    private readonly VoucherWarehouseDbContext context;
    private readonly AbpTenantBase tenantBase;
    public static IEnumerable<TaxVouchersTypes> TaxVouchersTypes = Enum.GetValues<VoucherType>()
                                                                    .Select(x => new TaxVouchersTypes()
                                                                    {
                                                                        Code = x.GetCode(),
                                                                        Name = x.GetName(),
                                                                        IsActive = true,
                                                                    });
    public HostTaxVoucherTypesCreator(VoucherWarehouseDbContext context, AbpTenantBase tenantBase)
    {
        this.context = context;
        this.tenantBase = tenantBase;
    }

    public void Create()
    {
        DefaultTaxVoucherTypes();
    }

    private void DefaultTaxVoucherTypes()
    {
        context.TaxVouchersTypes.AddOrSetValuesRange(
            entities: TaxVouchersTypes,
            tenant: tenantBase,
            filter: x => x.Code ,
            context: context);

        context.SaveChanges();
    }


    /*
     
     TipoPagos
        Contado  1
        Crédito  2
        Gratuito 3

        FormasPago
        1: Efectivo
        2: Cheque/Transferencia/Depósito 
        3: Tarjeta de Débito/Crédito
        4: Venta a Crédito
        5: Bonos o Certificados de regalo -- el e-CF debe ser  tipo E32 si o si
        6: Permuta
        7: Nota de crédito
        8: Otras Formas de pago
     */

}
