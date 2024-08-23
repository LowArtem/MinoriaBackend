using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MinoriaBackend.Core.Model._Base;
using MinoriaBackend.Core.Model.Accounts;
using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Core.Model;

/// <summary>
/// Транзакция
/// </summary>
public class Transaction : BaseEntity
{
    /// <summary>
    /// Сумма транзакции
    /// </summary>
    public decimal Amount { get; set; } = 0;
    
    /// <summary>
    /// Сумма комиссии
    /// </summary>
    public decimal Fee { get; set; } = 0;

    /// <summary>
    /// Тип транзакции
    /// </summary>
    public TransactionTypeEnum TransactionType { get; set; } = TransactionTypeEnum.EXPENSE;

    /// <summary>
    /// Текущий статус транзакции
    /// </summary>
    public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.NOT_STARTED;
    
    /// <summary>
    /// Описание транзакции
    /// </summary>
    public string? Description { get; set; } = null;
    
    /// <summary>
    /// Место транзакции (например, магазин)
    /// </summary>
    public string? Place { get; set; } = null;
    
    /// <summary>
    /// Дата и время транзакции
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    
    #region Связи
    
    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
    
    /// <summary>
    /// Id категории
    /// </summary>
    public Guid CategoryId { get; set; }
    
    /// <summary>
    /// Категория
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; }
    
    /// <summary>
    /// Id счета, с которого осуществлён платёж
    /// </summary>
    public Guid AccountId { get; set; }
    
    /// <summary>
    /// Счет, с которого осуществлён платёж
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(AccountId))]
    public virtual BaseAccount Account { get; set; }
    
    /// <summary>
    /// Id счёта, на который осуществлён перевод
    /// </summary>
    public Guid? TransferToId { get; set; }
    
    /// <summary>
    /// Счёт, на который осуществлён перевод
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(TransferToId))]
    public virtual BaseAccount? TransferTo { get; set; }

    #endregion
}