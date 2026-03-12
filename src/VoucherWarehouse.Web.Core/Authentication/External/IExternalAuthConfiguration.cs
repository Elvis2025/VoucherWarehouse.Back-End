using System.Collections.Generic;

namespace IBS.VoucherWarehouse.Authentication.External;

public interface IExternalAuthConfiguration
{
    List<ExternalLoginProviderInfo> Providers { get; }
}
