using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VoucherWarehouse.TenantLogoFileManager.Dto;

namespace VoucherWarehouse.TenantLogoFileManager;

public class TenantLogoFileManagerAppService : ITenantLogoFileManagerAppService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public TenantLogoFileManagerAppService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<TenantLogoStoredFileDto> SaveAsync(int tenantId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("No se recibió ningún archivo.");
        }

        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".webp", ".svg" };
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
        {
            throw new Exception("El formato del logo no es válido.");
        }

        if (file.Length > 2 * 1024 * 1024)
        {
            throw new Exception("El logo no puede exceder 2 MB.");
        }

        var folderRelative = Path.Combine("uploads", "tenants", tenantId.ToString());
        var folderAbsolute = Path.Combine(_webHostEnvironment.WebRootPath, folderRelative);

        if (!Directory.Exists(folderAbsolute))
        {
            Directory.CreateDirectory(folderAbsolute);
        }

        var fileName = $"tenant-logo-{tenantId}-{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(folderAbsolute, fileName);

        using (var stream = new FileStream(absolutePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new TenantLogoStoredFileDto
        {
            RelativePath = "/" + Path.Combine(folderRelative, fileName).Replace("\\", "/"),
            FileName = fileName,
            ContentType = file.ContentType,
            Size = file.Length
        };
    }

    public Task DeleteAsync(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        var sanitizedPath = relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
        var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, sanitizedPath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }
}

