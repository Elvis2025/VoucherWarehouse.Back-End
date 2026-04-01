using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto
{
    public sealed record class EcfVoucherJobStatusDto
    {
        public Guid JobId { get; set; }

        public string FileName { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }

        public bool IsCancellationRequested { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsActive { get; set; }

        public decimal ProgressPercentage { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
