using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using VoucherWarehouse.Authorization;
using VoucherWarehouse.Authorization.Roles;
using VoucherWarehouse.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace VoucherWarehouse.Modules.CoreSystem.Users;

[AbpAuthorize(PermissionNames.Pages_Users)]
public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
{
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IRepository<Role> _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAbpSession _abpSession;
    private readonly LogInManager _logInManager;
    private const string CoreSystemPermissionName = "CoreSystem";
    public UserAppService(
        IRepository<User, long> repository,
        UserManager userManager,
        RoleManager roleManager,
        IRepository<Role> roleRepository,
        IPasswordHasher<User> passwordHasher,
        IAbpSession abpSession,
        LogInManager logInManager)
        : base(repository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _abpSession = abpSession;
        _logInManager = logInManager;
    }

    public override async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        CheckCreatePermission();

        var user = ObjectMapper.Map<User>(input);

        user.TenantId = AbpSession.TenantId;
        user.IsEmailConfirmed = true;

        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

        CheckErrors(await _userManager.CreateAsync(user, input.Password));

        if (input.RoleNames != null)
        {
            CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
        }

        await CurrentUnitOfWork.SaveChangesAsync();

        return MapToEntityDto(user);
    }

    public override async Task<UserDto> UpdateAsync(UserDto input)
    {
        CheckUpdatePermission();

        var user = await _userManager.GetUserByIdAsync(input.Id);

        MapToEntity(input, user);

        CheckErrors(await _userManager.UpdateAsync(user));

        if (input.RoleNames != null)
        {
            CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
        }

        return await GetAsync(input);
    }

    public override async Task DeleteAsync(EntityDto<long> input)
    {
        var user = await _userManager.GetUserByIdAsync(input.Id);
        await _userManager.DeleteAsync(user);
    }

    [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
    public async Task Activate(EntityDto<long> user)
    {
        await Repository.UpdateAsync(user.Id, async (entity) =>
        {
            entity.IsActive = true;
        });
    }

    [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
    public async Task DeActivate(EntityDto<long> user)
    {
        await Repository.UpdateAsync(user.Id, async (entity) =>
        {
            entity.IsActive = false;
        });
    }

    public async Task<ListResultDto<RoleDto>> GetRoles()
    {
        var roles = await _roleRepository.GetAllListAsync();
        return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
    }

    public async Task ChangeLanguage(ChangeUserLanguageDto input)
    {
        await SettingManager.ChangeSettingForUserAsync(
            AbpSession.ToUserIdentifier(),
            LocalizationSettingNames.DefaultLanguage,
            input.LanguageName
        );
    }

    protected override User MapToEntity(CreateUserDto createInput)
    {
        var user = ObjectMapper.Map<User>(createInput);
        user.SetNormalizedNames();
        return user;
    }

    protected override void MapToEntity(UserDto input, User user)
    {
        ObjectMapper.Map(input, user);
        user.SetNormalizedNames();
    }

    protected override UserDto MapToEntityDto(User user)
    {
        var roleIds = user.Roles.Select(x => x.RoleId).ToArray();

        var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

        var userDto = base.MapToEntityDto(user);
        userDto.RoleNames = roles.ToArray();

        return userDto;
    }

    protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
    {
        return Repository.GetAllIncluding(x => x.Roles)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
    }

    protected override async Task<User> GetEntityByIdAsync(long id)
    {
        var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            throw new EntityNotFoundException(typeof(User), id);
        }

        return user;
    }

    protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected virtual void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }

    public async Task<bool> ChangePassword(ChangePasswordDto input)
    {
        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

        var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
        if (user == null)
        {
            throw new Exception("There is no current user!");
        }

        if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
        {
            CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
        }
        else
        {
            CheckErrors(IdentityResult.Failed(new IdentityError
            {
                Description = "Incorrect password."
            }));
        }

        return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordDto input)
    {
        if (_abpSession.UserId == null)
        {
            throw new UserFriendlyException("Please log in before attempting to reset password.");
        }

        var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
        var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
        if (loginAsync.Result != AbpLoginResultType.Success)
        {
            throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
        }

        if (currentUser.IsDeleted || !currentUser.IsActive)
        {
            return false;
        }

        var roles = await _userManager.GetRolesAsync(currentUser);
        if (!roles.Contains(StaticRoleNames.Tenants.Admin))
        {
            throw new UserFriendlyException("Only administrators may reset passwords.");
        }

        var user = await _userManager.GetUserByIdAsync(input.UserId);
        if (user != null)
        {
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return true;
    }

    [AbpAuthorize(PermissionNames.Pages_Users)]
    public virtual async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(long userId)
    {
        var user = await _userManager.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new UserFriendlyException("No se encontró el usuario.");
        }

        var currentSide = GetCurrentMultiTenancySide();

        var allPermissions = PermissionManager
            .GetAllPermissions()
            .Where(p => p.MultiTenancySides.HasFlag(currentSide))
            .ToList();

        var grantedPermissions = await _userManager.GetGrantedPermissionsAsync(user);
        var grantedPermissionNames = grantedPermissions
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        var output = new GetUserPermissionsForEditOutput
        {
            Permissions = allPermissions
                .Select(p => new UserFlatPermissionDto
                {
                    Name = p.Name,
                    DisplayName = p.DisplayName?.Localize(LocalizationManager) ?? p.Name,
                    Description = p.Description?.Localize(LocalizationManager)
                })
                .OrderBy(x => x.DisplayName)
                .ToList(),

            GrantedPermissionNames = grantedPermissionNames,

            PermissionsTree = BuildPermissionTree(allPermissions, grantedPermissionNames, currentSide)
        };

        return output;
    }

    [AbpAuthorize(PermissionNames.Pages_Users)]
    public virtual async Task UpdateUserPermissions(UpdateUserPermissionsInputDto input)
    {
        if (input == null)
        {
            throw new UserFriendlyException("La solicitud no puede estar vacía.");
        }

        var user = await _userManager.GetUserByIdAsync(input.Id);

        if (user == null)
        {
            throw new UserFriendlyException("No se encontró el usuario.");
        }

        var currentSide = GetCurrentMultiTenancySide();

        var allPermissions = PermissionManager
            .GetAllPermissions()
            .Where(p => p.MultiTenancySides.HasFlag(currentSide))
            .ToList();

        var grantedNames = (input.GrantedPermissionNames ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToHashSet();

        var permissionsToGrant = allPermissions
            .Where(p => grantedNames.Contains(p.Name))
            .ToList();

        await _userManager.SetGrantedPermissionsAsync(user, permissionsToGrant);
    }

    private MultiTenancySides GetCurrentMultiTenancySide()
    {
        return AbpSession.MultiTenancySide == MultiTenancySides.Host
            ? MultiTenancySides.Host
            : MultiTenancySides.Tenant;
    }

    private List<UserPermissionTreeDto> BuildPermissionTree(
        IReadOnlyCollection<Permission> allPermissions,
        IReadOnlyCollection<string> grantedPermissionNames,
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
            .Select(p => MapPermissionTree(p, grantedPermissionNames, currentSide, allPermissions))
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

    private UserPermissionTreeDto MapPermissionTree(
        Permission permission,
        IReadOnlyCollection<string> grantedPermissionNames,
        MultiTenancySides currentSide,
        IReadOnlyCollection<Permission> allPermissions)
    {
        var children = GetPermissionChildrenWithBusinessRules(permission, currentSide, allPermissions);

        return new UserPermissionTreeDto
        {
            Name = permission.Name,
            DisplayName = permission.DisplayName?.Localize(LocalizationManager) ?? permission.Name,
            Description = permission.Description?.Localize(LocalizationManager),
            IsGranted = grantedPermissionNames.Contains(permission.Name),
            Children = children
                .Select(c => MapPermissionTree(c, grantedPermissionNames, currentSide, allPermissions))
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
            .OrderBy(c => c.DisplayName?.Localize(LocalizationManager) ?? c.Name)
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
            .OrderBy(p => p.DisplayName?.Localize(LocalizationManager) ?? p.Name)
            .ToList();
    }
}

