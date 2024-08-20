using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MinoriaBackend.Core.Model._Base;

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
}