using Abp.Data;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.MultiTenancy;
using Abp.Zero.EntityFrameworkCore;
using IBS.VoucherWarehouse.Abstractions;
using IBS.VoucherWarehouse.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IBS.VoucherWarehouse.EntityFrameworkCore;

public class IbsDbMigrator :  IIbsDbMigrator 
{
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IDbPerTenantConnectionStringResolver connectionStringResolver;
    private readonly AbpZeroDbMigrator abpZeroDbMigrator;
    private readonly IDbContextResolver dbContextResolver;

    public IbsDbMigrator(IUnitOfWorkManager unitOfWorkManager, 
                         IDbPerTenantConnectionStringResolver connectionStringResolver,
                         AbpZeroDbMigrator abpZeroDbMigrator ,
                         IDbContextResolver dbContextResolver) 
        
    {
        this.unitOfWorkManager = unitOfWorkManager;
        this.connectionStringResolver = connectionStringResolver;
        this.abpZeroDbMigrator = abpZeroDbMigrator;
        this.dbContextResolver = dbContextResolver;
    }


  
    public virtual void CreateOrMigrateForHostByTenant(AbpTenantBase tenant)
    {
        CreateOrMigrateForHostByTenant(tenant, SeedHelper.SeedHostDbByTenant);
    }


    public virtual void CreateOrMigrateForHostByTenant(AbpTenantBase tenant, Action<VoucherWarehouseDbContext, AbpTenantBase> seedAction)
    {
        CreateOrMigrate(tenant, seedAction);
    }

    private void CreateOrMigrate(AbpTenantBase tenant, Action<VoucherWarehouseDbContext, AbpTenantBase> seedAction)
    {
        var args = new DbPerTenantConnectionStringResolveArgs(
            tenant == null ? (int?)null : (int?)tenant.Id,
            tenant == null ? MultiTenancySides.Host : MultiTenancySides.Tenant
        );

        args["DbContextType"] = typeof(VoucherWarehouseDbContext);
        args["DbContextConcreteType"] = typeof(VoucherWarehouseDbContext);

        var nameOrConnectionString = ConnectionStringHelper.GetConnectionString(
            connectionStringResolver.GetNameOrConnectionString(args)
        );

        using (var uow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
        {

            using (var dbContext = dbContextResolver.Resolve<VoucherWarehouseDbContext>(nameOrConnectionString, null))
            {
                using (unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    dbContext.Database.Migrate();
                    seedAction?.Invoke(dbContext, tenant);

                    unitOfWorkManager.Current.SaveChanges();
                    uow.Complete();
                }
            }
        }
    }

}
