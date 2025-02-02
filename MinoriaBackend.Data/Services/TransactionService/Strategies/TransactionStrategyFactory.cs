using MinoriaBackend.Core.Model.Enum;
using MinoriaBackend.Core.Services.Transaction;

namespace MinoriaBackend.Data.Services.TransactionService.Strategies;

/// <inheritdoc />
public class TransactionStrategyFactory : ITransactionStrategyFactory
{
    private readonly IncomeSpendingTransactionStrategy _incomeExpenseTransactionStrategy;
    private readonly TransferTransactionStrategy _transferStrategy;
    private readonly ReservationTransactionStrategy _reservationStrategy;

    public TransactionStrategyFactory(
        IncomeSpendingTransactionStrategy incomeExpenseTransactionStrategy,
        TransferTransactionStrategy transferStrategy,
        ReservationTransactionStrategy reservationStrategy)
    {
        _incomeExpenseTransactionStrategy = incomeExpenseTransactionStrategy;
        _transferStrategy = transferStrategy;
        _reservationStrategy = reservationStrategy;
    }

    /// <inheritdoc />
    public ITransactionStrategy GetStrategy(TransactionTypeEnum transactionType)
    {
        return transactionType switch
        {
            TransactionTypeEnum.INCOME_EXPENSE => _incomeExpenseTransactionStrategy,
            TransactionTypeEnum.TRANSFER => _transferStrategy,
            TransactionTypeEnum.RESERVATION => _reservationStrategy,
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), $"Unknown transaction type {transactionType}")
        };
    }
}