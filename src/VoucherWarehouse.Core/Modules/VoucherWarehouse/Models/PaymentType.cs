using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("PaymentTypes")]
public class PaymentType : BaseEntity<int>
{
    [Required]
    [NotNull]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [NotNull]
    public int DgiiCode { get; set; }

}
