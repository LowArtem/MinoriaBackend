namespace MinoriaBackend.Core.Model.Enum;

/// <summary>
/// Статус транзакции
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// Не начата
    /// </summary>
    NOT_STARTED,

    /// <summary>
    /// Выполняется
    /// </summary>
    PROCESSING,

    /// <summary>
    /// Выполнена
    /// </summary>
    COMPLETED,

    /// <summary>
    /// Не удалась из-за ошибки
    /// </summary>
    FAILED,

    /// <summary>
    /// Отменена
    /// </summary>
    CANCELED
}