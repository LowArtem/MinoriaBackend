using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MinoriaBackend.Core.Repositories;

namespace MinoriaBackend.Data.Repositories;

/// <summary>
/// Класс для управления транзакциями
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _dbContext;
    private IDbContextTransaction? _transaction = null;
    
    public UnitOfWork(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public void BeginTransaction()
    {
        _transaction = _dbContext.Database.BeginTransaction();
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Commit()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction has not been started.");
        
        try
        {
            _dbContext.SaveChanges();
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            Dispose();
        }
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction has not been started.");
        
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            Dispose();
        }
    }

    /// <inheritdoc />
    public void Rollback()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction has not been started.");
        
        _transaction.Rollback();
        Dispose();
    }

    /// <inheritdoc />
    public async Task RollbackAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction has not been started.");
        
        await _transaction.RollbackAsync();
        Dispose();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}
