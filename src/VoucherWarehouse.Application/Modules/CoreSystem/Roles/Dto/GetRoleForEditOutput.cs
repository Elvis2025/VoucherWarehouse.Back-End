using System.Collections.Generic;

namespace VoucherWarehouse.Modules.CoreSystem.Roles.Dto;

public class GetRoleForEditOutput
{
    public RoleEditDto Role { get; set; }

    public List<FlatPermissionDto> Permissions { get; set; }

    public List<string> GrantedPermissionNames { get; set; }
    public List<PermissionTreeDto> PermissionsTree { get; set; }
}