using Abp.Domain.Entities;
using IBS.VoucherWarehouse.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Extensions;

public static class EntityExtensions
{

    public static async Task AddOrSetValuesRangeAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, TKey>> keySelector)
        where TEntity : class
    {
        var context = dbSet.GetService<DbContext>();

        var existingEntities = await dbSet.ToListAsync();
        var compiledKey = keySelector.Compile();

        foreach (var entity in entities)
        {
            var key = compiledKey(entity);

            var existing = existingEntities
                .FirstOrDefault(e => compiledKey(e).Equals(key));

            if (existing == null)
            {
                await dbSet.AddAsync(entity);
            }
            else
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
            }
        }
    }

    public static void AddOrSetValuesRange<TEntity, TMatchKey>(
          this DbSet<TEntity> dbSet,
          IEnumerable<TEntity> entities,
          Expression<Func<TEntity, TMatchKey>> matchSelector,
          DbContext context)
          where TEntity : class
    {
        var existingEntities = dbSet.ToList();
        var matchFunc = matchSelector.Compile();

        var entityType = context.Model.FindEntityType(typeof(TEntity));

        if (entityType is null) return;
        var primaryKey = entityType.FindPrimaryKey();

        if (primaryKey is null) return;

        var primaryKeyNames = primaryKey.Properties
            .Select(p => p.Name)
            .ToHashSet();

        var allProperties = entityType.GetProperties()
            .Where(p => !primaryKeyNames.Contains(p.Name))
            .ToList();

        foreach (var entity in entities)
        {
            var matchValue = matchFunc(entity);

            var existing = existingEntities
                .FirstOrDefault(e => EqualityComparer<TMatchKey>.Default.Equals(matchFunc(e), matchValue));

            if (existing == null)
            {
                SetPropertyIfExists(entity, "CreationTime", DateTime.Now);
                dbSet.Add(entity);
                continue;
            }

            var entry = context.Entry(existing);

            foreach (var property in allProperties)
            {
                var propertyInfo = typeof(TEntity).GetProperty(property.Name);
                if (propertyInfo == null || !propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;

                var newValue = propertyInfo.GetValue(entity);
                propertyInfo.SetValue(existing, newValue);
            }

            SetPropertyIfExists(existing, "LastModificationTime", DateTime.Now);
        }
    }



    private static void SetPropertyIfExists(object entity, string propertyName, object value)
    {
        var prop = entity.GetType().GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(entity, value);
        }
    }
}
