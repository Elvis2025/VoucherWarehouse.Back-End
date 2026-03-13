namespace IBS.VoucherWarehouse.Common.Mapping.Abstractions;

public interface IIbsTwoWayMapper<TSource, TDestination> : IIbsMapper<TSource, TDestination>
{
    TSource ReverseMap(TDestination destination);

    void ReverseMap(TDestination destination, TSource source);
}