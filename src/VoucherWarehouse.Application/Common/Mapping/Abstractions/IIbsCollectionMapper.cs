namespace IBS.VoucherWarehouse.Common.Mapping.Abstractions;

public interface IIbsCollectionMapper<in TSource, TDestination>
{ 
    IReadOnlyList<TDestination> MapList(IEnumerable<TSource> source);

    List<TDestination> MapToList(IEnumerable<TSource> source);
}