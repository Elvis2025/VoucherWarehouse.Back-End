using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehouseDetailAdditionalTaxes")]
public class EcfVoucherWarehouseDetailAdditionalTax : Entity<long>
{
    public long EcfVoucherWarehouseDetailsId { get; set; }

    [StringLength(50)]
    public string TipoImpuesto { get; set; }

    public decimal TasaImpuesto { get; set; }
    public decimal MontoImpuesto { get; set; }

    public virtual EcfVoucherWarehouseDetails EcfVoucherWarehouseDetails { get; set; }
}
