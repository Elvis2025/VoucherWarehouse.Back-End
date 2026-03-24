using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailRetentions")]
public class EcfVoucherWarehouseDetailRetention : BaseEntity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }

    [StringLength(50)]
    public string IndicadorAgenteRetencionOpcion { get; set; }

    public decimal MontoITBISRetenido { get; set; }
    public decimal MontoISRRetenido { get; set; }

    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}
