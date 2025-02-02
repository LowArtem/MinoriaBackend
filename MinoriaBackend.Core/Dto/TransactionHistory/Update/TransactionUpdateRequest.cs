using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Core.Dto.TransactionHistory.Update;

/// <summary>
/// Информация о транзакции для обновления
/// </summary>
/// <param name="Id">Id</param>
/// <param name="Amount">Сумма платежа</param>
/// <param name="Fee">Сумма комиссии</param>
/// <param name="Category">Категория транзакции</param>
/// <param name="TransactionType">Тип транзакции</param>
/// <param name="Description">Описание транзакции</param>
/// <param name="Place">Место покупки</param>
/// <param name="Date">Дата платежа</param>
/// <param name="AccountFrom">Счёт откуда была совершена транзакция</param>
/// <param name="AccountTo">Счёт на который был осуществлён перевод</param>
public record TransactionUpdateRequest(
    decimal Amount,
    decimal Fee,
    Guid Category,
    TransactionTypeEnum TransactionType,
    string? Description,
    string? Place,
    DateTime Date,
    Guid AccountFrom,
    Guid? AccountTo
);