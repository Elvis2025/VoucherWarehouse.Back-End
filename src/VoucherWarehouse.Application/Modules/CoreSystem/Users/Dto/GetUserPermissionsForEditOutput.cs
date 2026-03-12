using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoucherWarehouse.Modules.CoreSystem.Users.Dto;

[Serializable]
public sealed record class GetUserPermissionsForEditOutput
{
    public List<UserFlatPermissionDto> Permissions { get; set; } = new List<UserFlatPermissionDto>();

    public List<string> GrantedPermissionNames { get; set; } = new List<string>();

    public List<UserPermissionTreeDto> PermissionsTree { get; set; } = new List<UserPermissionTreeDto>();
}