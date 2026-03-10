using Abp;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VoucherWarehouse.TenantBranding.Dto;
using VoucherWarehouse.TenantLogoFileManager;

namespace VoucherWarehouse.TenantBranding;

public class TenantBrandingAppService : ApplicationService, ITenantBrandingAppService
{
    private readonly IRepository<MultiTenancy.TenantBranding, long> _tenantBrandingRepository;
    private readonly ITenantLogoFileManagerAppService _tenantLogoFileManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantBrandingAppService(
        IRepository<MultiTenancy.TenantBranding, long> tenantBrandingRepository,
        ITenantLogoFileManagerAppService tenantLogoFileManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _tenantBrandingRepository = tenantBrandingRepository;
        _tenantLogoFileManager = tenantLogoFileManager;
        _httpContextAccessor = httpContextAccessor;
    }

    [Consumes("multipart/form-data")]
    public async Task<TenantBrandingDto> UploadLogoAsync([FromForm] UploadTenantLogoRequestDto input)
    {
        if (input.TenantId <= 0)
        {
            throw new AbpException("No tenant context found.");
        }

        MultiTenancy.TenantBranding branding;

        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {
            branding = await _tenantBrandingRepository.FirstOrDefaultAsync(x => x.TenantId == input.TenantId);

            if (branding != null && !string.IsNullOrWhiteSpace(branding.LogoPath))
            {
                await _tenantLogoFileManager.DeleteAsync(branding.LogoPath);
            }

            var storedFile = await _tenantLogoFileManager.SaveAsync(input.TenantId, input.File);

            if (branding == null)
            {
                branding = new MultiTenancy.TenantBranding
                {
                    TenantId = input.TenantId,
                    LogoPath = storedFile.RelativePath,
                    LogoFileName = storedFile.FileName,
                    LogoContentType = storedFile.ContentType,
                    LogoSize = storedFile.Size,
                    CompanyDescription = input.CompanyDescription,
                    CompanyName = input.CompanyName,
                    CompanyType = input.CompanyType
                };

                await _tenantBrandingRepository.InsertAsync(branding);
            }
            else
            {
                branding.LogoPath = storedFile.RelativePath;
                branding.LogoFileName = storedFile.FileName;
                branding.LogoContentType = storedFile.ContentType;
                branding.LogoSize = storedFile.Size;
                branding.CompanyDescription = input.CompanyDescription;
                branding.CompanyName = input.CompanyName;
                branding.CompanyType = input.CompanyType;

                await _tenantBrandingRepository.UpdateAsync(branding);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return MapToDto(branding);
    }

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

    public async Task<TenantBrandingDto> GetCurrentTenantBrandingLogoByTenantId(int tenantId)
    {
        if (tenantId <= 0)
        {
            throw new AbpException("No tenant context found.");
        }
        MultiTenancy.TenantBranding branding;
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

    private TenantBrandingDto MapToDto(MultiTenancy.TenantBranding branding)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        var baseUrl = request == null
            ? ""
            : $"{request.Scheme}://{request.Host}";

        return new TenantBrandingDto
        {
            TenantId = branding.TenantId,
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

