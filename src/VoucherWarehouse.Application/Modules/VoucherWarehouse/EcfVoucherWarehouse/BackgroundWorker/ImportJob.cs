using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.BackgroundWorker
{
    public class ImportJob
    {
        public Guid JobId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
