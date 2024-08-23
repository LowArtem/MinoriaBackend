namespace MinoriaBackend.Core.Dto.TransactionHistory.Get;

/// <summary>
/// Ответ для счёта
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
public record AccountResponse(
    Guid Id,
    string Name
);