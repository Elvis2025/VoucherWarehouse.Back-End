using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.BackgroundWorker
{
    public class ProcessDgiiImportJobArgs
    {
        public Guid JobId { get; set; }
        public int? TenantId {  get; set; }
    }
}
