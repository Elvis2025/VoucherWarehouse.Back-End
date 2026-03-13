using IBS.VoucherWarehouse.Common.Mapping.Abstractions;
using IBS.VoucherWarehouse.Common.Mapping.Helpers;

namespace IBS.VoucherWarehouse.Common.Mapping.Base;

public abstract class IbsTwoWayMapperBase<TSource, TDestination> :
    IbsMapperBase<TSource, TDestination>,
    IIbsTwoWayMapper<TSource, TDestination>,
    ITransientDependency
{
    public TSource ReverseMap(TDestination destination)
    {
        IbsMappingGuard.AgainstNull(destination, nameof(destination));

        BeforeReverseMap(destination);

        var source = CreateSource(destination);

        ApplyAutomaticReverseMap(destination, source);
        ReverseMapCore(destination, source);

        AfterReverseMap(destination, source);

        return source;
    }

    public void ReverseMap(TDestination destination, TSource source)
    {
        IbsMappingGuard.AgainstNull(destination, nameof(destination));
        IbsMappingGuard.AgainstNull(source, nameof(source));

        BeforeReverseMap(destination);

        ApplyAutomaticReverseMap(destination, source);
        ReverseMapCore(destination, source);

        AfterReverseMap(destination, source);
    }

    protected abstract TSource CreateSource(TDestination destination);

    protected abstract void ReverseMapCore(TDestination destination, TSource source);

    protected virtual void BeforeReverseMap(TDestination destination)
    {
    }

    protected virtual void AfterReverseMap(TDestination destination, TSource source)
    {
    }

    protected virtual bool ShouldReverseMapProperty(string propertyName)
    {
        return true;
    }

    protected virtual object? TransformDestinationValue(string propertyName, object? value)
    {
        return value;
    }

    private void ApplyAutomaticReverseMap(TDestination destination, TSource source)
    {
        var mapDefinitions = IbsMapperConventionCache.GetMapDefinitions<TDestination, TSource>();

        foreach (var mapDefinition in mapDefinitions)
        {
            var propertyName = mapDefinition.DestinationProperty.Name;

            if (!ShouldReverseMapProperty(propertyName))
            {
                continue;
            }

            var value = mapDefinition.SourceProperty.GetValue(destination);
            value = TransformDestinationValue(propertyName, value);

            mapDefinition.DestinationProperty.SetValue(source, value);
        }
    }
}