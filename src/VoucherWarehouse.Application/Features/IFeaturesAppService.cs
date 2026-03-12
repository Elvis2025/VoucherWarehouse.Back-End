using VoucherWarehouse.Features.Dto;

namespace VoucherWarehouse.Features;

public interface IFeaturesAppService : IApplicationService
{
    Task AddFeaturesToTenant(AddFeaturesToOneTenantInputDto input);
    Task AddFeatureToTenants(AddFeatureToTenantsInputDto input);
    Task<ListResultDto<FeatureDto>> GetAllFeatures();
    Task<ListResultDto<FeatureDto>> GetTenantFeatures(GetTenantFeaturesInputDto input);
}
