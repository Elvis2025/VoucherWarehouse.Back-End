using VoucherWarehouse.Configuration.Dto;
using System.Threading.Tasks;

namespace VoucherWarehouse.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
