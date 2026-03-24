using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailDiscounts")]
public class EcfVoucherWarehouseDetailDiscount : BaseEntity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }

    [StringLength(50)]
    public string TipoDescuento { get; set; }

    [StringLength(255)]
    public string DescripcionDescuento { get; set; }

    public decimal MontoDescuento { get; set; }
    public decimal PorcentajeDescuento { get; set; }

    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}