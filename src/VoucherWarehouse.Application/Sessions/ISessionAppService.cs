using Abp.Application.Services;
using VoucherWarehouse.Sessions.Dto;
using System.Threading.Tasks;

namespace VoucherWarehouse.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
