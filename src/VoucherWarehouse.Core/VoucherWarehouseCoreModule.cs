using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using IBS.VoucherWarehouse.Authorization.Roles;
using IBS.VoucherWarehouse.Authorization.Users;
using IBS.VoucherWarehouse.Configuration;
using IBS.VoucherWarehouse.Features;
using IBS.VoucherWarehouse.Localization;
using IBS.VoucherWarehouse.MultiTenancy;
using IBS.VoucherWarehouse.Timing;

namespace IBS.VoucherWarehouse;

[DependsOn(typeof(AbpZeroCoreModule))]
public class VoucherWarehouseCoreModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Auditing.IsEnabledForAnonymousUsers = true;

        // Declare entity types
        Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
        Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
        Configuration.Modules.Zero().EntityTypes.User = typeof(User);

        VoucherWarehouseLocalizationConfigurer.Configure(Configuration.Localization);

        // Enable this line to create a multi-tenant application.
        Configuration.MultiTenancy.IsEnabled = VoucherWarehouseConsts.MultiTenancyEnabled;

        // Configure roles
        AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

        Configuration.Settings.Providers.Add<AppSettingProvider>();
        Configuration.Features.Providers.Add<AppFeatureProvider>();


        Configuration.Localization.Languages.Add(new LanguageInfo("fa", "فارسی", "famfamfam-flags ir"));

        Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = VoucherWarehouseConsts.DefaultPassPhrase;
        SimpleStringCipher.DefaultPassPhrase = VoucherWarehouseConsts.DefaultPassPhrase;
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(VoucherWarehouseCoreModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
    }
}
