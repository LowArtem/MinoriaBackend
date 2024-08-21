using System.ComponentModel.DataAnnotations;

namespace MinoriaBackend.Core.Dto.Auth;

/// <summary>
///  Данные для регистрации пользователяы
/// </summary>
/// <param name="Email">Почта</param>
/// <param name="Password">Пароль</param>
/// <param name="Name">Имя</param>
public record RegisterRequest
(
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    string Email,

    [Required] string Password,
    [Required] string Name
);