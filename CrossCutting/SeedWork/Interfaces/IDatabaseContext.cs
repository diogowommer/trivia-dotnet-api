using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.SeedWork.Interfaces
{
    public interface IDatabaseContext
    {
        DatabaseFacade Database { get; }
        bool HasActiveTransaction { get; }
        IDbContextTransaction GetCurrentTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
    }
}
