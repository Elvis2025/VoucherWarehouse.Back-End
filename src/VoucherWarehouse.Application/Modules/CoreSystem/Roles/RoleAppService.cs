using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using VoucherWarehouse.Authorization;
using VoucherWarehouse.Authorization.Roles;
using VoucherWarehouse.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace VoucherWarehouse.Modules.CoreSystem.Roles;

[AbpAuthorize(PermissionNames.Pages_Roles)]
public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
{
    private readonly RoleManager _roleManager;
    private readonly UserManager _userManager;
    private const string CoreSystemPermissionName = "CoreSystem";
    public RoleAppService(IRepository<Role> repository, RoleManager roleManager, UserManager userManager)
        : base(repository)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public override async Task<RoleDto> CreateAsync(CreateRoleDto input)
    {
        CheckCreatePermission();

        var role = ObjectMapper.Map<Role>(input);
        role.SetNormalizedName();

        CheckErrors(await _roleManager.CreateAsync(role));

        var grantedPermissions = PermissionManager
            .GetAllPermissions()
            .Where(p => input.GrantedPermissions.Contains(p.Name))
            .ToList();

        await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

        return MapToEntityDto(role);
    }

    public async Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input)
    {
        var roles = await _roleManager
            .Roles
            .WhereIf(
                !input.Permission.IsNullOrWhiteSpace(),
                r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted)
            )
            .ToListAsync();

        return new ListResultDto<RoleListDto>(
            ObjectMapper.Map<List<RoleListDto>>(roles)
        );
    }

    public override async Task<RoleDto> UpdateAsync(RoleDto input)
    {
        CheckUpdatePermission();

        var role = await _roleManager.GetRoleByIdAsync(input.Id);

        ObjectMapper.Map(input, role);

        CheckErrors(await _roleManager.UpdateAsync(role));

        var grantedPermissions = PermissionManager
            .GetAllPermissions()
            .Where(p => input.GrantedPermissions.Contains(p.Name))
            .ToList();

        await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

        return MapToEntityDto(role);
    }

    public override async Task DeleteAsync(EntityDto<int> input)
    {
        CheckDeletePermission();

        var role = await _roleManager.FindByIdAsync(input.Id.ToString());
        var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

        foreach (var user in users)
        {
            CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
        }

        CheckErrors(await _roleManager.DeleteAsync(role));
    }

    public Task<ListResultDto<PermissionDto>> GetAllPermissions()
    {
        var permissions = PermissionManager.GetAllPermissions();

        return Task.FromResult(
            new ListResultDto<PermissionDto>(
                ObjectMapper.Map<List<PermissionDto>>(permissions)
                    .OrderBy(p => p.DisplayName)
                    .ToList()
            )
        );
    }

    public Task<ListResultDto<PermissionTreeDto>> GetAllPermissionsTree()
    {
        var permissions = PermissionManager.GetAllPermissions().ToList();
        var currentSide = GetCurrentMultiTenancySide();
        var tree = BuildPermissionTree(permissions, new HashSet<string>(), currentSide);

        return Task.FromResult(new ListResultDto<PermissionTreeDto>(tree));
    }

    public async Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input)
    {
        var permissions = PermissionManager.GetAllPermissions().ToList();
        var role = await _roleManager.GetRoleByIdAsync(input.Id);
        var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
        var grantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList();
        var grantedPermissionSet = grantedPermissionNames.ToHashSet();
        var currentSide = GetCurrentMultiTenancySide();

        var roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

        return new GetRoleForEditOutput
        {
            Role = roleEditDto,
            Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(
                    permissions.Where(p => p.MultiTenancySides.HasFlag(currentSide)).ToList()
                )
                .OrderBy(p => p.DisplayName)
                .ToList(),
            GrantedPermissionNames = grantedPermissionNames,
            PermissionsTree = BuildPermissionTree(permissions, grantedPermissionSet, currentSide)
        };
    }

    protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
    {
        return Repository.GetAllIncluding(x => x.Permissions)
            .WhereIf(
                !input.Keyword.IsNullOrWhiteSpace(),
                x => x.Name.Contains(input.Keyword) ||
                     x.DisplayName.Contains(input.Keyword) ||
                     x.Description.Contains(input.Keyword)
            );
    }

    protected override async Task<Role> GetEntityByIdAsync(int id)
    {
        return await Repository
            .GetAllIncluding(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected virtual void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }

    private MultiTenancySides GetCurrentMultiTenancySide()
    {
        return AbpSession.MultiTenancySide == MultiTenancySides.Host
            ? MultiTenancySides.Host
            : MultiTenancySides.Tenant;
    }

    private List<PermissionTreeDto> BuildPermissionTree(
        IReadOnlyCollection<Permission> allPermissions,
        HashSet<string> grantedPermissionNames,
        MultiTenancySides currentSide)
    {
        var rootPermissions = allPermissions
            .Where(p => p.Parent == null && p.MultiTenancySides.HasFlag(currentSide))
            .ToList();

        var coreSystemRoot = rootPermissions.FirstOrDefault(p =>
            string.Equals(p.Name, CoreSystemPermissionName, StringComparison.OrdinalIgnoreCase));

        var usersRoot = rootPermissions.FirstOrDefault(p =>
            string.Equals(p.Name, PermissionNames.Pages_Users, StringComparison.OrdinalIgnoreCase));

        var result = rootPermissions
            .Where(p => !ShouldHideFromRoot(p, coreSystemRoot != null, usersRoot != null))
            .Select(p => MapPermissionNode(p, grantedPermissionNames, currentSide, allPermissions))
            .OrderBy(x => x.DisplayName)
            .ToList();

        return result;
    }

    private bool ShouldHideFromRoot(Permission permission, bool hasCoreSystemRoot, bool hasUsersRoot)
    {
        if (permission == null)
        {
            return false;
        }

        if (hasUsersRoot &&
            string.Equals(permission.Name, PermissionNames.Pages_Users_Activation, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!hasCoreSystemRoot)
        {
            return false;
        }

        return IsAbpRootPermissionToNestUnderCoreSystem(permission.Name);
    }

    private bool IsAbpRootPermissionToNestUnderCoreSystem(string permissionName)
    {
        return string.Equals(permissionName, PermissionNames.Pages_Users, StringComparison.OrdinalIgnoreCase)
               || string.Equals(permissionName, PermissionNames.Pages_Roles, StringComparison.OrdinalIgnoreCase)
               || string.Equals(permissionName, PermissionNames.Pages_Tenants, StringComparison.OrdinalIgnoreCase)
               || string.Equals(permissionName, PermissionNames.Pages_Users_Activation, StringComparison.OrdinalIgnoreCase);
    }

    private PermissionTreeDto MapPermissionNode(
        Permission permission,
        HashSet<string> grantedPermissionNames,
        MultiTenancySides currentSide,
        IReadOnlyCollection<Permission> allPermissions)
    {
        var children = GetPermissionChildrenWithBusinessRules(permission, currentSide, allPermissions);

        return new PermissionTreeDto
        {
            Name = permission.Name,
            DisplayName = GetLocalizedText(permission.DisplayName) ?? permission.Name,
            Description = GetLocalizedText(permission.Description),
            IsGranted = grantedPermissionNames.Contains(permission.Name),
            Children = children
                .Select(c => MapPermissionNode(c, grantedPermissionNames, currentSide, allPermissions))
                .OrderBy(c => c.DisplayName)
                .ToList()
        };
    }

    private List<Permission> GetPermissionChildrenWithBusinessRules(
        Permission permission,
        MultiTenancySides currentSide,
        IReadOnlyCollection<Permission> allPermissions)
    {
        var children = permission.Children
            .Where(c => c.MultiTenancySides.HasFlag(currentSide))
            .ToList();

        if (string.Equals(permission.Name, CoreSystemPermissionName, StringComparison.OrdinalIgnoreCase))
        {
            var abpPermissionsToNest = GetAbpRootPermissionsToNestUnderCoreSystem(allPermissions, currentSide);

            foreach (var abpPermission in abpPermissionsToNest)
            {
                if (children.All(c => !string.Equals(c.Name, abpPermission.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    children.Add(abpPermission);
                }
            }
        }

        if (string.Equals(permission.Name, PermissionNames.Pages_Users, StringComparison.OrdinalIgnoreCase))
        {
            var usersActivationRoot = allPermissions.FirstOrDefault(p =>
                p.Parent == null &&
                p.MultiTenancySides.HasFlag(currentSide) &&
                string.Equals(p.Name, PermissionNames.Pages_Users_Activation, StringComparison.OrdinalIgnoreCase));

            if (usersActivationRoot != null &&
                children.All(c => !string.Equals(c.Name, PermissionNames.Pages_Users_Activation, StringComparison.OrdinalIgnoreCase)))
            {
                children.Add(usersActivationRoot);
            }
        }

        return children
            .OrderBy(c => GetLocalizedText(c.DisplayName) ?? c.Name)
            .ToList();
    }

    private List<Permission> GetAbpRootPermissionsToNestUnderCoreSystem(
        IReadOnlyCollection<Permission> allPermissions,
        MultiTenancySides currentSide)
    {
        var result = new List<Permission>();

        var usersPermission = allPermissions.FirstOrDefault(p =>
            p.Parent == null &&
            p.MultiTenancySides.HasFlag(currentSide) &&
            string.Equals(p.Name, PermissionNames.Pages_Users, StringComparison.OrdinalIgnoreCase));

        var rolesPermission = allPermissions.FirstOrDefault(p =>
            p.Parent == null &&
            p.MultiTenancySides.HasFlag(currentSide) &&
            string.Equals(p.Name, PermissionNames.Pages_Roles, StringComparison.OrdinalIgnoreCase));

        var tenantsPermission = allPermissions.FirstOrDefault(p =>
            p.Parent == null &&
            p.MultiTenancySides.HasFlag(currentSide) &&
            string.Equals(p.Name, PermissionNames.Pages_Tenants, StringComparison.OrdinalIgnoreCase));

        if (usersPermission != null)
        {
            result.Add(usersPermission);
        }

        if (rolesPermission != null)
        {
            result.Add(rolesPermission);
        }

        if (tenantsPermission != null)
        {
            result.Add(tenantsPermission);
        }

        return result
            .OrderBy(p => GetLocalizedText(p.DisplayName) ?? p.Name)
            .ToList();
    }

    private string GetLocalizedText(ILocalizableString localizableString)
    {
        if (localizableString == null)
        {
            return null;
        }

        return localizableString.Localize(LocalizationManager);
    }
}

