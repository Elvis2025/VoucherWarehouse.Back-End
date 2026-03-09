using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using VoucherWarehouse.EntityFrameworkCore;
using VoucherWarehouse.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace VoucherWarehouse.Web.Tests;

[DependsOn(
    typeof(VoucherWarehouseWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class VoucherWarehouseWebTestModule : AbpModule
{
    public VoucherWarehouseWebTestModule(VoucherWarehouseEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(VoucherWarehouseWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(VoucherWarehouseWebMvcModule).Assembly);
    }
}