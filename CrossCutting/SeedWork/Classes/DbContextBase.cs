using CrossCutting.SeedWork.Extensions;
using CrossCutting.SeedWork.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;


namespace CrossCutting.SeedWork.Classes
{
    public abstract class DbContextBase : DbContext, IDatabaseContext
    {
        protected readonly IMediator mediator;

        protected IDbContextTransaction currentTransaction;

        public DbContextBase(DbContextOptions options, IMediator mediator) : base(options)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public virtual IDbContextTransaction GetCurrentTransaction()
        {
            return this.currentTransaction;
        }

        public virtual bool HasActiveTransaction
        {
            get
            {
                return this.currentTransaction != null;
            }
        }

        public virtual async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this.mediator.DispatchDomainEventsAsync(this);

            var result = await base.SaveChangesAsync(cancellationToken);

            return result > 0;
        }

        public virtual async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (this.currentTransaction != null) return null;

            this.currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return this.currentTransaction;
        }

        public virtual async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != this.currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Dispose();
                    this.currentTransaction = null;
                }
            }
        }
        public virtual void RollbackTransaction()
        {
            try
            {
                this.currentTransaction?.Rollback();
            }
            finally
            {
                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Dispose();
                    this.currentTransaction = null;
                }
            }
        }
    }
}
