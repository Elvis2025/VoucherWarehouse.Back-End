using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailSurcharges")]
public class EcfVoucherWarehouseDetailSurcharge : BaseEntity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }

    [StringLength(50)]
    public string TipoRecargo { get; set; }

    [StringLength(255)]
    public string DescripcionRecargo { get; set; }

    public decimal MontoRecargo { get; set; }
    public decimal PorcentajeRecargo { get; set; }

    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}
