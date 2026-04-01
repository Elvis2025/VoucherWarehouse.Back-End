using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto
{
    public sealed record class CancelEcfVoucherJobInputDto
    {
        public Guid JobId { get; set; }
    }
}
