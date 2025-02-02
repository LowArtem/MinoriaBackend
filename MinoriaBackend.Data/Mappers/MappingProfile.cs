using AutoMapper;
using MinoriaBackend.Core.Dto.TransactionHistory.Update;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Data.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TransactionUpdateRequest, Transaction>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreate, opt => opt.Ignore())
            .ForMember(dest => dest.DateUpdate, opt => opt.Ignore())
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(request => request.AccountFrom))
            .ForMember(dest => dest.TransferTo, opt => opt.MapFrom(request => request.AccountTo))
            .ForMember(dest => dest.TransactionStatus, opt => opt.MapFrom(_ => TransactionStatus.NOT_STARTED))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(request => request.Category))
            .ForMember(dest => dest.Category, opt => opt.Ignore());
    }
}