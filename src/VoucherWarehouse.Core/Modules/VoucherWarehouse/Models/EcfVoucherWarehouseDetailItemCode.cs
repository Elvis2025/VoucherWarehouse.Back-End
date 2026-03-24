using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailItemCodes")]
public class EcfVoucherWarehouseDetailItemCode : BaseEntity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }

    [StringLength(50)]
    public string TipoCodigo { get; set; }

    [StringLength(100)]
    public string CodigoItem { get; set; }
    [ForeignKey("EcfVoucherWarehouseDetailsId")]
    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}
