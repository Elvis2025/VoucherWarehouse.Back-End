namespace IBS.VoucherWarehouse.Common.Mapping.Abstractions;

public interface IIbsMapper<in TSource, TDestination>
{
    TDestination Map(TSource source);

    void Map(TSource source, TDestination destination);
}