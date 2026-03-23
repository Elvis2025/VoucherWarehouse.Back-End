using Abp.Domain.Entities.Auditing;
using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;


public class TaxVouchersTypes : BaseEntity<int>
{
    [StringLength(5)]
    public string Code { get; set; }
    [StringLength(100)]
    public string Description { get; set; }
    public string CodeAndDescription { get { return string.Join(" - ", new string[] { Code, Description }); } }
    public int TaxVoucherLenght { get; set; }
    public string Format { get; set; }
}
