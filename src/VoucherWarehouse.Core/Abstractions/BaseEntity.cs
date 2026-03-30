using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract class BaseEntity<TKey> : FullAuditedEntity<TKey>,IPassivable,IMayHaveTenant
{
    public int? TenantId { get; set; }
    public bool IsActive { get; set; }
}
