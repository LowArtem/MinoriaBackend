namespace MinoriaBackend.Core.Services.Transaction;

/// <summary>
/// Стратегия выполнения транзакции
/// </summary>
public interface ITransactionStrategy
{
    /// <summary>
    /// Выполнить транзакцию
    /// </summary>
    /// <param name="transaction">транзакция</param>
    void Execute(Model.Transaction transaction);
    
    /// <summary>
    /// Откатить транзакцию
    /// </summary>
    /// <param name="transaction">транзакция</param>
    void Undo(Model.Transaction transaction);
}