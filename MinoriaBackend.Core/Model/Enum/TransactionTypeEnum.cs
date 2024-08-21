namespace MinoriaBackend.Core.Model.Enum;

/// <summary>
/// Тип транзакции
/// </summary>
public enum TransactionTypeEnum
{
    /// <summary>
    /// Доход
    /// </summary>
    INCOME,
    
    /// <summary>
    /// Расход
    /// </summary>
    EXPENSE,
    
    /// <summary>
    /// Перевод
    /// </summary>
    TRANSFER,
    
    /// <summary>
    /// Резерв (сумма для накопления)
    /// </summary>
    RESERVATION
}