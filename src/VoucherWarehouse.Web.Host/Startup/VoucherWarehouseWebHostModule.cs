using Abp.Modules;
using Abp.Reflection.Extensions;
using VoucherWarehouse.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace VoucherWarehouse.Web.Host.Startup
{
    [DependsOn(
       typeof(VoucherWarehouseWebCoreModule))]
    public class VoucherWarehouseWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public VoucherWarehouseWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(VoucherWarehouseWebHostModule).GetAssembly());
        }
    }
}
