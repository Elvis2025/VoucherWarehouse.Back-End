using IBS.VoucherWarehouse.Common.Mapping.Base;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Mappers;


public class MapEntityToUpdateTwoWay : IbsTwoWayMapperBase<Models.EcfVoucherWarehouse, EcfVoucherWarehouseUpdateDto>
{

    private static readonly Lazy<MapEntityToUpdateTwoWay> auto =
        new Lazy<MapEntityToUpdateTwoWay>(() => new MapEntityToUpdateTwoWay());

    public static MapEntityToUpdateTwoWay Auto => auto.Value;
    private MapEntityToUpdateTwoWay()
    {
    }

    protected override EcfVoucherWarehouseUpdateDto CreateDestination(Models.EcfVoucherWarehouse source)
    => new();
    protected override Models.EcfVoucherWarehouse CreateSource(EcfVoucherWarehouseUpdateDto destination)
    => new();

    protected override void MapCore(Models.EcfVoucherWarehouse source, EcfVoucherWarehouseUpdateDto destination)
    {

    }

    protected override void ReverseMapCore(EcfVoucherWarehouseUpdateDto destination, Models.EcfVoucherWarehouse source)
    {

    }
}
