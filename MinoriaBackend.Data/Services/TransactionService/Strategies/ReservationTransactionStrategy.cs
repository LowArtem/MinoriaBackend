using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Accounts;
using MinoriaBackend.Core.Repositories;
using MinoriaBackend.Core.Services.Transaction;

namespace MinoriaBackend.Data.Services.TransactionService.Strategies;

/// <summary>
/// Стратегия выполнения транзакции бронирования
/// </summary>
public class ReservationTransactionStrategy : ITransactionStrategy
{
    private readonly IEfCoreRepository<Account> _accountRepository;
    private readonly IEfCoreRepository<VirtualAccount> _virtualAccountRepository;

    public ReservationTransactionStrategy(IEfCoreRepository<Account> accountRepository,
        IEfCoreRepository<VirtualAccount> virtualAccountRepository)
    {
        _accountRepository = accountRepository;
        _virtualAccountRepository = virtualAccountRepository;
    }

    /// <inheritdoc />
    public void Execute(Transaction transaction)
    {
        var targetAccount = _accountRepository.Get(transaction.AccountId);
        var virtualAccount = _virtualAccountRepository.Get(transaction.TransferToId);

        if (targetAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.AccountId);

        if (virtualAccount == null)
            throw new EntityNotFoundException(typeof(VirtualAccount), transaction.TransferToId ?? default);

        targetAccount.AmountReserved += transaction.Amount;
        virtualAccount.Amount += transaction.Amount;

        _accountRepository.Update(targetAccount);
        _virtualAccountRepository.Update(virtualAccount);
    }

    /// <inheritdoc />
    public void Undo(Transaction transaction)
    {
        var targetAccount = _accountRepository.Get(transaction.AccountId);
        var virtualAccount = _virtualAccountRepository.Get(transaction.TransferToId);

        if (targetAccount == null)
            throw new EntityNotFoundException(typeof(Account), transaction.AccountId);

        if (virtualAccount == null)
            throw new EntityNotFoundException(typeof(VirtualAccount), transaction.TransferToId ?? default);

        targetAccount.AmountReserved -= transaction.Amount;
        virtualAccount.Amount -= transaction.Amount;

        _accountRepository.Update(targetAccount);
        _virtualAccountRepository.Update(virtualAccount);
    }
}