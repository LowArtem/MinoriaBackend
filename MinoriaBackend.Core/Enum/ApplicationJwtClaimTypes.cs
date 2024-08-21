namespace MinoriaBackend.Core.Enum;

/// <summary>
/// Типы claims в JWT
/// </summary>
public static class ApplicationJwtClaimTypes
{
    /// <summary>
    /// Email пользователя (name claim)
    /// </summary>
    public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
    
    /// <summary>
    /// Id пользователя (id claim)
    /// </summary>
    public const string Id = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
}