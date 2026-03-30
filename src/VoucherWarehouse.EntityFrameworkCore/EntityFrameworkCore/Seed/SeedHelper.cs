using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using IBS.VoucherWarehouse.Common.Services;
using IBS.VoucherWarehouse.EntityFrameworkCore.Seed.Host;
using IBS.VoucherWarehouse.EntityFrameworkCore.Seed.Host.VoucherWarehouse.TaxVoucherTypes;
using IBS.VoucherWarehouse.EntityFrameworkCore.Seed.Tenants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Transactions;

namespace IBS.VoucherWarehouse.EntityFrameworkCore.Seed;

public static class SeedHelper
{
    public static void SeedHostDb(IIocResolver iocResolver)
    {
        WithDbContext<VoucherWarehouseDbContext>(iocResolver, SeedHostDb);
     
    }

    public static void SeedHostDb(VoucherWarehouseDbContext context)
    {
        context.SuppressAutoSetTenantId = true;

        // Host seed
        new InitialHostDbBuilder(context).Create();

        // Default tenant seed (in host database).
        new DefaultTenantBuilder(context).Create();
        new TenantRoleAndUserBuilder(context, 1).Create();
       // SeedHostDbByTenant(context);
    }

    public static void SeedHostDbByTenant(VoucherWarehouseDbContext context, AbpTenantBase tenantBase)
    {
        context.SuppressAutoSetTenantId = true;

        new HostTaxVoucherTypesCreator(context, tenantBase).Create();
    }

    private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
        where TDbContext : DbContext
    {
        using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
        {
            using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
            {
                var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                contextAction(context);

                uow.Complete();
            }
        }
    }
}
