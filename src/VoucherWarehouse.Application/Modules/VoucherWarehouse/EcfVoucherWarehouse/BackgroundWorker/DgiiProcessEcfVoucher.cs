using Abp.Auditing;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Dto;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.ExcelManager;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int HeartbeatSeconds = 10;
        private const int LeaseDurationMinutes = 2;
        private const string JobLoggerName = "EcfVoucherBackgroundJob.DgiiProcessEcfVoucher";

        private readonly IIocResolver _iocResolver;
        private readonly IEcfVoucherDocumentJobManagerService _jobManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger _jobLogger;
        private readonly IAuditingStore _auditingStore;
        private readonly IAbpSession _abpSession;

        public DgiiProcessEcfVoucher(
            IIocResolver iocResolver,
            IEcfVoucherDocumentJobManagerService jobManager,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory,
            IAuditingStore auditingStore,
            IAbpSession abpSession)
        {
            _iocResolver = iocResolver;
            _jobManager = jobManager;
            _unitOfWorkManager = unitOfWorkManager;
            _jobLogger = loggerFactory.CreateLogger(JobLoggerName);
            _auditingStore = auditingStore;
            _abpSession = abpSession;
        }

        public override async Task ExecuteAsync(ProcessDgiiImportJobArgs args)
        {
            var workerInstanceId = Guid.NewGuid().ToString("N");

            var job = await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.GetAsync(args.JobId));

            if (job is null)
            {
                var notFoundException = new Exception($"No se encontró el documento de importación con Id: {args.JobId}");
                await LogAndAuditAsync(notFoundException, nameof(ExecuteAsync), new { args.JobId, workerInstanceId });
                return;
            }

            var acquired = await ExecuteInTenantScopeAsync(
                args.TenantId,
                () => _jobManager.TryAcquireProcessingLeaseAsync(
                    args.JobId,
                    workerInstanceId,
                    TimeSpan.FromMinutes(LeaseDurationMinutes)));

            if (!acquired)
            {
                _jobLogger.LogInformation(
                    "El job {JobId} no pudo ser adquirido por el worker {WorkerInstanceId} porque ya está siendo procesado por otra instancia activa.",
                    args.JobId,
                    workerInstanceId);

                return;
            }

            using var heartbeatCts = new CancellationTokenSource();
            var heartbeatTask = RunHeartbeatLoopAsync(args.JobId, args.TenantId, workerInstanceId, heartbeatCts.Token);

            try
            {
                job = await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.GetAsync(args.JobId));

                if (job == null)
                {
                    throw new Exception($"No se encontró el documento de importación con Id: {args.JobId} después de adquirir el lease.");
                }

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
                        if (!await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.IsOwnedByWorkerAsync(args.JobId, workerInstanceId)))
                        {
                            _jobLogger.LogWarning(
                                "El worker {WorkerInstanceId} perdió la propiedad del job {JobId} antes de iniciar el procesamiento de filas.",
                                workerInstanceId,
                                args.JobId);

                            return;
                        }

                        await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.SetTotalRowsAsync(args.JobId, rows.Count, workerInstanceId));

                        var currentState = await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.GetAsync(args.JobId));
                        if (currentState == null)
                        {
                            throw new Exception($"No se pudo obtener el estado actual del job {args.JobId}.");
                        }

                        var result = await ProcessRowsInParallelAsync(
                            args.JobId,
                            args.TenantId,
                            workerInstanceId,
                            rows,
                            currentState.ProcessedRows,
                            currentState.SuccessRows,
                            currentState.FailedRows);

                        if (!await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.IsOwnedByWorkerAsync(args.JobId, workerInstanceId)))
                        {
                            _jobLogger.LogWarning(
                                "El worker {WorkerInstanceId} perdió la propiedad del job {JobId} antes del cierre del procesamiento.",
                                workerInstanceId,
                                args.JobId);

                            return;
                        }

                        var isCancellationRequested = await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.IsCancellationRequestedAsync(args.JobId));

                        if (isCancellationRequested)
                        {
                            await ExecuteInTenantScopeAsync(
                                args.TenantId,
                                () => _jobManager.MarkAsCancelledAsync(
                                    args.JobId,
                                    result.ProcessedRows,
                                    result.SuccessRows,
                                    result.FailedRows,
                                    workerInstanceId));

                            return;
                        }

                        if (result.FailedRows > 0)
                        {
                            await ExecuteInTenantScopeAsync(
                                args.TenantId,
                                () => _jobManager.MarkAsCompletedWithErrorsAsync(
                                    args.JobId,
                                    result.ProcessedRows,
                                    result.SuccessRows,
                                    result.FailedRows,
                                    workerInstanceId));
                        }
                        else
                        {
                            await ExecuteInTenantScopeAsync(
                                args.TenantId,
                                () => _jobManager.MarkAsCompletedAsync(
                                    args.JobId,
                                    result.ProcessedRows,
                                    result.SuccessRows,
                                    result.FailedRows,
                                    workerInstanceId));
                        }
                    });

                if (await ExecuteInTenantScopeAsync(args.TenantId, () => _jobManager.CanDeleteFileAsync(args.JobId, workerInstanceId)))
                {
                    await TryDeleteFileAsync(job.FilePath);
                }
            }
            catch (OwnershipLostException ownershipEx)
            {
                _jobLogger.LogWarning(
                    ownershipEx,
                    "El worker {WorkerInstanceId} perdió la propiedad del job {JobId}. Se detendrá sin marcar error terminal.",
                    workerInstanceId,
                    args.JobId);
            }
            catch (Exception ex)
            {
                var errorMessage = BuildErrorMessage(ex);

                await ExecuteInTenantScopeAsync(
                    args.TenantId,
                    () => _jobManager.MarkAsFailedAsync(args.JobId, errorMessage, workerInstanceId));

                await LogAndAuditAsync(ex, nameof(ExecuteAsync), new
                {
                    args.JobId,
                    workerInstanceId,
                    job?.FileName,
                    job?.FilePath
                });
            }
            finally
            {
                heartbeatCts.Cancel();

                try
                {
                    await heartbeatTask;
                }
                catch
                {
                }
            }
        }

        private async Task<ParallelProcessResult> ProcessRowsInParallelAsync(
            Guid jobId,
            int? tenantId,
            string workerInstanceId,
            IReadOnlyList<DgiiExcelImportDto> rows,
            int alreadyProcessedRows,
            int currentSuccessRows,
            int currentFailedRows)
        {
            var totalRows = rows.Count;
            var processedRows = alreadyProcessedRows;
            var successRows = currentSuccessRows;
            var failedRows = currentFailedRows;

            if (alreadyProcessedRows >= totalRows)
            {
                await ExecuteInTenantScopeAsync(
                    tenantId,
                    () => _jobManager.SaveProgressSnapshotAsync(
                        jobId,
                        processedRows,
                        successRows,
                        failedRows,
                        workerInstanceId));

                return new ParallelProcessResult
                {
                    ProcessedRows = processedRows,
                    SuccessRows = successRows,
                    FailedRows = failedRows
                };
            }

            var rowsToProcess = rows
                .Skip(alreadyProcessedRows)
                .ToList();

            using var workerSemaphore = new SemaphoreSlim(MaxDegreeOfParallelism, MaxDegreeOfParallelism);
            using var progressSemaphore = new SemaphoreSlim(1, 1);

            var tasks = rowsToProcess.Select((row, index) =>
                ProcessRowAsync(
                    jobId,
                    tenantId,
                    workerInstanceId,
                    row,
                    alreadyProcessedRows + index + 1,
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

            await EnsureOwnershipAsync(jobId, tenantId, workerInstanceId);

            await ExecuteInTenantScopeAsync(
                tenantId,
                () => _jobManager.SaveProgressSnapshotAsync(
                    jobId,
                    processedRows,
                    successRows,
                    failedRows,
                    workerInstanceId));

            return new ParallelProcessResult
            {
                ProcessedRows = processedRows,
                SuccessRows = successRows,
                FailedRows = failedRows
            };
        }

        private async Task ProcessRowAsync(
            Guid jobId,
            int? tenantId,
            string workerInstanceId,
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
                await EnsureOwnershipAsync(jobId, tenantId, workerInstanceId);

                var isCancellationRequested = await ExecuteInTenantScopeAsync(tenantId, () => _jobManager.IsCancellationRequestedAsync(jobId));
                if (isCancellationRequested)
                {
                    return;
                }

                try
                {
                    await ProcessRowInIsolatedScopeAsync(tenantId, row, rowNumber);
                    incrementSuccess();
                }
                catch (Exception ex)
                {
                    incrementFailed();

                    var errorMessage = BuildErrorMessage(ex);

                    await ExecuteInTenantScopeAsync(
                        tenantId,
                        () => _jobManager.AppendRowErrorAsync(jobId, rowNumber, errorMessage, workerInstanceId));

                    await LogAndAuditAsync(ex, nameof(ProcessRowAsync), new
                    {
                        jobId,
                        workerInstanceId,
                        rowNumber,
                        eNCF = row?.ENCF,
                        tipoeCF = row?.TipoeCF
                    });
                }
                finally
                {
                    var processed = incrementProcessed();

                    await ExecuteInTenantScopeAsync(
                        tenantId,
                        () => _jobManager.SaveProgressSnapshotAsync(
                            jobId,
                            processedRowsRef(),
                            successRowsRef(),
                            failedRowsRef(),
                            workerInstanceId));

                    if (processed % ProgressSaveBatchSize == 0 || processed == totalRows)
                    {
                        await progressSemaphore.WaitAsync();

                        try
                        {
                            await ExecuteInTenantScopeAsync(
                                tenantId,
                                () => _jobManager.SaveProgressSnapshotAsync(
                                    jobId,
                                    processedRowsRef(),
                                    successRowsRef(),
                                    failedRowsRef(),
                                    workerInstanceId));
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
            int? tenantId,
            DgiiExcelImportDto row,
            int rowNumber)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                using var processor = _iocResolver.ResolveAsDisposable<IEcfVoucherWarehouseAppService>();

                await processor.Object.ProcessAsync(row, rowNumber, tenantId);
            }

            await uow.CompleteAsync();
        }

        private async Task RunHeartbeatLoopAsync(Guid jobId, int? tenantId, string workerInstanceId, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(HeartbeatSeconds), cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var renewed = await ExecuteInTenantScopeAsync(
                        tenantId,
                        () => _jobManager.RenewLeaseAsync(
                            jobId,
                            workerInstanceId,
                            TimeSpan.FromMinutes(LeaseDurationMinutes)));

                    if (!renewed)
                    {
                        _jobLogger.LogWarning(
                            "No se pudo renovar el lease del job {JobId} para el worker {WorkerInstanceId}.",
                            jobId,
                            workerInstanceId);

                        break;
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _jobLogger.LogError(
                        ex,
                        "Error renovando heartbeat del job {JobId} para el worker {WorkerInstanceId}.",
                        jobId,
                        workerInstanceId);
                }
            }
        }

        private async Task EnsureOwnershipAsync(Guid jobId, int? tenantId, string workerInstanceId)
        {
            var isOwned = await ExecuteInTenantScopeAsync(
                tenantId,
                () => _jobManager.IsOwnedByWorkerAsync(jobId, workerInstanceId));

            if (!isOwned)
            {
                return;
            }
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

        private async Task ExecuteInTenantScopeAsync(int? tenantId, Func<Task> action)
        {
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            }))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await action();
                    await uow.CompleteAsync();
                }
            }
        }

        private async Task<T> ExecuteInTenantScopeAsync<T>(int? tenantId, Func<Task<T>> action)
        {
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            }))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var result = await action();
                    await uow.CompleteAsync();
                    return result;
                }
            }
        }

        private async Task LogAndAuditAsync(Exception ex, string methodName, object parameters = null)
        {
            _jobLogger.LogError(ex, "Error en {MethodName}.", methodName);

            try
            {
                await _auditingStore.SaveAsync(new AuditInfo
                {
                    ServiceName = nameof(DgiiProcessEcfVoucher),
                    MethodName = methodName,
                    Parameters = parameters == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(parameters),
                    ExecutionTime = DateTime.Now,
                    ExecutionDuration = 0,
                    TenantId = _abpSession.TenantId,
                    UserId = _abpSession.UserId,
                    Exception = ex,
                    BrowserInfo = JobLoggerName,
                    ClientName = Environment.MachineName,
                    CustomData = $"Background job error: {ex.Message}"
                });
            }
            catch (Exception auditEx)
            {
                _jobLogger.LogError(auditEx, "No se pudo registrar la auditoría para {MethodName}.", methodName);
            }
        }

        private static string BuildErrorMessage(Exception ex)
        {
            if (ex == null)
            {
                return "Error no controlado.";
            }

            return ex.InnerException == null
                ? ex.Message
                : $"{ex.Message} | Inner: {ex.InnerException.Message}";
        }

        private sealed class ParallelProcessResult
        {
            public int ProcessedRows { get; set; }
            public int SuccessRows { get; set; }
            public int FailedRows { get; set; }
        }

        private sealed class OwnershipLostException : Exception
        {
            public OwnershipLostException(string message)
                : base(message)
            {
            }
        }
    }
}