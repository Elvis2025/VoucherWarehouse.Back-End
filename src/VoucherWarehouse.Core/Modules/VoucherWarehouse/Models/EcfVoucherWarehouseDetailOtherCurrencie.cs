using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailOtherCurrencies")]
public class EcfVoucherWarehouseDetailOtherCurrency : BaseEntity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }

    [StringLength(10)]
    public string TipoMoneda { get; set; }

    public decimal TipoCambio { get; set; }
    public decimal PrecioOtraMoneda { get; set; }
    public decimal MontoOtraMoneda { get; set; }

    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}
