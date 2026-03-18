#pragma warning disable RMG012, S2094,RMG089
using IBS.VoucherWarehouse.Common.Mapping.Base;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Mappers;

public class EcfApiAuthenticationMapping : IbsTwoWayMapperBase<Models.EcfApiAuthentication, EcfApiAuthenticationOutputDto>
{

    protected override EcfApiAuthenticationOutputDto CreateDestination(Models.EcfApiAuthentication source) => new EcfApiAuthenticationOutputDto();

    protected override Models.EcfApiAuthentication CreateSource(EcfApiAuthenticationOutputDto destination)=> new Models.EcfApiAuthentication();

    protected override void MapCore(Models.EcfApiAuthentication source, EcfApiAuthenticationOutputDto destination)
    {
        


    }

    protected override void ReverseMapCore(EcfApiAuthenticationOutputDto destination, Models.EcfApiAuthentication source)
    {
     
    }
}
