using IBS.VoucherWarehouse.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("PaymentMethods")]
public class PaymentMethod : BaseEntity<int>
{
    [Required]
    [NotNull]
    [StringLength(50)]
    public string Name { get; set; }
    [Required]
    [NotNull]
    public int DggiiCode { get; set; }
}
