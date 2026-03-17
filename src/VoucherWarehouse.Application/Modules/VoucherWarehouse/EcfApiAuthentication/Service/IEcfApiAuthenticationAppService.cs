using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Service;

public interface IEcfApiAuthenticationAppService : ITransientDependency
{
    Task<AuthenticationResponseOutputDto> AuthenticateAPIAsync(LoginInputDto loginViewModel);
    Task<AuthenticateInputDto> GetEcfUserAuthenticationAsync();
}
