using IBS.VoucherWarehouse.Abstractions;

namespace IBS.VoucherWarehouse.Common.Abstraction;

public abstract class DefaultData<TEntity> : IDefaultData where TEntity : class
{
    public TEntity Entity => IocManager.Instance.Resolve<TEntity>();
    public abstract Task CreateAsync();
}
