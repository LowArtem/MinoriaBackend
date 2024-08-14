using MinoriaBackend.Core.Model.Auth;
using AutoMapper;
using MinoriaBackend.Api.Api.InternalApi.v1.User.Requests;
using MinoriaBackend.Api.Api.InternalApi.v1.User.Responses;
using MinoriaBackend.Core.Extensions;

namespace MinoriaBackend.Api.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRequestDto, User>()
            .ForMember(dest => dest.PasswordHash,
                opt => opt.MapFrom(x => x.Password.Hash()));

        CreateMap<User, UserResponseDto>();
    }
}