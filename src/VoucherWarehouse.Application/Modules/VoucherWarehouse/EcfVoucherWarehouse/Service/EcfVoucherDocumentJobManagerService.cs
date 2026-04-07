using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using IBS.VoucherWarehouse.Common.Constants;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service
{
    public class EcfVoucherDocumentJobManagerService : IEcfVoucherDocumentJobManagerService, ITransientDependency
    {
        private const string JobLoggerName = "EcfVoucherBackgroundJob.JobManager";

        private readonly ILogger _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIocResolver _iocResolver;
        private readonly IAuditingStore _auditingStore;
        private readonly IAbpSession _abpSession;

        public EcfVoucherDocumentJobManagerService(
            ILoggerFactory loggerFactory,
            IUnitOfWorkManager unitOfWorkManager,
            IIocResolver iocResolver,
            IAuditingStore auditingStore,
            IAbpSession abpSession)
        {
            _logger = loggerFactory.CreateLogger(JobLoggerName);
            _unitOfWorkManager = unitOfWorkManager;
            _iocResolver = iocResolver;
            _auditingStore = auditingStore;
            _abpSession = abpSession;
        }

        public async Task<EcfVoucherDocumentJob> GetAsync(Guid jobId)
        {
            try
            {
                return await ExecuteReadAsync(async repository =>
                {
                    return await repository.FirstOrDefaultAsync(jobId);
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(GetAsync), new { jobId });
                return null;
            }
        }

        public async Task<bool> TryAcquireProcessingLeaseAsync(
            Guid jobId,
            string workerInstanceId,
            TimeSpan leaseDuration)
        {
            try
            {
                return await ExecuteWriteWithResultAsync(jobId, async job =>
                {
                    if (job == null || job.IsDeleted)
                    {
                        return false;
                    }

                    var now = DateTime.Now;

                    var canAcquire =
                        job.Status == JobStatus.Pending ||
                        (job.Status == JobStatus.Processing &&
                         (!job.LeaseExpiresAt.HasValue || job.LeaseExpiresAt.Value <= now));

                    if (!canAcquire)
                    {
                        return false;
                    }

                    job.Status = JobStatus.Processing;

                    if (!job.StartTime.HasValue)
                    {
                        job.StartTime = now;
                    }

                    job.EndTime = null;
                    job.ErrorMessage = null;
                    job.WorkerInstanceId = workerInstanceId;
                    job.HeartbeatAt = now;
                    job.LeaseExpiresAt = now.Add(leaseDuration);

                    // Si agregaste ExecutionId en BD y quieres reutilizarlo:
                    job.ExecutionId = workerInstanceId;

                    return true;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(jobId, ex, nameof(TryAcquireProcessingLeaseAsync), new
                {
                    jobId,
                    workerInstanceId,
                    leaseDuration = leaseDuration.TotalSeconds
                });

                throw;
            }
        }

        public async Task<bool> RenewLeaseAsync(
            Guid jobId,
            string workerInstanceId,
            TimeSpan leaseDuration)
        {
            try
            {
                return await ExecuteWriteWithResultAsync(jobId, async job =>
                {
                    if (job == null || job.IsDeleted)
                    {
                        return false;
                    }

                    if (job.Status != JobStatus.Processing)
                    {
                        return false;
                    }

                    if (!string.Equals(job.WorkerInstanceId, workerInstanceId, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    var now = DateTime.Now;

                    job.HeartbeatAt = now;
                    job.LeaseExpiresAt = now.Add(leaseDuration);

                    // Opcional, por consistencia si existe columna
                    job.ExecutionId = workerInstanceId;

                    return true;
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(RenewLeaseAsync), new
                {
                    jobId,
                    workerInstanceId,
                    leaseDuration = leaseDuration.TotalSeconds
                });

                return false;
            }
        }

        public async Task<bool> IsOwnedByWorkerAsync(Guid jobId, string workerInstanceId)
        {
            try
            {
                return await ExecuteReadAsync(async repository =>
                {
                    var job = await repository.FirstOrDefaultAsync(jobId);

                    if (job == null || job.IsDeleted)
                    {
                        return false;
                    }

                    if (job.Status != JobStatus.Processing)
                    {
                        return false;
                    }

                    if (!string.Equals(job.WorkerInstanceId, workerInstanceId, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    if (job.LeaseExpiresAt.HasValue && job.LeaseExpiresAt.Value < DateTime.Now)
                    {
                        return false;
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(IsOwnedByWorkerAsync), new { jobId, workerInstanceId });
                return false;
            }
        }

        public async Task<bool> CanDeleteFileAsync(Guid jobId, string workerInstanceId)
        {
            try
            {
                return await ExecuteReadAsync(async repository =>
                {
                    var job = await repository.FirstOrDefaultAsync(jobId);

                    if (job == null || job.IsDeleted)
                    {
                        return false;
                    }

                    var isTerminal =
                        job.Status == JobStatus.Completed ||
                        job.Status == JobStatus.CompletedWithErrors ||
                        job.Status == JobStatus.Failed ||
                        job.Status == JobStatus.Cancelled;

                    if (!isTerminal)
                    {
                        return false;
                    }

                    // Permite borrar si ya terminó y no tiene owner,
                    // o si todavía el owner es el mismo worker.
                    return string.IsNullOrWhiteSpace(job.WorkerInstanceId) ||
                           string.Equals(job.WorkerInstanceId, workerInstanceId, StringComparison.Ordinal);
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(CanDeleteFileAsync), new { jobId, workerInstanceId });
                return false;
            }
        }

        public async Task SetTotalRowsAsync(Guid jobId, int totalRows, string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    if (job.TotalRows <= 0)
                    {
                        job.TotalRows = totalRows;
                    }

                    job.HeartbeatAt = DateTime.Now;
                    job.LeaseExpiresAt = DateTime.Now.AddMinutes(2);
                    job.ExecutionId = workerInstanceId;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(
                    jobId,
                    ex,
                    nameof(SetTotalRowsAsync),
                    new { jobId, totalRows, workerInstanceId });

                throw;
            }
        }

        public async Task SaveProgressSnapshotAsync(
            Guid jobId,
            int processedRows,
            int successRows,
            int failedRows,
            string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    job.ProcessedRows = processedRows;
                    job.SuccessRows = successRows;
                    job.FailedRows = failedRows;
                    job.HeartbeatAt = DateTime.Now;
                    job.LeaseExpiresAt = DateTime.Now.AddMinutes(2);
                    job.ExecutionId = workerInstanceId;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(
                    jobId,
                    ex,
                    nameof(SaveProgressSnapshotAsync),
                    new { jobId, processedRows, successRows, failedRows, workerInstanceId });

                throw;
            }
        }

        public async Task AppendRowErrorAsync(
            Guid jobId,
            int rowNumber,
            string errorMessage,
            string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    var newErrorLine = $"Row {rowNumber}: {errorMessage}";

                    job.ErrorMessage = string.IsNullOrWhiteSpace(job.ErrorMessage)
                        ? newErrorLine
                        : $"{job.ErrorMessage}{Environment.NewLine}{newErrorLine}";

                    job.HeartbeatAt = DateTime.Now;
                    job.LeaseExpiresAt = DateTime.Now.AddMinutes(2);
                    job.ExecutionId = workerInstanceId;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(
                    jobId,
                    ex,
                    nameof(AppendRowErrorAsync),
                    new { jobId, rowNumber, errorMessage, workerInstanceId });

                throw;
            }
        }

        public async Task MarkAsCompletedAsync(
            Guid jobId,
            int processedRows,
            int successRows,
            int failedRows,
            string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    job.Status = JobStatus.Completed;
                    job.ProcessedRows = processedRows;
                    job.SuccessRows = successRows;
                    job.FailedRows = failedRows;
                    job.EndTime = DateTime.Now;
                    job.HeartbeatAt = null;
                    job.LeaseExpiresAt = null;

                    // Ojo: dejamos el owner null al cerrar
                    job.WorkerInstanceId = null;
                    job.ExecutionId = null;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(
                    jobId,
                    ex,
                    nameof(MarkAsCompletedAsync),
                    new { jobId, processedRows, successRows, failedRows, workerInstanceId });

                throw;
            }
        }

        public async Task MarkAsCompletedWithErrorsAsync(
            Guid jobId,
            int processedRows,
            int successRows,
            int failedRows,
            string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    job.Status = JobStatus.CompletedWithErrors;
                    job.ProcessedRows = processedRows;
                    job.SuccessRows = successRows;
                    job.FailedRows = failedRows;
                    job.EndTime = DateTime.Now;
                    job.HeartbeatAt = null;
                    job.LeaseExpiresAt = null;
                    job.WorkerInstanceId = null;
                    job.ExecutionId = null;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(
                    jobId,
                    ex,
                    nameof(MarkAsCompletedWithErrorsAsync),
                    new { jobId, processedRows, successRows, failedRows, workerInstanceId });

                throw;
            }
        }

        public async Task MarkAsCancelledAsync(
            Guid jobId,
            int processedRows,
            int successRows,
            int failedRows,
            string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    job.Status = JobStatus.Cancelled;
                    job.ProcessedRows = processedRows;
                    job.SuccessRows = successRows;
                    job.FailedRows = failedRows;
                    job.EndTime = DateTime.Now;
                    job.HeartbeatAt = null;
                    job.LeaseExpiresAt = null;
                    job.WorkerInstanceId = null;
                    job.ExecutionId = null;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(
                    jobId,
                    ex,
                    nameof(MarkAsCancelledAsync),
                    new { jobId, processedRows, successRows, failedRows, workerInstanceId });

                throw;
            }
        }

        public async Task MarkAsFailedAsync(Guid jobId, string errorMessage, string workerInstanceId)
        {
            try
            {
                await ExecuteOwnedWriteAsync(jobId, workerInstanceId, async job =>
                {
                    job.Status = JobStatus.Failed;
                    job.ErrorMessage = errorMessage;
                    job.EndTime = DateTime.Now;
                    job.HeartbeatAt = null;
                    job.LeaseExpiresAt = null;
                    job.WorkerInstanceId = null;
                    job.ExecutionId = null;
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(
                    ex,
                    nameof(MarkAsFailedAsync),
                    new { jobId, errorMessage, workerInstanceId });

                throw;
            }
        }

        public async Task<bool> IsCancellationRequestedAsync(Guid jobId)
        {
            try
            {
                return await ExecuteReadAsync(async repository =>
                {
                    var job = await repository.GetAsync(jobId);
                    return job.IsCancellationRequested;
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(IsCancellationRequestedAsync), new { jobId });
                throw;
            }
        }

        public async Task RequestCancellationAsync(Guid jobId)
        {
            try
            {
                await ExecuteWriteAsync(jobId, async job =>
                {
                    if (job.Status == JobStatus.Completed ||
                        job.Status == JobStatus.CompletedWithErrors ||
                        job.Status == JobStatus.Failed ||
                        job.Status == JobStatus.Cancelled)
                    {
                        return;
                    }

                    job.IsCancellationRequested = true;
                });
            }
            catch (Exception ex)
            {
                await HandleWriteFailureAsync(jobId, ex, nameof(RequestCancellationAsync), new { jobId });
                throw;
            }
        }

        public async Task<List<EcfVoucherDocumentJob>> GetRecoverableJobsAsync()
        {
            try
            {
                return await ExecuteReadAsync(async repository =>
                {
                    var now = DateTime.Now;

                    return await repository.GetAll()
                        .Where(x =>
                            !x.IsDeleted &&
                            x.Status == JobStatus.Processing &&
                            x.LeaseExpiresAt.HasValue &&
                            x.LeaseExpiresAt.Value <= now)
                        .ToListAsync();
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(GetRecoverableJobsAsync));
                return new List<EcfVoucherDocumentJob>();
            }
        }

        public async Task MarkAsPendingForRecoveryAsync(Guid jobId)
        {
            try
            {
                await ExecuteWriteAsync(jobId, async job =>
                {
                    if (job.Status != JobStatus.Processing)
                    {
                        return;
                    }

                    if (job.LeaseExpiresAt.HasValue && job.LeaseExpiresAt.Value > DateTime.Now)
                    {
                        return;
                    }

                    job.Status = JobStatus.Pending;
                    job.WorkerInstanceId = null;
                    job.ExecutionId = null;
                    job.HeartbeatAt = null;
                    job.LeaseExpiresAt = null;
                });
            }
            catch (Exception ex)
            {
                await LogAndAuditAsync(ex, nameof(MarkAsPendingForRecoveryAsync), new { jobId });
                throw;
            }
        }

        private async Task ExecuteOwnedWriteAsync(Guid jobId, string workerInstanceId, Func<EcfVoucherDocumentJob, Task> action)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            if (job == null ||
                job.IsDeleted ||
                job.Status != JobStatus.Processing ||
                !string.Equals(job.WorkerInstanceId, workerInstanceId, StringComparison.Ordinal) ||
                (job.LeaseExpiresAt.HasValue && job.LeaseExpiresAt.Value < DateTime.Now))
            {
                await uow.CompleteAsync();
                return;
            }

            await action(job);
            await uow.CompleteAsync();
        }

        private async Task ExecuteWriteAsync(Guid jobId, Func<EcfVoucherDocumentJob, Task> action)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            await action(job);
            await uow.CompleteAsync();
        }

        private async Task<TResult> ExecuteWriteWithResultAsync<TResult>(Guid jobId, Func<EcfVoucherDocumentJob, Task<TResult>> action)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);
            var result = await action(job);

            await uow.CompleteAsync();
            return result;
        }

        private async Task<T> ExecuteReadAsync<T>(Func<IRepository<EcfVoucherDocumentJob, Guid>, Task<T>> action)
        {
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            }))
            {
                //using (_unitOfWorkManager.Current.SetTenantId(_abpSession.TenantId))
                using (var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>())
                {
                    var result = await action(repository.Object);
                    await uow.CompleteAsync();
                    return result;
                }
            }
        }

        private async Task HandleWriteFailureAsync(Guid jobId, Exception ex, string methodName, object parameters = null)
        {
            await TryMarkJobAsFailedSilentlyAsync(jobId, BuildErrorMessage(ex));
            await LogAndAuditAsync(ex, methodName, parameters ?? new { jobId });
        }

        private async Task TryMarkJobAsFailedSilentlyAsync(Guid jobId, string errorMessage)
        {
            try
            {
                using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                {
                    IsTransactional = false,
                    Scope = TransactionScopeOption.RequiresNew
                });

                using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();
                var job = await repository.Object.FirstOrDefaultAsync(jobId);

                if (job == null)
                {
                    return;
                }

                job.Status = JobStatus.Failed;
                job.ErrorMessage = errorMessage;
                job.EndTime = DateTime.Now;
                job.HeartbeatAt = null;
                job.LeaseExpiresAt = null;
                job.WorkerInstanceId = null;
                job.ExecutionId = null;

                await uow.CompleteAsync();
            }
            catch (Exception innerEx)
            {
                _logger.LogError(innerEx, "No se pudo marcar el job {JobId} como Failed.", jobId);
            }
        }

        private async Task LogAndAuditAsync(Exception ex, string methodName, object parameters = null)
        {
            _logger.LogError(ex, "Error en {MethodName}.", methodName);

            try
            {
                await _auditingStore.SaveAsync(new AuditInfo
                {
                    ServiceName = nameof(EcfVoucherDocumentJobManagerService),
                    MethodName = methodName,
                    Parameters = parameters == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(parameters),
                    ExecutionTime = DateTime.Now,
                    ExecutionDuration = 0,
                    TenantId = _abpSession.TenantId,
                    UserId = _abpSession.UserId,
                    Exception = ex,
                    BrowserInfo = JobLoggerName,
                    ClientName = Environment.MachineName,
                    CustomData = $"JobManager error: {ex.Message}"
                });
            }
            catch (Exception auditEx)
            {
                _logger.LogError(auditEx, "No se pudo registrar la auditoría del error en {MethodName}.", methodName);
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
    }
}