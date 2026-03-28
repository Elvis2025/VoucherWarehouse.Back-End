using IBS.VoucherWarehouse.Common.GlobalHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.TaxVoucher.Dto;

public sealed record class TaxVoucherSecuenceDto
{
    public string Number { get; set; }

	private string expirationDate;
	public string ExpirationDate
    {
		get => expirationDate.ToDateDgiiFormat();

		set => expirationDate = value; 
	}

}
