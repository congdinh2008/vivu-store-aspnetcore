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
        
        // Product mappings
        CreateMap<Product, ProductViewModel>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null));
        CreateMap<ProductCreateUpdateCommand, Product>();
        
        // Order mappings
        CreateMap<Order, OrderViewModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
            
        CreateMap<OrderDetail, OrderDetailViewModel>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.ProductThumbnail, opt => opt.MapFrom(src => src.Product != null ? src.Product.Thumbnail : null));
    }
}
