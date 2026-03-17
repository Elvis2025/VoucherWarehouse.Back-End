using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract class BaseEntity<TKey> : FullAuditedEntity<TKey>, IMayHaveTenant,IPassivable
{
    public int? TenantId { get; set; }
    public bool IsActive { get; set; }
}
