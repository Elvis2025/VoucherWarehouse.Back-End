using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract class BaseEntity<TKey> : AuditedAggregateRoot<TKey>, IMayHaveTenant, ISoftDelete,IPassivable
{
    public int? TenantId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
}
