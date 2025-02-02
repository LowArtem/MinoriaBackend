using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Accounts;
using MinoriaBackend.Core.Model.Enum;
using MinoriaBackend.Core.Repositories;
using MinoriaBackend.Core.Services.Transaction;
using MinoriaBackend.Data.Services.TransactionService.Strategies;
using NSubstitute;

namespace MinoriaBackend.Test.Data.Services.TransactionService;

public class TransactionServiceTests
{
    private readonly IEfCoreRepository<Transaction> _transactionRepository = Substitute.For<IEfCoreRepository<Transaction>>();
    private readonly IEfCoreRepository<Account> _accountRepository = Substitute.For<IEfCoreRepository<Account>>();
    private readonly IEfCoreRepository<VirtualAccount> _virtualAccountRepository = Substitute.For<IEfCoreRepository<VirtualAccount>>();
    private readonly MinoriaBackend.Data.Services.TransactionService.TransactionService _transactionService;

    public TransactionServiceTests()
    {
        ITransactionStrategyFactory transactionStrategyFactory = new TransactionStrategyFactory(
            new IncomeSpendingTransactionStrategy(_accountRepository),
            new TransferTransactionStrategy(_accountRepository),
            new ReservationTransactionStrategy(_accountRepository, _virtualAccountRepository));
        
        _transactionService = new MinoriaBackend.Data.Services.TransactionService.TransactionService(_transactionRepository, transactionStrategyFactory);
    }

    [Fact]
    public void DoTransaction_IncomeExpense_ShouldUpdateAccountAndSetTransactionStatusCompleted()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = 100,
            Fee = 5
        };
        var account = new Account { Id = accountId, Amount = 0 };

        _accountRepository.Get(accountId).Returns(account);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        _accountRepository.Received(1).Update(account);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Transfer_ShouldUpdateBothAccountsAndSetTransactionStatusCompleted()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.TRANSFER,
            AccountId = sourceAccountId,
            TransferToId = targetAccountId,
            Amount = 100,
            Fee = 5
        };
        var sourceAccount = new Account { Id = sourceAccountId, Amount = 200 };
        var targetAccount = new Account { Id = targetAccountId, Amount = 50 };

        _accountRepository.Get(sourceAccountId).Returns(sourceAccount);
        _accountRepository.Get(targetAccountId).Returns(targetAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        _accountRepository.Received(1).Update(sourceAccount);
        _accountRepository.Received(1).Update(targetAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Reservation_ShouldUpdateBothAccountAndVirtualAccountAndSetTransactionStatusCompleted()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var virtualAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.RESERVATION,
            AccountId = accountId,
            TransferToId = virtualAccountId,
            Amount = 100
        };
        var account = new Account { Id = accountId, Amount = 200 };
        var virtualAccount = new VirtualAccount { Id = virtualAccountId, Amount = 50 };

        _accountRepository.Get(accountId).Returns(account);
        _virtualAccountRepository.Get(virtualAccountId).Returns(virtualAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        _accountRepository.Received(1).Update(account);
        _virtualAccountRepository.Received(1).Update(virtualAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_IncomeExpense_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = 100,
            Fee = 5
        };

        _accountRepository.Get(accountId).Returns((Account)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _transactionService.DoTransaction(transaction));
    }

    [Fact]
    public void DoTransaction_Transfer_ShouldThrowEntityNotFoundExceptionForTargetAccount()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.TRANSFER,
            AccountId = sourceAccountId,
            TransferToId = targetAccountId,
            Amount = 100,
            Fee = 5
        };
        var sourceAccount = new Account { Id = sourceAccountId, Amount = 200 };

        _accountRepository.Get(sourceAccountId).Returns(sourceAccount);
        _accountRepository.Get(targetAccountId).Returns((Account)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _transactionService.DoTransaction(transaction));
    }

    [Fact]
    public void UndoTransaction_IncomeExpense_ShouldReverseAmountAndSetTransactionStatusCanceled()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = 100,
            Fee = 5
        };
        var account = new Account { Id = accountId, Amount = 200 };

        _accountRepository.Get(accountId).Returns(account);

        // Act
        _transactionService.UndoTransaction(transaction);

        // Assert
        _accountRepository.Received(1).Update(account);
        Assert.Equal(TransactionStatus.CANCELED, transaction.TransactionStatus);
    }

    [Fact]
    public void UndoTransaction_Transfer_ShouldReverseTransferAndSetTransactionStatusCanceled()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.TRANSFER,
            AccountId = sourceAccountId,
            TransferToId = targetAccountId,
            Amount = 100,
            Fee = 5
        };
        var sourceAccount = new Account { Id = sourceAccountId, Amount = 200 };
        var targetAccount = new Account { Id = targetAccountId, Amount = 50 };

        _accountRepository.Get(sourceAccountId).Returns(sourceAccount);
        _accountRepository.Get(targetAccountId).Returns(targetAccount);

        // Act
        _transactionService.UndoTransaction(transaction);

        // Assert
        _accountRepository.Received(1).Update(sourceAccount);
        _accountRepository.Received(1).Update(targetAccount);
        Assert.Equal(TransactionStatus.CANCELED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_ShouldSetTransactionStatusFailed_WhenExceptionThrown()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = 100,
            Fee = 5
        };
        var account = new Account { Id = accountId, Amount = 0 };

        _accountRepository.Get(accountId).Returns(account);
        _accountRepository.When(x => x.Update(account)).Do(x => { throw new Exception("Update failed"); });

        // Act
        Assert.Throws<Exception>(() => _transactionService.DoTransaction(transaction));

        // Assert
        Assert.Equal(TransactionStatus.FAILED, transaction.TransactionStatus);
        _transactionRepository.Received(1).Update(transaction);
    }
    
    [Fact]
    public void DoTransaction_IncomeExpense_ShouldCorrectlyUpdateAccountAmount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = 100m,
            Fee = 5m
        };
        var account = new Account { Id = accountId, Amount = 50m };

        _accountRepository.Get(accountId).Returns(account);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(145m, account.Amount); // 50 + 100 - 5
        _accountRepository.Received(1).Update(account);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_IncomeExpense_NegativeAmount_ShouldCorrectlyUpdateAccountAmount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = -50m,
            Fee = 0m
        };
        var account = new Account { Id = accountId, Amount = 100m };

        _accountRepository.Get(accountId).Returns(account);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(50m, account.Amount); // 100 - 50
        _accountRepository.Received(1).Update(account);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_IncomeExpense_DecimalAmountAndFee_ShouldCorrectlyUpdateAccountAmount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.INCOME_EXPENSE,
            AccountId = accountId,
            Amount = 50.75m,
            Fee = 2.25m
        };
        var account = new Account { Id = accountId, Amount = 10.00m };

        _accountRepository.Get(accountId).Returns(account);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(58.50m, account.Amount); // 10 + 50.75 - 2.25
        _accountRepository.Received(1).Update(account);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Transfer_ShouldCorrectlyUpdateSourceAndTargetAccountAmounts()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.TRANSFER,
            AccountId = sourceAccountId,
            TransferToId = targetAccountId,
            Amount = 100m,
            Fee = 5m
        };
        var sourceAccount = new Account { Id = sourceAccountId, Amount = 200m };
        var targetAccount = new Account { Id = targetAccountId, Amount = 50m };

        _accountRepository.Get(sourceAccountId).Returns(sourceAccount);
        _accountRepository.Get(targetAccountId).Returns(targetAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(95m, sourceAccount.Amount); // 200 - 100 - 5
        Assert.Equal(150m, targetAccount.Amount); // 50 + 100
        _accountRepository.Received(1).Update(sourceAccount);
        _accountRepository.Received(1).Update(targetAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Transfer_NegativeAmount_ShouldCorrectlyUpdateSourceAndTargetAccountAmounts()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.TRANSFER,
            AccountId = sourceAccountId,
            TransferToId = targetAccountId,
            Amount = -50m,
            Fee = 0m
        };
        var sourceAccount = new Account { Id = sourceAccountId, Amount = 100m };
        var targetAccount = new Account { Id = targetAccountId, Amount = 200m };

        _accountRepository.Get(sourceAccountId).Returns(sourceAccount);
        _accountRepository.Get(targetAccountId).Returns(targetAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(150m, sourceAccount.Amount); // 100 - (-50)
        Assert.Equal(150m, targetAccount.Amount); // 200 - 50
        _accountRepository.Received(1).Update(sourceAccount);
        _accountRepository.Received(1).Update(targetAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Transfer_DecimalAmountAndFee_ShouldCorrectlyUpdateSourceAndTargetAccountAmounts()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.TRANSFER,
            AccountId = sourceAccountId,
            TransferToId = targetAccountId,
            Amount = 75.50m,
            Fee = 3.25m
        };
        var sourceAccount = new Account { Id = sourceAccountId, Amount = 100.00m };
        var targetAccount = new Account { Id = targetAccountId, Amount = 20.00m };

        _accountRepository.Get(sourceAccountId).Returns(sourceAccount);
        _accountRepository.Get(targetAccountId).Returns(targetAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(21.25m, sourceAccount.Amount); // 100 - 75.50 - 3.25
        Assert.Equal(95.50m, targetAccount.Amount); // 20 + 75.50
        _accountRepository.Received(1).Update(sourceAccount);
        _accountRepository.Received(1).Update(targetAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Reservation_ShouldCorrectlyUpdateAccountAndVirtualAccountAmounts()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var virtualAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.RESERVATION,
            AccountId = accountId,
            TransferToId = virtualAccountId,
            Amount = 150m,
            Fee = 0m
        };
        var account = new Account { Id = accountId, Amount = 300m, AmountReserved = 0m };
        var virtualAccount = new VirtualAccount { Id = virtualAccountId, Amount = 50m };

        _accountRepository.Get(accountId).Returns(account);
        _virtualAccountRepository.Get(virtualAccountId).Returns(virtualAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(300m, account.Amount); // 300 (150 никуда не уходит, просто резервируется)
        Assert.Equal(150m, account.AmountReserved); // 150 в резерве
        Assert.Equal(200m, virtualAccount.Amount); // 50 + 150
        _accountRepository.Received(1).Update(account);
        _virtualAccountRepository.Received(1).Update(virtualAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }

    [Fact]
    public void DoTransaction_Reservation_DecimalAmount_ShouldCorrectlyUpdateAccountAndVirtualAccountAmounts()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var virtualAccountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            TransactionType = TransactionTypeEnum.RESERVATION,
            AccountId = accountId,
            TransferToId = virtualAccountId,
            Amount = 125.75m,
            Fee = 0m
        };
        var account = new Account { Id = accountId, Amount = 300.00m, AmountReserved = 50.00m };
        var virtualAccount = new VirtualAccount { Id = virtualAccountId, Amount = 75.00m };

        _accountRepository.Get(accountId).Returns(account);
        _virtualAccountRepository.Get(virtualAccountId).Returns(virtualAccount);

        // Act
        _transactionService.DoTransaction(transaction);

        // Assert
        Assert.Equal(300.00m, account.Amount); // 300
        Assert.Equal(175.75m, account.AmountReserved); // 50 + 125.75
        Assert.Equal(200.75m, virtualAccount.Amount); // 75 + 125.75
        _accountRepository.Received(1).Update(account);
        _virtualAccountRepository.Received(1).Update(virtualAccount);
        Assert.Equal(TransactionStatus.COMPLETED, transaction.TransactionStatus);
    }
}