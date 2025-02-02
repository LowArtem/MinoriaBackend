using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinoriaBackend.Core.Dto.TransactionHistory.Get;
using MinoriaBackend.Core.Dto.TransactionHistory.Update;
using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Enum;
using MinoriaBackend.Core.Repositories;

namespace MinoriaBackend.Data.Services.TransactionHistory;

/// <summary>
/// Сервис для истории транзакций
/// </summary>
public class TransactionHistoryService
{
    private readonly IEfCoreRepository<Transaction> _transactionRepository;
    private readonly TransactionService.TransactionService _transactionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionHistoryService(IEfCoreRepository<Transaction> transactionRepository, IMapper mapper, TransactionService.TransactionService transactionService, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _transactionService = transactionService;
        _unitOfWork = unitOfWork;
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
            .Where(x => request.AccountId == null || x.AccountId == request.AccountId)
            .Where(x => x.TransactionStatus == TransactionStatus.COMPLETED)
            .OrderByDescending(x => x.Date);

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

    /// <summary>
    /// Обновить элемент истории транзакций
    /// </summary>
    /// <param name="transactionId">Id транзакции</param>
    /// <param name="request">запрос</param>
    /// <param name="token"></param>
    /// <exception cref="EntityNotFoundException">если такой элемент не существует</exception>
    public async Task UpdateTransactionHistory(Guid transactionId, TransactionUpdateRequest request, CancellationToken token)
    {
        var transaction = _transactionRepository.Get(transactionId);
        if (transaction == null) throw new EntityNotFoundException(typeof(Transaction), transactionId);

        await _unitOfWork.BeginTransactionAsync(token);
        try
        {
            _transactionService.UndoTransaction(transaction, token);
        
            _mapper.Map(request, transaction);
            _transactionRepository.Update(transaction);

            _transactionService.DoTransaction(transaction, token);
            
            await _unitOfWork.CommitAsync(token);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Удалить элемент истории транзакций
    /// </summary>
    /// <param name="transactionId">Id транзакции</param>
    public void DeleteTransaction(Guid transactionId)
    {
        try
        {
            _transactionRepository.Remove(transactionId);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}

