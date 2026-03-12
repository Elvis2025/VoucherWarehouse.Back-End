using Abp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IBS.VoucherWarehouse.Modules.CoreSystem.TenantBranding.Dto;
using IBS.VoucherWarehouse.Modules.CoreSystem.TenantLogoFileManager;

namespace IBS.VoucherWarehouse.Modules.CoreSystem.TenantBranding;

public class TenantBrandingAppService : VoucherWarehouseAppServiceBase, ITenantBrandingAppService
{
    private readonly IRepository<IBS.VoucherWarehouse.MultiTenancy.TenantBranding, long> _tenantBrandingRepository;
    private readonly ITenantLogoFileManagerAppService _tenantLogoFileManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantBrandingAppService(
        IRepository<IBS.VoucherWarehouse.MultiTenancy.TenantBranding, long> tenantBrandingRepository,
        ITenantLogoFileManagerAppService tenantLogoFileManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _tenantBrandingRepository = tenantBrandingRepository;
        _tenantLogoFileManager = tenantLogoFileManager;
        _httpContextAccessor = httpContextAccessor;
    }

    [Consumes("multipart/form-data")]
    public async Task<TenantBrandingDto> UploadLogoAsync([FromForm] UploadTenantLogoInputDto inputDto)
    {
        var tenantId = inputDto.TenantId <= 0 ? AbpSession.GetTenantId() : inputDto.TenantId;
        IBS.VoucherWarehouse.MultiTenancy.TenantBranding branding = new();
        using (CurrentUnitOfWork.SetTenantId(tenantId))
        {
            branding = await _tenantBrandingRepository.FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (branding != null && !string.IsNullOrWhiteSpace(branding.LogoPath))
            {
                await _tenantLogoFileManager.DeleteAsync(branding.LogoPath);
            }

            var storedFile = await _tenantLogoFileManager.SaveAsync(tenantId, inputDto.File);

            if (branding == null)
            {
                branding = new IBS.VoucherWarehouse.MultiTenancy.TenantBranding
                {
                    TenantId = tenantId,
                    LogoPath = storedFile.RelativePath,
                    LogoFileName = storedFile.FileName,
                    LogoContentType = storedFile.ContentType,
                    LogoSize = storedFile.Size,
                    CompanyDescription = inputDto.CompanyDescription,
                    CompanyName = inputDto.CompanyName,
                    CompanyType = inputDto.CompanyType
                };

                await _tenantBrandingRepository.InsertAsync(branding);
            }
            else
            {
                branding.LogoPath = storedFile.RelativePath;
                branding.LogoFileName = storedFile.FileName;
                branding.LogoContentType = storedFile.ContentType;
                branding.LogoSize = storedFile.Size;
                branding.CompanyDescription = inputDto.CompanyDescription;
                branding.CompanyName = inputDto.CompanyName;
                branding.CompanyType = inputDto.CompanyType;

                await _tenantBrandingRepository.UpdateAsync(branding);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return MapToDto(branding);
    }

    [AbpAllowAnonymous]
    public async Task<TenantBrandingDto> GetCurrentTenantLogoAsync()
    {
        if (!AbpSession.TenantId.HasValue)
        {
            return null;
        }

        var branding = await _tenantBrandingRepository.FirstOrDefaultAsync(x => x.TenantId == AbpSession.GetTenantId());

        if (branding == null)
        {
            return null;
        }

        return MapToDto(branding);
    }

    public async Task<TenantBrandingDto> GetCurrentTenantBrandingLogoByTenantIdAsync(int tenantId)
    {
        IBS.VoucherWarehouse.MultiTenancy.TenantBranding branding = new();

        using (CurrentUnitOfWork.SetTenantId(tenantId))
        {
            branding = await _tenantBrandingRepository.FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (branding == null)
            {
                return null;
            }
        }

        return MapToDto(branding);
    }

    public async Task RemoveCurrentTenantLogoAsync()
    {
        var tenantId = AbpSession.GetTenantId();

        var branding = await _tenantBrandingRepository.FirstOrDefaultAsync(x => x.TenantId == tenantId);

        if (branding == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(branding.LogoPath))
        {
            await _tenantLogoFileManager.DeleteAsync(branding.LogoPath);
        }

        branding.LogoPath = null;
        branding.LogoFileName = null;
        branding.LogoContentType = null;
        branding.LogoSize = null;

        await _tenantBrandingRepository.UpdateAsync(branding);
    }

    private TenantBrandingDto MapToDto(IBS.VoucherWarehouse.MultiTenancy.TenantBranding branding)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        var baseUrl = request == null
            ? ""
            : $"{request.Scheme}://{request.Host}";

        return new TenantBrandingDto
        {
            TenantId = (int)branding.TenantId,
            LogoPath = branding.LogoPath,
            LogoFileName = branding.LogoFileName,
            LogoContentType = branding.LogoContentType,
            LogoSize = branding.LogoSize,
            FullLogoUrl = string.IsNullOrWhiteSpace(branding.LogoPath)
                ? null
                : $"{baseUrl}{branding.LogoPath}",
            CompanyDescription = branding.CompanyDescription,
            CompanyName = branding.CompanyName,
            CompanyType = branding.CompanyType
        };
    }
}

