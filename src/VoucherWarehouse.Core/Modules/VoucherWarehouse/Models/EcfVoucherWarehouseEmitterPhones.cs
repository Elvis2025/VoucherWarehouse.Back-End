using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseEmitterPhones")]
public class EcfVoucherWarehouseEmitterPhone : BaseEntity<long>
{
    public long EcfVoucherWarehouseId { get; set; }

    [Required]
    [StringLength(50)]
    public string PhoneNumber { get; set; }
    [ForeignKey("EcfVoucherWarehouseId")]
    public virtual EcfVoucherWarehouse EcfVoucherWarehouse { get; set; }
}
