using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseAdditionalTaxes")]
public class EcfVoucherWarehouseAdditionalTax : BaseEntity<long>
{
    public long EcfVoucherWarehouseId { get; set; }

    [StringLength(50)]
    public string TipoImpuesto { get; set; }

    public decimal TasaImpuesto { get; set; }
    public decimal MontoImpuesto { get; set; }

    public virtual EcfVoucherWarehouse EcfVoucherWarehouse { get; set; }
}
