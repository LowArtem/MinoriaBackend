using System.ComponentModel.DataAnnotations;

namespace MinoriaBackend.Core.Dto.Auth;

/// <summary>
/// Данные для входа пользователя
/// </summary>
/// <param name="Email">Почта</param>
/// <param name="Password">Пароль</param>
public record LoginRequest
(
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    string Email,

    [Required] string Password
);