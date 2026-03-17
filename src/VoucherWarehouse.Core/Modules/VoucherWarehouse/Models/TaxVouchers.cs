using Abp.Domain.Entities.Auditing;
using IBS.VoucherWarehouse.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

public class TaxVouchers : BaseEntity<int>
{
    public int CompanyToProcessId { get; set; }
    public int? BranchId { get; set; }
    public int TaxVoucherTypeId { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }
    public string Prefix { get; set; }
    public int InitialSequence { get; set; }
    public int CurrentSequence { get; set; }
    public int FinalSequence { get; set; }
    public int RegisteredQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int MinimumToAlert { get; set; }
    public DateTime ExpeditionDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    [ForeignKey("TaxVoucherTypeId")]
    public virtual TaxVouchersTypes TaxVouchersTypes { get; set; }
}
