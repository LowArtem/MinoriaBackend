using AutoMapper;
using MinoriaBackend.Api.Api._Base;
using MinoriaBackend.Api.Attributes;
using MinoriaBackend.Api.Dto.Role;
using MinoriaBackend.Core.Dto;
using MinoriaBackend.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MinoriaBackend.Api.Api.Role;

/// <summary>
/// Контроллер управления ролями
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[SetRoute]
[Authorize]
public class RoleController : BaseCrudController<Core.Model.Auth.Role, RoleRequestDto, RoleResponseDto>
{
    /// <inheritdoc />
    public RoleController(IEfCoreRepository<Core.Model.Auth.Role> repository,
        ILogger<RoleController> logger, IMapper mapper) :
        base(repository, logger, mapper)
    {
    }
}