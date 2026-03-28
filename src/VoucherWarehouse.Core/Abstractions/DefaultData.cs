using Abp.Dependency;
using Abp.Domain.Uow;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract class DefaultData<TEntity> where TEntity : class
{
    protected TEntity Entity => IocManager.Instance.Resolve<TEntity>();
    protected IUnitOfWorkManager UnitOfWorkManager => IocManager.Instance.Resolve<IUnitOfWorkManager>();

    public abstract Task CreateAsync();
}
