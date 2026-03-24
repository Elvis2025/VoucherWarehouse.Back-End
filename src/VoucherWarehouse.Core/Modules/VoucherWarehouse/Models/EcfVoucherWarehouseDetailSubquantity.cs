using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailSubquantities")]
public class EcfVoucherWarehouseDetailSubquantity : BaseEntity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }
    public decimal Subcantidad { get; set; }
    public int CodigoSubcantidad { get; set; }
    [ForeignKey("EcfVoucherWarehouseDetailsId")]
    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}

