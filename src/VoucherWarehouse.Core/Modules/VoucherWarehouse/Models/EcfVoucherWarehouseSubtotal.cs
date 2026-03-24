using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseSubtotals")]
public class EcfVoucherWarehouseSubtotal : BaseEntity<long>
{
    public long EcfVoucherWarehouseId { get; set; }

    public int NumeroSubTotal { get; set; }

    [StringLength(255)]
    public string DescripcionSubTotal { get; set; }

    public int Orden { get; set; }
    public decimal SubTotal { get; set; }

    public virtual EcfVoucherWarehouse EcfVoucherWarehouse { get; set; }
}
