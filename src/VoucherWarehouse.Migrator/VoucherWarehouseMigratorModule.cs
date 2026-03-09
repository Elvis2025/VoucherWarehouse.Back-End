using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using VoucherWarehouse.Configuration;
using VoucherWarehouse.EntityFrameworkCore;
using VoucherWarehouse.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace VoucherWarehouse.Migrator;

[DependsOn(typeof(VoucherWarehouseEntityFrameworkModule))]
public class VoucherWarehouseMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public VoucherWarehouseMigratorModule(VoucherWarehouseEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(VoucherWarehouseMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            VoucherWarehouseConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(VoucherWarehouseMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
