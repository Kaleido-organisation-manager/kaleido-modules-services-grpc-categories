using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Mappers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>, CategoryResponse>()
            .ForMember(c => c.Category, opt => opt.MapFrom(src => src.Entity))
            .ForMember(c => c.Revision, opt => opt.MapFrom(src => src.Revision))
            .ForMember(c => c.Key, opt => opt.MapFrom(src => src.Key));

        CreateMap<CategoryEntity, Category>().ReverseMap();
        CreateMap<BaseRevisionEntity, CategoryRevision>();

        CreateMap<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>, CategoryListResponse>()
            .ForMember(c => c.Categories, opt => opt.MapFrom(c => c));

        // DateTime <-> Timestamp conversions
        CreateMap<Timestamp, DateTime>().ConvertUsing(src => src.ToDateTime());
        CreateMap<DateTime, Timestamp>().ConvertUsing(src => Timestamp.FromDateTime(src.ToUniversalTime()));
    }
}