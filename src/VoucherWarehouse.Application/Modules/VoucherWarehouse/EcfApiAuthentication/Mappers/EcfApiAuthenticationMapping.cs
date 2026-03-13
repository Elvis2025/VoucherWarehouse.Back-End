#pragma warning disable RMG012, S2094,RMG089
using IBS.VoucherWarehouse.Common.Mapping.Base;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Mappers;

public class EcfApiAuthenticationMapping : IbsTwoWayMapperBase<Models.EcfApiAuthentication, EcfApiAuthenticationDto>
{
    protected override EcfApiAuthenticationDto CreateDestination(Models.EcfApiAuthentication source) => new EcfApiAuthenticationDto();

    protected override Models.EcfApiAuthentication CreateSource(EcfApiAuthenticationDto destination)=> new Models.EcfApiAuthentication();

    protected override void MapCore(Models.EcfApiAuthentication source, EcfApiAuthenticationDto destination)
    {
        


    }

    protected override void ReverseMapCore(EcfApiAuthenticationDto destination, Models.EcfApiAuthentication source)
    {
     
    }
}
