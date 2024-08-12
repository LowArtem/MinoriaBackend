using MinoriaBackend.Core.Dto;
using MinoriaBackend.Core.Model.Auth;
using AutoMapper;
using MinoriaBackend.Api.Dto.Role;
using MinoriaBackend.Api.Dto.User;
using MinoriaBackend.Core.Extensions;

namespace MinoriaBackend.Api.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, AuthResponseDto>()
            .ForMember(
                dest => dest.Roles,
                opt =>
                    opt.MapFrom(u => u.UserRoles.Select(r => r.Id)));

        CreateMap<UserRequestDto, User>()
            .ForMember(dest => dest.PasswordHash,
                opt => opt.MapFrom(x => x.Password.Hash()));

        CreateMap<User, UserResponseDto>();
        CreateMap<Role, RoleResponseDto>();
        CreateMap<RoleRequestDto, Role>();
    }
}