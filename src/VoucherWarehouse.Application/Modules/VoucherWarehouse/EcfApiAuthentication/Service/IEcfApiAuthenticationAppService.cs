using IBS.VoucherWarehouse.Common.Mapping.Abstractions;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;

public interface IEcfApiAuthenticationAppService : IIbsAsyncCrudAppService<EcfApiAuthenticationOutputDto, int, EcfApiAuthenticationInputDto, EcfApiAuthenticationCreateDto, EcfApiAuthenticationUpdateDto>
{
    Task<AuthenticationResponseOutputDto> AuthenticateAPIAsync(LoginInputDto loginViewModel);
    Task<AuthenticateInputDto> GetEcfUserAuthenticationAsync();
}
