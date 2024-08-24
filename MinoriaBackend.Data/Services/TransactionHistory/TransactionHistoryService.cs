using Microsoft.EntityFrameworkCore;
using MinoriaBackend.Core.Dto.TransactionHistory;
using MinoriaBackend.Core.Dto.TransactionHistory.Get;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Repositories;

namespace MinoriaBackend.Data.Services.TransactionHistory;

/// <summary>
/// Сервис для истории транзакций
/// </summary>
public class TransactionHistoryService
{
    private readonly IEfCoreRepository<Transaction> _transactionRepository;

    public TransactionHistoryService(IEfCoreRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Получить историю транзакций
    /// </summary>
    /// <param name="userId">Id текущего пользователя</param>
    /// <param name="request">запрос</param>
    /// <param name="token"></param>
    /// <returns>список транзакций</returns>
    public async Task<TransactionHistoryResponse> GetTransactionHistory(Guid userId, TransactionHistoryRequest request,
        CancellationToken token)
    {
        var query = _transactionRepository
            .GetListQuery()
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransferTo)
            .Where(x => x.UserId == userId)
            .Where(x => request.DateFrom == null || x.Date >= request.DateFrom)
            .Where(x => request.DateTo == null || x.Date <= request.DateTo)
            .Where(x => request.TransactionType == null || x.TransactionType == request.TransactionType)
            .Where(x => request.CategoryId == null || x.CategoryId == request.CategoryId)
            .Where(x => request.AccountId == null || x.AccountId == request.AccountId);

        // Запрос для подсчета общего количества подходящих сущностей
        var totalCount = await query.CountAsync(cancellationToken: token);

        // Запрос для получения данных с учетом пагинации
        var transactions = await query
            .Skip(request.From)
            .Take(request.Count)
            .Select(item => item.ToTransactionHistoryItem())
            .ToListAsync(cancellationToken: token);

        return new TransactionHistoryResponse(totalCount, transactions);
    }
}