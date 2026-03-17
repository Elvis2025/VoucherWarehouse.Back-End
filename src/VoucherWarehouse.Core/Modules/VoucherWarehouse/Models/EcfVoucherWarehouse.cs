using Abp.Domain.Entities.Auditing;
using IBS.VoucherWarehouse.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
[Table("EcfVoucherWarehouses")]
public class EcfVoucherWarehouse : BaseEntity<int>
{
    public string Status { get; set; }

}
