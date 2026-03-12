using IBS.VoucherWarehouse.Features.Dto;

namespace IBS.VoucherWarehouse.Features;

public interface IFeaturesAppService : IApplicationService
{
    Task AddFeaturesToTenant(AddFeaturesToOneTenantInputDto input);
    Task AddFeatureToTenants(AddFeatureToTenantsInputDto input);
    Task<ListResultDto<FeatureDto>> GetAllFeatures();
    Task<ListResultDto<FeatureDto>> GetTenantFeatures(GetTenantFeaturesInputDto input);
}
