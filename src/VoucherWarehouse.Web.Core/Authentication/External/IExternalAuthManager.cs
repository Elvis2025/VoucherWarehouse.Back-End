using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Authentication.External;

public interface IExternalAuthManager
{
    Task<bool> IsValidUser(string provider, string providerKey, string providerAccessCode);

    Task<ExternalAuthUserInfo> GetUserInfo(string provider, string accessCode);
}
