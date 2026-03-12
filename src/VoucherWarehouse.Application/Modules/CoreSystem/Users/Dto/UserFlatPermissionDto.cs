using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.CoreSystem.Users.Dto;

[Serializable]
public sealed record class UserFlatPermissionDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }
}
