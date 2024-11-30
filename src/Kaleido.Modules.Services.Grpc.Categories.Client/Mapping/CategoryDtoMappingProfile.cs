using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Client.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Client.Mapping;

public class CategoryDtoMappingProfile : Profile
{
    public CategoryDtoMappingProfile()
    {
        CreateMap<CategoryResponse, CategoryDto>()
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Revision, opt => opt.MapFrom(src => src.Revision));

        CreateMap<Category, CategoryEntityDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<CategoryEntityDto, Category>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<CategoryEntityDto, CategoryActionRequest>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Key, opt => opt.Ignore());

        CreateMap<CategoryRevision, CategoryRevisionDto>();
        //     .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
        //     .ForMember(dest => dest.Revision, opt => opt.MapFrom(src => src.Revision))
        //     .ForMember(dest => dest.Action, opt => opt.MapFrom(src => System.Enum.Parse<RevisionAction>(src.Action)))
        //     .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        //     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => System.Enum.Parse<RevisionStatus>(src.Status)));

        CreateMap<CategoryListResponse, IEnumerable<CategoryDto>>()
            .ConvertUsing((src, dest, context) =>
                src.Categories.Select(category => context.Mapper.Map<CategoryDto>(category)));

        // DateTime <-> Timestamp conversions
        CreateMap<Timestamp, DateTime>().ConvertUsing(src => src.ToDateTime());
        CreateMap<DateTime, Timestamp>().ConvertUsing(src => Timestamp.FromDateTime(src.ToUniversalTime()));

        // Guid <-> string conversions
        CreateMap<Guid, string>().ConvertUsing(src => src.ToString());
        CreateMap<string, Guid>().ConvertUsing(src => Guid.Parse(src));
    }
}