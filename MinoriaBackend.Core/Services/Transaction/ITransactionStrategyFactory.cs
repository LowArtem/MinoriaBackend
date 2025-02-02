using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Core.Services.Transaction;

/// <summary>
/// Фабрика стратегий транзакций
/// </summary>
public interface ITransactionStrategyFactory
{
    /// <summary>
    /// Получить подходящую стретегию
    /// </summary>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    ITransactionStrategy GetStrategy(TransactionTypeEnum transactionType);
}