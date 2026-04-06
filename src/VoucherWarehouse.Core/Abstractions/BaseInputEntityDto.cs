using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace IBS.VoucherWarehouse.Abstractions;

public abstract record class BaseInputEntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>, IPagedResultRequest,ISortedResultRequest
{
    [Range(0, int.MaxValue)]
    public int SkipCount { get; set; } = 0;
    public static int DefaultMaxResultCount { get; set; } = 10;

    [Range(0, int.MaxValue)]
    public int MaxResultCount { get; set; } = DefaultMaxResultCount;
    public string Sorting { get; set; } = string.Empty;
    public string FilterText { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TPrimaryKey Id { get; set; } = default!;

    
}
