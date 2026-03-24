using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseGlobalAdjustments")]
public class EcfVoucherWarehouseGlobalAdjustment : BaseEntity<long>
{
    public long EcfVoucherWarehouseId { get; set; }

    [StringLength(20)]
    public string TipoAjuste { get; set; }

    [StringLength(20)]
    public string IndicadorNorma1007 { get; set; }

    [StringLength(255)]
    public string DescripcionDescuentooRecargo { get; set; }

    [StringLength(20)]
    public string TipoValor { get; set; }

    public decimal ValorDecimal { get; set; }
    public decimal MontoAjuste { get; set; }

    public virtual EcfVoucherWarehouse EcfVoucherWarehouse { get; set; }
}
