using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MinoriaBackend.Core.Model._Base;

namespace MinoriaBackend.Core.Model.Accounts;

/// <summary>
/// Базовый тип для счетов пользователей
/// </summary>
public class BaseAccount : BaseEntity
{
    /// <summary>
    /// Количество денег на счету
    /// </summary>
    public decimal Amount { get; set; } = 0;
    
    /// <summary>
    /// Название счета
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Ссылка на изображение
    /// </summary>
    public string? ImageLink { get; set; } = null;
    
    /// <summary>
    /// Идентификатор пользователя владельца счёта
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Пользователь - владелец счёта
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    
    #region Связи

    /// <summary>
    /// Транзакции, где данный счёт источник платежа
    /// </summary>
    [JsonIgnore]
    [InverseProperty("Account")]
    public virtual List<Transaction> TransactionsFromAccount { get; set; } = new();

    /// <summary>
    /// Транзакции, где данный счёт цель перевода
    /// </summary>
    [JsonIgnore]
    [InverseProperty("TransferTo")]
    public virtual List<Transaction> TransactionsToAccount { get; set; } = new();

    /// <summary>
    /// Все транзакции данного счёта
    /// </summary>
    [JsonIgnore]
    [NotMapped]
    public virtual IEnumerable<Transaction> AllTransactions => TransactionsFromAccount.Concat(TransactionsToAccount);

    #endregion
}