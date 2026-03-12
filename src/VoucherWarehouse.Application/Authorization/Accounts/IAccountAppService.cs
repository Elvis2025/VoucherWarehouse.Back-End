using Abp.Application.Services;
using IBS.VoucherWarehouse.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
