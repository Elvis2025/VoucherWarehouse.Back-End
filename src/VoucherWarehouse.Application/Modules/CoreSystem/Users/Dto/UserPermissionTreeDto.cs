using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoucherWarehouse.Modules.CoreSystem.Users.Dto;

[Serializable]
public sealed record class UserPermissionTreeDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool IsGranted { get; set; }

    public List<UserPermissionTreeDto> Children { get; set; } = new List<UserPermissionTreeDto>();
}
