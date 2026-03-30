using IBS.VoucherWarehouse.Abstractions;
using IBS.VoucherWarehouse.Common.GlobalHelpers;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucherTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;

public sealed record class TaxVoucherOutputDto : BaseEntityDto<int>
{
    public string Comment { get; set; }
    public int InitialSequence { get; set; }
    public int TaxVoucherTypeId { get; set; }
    public int CurrentSequence { get; set; }
    public int FinalSequence { get; set; }
    public int RegisteredQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int MinimumToAlert { get; set; }
    public DateTime ExpeditionDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public string ExpirationDateFormatted => ExpirationDate.ToDateDgiiFormat();
    public string ExpeditionDateFormatted => ExpeditionDate.ToDateDgiiFormat();

    public TaxVoucherTypesOutputDto TaxVoucherType { get; set; }
    public string CodeAndDescription => TaxVoucherType is null ? string.Empty : TaxVoucherType.CodeAndDescription;
}
