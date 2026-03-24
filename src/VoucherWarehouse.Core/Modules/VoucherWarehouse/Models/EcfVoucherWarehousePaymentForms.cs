using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherWarehousePaymentForms")]
public class EcfVoucherWarehousePaymentForm : Entity<long>
{
    public long EcfVoucherWarehouseId { get; set; }
    public int FormaPago { get; set; }
    public decimal MontoPago { get; set; }
    [ForeignKey("EcfVoucherWarehouseId")]
    public virtual EcfVoucherWarehouse EcfVoucherWarehouse { get; set; }
}
