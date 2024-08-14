namespace MinoriaBackend.Api.Api.InternalApi.v1.User.Responses;

/// <summary>
/// Информация о пользователе
/// </summary>
/// <param name="Id">Id</param>
/// <param name="Email">Электронная почта</param>
/// <param name="Name">Имя</param>
public record UserResponseDto
(
    int Id,
    string Email,
    string Name
);