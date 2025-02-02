using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Accounts;
using MinoriaBackend.Core.Repositories;
using MinoriaBackend.Core.Services.Transaction;

namespace MinoriaBackend.Data.Services.TransactionService.Strategies;

/// <summary>
/// Стратегия выполнения транзакции перевода
/// </summary>
public class TransferTransactionStrategy : ITransactionStrategy
{
    private readonly IEfCoreRepository<Account> _accountRepository;

    public TransferTransactionStrategy(IEfCoreRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    /// <inheritdoc />
    public void Execute(Transaction transaction)
    {
        var sourceAccount = _accountRepository.Get(transaction.AccountId);
        var targetAccount = _accountRepository.Get(transaction.TransferToId);
        
        if (sourceAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.AccountId);
        
        if (targetAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.TransferToId ?? default);
        
        sourceAccount.Amount -= transaction.Amount + transaction.Fee;
        targetAccount.Amount += transaction.Amount;
        
        _accountRepository.Update(sourceAccount);
        _accountRepository.Update(targetAccount);
    }

    /// <inheritdoc />
    public void Undo(Transaction transaction)
    {
        var targetAccount = _accountRepository.Get(transaction.AccountId);
        var sourceAccount = _accountRepository.Get(transaction.TransferToId);
        
        if (sourceAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.AccountId);
        
        if (targetAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.TransferToId ?? default);
        
        sourceAccount.Amount -= transaction.Amount + transaction.Fee;
        targetAccount.Amount += transaction.Amount;
        
        _accountRepository.Update(sourceAccount);
        _accountRepository.Update(targetAccount);
    }
}