using IBS.VoucherWarehouse.Common.Mapping.Base;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfApiAuthentication.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Mappers;

public class MapEntityToOutputTwoWay : IbsTwoWayMapperBase<Models.EcfVoucherWarehouse, EcfVoucherWarehouseOutputDto>
{

    private static readonly Lazy<MapEntityToOutputTwoWay> map =
        new Lazy<MapEntityToOutputTwoWay>(() => new MapEntityToOutputTwoWay());

    public static MapEntityToOutputTwoWay Auto => map.Value;
    private MapEntityToOutputTwoWay()
    {
    }

    protected override EcfVoucherWarehouseOutputDto CreateDestination(Models.EcfVoucherWarehouse source)
    => new();
    protected override Models.EcfVoucherWarehouse CreateSource(EcfVoucherWarehouseOutputDto destination)
    => new();

    protected override void MapCore(Models.EcfVoucherWarehouse source, EcfVoucherWarehouseOutputDto destination)
    {
       
    }

    protected override void ReverseMapCore(EcfVoucherWarehouseOutputDto destination, Models.EcfVoucherWarehouse source)
    {
       
    }
}
