using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using VoucherWarehouse.EntityFrameworkCore.Seed;

namespace VoucherWarehouse.EntityFrameworkCore;

[DependsOn(
    typeof(VoucherWarehouseCoreModule),
    typeof(AbpZeroCoreEntityFrameworkCoreModule))]
public class VoucherWarehouseEntityFrameworkModule : AbpModule
{
    /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
    public bool SkipDbContextRegistration { get; set; }

    public bool SkipDbSeed { get; set; }

    public override void PreInitialize()
    {
        if (!SkipDbContextRegistration)
        {
            Configuration.Modules.AbpEfCore().AddDbContext<VoucherWarehouseDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    VoucherWarehouseDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    VoucherWarehouseDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                }
            });
        }
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(VoucherWarehouseEntityFrameworkModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        if (!SkipDbSeed)
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}
