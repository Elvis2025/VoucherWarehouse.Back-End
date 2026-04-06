using IBS.VoucherWarehouse.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;

[Table("EcfVoucherDocumentJobs")]
public class EcfVoucherDocumentJob : BaseEntity<Guid>
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public int TotalRows { get; set; }
    public int ProcessedRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public string Status { get; set; }
    public bool IsCancellationRequested { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string WorkerInstanceId { get; set; }
    public DateTime? HeartbeatAt { get; set; }
    public DateTime? LeaseExpiresAt { get; set; }
    public DateTime? LastProgressAt { get; set; }
    public int ConcurrencyStamp { get; set; }
    public string ExecutionId { get; set; }
}
