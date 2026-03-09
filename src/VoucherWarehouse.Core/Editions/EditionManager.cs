using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace VoucherWarehouse.Editions;

public class EditionManager : AbpEditionManager
{
    public const string DefaultEditionName = "Standard";

    public EditionManager(
        IRepository<Edition> editionRepository,
        IAbpZeroFeatureValueStore featureValueStore,
        IUnitOfWorkManager unitOfWorkManager)
        : base(editionRepository, featureValueStore, unitOfWorkManager)
    {
    }
}
