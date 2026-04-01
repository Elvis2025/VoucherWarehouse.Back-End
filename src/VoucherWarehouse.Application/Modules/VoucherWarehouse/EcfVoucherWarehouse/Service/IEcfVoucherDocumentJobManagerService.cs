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

    Task MarkAsProcessingAsync(Guid jobId);

    Task SetTotalRowsAsync(Guid jobId, int totalRows);

    Task SaveProgressSnapshotAsync(Guid jobId, int processedRows, int successRows, int failedRows);

    Task AppendRowErrorAsync(Guid jobId, int rowNumber, string errorMessage);

    Task MarkAsCompletedAsync(Guid jobId, int processedRows, int successRows, int failedRows);

    Task MarkAsCompletedWithErrorsAsync(Guid jobId, int processedRows, int successRows, int failedRows);

    Task MarkAsCancelledAsync(Guid jobId, int processedRows, int successRows, int failedRows);

    Task MarkAsFailedAsync(Guid jobId, string errorMessage);

    Task<bool> IsCancellationRequestedAsync(Guid jobId);
    Task RequestCancellationAsync(Guid jobId);
}
