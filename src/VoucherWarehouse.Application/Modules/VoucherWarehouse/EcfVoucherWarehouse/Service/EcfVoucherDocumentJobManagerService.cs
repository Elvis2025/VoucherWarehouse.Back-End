using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using IBS.VoucherWarehouse.Common.Constants;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.Service;
using IBS.VoucherWarehouse.Modules.VoucherWarehouse.Models;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace IBS.VoucherWarehouse.Modules.VoucherWarehouse.EcfVoucherWarehouse.BackgroundWorker
{
    public class EcfVoucherDocumentJobManagerService : IEcfVoucherDocumentJobManagerService, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIocResolver _iocResolver;

        public EcfVoucherDocumentJobManagerService(
            IUnitOfWorkManager unitOfWorkManager,
            IIocResolver iocResolver)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _iocResolver = iocResolver;
        }

        public async Task<EcfVoucherDocumentJob> GetAsync(Guid jobId)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.FirstOrDefaultAsync(jobId);

            await uow.CompleteAsync();

            return job;
        }

        public async Task MarkAsProcessingAsync(Guid jobId)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.Status = JobStatus.Processing;
            job.StartTime = DateTime.Now;
            job.EndTime = null;
            job.ErrorMessage = null;

            await uow.CompleteAsync();
        }

        public async Task SetTotalRowsAsync(Guid jobId, int totalRows)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.TotalRows = totalRows;
            job.ProcessedRows = 0;
            job.SuccessRows = 0;
            job.FailedRows = 0;

            await uow.CompleteAsync();
        }

        public async Task SaveProgressSnapshotAsync(Guid jobId, int processedRows, int successRows, int failedRows)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.ProcessedRows = processedRows;
            job.SuccessRows = successRows;
            job.FailedRows = failedRows;

            await uow.CompleteAsync();
        }

        public async Task AppendRowErrorAsync(Guid jobId, int rowNumber, string errorMessage)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            var newErrorLine = $"Row {rowNumber}: {errorMessage}";

            job.ErrorMessage = string.IsNullOrWhiteSpace(job.ErrorMessage)
                ? newErrorLine
                : $"{job.ErrorMessage}{Environment.NewLine}{newErrorLine}";

            await uow.CompleteAsync();
        }

        public async Task MarkAsCompletedAsync(Guid jobId, int processedRows, int successRows, int failedRows)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.Status = JobStatus.Completed;
            job.ProcessedRows = processedRows;
            job.SuccessRows = successRows;
            job.FailedRows = failedRows;
            job.EndTime = DateTime.Now;

            await uow.CompleteAsync();
        }

        public async Task MarkAsCompletedWithErrorsAsync(Guid jobId, int processedRows, int successRows, int failedRows)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.Status = JobStatus.CompletedWithErrors;
            job.ProcessedRows = processedRows;
            job.SuccessRows = successRows;
            job.FailedRows = failedRows;
            job.EndTime = DateTime.Now;

            await uow.CompleteAsync();
        }

        public async Task MarkAsCancelledAsync(Guid jobId, int processedRows, int successRows, int failedRows)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.Status = JobStatus.Cancelled;
            job.ProcessedRows = processedRows;
            job.SuccessRows = successRows;
            job.FailedRows = failedRows;
            job.EndTime = DateTime.Now;

            await uow.CompleteAsync();
        }

        public async Task MarkAsFailedAsync(Guid jobId, string errorMessage)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            job.Status = JobStatus.Failed;
            job.ErrorMessage = errorMessage;
            job.EndTime = DateTime.Now;

            await uow.CompleteAsync();
        }

        public async Task<bool> IsCancellationRequestedAsync(Guid jobId)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            await uow.CompleteAsync();

            return job.IsCancellationRequested;
        }

        public async Task RequestCancellationAsync(Guid jobId)
        {
            using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                IsTransactional = false,
                Scope = TransactionScopeOption.RequiresNew
            });

            using var repository = _iocResolver.ResolveAsDisposable<IRepository<EcfVoucherDocumentJob, Guid>>();

            var job = await repository.Object.GetAsync(jobId);

            if (job.Status == JobStatus.Completed ||
                job.Status == JobStatus.CompletedWithErrors ||
                job.Status == JobStatus.Failed ||
                job.Status == JobStatus.Cancelled)
            {
                await uow.CompleteAsync();
                return;
            }

            job.IsCancellationRequested = true;
            job.Status = JobStatus.Cancelled;

            await uow.CompleteAsync();
        }
    }
}