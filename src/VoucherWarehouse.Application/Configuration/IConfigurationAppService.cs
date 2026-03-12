using IBS.VoucherWarehouse.Configuration.Dto;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
