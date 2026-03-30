using Abp.Data;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.MultiTenancy;
using Abp.Zero.EntityFrameworkCore;
using IBS.VoucherWarehouse.Abstractions;
using IBS.VoucherWarehouse.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Transactions;

namespace IBS.VoucherWarehouse.EntityFrameworkCore;

public class AbpZeroDbMigrator : AbpZeroDbMigrator<VoucherWarehouseDbContext>
{
    public AbpZeroDbMigrator(
        IUnitOfWorkManager unitOfWorkManager,
        IDbPerTenantConnectionStringResolver connectionStringResolver,
        IDbContextResolver dbContextResolver)
        : base(
            unitOfWorkManager,
            connectionStringResolver,
            dbContextResolver)
    {
    }
}
