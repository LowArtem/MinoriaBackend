using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Core.Dto.TransactionHistory.Get;

/// <summary>
/// Данные для получения истории транзакций
/// </summary>
/// <param name="From">Номер записи, начиная с которой получать транзакции</param>
/// <param name="Count">Количество транзакций для получения</param>
/// <param name="DateFrom">Фильтр по дате начала</param>
/// <param name="DateTo">Фильтр по дате окончания</param>
/// <param name="TransactionType">Фильтр по типу транзакции</param>
/// <param name="TransactionStatus">Фильтр по статусу транзакции</param>
/// <param name="CategoryId">Фильтр по категории</param>
/// <param name="AccountId">Фильтр по счёту</param>
public record TransactionHistoryRequest(
    int From = 0,
    int Count = 100,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    TransactionTypeEnum? TransactionType = null,
    TransactionStatus? TransactionStatus = null,
    Guid? CategoryId = null,
    Guid? AccountId = null
);
