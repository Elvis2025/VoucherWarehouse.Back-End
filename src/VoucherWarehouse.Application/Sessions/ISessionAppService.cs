using Abp.Application.Services;
using IBS.VoucherWarehouse.Sessions.Dto;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
