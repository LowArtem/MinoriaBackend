using System.Security.Claims;
using MinoriaBackend.Core.Enum;

namespace MinoriaBackend.Api.Extensions.Api;

/// <summary>
/// Расширения ClaimsPrincipal для доступа к данным пользователя
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Получить идентификатор текущего пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Guid идентификатор текущего пользователя</returns>
    public static Guid GetUserId(this ClaimsPrincipal user) => Guid.Parse(user.Claims.Single(x => x.Type == ApplicationJwtClaimTypes.Id).Value);
    
    /// <summary>
    /// Получить email пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Email текущего пользователя</returns>
    public static string GetUserEmail(this ClaimsPrincipal user) => user.Claims.Single(x => x.Type == ApplicationJwtClaimTypes.Email).Value;
}