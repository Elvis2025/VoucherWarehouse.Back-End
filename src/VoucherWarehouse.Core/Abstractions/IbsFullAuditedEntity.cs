using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace IBS.VoucherWarehouse.Authorization.Abstractions;

public abstract class IbsFullAuditedEntity<TEntity> : FullAuditedEntity<TEntity>, IMayHaveTenant
{
    public int? TenantId { get; set; }
}
