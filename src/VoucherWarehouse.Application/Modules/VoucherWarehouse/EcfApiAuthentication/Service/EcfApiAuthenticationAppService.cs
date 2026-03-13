
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Mappers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;

public class EcfApiAuthenticationAppService : IEcfApiAuthenticationAppService
{
    private readonly IRepository<Models.EcfApiAuthentication, int> ecfApiAuthenticationRepository;
    private readonly EcfApiAuthenticationMapping ecfApiAuthenticationMapping;

    public EcfApiAuthenticationAppService(IRepository<Models.EcfApiAuthentication, int> ecfApiAuthenticationRepository, EcfApiAuthenticationMapping ecfApiAuthenticationMapping)
    {
        this.ecfApiAuthenticationRepository = ecfApiAuthenticationRepository;
        this.ecfApiAuthenticationMapping = ecfApiAuthenticationMapping;
    }

    public async Task<AuthenticateInputDto> GetEcfUserAuthenticationAsync()
    {
        var ecfApiAuthentication = await ecfApiAuthenticationRepository.GetAll().FirstOrDefaultAsync();
        if (ecfApiAuthentication == null)
        {
            throw new UserFriendlyException("Ecf API Authentication details not found.");
        }

        return new()
        {
            AuthenticateUrlIbsApiDgii = ecfApiAuthentication.AuthUrl,
            BaseUrlIbsApiDgii = ecfApiAuthentication.BaseUrl,
            Password = ecfApiAuthentication.Password,
            TenancyName = ecfApiAuthentication.TenancyName,
            UsernameOrEmailAddress = ecfApiAuthentication.UsernameOrEmailAddress



        };
    }
}
