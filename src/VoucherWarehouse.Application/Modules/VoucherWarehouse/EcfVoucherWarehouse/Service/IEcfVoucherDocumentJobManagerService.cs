using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;

public interface IEcfVoucherDocumentJobManagerService
{
    Task<EcfVoucherDocumentJob> GetAsync(Guid jobId);

    Task<bool> TryAcquireProcessingLeaseAsync(
        Guid jobId,
        string workerInstanceId,
        TimeSpan leaseDuration);

    Task<bool> RenewLeaseAsync(
        Guid jobId,
        string workerInstanceId,
        TimeSpan leaseDuration);

    Task<bool> IsOwnedByWorkerAsync(
        Guid jobId,
        string workerInstanceId);

    Task<bool> CanDeleteFileAsync(
        Guid jobId,
        string workerInstanceId);

    Task SetTotalRowsAsync(
        Guid jobId,
        int totalRows,
        string workerInstanceId);

    Task SaveProgressSnapshotAsync(
        Guid jobId,
        int processedRows,
        int successRows,
        int failedRows,
        string workerInstanceId);

    Task AppendRowErrorAsync(
        Guid jobId,
        int rowNumber,
        string errorMessage,
        string workerInstanceId);

    Task MarkAsCompletedAsync(
        Guid jobId,
        int processedRows,
        int successRows,
        int failedRows,
        string workerInstanceId);

    Task MarkAsCompletedWithErrorsAsync(
        Guid jobId,
        int processedRows,
        int successRows,
        int failedRows,
        string workerInstanceId);

    Task MarkAsCancelledAsync(
        Guid jobId,
        int processedRows,
        int successRows,
        int failedRows,
        string workerInstanceId);

    Task MarkAsFailedAsync(
        Guid jobId,
        string errorMessage,
        string workerInstanceId);

    Task<bool> IsCancellationRequestedAsync(Guid jobId);

    Task RequestCancellationAsync(Guid jobId);

    Task<List<EcfVoucherDocumentJob>> GetRecoverableJobsAsync();

    Task MarkAsPendingForRecoveryAsync(Guid jobId);
}
