using Abp.Application.Features;
using VoucherWarehouse.Features.Dto;


namespace VoucherWarehouse.Features;

public class FeaturesAppService : VoucherWarehouseAppServiceBase, IFeaturesAppService
{

    private readonly IFeatureManager _featureManager;
    private readonly IRepository<Tenant, int> _tenantRepository;
    private readonly IRepository<TenantFeatureSetting, long> _tenantFeatureRepository;

    public FeaturesAppService(
        IFeatureManager featureManager,
        IRepository<Tenant, int> tenantRepository,
        IRepository<TenantFeatureSetting, long> tenantFeatureRepository)
    {
        _featureManager = featureManager;
        _tenantRepository = tenantRepository;
        _tenantFeatureRepository = tenantFeatureRepository;
    }

    public async Task<ListResultDto<FeatureDto>> GetAllFeatures()
    {
        List<FeatureDto> result = new();
        
            var features = _featureManager
                .GetAll()
                .OrderBy(x => x.Name)
                .ToList();

            result = features.Select(x => new FeatureDto
            {
                Name = x.Name,
                DisplayName = x.DisplayName?.Localize(LocalizationManager),
                Description = x.Description?.Localize(LocalizationManager),
                DefaultValue = x.DefaultValue,
                CurrentValue = x.DefaultValue,
                HasCustomValueForTenant = false
            }).ToList();
        

        return new ListResultDto<FeatureDto>(result);
    }

    public async Task<ListResultDto<FeatureDto>> GetTenantFeatures(GetTenantFeaturesInputDto input)
    {
        if (input.TenantId == null)
        {
            throw new UserFriendlyException(L("TenantIdIsRequired"));
        }
        await _tenantRepository.GetAsync((int)input.TenantId);
        List<FeatureDto> result = new();

        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {

            var features = _featureManager
                .GetAll()
                .OrderBy(x => x.Name)
                .ToList();

            var tenantSettings = await _tenantFeatureRepository
                .GetAll()
                .Where(x => x.TenantId == (int)input.TenantId)
                .ToListAsync();

            var tenantSettingsMap = tenantSettings.ToDictionary(x => x.Name, x => x.Value);

            result = features.Select(feature =>
            {
                var hasCustom = tenantSettingsMap.ContainsKey(feature.Name);
                var currentValue = hasCustom
                    ? tenantSettingsMap[feature.Name]
                    : feature.DefaultValue;

                return new FeatureDto
                {
                    Name = feature.Name,
                    DisplayName = feature.DisplayName?.Localize(LocalizationManager),
                    Description = feature.Description?.Localize(LocalizationManager),
                    DefaultValue = feature.DefaultValue,
                    CurrentValue = currentValue,
                    HasCustomValueForTenant = hasCustom
                };
            }).ToList();
        }

        return new ListResultDto<FeatureDto>(result);
    }

    public async Task AddFeatureToTenants(AddFeatureToTenantsInputDto input)
    {
        if (input.TenantIds is null)
        {
            throw new UserFriendlyException(L("TenantIdIsRequired"));
        }

        var feature = _featureManager.Get(input.FeatureName);
        if (feature == null)
        {
            throw new Abp.UI.UserFriendlyException(L("FeatureNotFound"), input.FeatureName);
        }

        foreach (var tenantId in input.TenantIds.Distinct())
        {

            await _tenantRepository.GetAsync(tenantId);

            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {

                var existing = await _tenantFeatureRepository
                    .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == input.FeatureName);

                if (existing == null)
                {
                    await _tenantFeatureRepository.InsertAsync(new TenantFeatureSetting
                    {
                        TenantId = tenantId,
                        Name = input.FeatureName,
                        Value = input.Value
                    });
                }
                else
                {
                    existing.Value = input.Value;
                    await _tenantFeatureRepository.UpdateAsync(existing);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

    }

    public async Task AddFeaturesToTenant(AddFeaturesToOneTenantInputDto input)
    {
        await _tenantRepository.GetAsync(input.TenantId);

        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {


            foreach (var item in input.Features
                         .Where(x => !x.FeatureName.IsNullOrWhiteSpace())
                         .GroupBy(x => x.FeatureName)
                         .Select(g => g.Last()))
            {
                var feature = _featureManager.Get(item.FeatureName);
                if (feature == null)
                {
                    throw new Abp.UI.UserFriendlyException(L("FeatureNotFound"), item.FeatureName);
                }

                var existing = await _tenantFeatureRepository
                    .FirstOrDefaultAsync(x => x.TenantId == input.TenantId && x.Name == item.FeatureName);

                if (existing == null)
                {
                    await _tenantFeatureRepository.InsertAsync(new TenantFeatureSetting
                    {
                        TenantId = input.TenantId,
                        Name = item.FeatureName,
                        Value = item.Value
                    });
                }
                else
                {
                    existing.Value = item.Value;
                    await _tenantFeatureRepository.UpdateAsync(existing);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }

    private string L(string name)
    {
        return name;
    }

}
