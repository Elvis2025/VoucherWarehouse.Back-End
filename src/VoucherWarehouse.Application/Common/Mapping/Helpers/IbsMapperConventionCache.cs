using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IBS.VoucherWarehouse.Common.Mapping.Helpers;

internal static class IbsMapperConventionCache
{
    private static readonly ConcurrentDictionary<string, IReadOnlyList<PropertyMapDefinition>> Cache = new();

    public static IReadOnlyList<PropertyMapDefinition> GetMapDefinitions<TSource, TDestination>()
    {
        var key = $"{typeof(TSource).FullName}->{typeof(TDestination).FullName}";
        return Cache.GetOrAdd(key, _ => BuildMapDefinitions(typeof(TSource), typeof(TDestination)));
    }

    private static IReadOnlyList<PropertyMapDefinition> BuildMapDefinitions(Type sourceType, Type destinationType)
    {
        var sourceProperties = sourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var destinationProperties = destinationType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

        var result = new List<PropertyMapDefinition>();

        foreach (var destinationProperty in destinationProperties)
        {
            if (!sourceProperties.TryGetValue(destinationProperty.Name, out var sourceProperty))
            {
                continue;
            }

            if (!AreTypesCompatible(sourceProperty.PropertyType, destinationProperty.PropertyType))
            {
                continue;
            }

            result.Add(new PropertyMapDefinition(sourceProperty, destinationProperty));
        }

        return result;
    }

    private static bool AreTypesCompatible(Type sourceType, Type destinationType)
    {
        if (destinationType.IsAssignableFrom(sourceType))
        {
            return true;
        }

        var sourceUnderlyingType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
        var destinationUnderlyingType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

        return destinationUnderlyingType.IsAssignableFrom(sourceUnderlyingType);
    }

    internal sealed class PropertyMapDefinition
    {
        public PropertyMapDefinition(PropertyInfo sourceProperty, PropertyInfo destinationProperty)
        {
            SourceProperty = sourceProperty;
            DestinationProperty = destinationProperty;
        }

        public PropertyInfo SourceProperty { get; }

        public PropertyInfo DestinationProperty { get; }
    }
}