using Abp.Auditing;
using IBS.VoucherWarehouse.Common.Constants;
using IBS.VoucherWarehouse.Sessions.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Sessions;

public class SessionAppService : VoucherWarehouseAppServiceBase, ISessionAppService
{
    [DisableAuditing]
    public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
    {
        var output = new GetCurrentLoginInformationsOutput
        {
            Application = new ApplicationInfoDto
            {
                Version = AppVersionHelper.Version,
                ReleaseDate = AppVersionHelper.ReleaseDate,
                Features = new Dictionary<string, bool>()
            }
        };

        if (AbpSession.TenantId.HasValue)
        {
            output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            IbsSession.Tenant = output.Tenant;
        }

        if (AbpSession.UserId.HasValue)
        {
            output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            IbsSession.User = output.User;
        }
        IbsSession.TenantId = AbpSession.TenantId;
        return output;
    }
}
