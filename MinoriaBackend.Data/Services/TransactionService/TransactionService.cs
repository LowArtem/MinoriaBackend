using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Enum;
using MinoriaBackend.Core.Repositories;
using MinoriaBackend.Core.Services.Transaction;

namespace MinoriaBackend.Data.Services.TransactionService;

/// <summary>
/// Сервис обработки транзакций
/// </summary>
public class TransactionService
{
    private readonly IEfCoreRepository<Transaction> _transactionRepository;
    private readonly ITransactionStrategyFactory _strategyFactory;

    public TransactionService(IEfCoreRepository<Transaction> transactionRepository,
        ITransactionStrategyFactory strategyFactory)
    {
        _transactionRepository = transactionRepository;
        _strategyFactory = strategyFactory;
    }

    /// <summary>
    /// Выполнить транзакцию
    /// </summary>
    /// <remarks>Функция не имеет обработки ошибок, оставляя это за вызывающим кодом</remarks>
    /// <param name="transaction">транзакция</param>
    /// <param name="token">токен отмена</param>
    public void DoTransaction(Transaction transaction, CancellationToken token = default)
    {
        try
        {
            var strategy = _strategyFactory.GetStrategy(transaction.TransactionType);
            strategy.Execute(transaction);

            token.ThrowIfCancellationRequested();

            transaction.TransactionStatus = TransactionStatus.COMPLETED;
            _transactionRepository.Update(transaction);
        }
        catch (Exception e)
        {
            transaction.TransactionStatus = TransactionStatus.FAILED;
            _transactionRepository.Update(transaction);
            throw;
        }
    }

    /// <summary>
    /// Откатить транзакцию
    /// </summary>
    /// <remarks>Функция не имеет обработки ошибок, оставляя это за вызывающим кодом</remarks>
    /// <param name="transaction">транзакция</param>
    /// <param name="token">токен отмена</param>
    public void UndoTransaction(Transaction transaction, CancellationToken token = default)
    {
        var strategy = _strategyFactory.GetStrategy(transaction.TransactionType);
        strategy.Undo(transaction);
            
        token.ThrowIfCancellationRequested();
            
        transaction.TransactionStatus = TransactionStatus.CANCELED;
        _transactionRepository.Update(transaction);
    }
}