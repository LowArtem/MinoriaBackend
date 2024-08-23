using MinoriaBackend.Core.Dto.TransactionHistory;
using MinoriaBackend.Core.Dto.TransactionHistory.Get;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Accounts;

namespace MinoriaBackend.Data.Services.TransactionHistory;

/// <summary>
/// Расширения для маппинга транзакций
/// </summary>
public static class TransactionExtensions
{
    /// <summary>
    /// Маппинг к <see cref="TransactionHistoryItem"/>
    /// </summary>
    /// <param name="transaction"><see cref="Transaction"/></param>
    /// <returns></returns>
    public static TransactionHistoryItem ToTransactionHistoryItem(this Transaction transaction)
    {
        return new TransactionHistoryItem(
            transaction.Id,
            transaction.Amount,
            transaction.Fee,
            transaction.Category.ToCategoryResponse(),
            transaction.TransactionType,
            transaction.Description,
            transaction.Place,
            transaction.Date,
            transaction.Account.ToAccountResponse(),
            transaction.TransferTo?.ToAccountResponse()
        );
    }
    
    /// <summary>
    /// Маппинг к <see cref="CategoryResponse"/>
    /// </summary>
    /// <param name="category"><see cref="Category"/></param>
    /// <returns></returns>
    private static CategoryResponse ToCategoryResponse(this Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.IsNecessary,
            category.IsIncome
        );
    }
    
    /// <summary>
    /// Маппинг к <see cref="AccountResponse"/>
    /// </summary>
    /// <param name="account"><see cref="BaseAccount"/></param>
    /// <returns></returns>
    private static AccountResponse ToAccountResponse(this BaseAccount account)
    {
        return new AccountResponse(
            account.Id,
            account.Name
        );
    }
}