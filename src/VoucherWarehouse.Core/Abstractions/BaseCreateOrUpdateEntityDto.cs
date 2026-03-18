using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract record class BaseCreateOrUpdateEntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>, IPassivable
{
    public TPrimaryKey Id { get; set; }
    public bool IsActive { get; set; }
}
