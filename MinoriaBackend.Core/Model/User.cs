using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MinoriaBackend.Core.Model._Base;
using MinoriaBackend.Core.Model.Accounts;

namespace MinoriaBackend.Core.Model;

/// <summary>
/// Пользователь
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// ХЭШ пароля
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public string PasswordHash { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Id телеграм чата
    /// </summary>
    public string? TelegramChatId { get; set; } = null;

    #region Связи

    /// <summary>
    /// Список счетов пользователя
    /// </summary>
    [JsonIgnore]
    public virtual List<BaseAccount> Accounts { get; set; }
    
    /// <summary>
    /// Список транзакций пользователя
    /// </summary>
    [JsonIgnore]
    public virtual List<Transaction> Transactions { get; set; }

    #endregion
}