using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MinoriaBackend.Core.Dto.Auth;
using MinoriaBackend.Core.Enum;
using MinoriaBackend.Core.Exceptions;
using MinoriaBackend.Core.Extensions;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Auth;
using MinoriaBackend.Core.Repositories;

namespace MinoriaBackend.Data.Services.Auth;

public class UserService
{
    private readonly IEfCoreRepository<User> _userRepository;
    private readonly IJwtService _jwtService;

    public UserService(IEfCoreRepository<User> userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Зарегистрировать нового пользователя
    /// </summary>
    /// <param name="registerRequest">данные для регистрации</param>
    /// <returns></returns>
    /// <exception cref="EntityExistsException">если пользователь с таким email уже существует</exception>
    /// <exception cref="ApplicationException">ошибка при создании пользователя</exception>
    public AuthResponse RegisterUser(RegisterRequest registerRequest)
    {
        if (_userRepository.Any(u => u.Email == registerRequest.Email))
            throw new EntityExistsException(typeof(User), registerRequest.Email);

        var passwordHash = registerRequest.Password.Hash();

        var user = new User
        {
            Email = registerRequest.Email,
            PasswordHash = passwordHash,
            Name = registerRequest.Name
        };

        _userRepository.Add(user);
        _userRepository.SaveChanges();

        var token = GetToken(user.Email, registerRequest.Password);
        if (token == null)
        {
            throw new ApplicationException("Error while creating a user");
        }

        return new AuthResponse(
            Email: user.Email,
            AccessToken: token
        );
    }

    /// <summary>
    /// Предоставить доступ для зарегистрированного пользователя
    /// </summary>
    /// <param name="loginRequest">данные для входа</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException">если пользователя с таким email не существует</exception>
    /// <exception cref="AuthenticationException">неверные данные авторизации</exception>
    public AuthResponse LoginUser(LoginRequest loginRequest)
    {
        var user = _userRepository.GetListQuery()
            .FirstOrDefault(u => u.Email == loginRequest.Email);

        if (user == null)
        {
            throw new EntityNotFoundException(typeof(User), loginRequest.Email);
        }

        if (loginRequest.Password.Hash() != user.PasswordHash)
        {
            throw new AuthenticationException("Wrong password");
        }

        return new AuthResponse(
            Email: user.Email,
            AccessToken: GetToken(loginRequest.Email, loginRequest.Password)!
        );
    }

    private ClaimsIdentity? GetIdentity(string email, string password)
    {
        var passwordHash = password.Hash();

        // Информация о пользователе
        var user = _userRepository.GetListQuery()
            .FirstOrDefault(x => x.Email == email && x.PasswordHash == passwordHash);

        // Если пользователя нет
        if (user == null)
            return null;

        // Параметры токена
        var claims = new List<Claim>
        {
            new Claim(ApplicationJwtClaimTypes.Email, user.Email),
            new Claim(ApplicationJwtClaimTypes.Id, user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", ApplicationJwtClaimTypes.Email, "Role");

        return claimsIdentity;
    }


    private string? GetToken(string email, string password)
    {
        var identity = GetIdentity(email, password);

        if (identity == null)
            return null;

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: _jwtService.Issuer,
            audience: _jwtService.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(_jwtService.Lifetime)),
            signingCredentials: new SigningCredentials(_jwtService.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}