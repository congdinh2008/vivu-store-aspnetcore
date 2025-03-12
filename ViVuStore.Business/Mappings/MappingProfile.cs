using AutoMapper;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.ViewModels;
using ViVuStore.Models.Common;
using ViVuStore.Models.Security;

namespace ViVuStore.Business.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryCreateUpdateCommand, Category>();
        
        // Supplier mappings
        CreateMap<Supplier, SupplierViewModel>();
        CreateMap<SupplierCreateUpdateCommand, Supplier>();
    }
}
