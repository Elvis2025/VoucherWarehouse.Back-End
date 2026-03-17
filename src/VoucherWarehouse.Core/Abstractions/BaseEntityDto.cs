using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract record class BaseEntityDto<TKey> : IEntityDto<TKey>,IFullAudited, IMayHaveTenant, ISoftDelete, IPassivable
{
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public DateTime? DeletionTime { get; set; }
    public long? CreatorUserId { get; set; }
    public long? LastModifierUserId { get; set; }
    public long? DeleterUserId { get; set; }
    public bool IsDeleted { get; set; }
    public int? TenantId { get; set; }
    public bool IsActive { get; set; }
    public TKey Id { get; set; }

    public bool IsTransient()
    {
        throw new NotImplementedException();
    }
}
