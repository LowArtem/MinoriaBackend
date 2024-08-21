using System.Text.Json.Serialization;
using MinoriaBackend.Core.Model._Base;

namespace MinoriaBackend.Core.Model;

/// <summary>
/// Категория транзакций
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Необходимая ли категория (необходима ли она для выживания)
    /// </summary>
    public bool IsNecessary { get; set; } = false;

    /// <summary>
    /// Является ли категория доходом
    /// </summary>
    public bool IsIncome { get; set; } = false;

    
    #region Связи

    /// <summary>
    /// Транзакции по данной категории
    /// </summary>
    [JsonIgnore]
    public virtual List<Transaction> Transactions { get; set; }

    #endregion
}