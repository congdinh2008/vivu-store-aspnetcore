using AutoMapper;
using ViVuStore.Business.ViewModels;
using ViVuStore.Models.Common;

namespace ViVuStore.API.Configuration;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CategoryViewModel, Category>().ReverseMap()
            .ForMember(x => x.CreatedBy, opt => opt.MapFrom(e => e.CreatedBy != null ? e.CreatedBy.DisplayName: string.Empty))
            .ForMember(x => x.UpdatedBy, opt => opt.MapFrom(e => e.UpdatedBy != null ? e.UpdatedBy.DisplayName: string.Empty))
            .ForMember(x => x.DeletedBy, opt => opt.MapFrom(e => e.DeletedBy != null ? e.DeletedBy.DisplayName: string.Empty));
    }
}
