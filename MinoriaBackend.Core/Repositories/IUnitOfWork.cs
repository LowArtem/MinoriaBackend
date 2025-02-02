namespace MinoriaBackend.Core.Repositories;

/// <summary>
/// Интерфейс для работы с транзакциями
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Начать транзакцию
    /// </summary>
    void BeginTransaction();
    
    /// <summary>
    /// Начать транзакцию (асинхронно)
    /// </summary>
    /// <returns></returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Выполнить транзакцию
    /// </summary>
    void Commit();
    
    /// <summary>
    /// Выполнить транзакцию (асинхронно)
    /// </summary>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отменить транзакцию
    /// </summary>
    void Rollback();
    
    /// <summary>
    /// Отменить транзакцию (асинхронно)
    /// </summary>
    /// <returns></returns>
    Task RollbackAsync();
}
