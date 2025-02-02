namespace MinoriaBackend.Core.Model.Enum;

/// <summary>
/// Тип транзакции
/// </summary>
public enum TransactionTypeEnum
{
    /// <summary>
    /// Доход/Расход
    /// </summary>
    INCOME_EXPENSE,
    
    /// <summary>
    /// Перевод
    /// </summary>
    TRANSFER,
    
    /// <summary>
    /// Резерв (сумма для накопления)
    /// </summary>
    RESERVATION
}