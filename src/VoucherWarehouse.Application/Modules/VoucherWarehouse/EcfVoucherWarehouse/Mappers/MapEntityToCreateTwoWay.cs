using IBS.VoucherWarehouse.Common.Mapping.Base;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Mappers;

public class MapEntityToCreateTwoWay : IbsTwoWayMapperBase<Models.EcfVoucherWarehouse, EcfVoucherWarehouseCreateDto>
{
    private static readonly Lazy<MapEntityToCreateTwoWay> auto= new Lazy<MapEntityToCreateTwoWay>(() => new());

    public static MapEntityToCreateTwoWay Auto => auto.Value;

    private MapEntityToCreateTwoWay()
    {
    }
    protected override EcfVoucherWarehouseCreateDto CreateDestination(Models.EcfVoucherWarehouse source) => new();

    protected override Models.EcfVoucherWarehouse CreateSource(EcfVoucherWarehouseCreateDto destination) => new();

    protected override void MapCore(Models.EcfVoucherWarehouse source, EcfVoucherWarehouseCreateDto destination)
    {

    }

    protected override void ReverseMapCore(EcfVoucherWarehouseCreateDto destination, Models.EcfVoucherWarehouse source)
    {

    }
}
