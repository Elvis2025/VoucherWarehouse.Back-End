using Abp.Application.Services;
using VoucherWarehouse.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace VoucherWarehouse.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
