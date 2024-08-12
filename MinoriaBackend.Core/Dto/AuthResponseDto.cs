namespace MinoriaBackend.Core.Dto;

/// <summary>
/// Данные ответа на авторизацию
/// </summary>
/// <param name="Email">Почта</param>
/// <param name="AccessToken">Токен</param>
public record AuthResponseDto (
    string Email,
    string AccessToken
);