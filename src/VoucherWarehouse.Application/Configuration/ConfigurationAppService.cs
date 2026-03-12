using Abp.Authorization;
using Abp.Runtime.Session;
using IBS.VoucherWarehouse.Configuration.Dto;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : VoucherWarehouseAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
