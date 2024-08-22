namespace MinoriaBackend.Core.Dto.TransactionHistory;

/// <summary>
/// Ответ для истории транзакций
/// </summary>
/// <param name="TotalCount">Общее количество записей (с учётом применённых фильтров)</param>
/// <param name="Items">Список транзакций</param>
public record TransactionHistoryResponse(
    int TotalCount,
    List<TransactionHistoryItem> Items
);