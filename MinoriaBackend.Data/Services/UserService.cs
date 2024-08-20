using System.Security.Claims;
using MinoriaBackend.Core.Dto;
using MinoriaBackend.Core.Extensions;
using MinoriaBackend.Core.Model.Auth;
using MinoriaBackend.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using AutoMapper;
using MinoriaBackend.Core.Configurations;
using MinoriaBackend.Core.Exceptions;
using Microsoft.IdentityModel.Tokens;
using MinoriaBackend.Core.Model;

namespace MinoriaBackend.Data.Services;

public class UserService
{
    private readonly IEfCoreRepository<User> _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public UserService(IEfCoreRepository<User> userRepository, IMapper mapper, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Зарегистрировать нового пользователя
    /// </summary>
    /// <param name="registerDto">данные для регистрации</param>
    /// <returns></returns>
    /// <exception cref="EntityExistsException">если пользователь с таким email уже существует</exception>
    /// <exception cref="ApplicationException">ошибка при создании пользователя</exception>
    public AuthResponseDto RegisterUser(RegisterDto registerDto)
    {
        if (_userRepository.Any(u => u.Email == registerDto.Email))
            throw new EntityExistsException(typeof(User), registerDto.Email);

        var passwordHash = registerDto.Password.Hash();

        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            Name = registerDto.Name
        };

        _userRepository.Add(user);
        _userRepository.SaveChanges();

        var token = GetToken(user.Email, registerDto.Password);
        if (token == null)
        {
            throw new ApplicationException("Error while creating a user");
        }

        return new AuthResponseDto(
            Email: user.Email,
            AccessToken: token
        );
    }

    /// <summary>
    /// Предоставить доступ для зарегистрированного пользователя
    /// </summary>
    /// <param name="loginDto">данные для входа</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException">если пользователя с таким email не существует</exception>
    /// <exception cref="AuthenticationException">неверные данные авторизации</exception>
    public AuthResponseDto LoginUser(LoginDto loginDto)
    {
        var user = _userRepository.GetListQuery()
            .FirstOrDefault(u => u.Email == loginDto.Email);

        if (user == null)
        {
            throw new EntityNotFoundException(typeof(User), loginDto.Email);
        }

        if (loginDto.Password.Hash() != user.PasswordHash)
        {
            throw new AuthenticationException("Wrong password");
        }

        return new AuthResponseDto(
            Email: user.Email,
            AccessToken: GetToken(loginDto.Email, loginDto.Password)!
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
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new Claim("id", user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, "Role");

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