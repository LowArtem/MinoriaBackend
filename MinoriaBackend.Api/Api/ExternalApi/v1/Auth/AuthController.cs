using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinoriaBackend.Api.Attributes;
using MinoriaBackend.Core.Dto.Auth;
using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Data.Services.Auth;
using Swashbuckle.AspNetCore.Annotations;

namespace MinoriaBackend.Api.Api.ExternalApi.v1.Auth;

/// <summary>
/// Авторизация
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[SetRoute]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly UserService _service;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Пинг
    /// </summary>
    /// <returns></returns>
    [HttpGet("hello")]
    [AllowAnonymous]
    public IActionResult Ping() => Ok("Hello");

    /// <summary>
    /// Пинг с авторизацией
    /// </summary>
    /// <returns></returns>
    [HttpGet("hello-auth")]
    [SwaggerResponse(200, "Hello ответ", typeof(string))]
    [SwaggerResponse(401, "Ошибка авторизации")]
    [SwaggerResponse(403, "Нет доступа")]
    public IActionResult PingAuth() =>
        Ok($"Hello, {User.Identity!.Name}");

    /// <summary>
    /// Зарегистрировать нового пользователя
    /// </summary>
    /// <param name="registerRequest">данные регистрации</param>
    /// <returns>логин и токен пользователя</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerResponse(200, "Пользователь успешно зарегистрирован", typeof(AuthResponse))]
    [SwaggerResponse(400, "Неверный формат данных")]
    [SwaggerResponse(409, "Пользователь с таким email уже существует", typeof(string))]
    [SwaggerResponse(500, "Ошибка при регистрации пользователя", typeof(string))]
    public IActionResult Register(RegisterRequest registerRequest)
    {
        try
        {
            return Ok(_service.RegisterUser(registerRequest));
        }
        catch (EntityExistsException)
        {
            return Conflict("Пользователь с таким email уже существует");
        }
        catch (Exception e)
        {
            _logger.LogError($"Произошла ошибка при регистрации пользователя: {e}");
            return StatusCode(500, $"Произошла ошибка при регистрации пользователя: {e}");
        }
    }

    /// <summary>
    /// Вход для зарегистрированного пользователя
    /// </summary>
    /// <param name="loginRequest">данные для входа</param>
    /// <returns>логин и токен пользователя</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerResponse(200, "Пользователь успешно залогинен", typeof(AuthResponse))]
    [SwaggerResponse(400, "Неверный формат данных")]
    [SwaggerResponse(404, "Пользователь с таким email не существует", typeof(string))]
    [SwaggerResponse(500, "Ошибка при логине пользователя", typeof(string))]
    public IActionResult Login(LoginRequest loginRequest)
    {
        try
        {
            return Ok(_service.LoginUser(loginRequest));
        }
        catch (EntityNotFoundException)
        {
            return NotFound("Пользователь с таким email не существует");
        }
        catch (Exception e)
        {
            _logger.LogError($"Произошла ошибка при получении пользователя: {e}");
            return StatusCode(500, $"Произошла ошибка при получении пользователя: {e}");
        }
    }
}