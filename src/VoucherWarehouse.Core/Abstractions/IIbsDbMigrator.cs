using Abp.Dependency;
using Abp.MultiTenancy;

namespace IBS.VoucherWarehouse.Abstractions;

public interface IIbsDbMigrator: ITransientDependency
{
    void CreateOrMigrateForHostByTenant(AbpTenantBase tenant);
}
