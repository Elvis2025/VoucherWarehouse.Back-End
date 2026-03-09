using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using VoucherWarehouse.Authorization;

namespace VoucherWarehouse;

[DependsOn(
    typeof(VoucherWarehouseCoreModule),
    typeof(AbpAutoMapperModule))]
public class VoucherWarehouseApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<VoucherWarehouseAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(VoucherWarehouseApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
