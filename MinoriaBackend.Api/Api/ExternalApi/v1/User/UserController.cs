using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinoriaBackend.Api.Api._Base;
using MinoriaBackend.Api.Api.ExternalApi.v1.User.Requests;
using MinoriaBackend.Api.Api.ExternalApi.v1.User.Responses;
using MinoriaBackend.Api.Attributes;
using MinoriaBackend.Core.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace MinoriaBackend.Api.Api.ExternalApi.v1.User;

/// <summary>
/// Контроллер управления пользователями
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[SetRoute]
[Authorize]
public class UserController : BaseCrudController<Core.Model.Auth.User, UserRequestDto, UserResponseDto>
{
    /// <inheritdoc />
    public UserController(IEfCoreRepository<Core.Model.Auth.User> repository,
        ILogger<UserController> logger, IMapper mapper) :
        base(repository, logger, mapper)
    {
    }

    /// <summary>
    /// Добавление записи
    /// </summary>
    /// <param name="model">Запись</param>
    /// <response code="400">Запись не прошла валидацию</response>
    /// <response code="409">Запись уже существует</response>
    /// <response code="500">При добавлении записи произошла ошибка на сервере</response>   
    /// <returns>Добавленная запись</returns>
    [HttpPost]
    [SwaggerResponse(200, "Запись успешно добавлена. Содержит информацию о добавленной записи", typeof(UserResponseDto))]
    [SwaggerResponse(400, "Ошибка валидации")]
    [SwaggerResponse(409, "Запись уже существует")]
    [SwaggerResponse(500, "Ошибка при добавлении записи")]
    public override ActionResult<UserResponseDto> Add(UserRequestDto model)
    {
        return List.Any(u => u.Email == model.Email)
            ? Conflict("Пользователь с таким Email уже существует")
            : base.Add(model);
    }

    /// <summary>
    /// Добавление записей
    /// </summary>
    /// <param name="models">Записи</param>
    /// <response code="400">Записи не прошли валидацию</response>
    /// <response code="409">Запись уже существует</response>
    /// <response code="500">При добавлении записи произошла ошибка на сервере</response>   
    /// <returns>Добавленная запись</returns>
    [HttpPost("range")]
    [SwaggerResponse(200, "Записи успешно добавлены. Содержит список добавленных записей", typeof(List<UserResponseDto>))]
    [SwaggerResponse(409, "Запись уже существует")]
    [SwaggerResponse(500, "Произошла ошибка при добавлении записей")]
    public override ActionResult<List<UserResponseDto>> AddRange(List<UserRequestDto> models)
    {
        return List.Any(u => models.Select(m => m.Email).Contains(u.Email))
            ? Conflict("Пользователь с таким Email уже существует")
            : base.AddRange(models);
    }
}