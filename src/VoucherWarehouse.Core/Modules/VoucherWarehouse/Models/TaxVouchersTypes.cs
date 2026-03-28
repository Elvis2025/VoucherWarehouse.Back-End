using Abp.Domain.Entities.Auditing;
using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;


public class TaxVouchersTypes : BaseEntity<int>
{
    [StringLength(5)]
    public string Code { get; set; }
    [StringLength(100)]
    public string Name { get; set; }
}
