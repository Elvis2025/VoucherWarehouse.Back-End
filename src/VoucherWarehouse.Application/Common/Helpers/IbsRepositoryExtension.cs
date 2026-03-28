using Abp.EntityFrameworkCore.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IBS.VoucherWarehouse.Common.Helpers;

public static class IbsRepositoryExtension
{
    public static TEntity GetFisrtOrDefault<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var entity = repository.GetAll()
                               .FirstOrDefault();


        return entity;
    }

    public async static Task<TEntity> GetFisrtOrDefaultAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var entitys = await (await repository.GetAllAsync()).ToListAsync();
        if (entitys == null || !entitys.Any()) return null;
        
        var firstEntity = entitys.FirstOrDefault() ?? null;


        return firstEntity;
    }

    public static TEntity GetFisrtOrDefault<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository,Func<TEntity,int,bool> where)
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var entity = repository.GetAll()
                               .Where(where)
                               .FirstOrDefault();


        return entity;
    }

    public async static Task<TEntity> GetFisrtOrDefaultAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Func<TEntity, int, bool> where)
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var entitys = await (await repository.GetAllAsync()).ToListAsync();
        if (entitys == null || !entitys.Any()) return null;
        
        var firstEntity = entitys.Where(where)
                                 .FirstOrDefault() ?? null;


        return firstEntity;
    }



    public static TEntity InsertIfNotExit<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var entity = repository.GetAll()
                               .FirstOrDefault();


        return entity;
    }

    public async static Task InsertOrUpdateRangeAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository,IEnumerable<TEntity> entitys)
        where TEntity : class, IEntity<TPrimaryKey>
    {
          
        foreach (var entity in entitys)
        {
            await repository.InsertOrUpdateAsync(entity);
        }
    }

   


}
