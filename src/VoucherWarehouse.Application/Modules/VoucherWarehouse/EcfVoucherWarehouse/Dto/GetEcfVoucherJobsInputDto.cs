using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto
{
    public sealed record class GetEcfVoucherJobsInputDto
    {
        public bool OnlyActive { get; set; } = true;
        public int MaxResultCount { get; set; } = 20;
    }
}
