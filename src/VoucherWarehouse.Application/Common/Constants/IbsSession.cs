using IBS.VoucherWarehouse.Sessions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Common.Constants;

public static class IbsSession
{
    public static TenantLoginInfoDto Tenant { get; set; } = new();
    public static UserLoginInfoDto User { get; set; } = new();
    public static int? TenantId { get; set; } = new();
}
