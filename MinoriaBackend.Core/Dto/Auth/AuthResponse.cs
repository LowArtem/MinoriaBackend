namespace MinoriaBackend.Core.Dto.Auth;

/// <summary>
/// Данные ответа на авторизацию
/// </summary>
/// <param name="Email">Почта</param>
/// <param name="AccessToken">Токен</param>
public record AuthResponse (
    string Email,
    string AccessToken
);