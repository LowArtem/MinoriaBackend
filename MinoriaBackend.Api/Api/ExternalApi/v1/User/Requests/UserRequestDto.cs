using System.ComponentModel.DataAnnotations;

namespace MinoriaBackend.Api.Api.ExternalApi.v1.User.Requests;

/// <summary>
/// Данные для создания пользователя
/// </summary>
/// <param name="Email">Электронная почта</param>
/// <param name="Password">Пароль</param>
/// <param name="Name">Имя</param>
public record UserRequestDto
(
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    string Email,

    [Required] string Password,
    [Required] string Name
);