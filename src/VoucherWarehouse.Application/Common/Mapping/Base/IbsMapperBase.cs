using Abp.Application.Services.Dto;
using IBS.VoucherWarehouse.Common.Mapping.Abstractions;
using IBS.VoucherWarehouse.Common.Mapping.Helpers;


namespace IBS.VoucherWarehouse.Common.Mapping.Base;

public abstract class IbsMapperBase<TSource, TDestination> :
    IIbsMapper<TSource, TDestination>,
    IIbsCollectionMapper<TSource, TDestination>,
    ITransientDependency
{
    public TDestination Map(TSource source)
    {
        try
        {

            IbsMappingGuard.AgainstNull(source, nameof(source));

            BeforeMap(source);

            var destination = CreateDestination(source);

            ApplyAutomaticMap(source, destination);
            MapCore(source, destination);

            AfterMap(source, destination);

            return destination;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public void Map(TSource source, TDestination destination)
    {
        try
        {

            IbsMappingGuard.AgainstNull(source, nameof(source));
            IbsMappingGuard.AgainstNull(destination, nameof(destination));

            BeforeMap(source);

            ApplyAutomaticMap(source, destination);
            MapCore(source, destination);

            AfterMap(source, destination);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public IReadOnlyList<TDestination> MapList(IEnumerable<TSource> source)
    {
        IbsMappingGuard.AgainstNull(source, nameof(source));

        return source.Select(Map).ToList();
    }

    public List<TDestination> MapToList(IEnumerable<TSource> source)
    {
        IbsMappingGuard.AgainstNull(source, nameof(source));

        return source.Select(Map).ToList();
    }

    public PagedResultDto<TDestination> MapToPagedResult(IEnumerable<TSource> listDto, int count)
    {
        IbsMappingGuard.AgainstNull(listDto, nameof(listDto));



        return new PagedResultDto<TDestination>()
        {
            TotalCount = count,
            Items = listDto.Select(Map).ToList()
        };


    }

    protected abstract TDestination CreateDestination(TSource source);

    protected abstract void MapCore(TSource source, TDestination destination);

    protected virtual void BeforeMap(TSource source)
    {
    }

    protected virtual void AfterMap(TSource source, TDestination destination)
    {
    }

    protected virtual bool ShouldMapProperty(string propertyName)
    {
        return true;
    }

    protected virtual object? TransformSourceValue(string propertyName, object? value)
    {
        return value;
    }

    private void ApplyAutomaticMap(TSource source, TDestination destination)
    {
        var mapDefinitions = IbsMapperConventionCache.GetMapDefinitions<TSource, TDestination>();

        foreach (var mapDefinition in mapDefinitions)
        {
            var propertyName = mapDefinition.DestinationProperty.Name;

            if (!ShouldMapProperty(propertyName))
            {
                continue;
            }

            var value = mapDefinition.SourceProperty.GetValue(source);
            value = TransformSourceValue(propertyName, value);

            mapDefinition.DestinationProperty.SetValue(destination, value);
        }
    }
}