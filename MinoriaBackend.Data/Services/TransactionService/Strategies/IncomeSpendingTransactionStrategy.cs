using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Accounts;
using MinoriaBackend.Core.Repositories;
using MinoriaBackend.Core.Services.Transaction;

namespace MinoriaBackend.Data.Services.TransactionService.Strategies;

/// <summary>
/// Стратегия выполнения транзакции по приходам и расходам
/// </summary>
public class IncomeSpendingTransactionStrategy : ITransactionStrategy
{
    private readonly IEfCoreRepository<Account> _accountRepository;
    
    public IncomeSpendingTransactionStrategy(IEfCoreRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    /// <inheritdoc />
    public void Execute(Transaction transaction)
    {
        var targetAccount = _accountRepository.Get(transaction.AccountId);
        
        if (targetAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.AccountId);
        
        targetAccount.Amount += transaction.Amount - transaction.Fee;
        _accountRepository.Update(targetAccount);
    }

    /// <inheritdoc />
    public void Undo(Transaction transaction)
    {
        var targetAccount = _accountRepository.Get(transaction.AccountId);
        if (targetAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.AccountId);

        targetAccount.Amount -= transaction.Amount - transaction.Fee;
        _accountRepository.Update(targetAccount);
    }
}