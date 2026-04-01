using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.ExcelManager;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.BackgroundWorker
{
    [UnitOfWork(false)]
    public class DgiiProcessEcfVoucher : AsyncBackgroundJob<ProcessDgiiImportJobArgs>, ITransientDependency
    {
        private const int MaxDegreeOfParallelism = 1;
        private const int ProgressSaveBatchSize = 25;

        private readonly IIocResolver _iocResolver;
        private readonly IEcfVoucherDocumentJobManagerService _jobManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DgiiProcessEcfVoucher(
            IIocResolver iocResolver,
            IEcfVoucherDocumentJobManagerService jobManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _iocResolver = iocResolver;
            _jobManager = jobManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task ExecuteAsync(ProcessDgiiImportJobArgs args)
        {
            var job = await _jobManager.GetAsync(args.JobId);

            if (job == null)
            {
                throw new Exception($"No se encontró el documento de importación con Id: {args.JobId}");
            }

            await _jobManager.MarkAsProcessingAsync(args.JobId);

            try
            {
                await using var stream = new FileStream(
                    job.FilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                await ExcelEcfManager.ImportAsync(
                    stream,
                    job.FileName,
                    async rows =>
                    {
                        await _jobManager.SetTotalRowsAsync(args.JobId, rows.Count);

                        var result = await ProcessRowsInParallelAsync(args.JobId, rows);

                        var isCancellationRequested = await _jobManager.IsCancellationRequestedAsync(args.JobId);

                        if (isCancellationRequested)
                        {
                            await _jobManager.MarkAsCancelledAsync(
                                args.JobId,
                                result.ProcessedRows,
                                result.SuccessRows,
                                result.FailedRows);

                            return;
                        }

                        if (result.FailedRows > 0)
                        {
                            await _jobManager.MarkAsCompletedWithErrorsAsync(
                                args.JobId,
                                result.ProcessedRows,
                                result.SuccessRows,
                                result.FailedRows);
                        }
                        else
                        {
                            await _jobManager.MarkAsCompletedAsync(
                                args.JobId,
                                result.ProcessedRows,
                                result.SuccessRows,
                                result.FailedRows);
                        }
                    });

                await TryDeleteFileAsync(job.FilePath);
            }
            catch (Exception ex)
            {
                await _jobManager.MarkAsFailedAsync(args.JobId, ex.Message);
                throw;
            }
        }

        private async Task<ParallelProcessResult> ProcessRowsInParallelAsync(
            Guid jobId,
            IReadOnlyList<DgiiExcelImportDto> rows)
        {
            var totalRows = rows.Count;
            var processedRows = 0;
            var successRows = 0;
            var failedRows = 0;

            using var workerSemaphore = new SemaphoreSlim(MaxDegreeOfParallelism, MaxDegreeOfParallelism);
            using var progressSemaphore = new SemaphoreSlim(1, 1);

            var tasks = rows.Select((row, index) =>
                ProcessRowAsync(
                    jobId,
                    row,
                    index + 1,
                    totalRows,
                    workerSemaphore,
                    progressSemaphore,
                    () => processedRows,
                    () => successRows,
                    () => failedRows,
                    () => Interlocked.Increment(ref processedRows),
                    () => Interlocked.Increment(ref successRows),
                    () => Interlocked.Increment(ref failedRows)))
                .ToArray();

            await Task.WhenAll(tasks);

            await _jobManager.SaveProgressSnapshotAsync(
                jobId,
                processedRows,
                successRows,
                failedRows);

            return new ParallelProcessResult
            {
                ProcessedRows = processedRows,
                SuccessRows = successRows,
                FailedRows = failedRows
            };
        }

        private async Task ProcessRowAsync(
            Guid jobId,
            DgiiExcelImportDto row,
            int rowNumber,
            int totalRows,
            SemaphoreSlim workerSemaphore,
            SemaphoreSlim progressSemaphore,
            Func<int> processedRowsRef,
            Func<int> successRowsRef,
            Func<int> failedRowsRef,
            Func<int> incrementProcessed,
            Func<int> incrementSuccess,
            Func<int> incrementFailed)
        {
            await workerSemaphore.WaitAsync();

            try
            {
                var isCancellationRequested = await _jobManager.IsCancellationRequestedAsync(jobId);
                if (isCancellationRequested)
                {
                    return;
                }

                try
                {
                    await ProcessRowInIsolatedScopeAsync(row, rowNumber);
                    incrementSuccess();
                }
                catch (Exception ex)
                {
                    incrementFailed();
                    await _jobManager.AppendRowErrorAsync(jobId, rowNumber, ex.Message);
                }
                finally
                {
                    var processed = incrementProcessed();

                    if (processed % ProgressSaveBatchSize == 0 || processed == totalRows)
                    {
                        await progressSemaphore.WaitAsync();

                        try
                        {
                            await _jobManager.SaveProgressSnapshotAsync(
                                jobId,
                                processedRowsRef(),
                                successRowsRef(),
                                failedRowsRef());
                        }
                        finally
                        {
                            progressSemaphore.Release();
                        }
                    }
                }
            }
            finally
            {
                workerSemaphore.Release();
            }
        }

        private async Task ProcessRowInIsolatedScopeAsync(
            DgiiExcelImportDto row,
            int rowNumber)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var processor = _iocResolver.ResolveAsDisposable<IEcfVoucherWarehouseAppService>();

            await processor.Object.ProcessAsync(row, rowNumber);

            await uow.CompleteAsync();
        }

        private static Task TryDeleteFileAsync(string filePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }

            return Task.CompletedTask;
        }

        private sealed class ParallelProcessResult
        {
            public int ProcessedRows { get; set; }
            public int SuccessRows { get; set; }
            public int FailedRows { get; set; }
        }
    }
}