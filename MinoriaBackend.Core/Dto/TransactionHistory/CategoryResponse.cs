namespace MinoriaBackend.Core.Dto.TransactionHistory;

/// <summary>
/// Ответ для категории транзакций
/// </summary>
/// <param name="Id">Id</param>
/// <param name="Name">Название категории</param>
/// <param name="IsNecessary">Необходимая ли категория</param>
/// <param name="IsIncome">Является ли категория доходом</param>
public record CategoryResponse(
    Guid Id,
    string Name,
    bool IsNecessary,
    bool IsIncome
);