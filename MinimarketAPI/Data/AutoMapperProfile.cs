using System.Globalization;
using AutoMapper;
using Minimarket.DTO;
using Minimarket.Model;

namespace Minimarket.Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Usuario, UserDTO>();
        CreateMap<UserDTO, Usuario>();

        CreateMap<ResponseUserDTO, Usuario>().ForMember(d => d.Id, opt => opt.Ignore());
        CreateMap<Usuario, ResponseUserDTO>();

        CreateMap<Product, ProductDTO>();
        CreateMap<ProductDTO, Product>().ForMember(
            d => d.Id, opt => opt.Ignore()
        ).ForMember(d => d.RegisterDate, opt => opt.Ignore());

        //CreateMap<Sale, SaleDTO>();
        //CreateMap<SaleDTO, Sale>().ForMember(
        //    d => d.Id, opt => opt.Ignore()
        // ).ForMember(d => d.RegisterDate, opt => opt.Ignore());

        CreateMap<Sale, SaleDTO>().ForMember(
            d => d.TotalText, opt => opt.MapFrom(origen => Convert.ToString(origen.Total!.Value, CultureInfo.InvariantCulture))
        );
        //.ForMember(d => d.RegisterDate, opt => opt.MapFrom(origin => origin.RegisterDate!.Value.ToString("d/M/yyyy", new CultureInfo("es-ES"))));

        CreateMap<SaleDTO, Sale>()
        .ForMember(d => d.Total,
        opt => opt.MapFrom(origin => Convert.ToDecimal(origin.TotalText, CultureInfo.InvariantCulture)));

        CreateMap<SaleDetail, SaleDetailDTO>()
        .ForMember(d => d.IdProductDescription,
            opt => opt.MapFrom(origin => origin.IdProductNavigation!.Name))
        .ForMember(d => d.PriceText,
            opt => opt.MapFrom(origin => Convert.ToString(origin.Price!.Value, CultureInfo.InvariantCulture)))
        .ForMember(d => d.TotalText,
            opt => opt.MapFrom(origin => Convert.ToString(origin.Total!.Value, CultureInfo.InvariantCulture)));

        CreateMap<SaleDetailDTO, SaleDetail>()
        .ForMember(
            d => d.Price, opt => opt.MapFrom(origin => Convert.ToDecimal(origin.PriceText, CultureInfo.InvariantCulture))
        )
         .ForMember(
            d => d.Total, opt => opt.MapFrom(origin => Convert.ToDecimal(origin.TotalText, CultureInfo.InvariantCulture))
        );

        CreateMap<SaleDetail, ReportDTO>()
        //.ForMember(d => d.RegisterDate,
        //    opt => opt.MapFrom(origin => origin.IdSaleNavigation!.RegisterDate!.Value.ToString("d/M/yyyy", new CultureInfo("es-ES"))))
        .ForMember(d => d.DocumentNumber,
            opt => opt.MapFrom(origin => origin.IdSaleNavigation!.DocumentNumber))
        .ForMember(d => d.PaidType,
            opt => opt.MapFrom(origin => origin.IdSaleNavigation!.PaidType))
        .ForMember(d => d.SaleTotal,
            opt => opt.MapFrom(origin => Convert.ToString(origin.IdSaleNavigation!.Total!.Value, CultureInfo.InvariantCulture)))
        .ForMember(d => d.Product,
            opt => opt.MapFrom(origin => origin.IdProductNavigation!.Name))
        .ForMember(d => d.Price,
            opt => opt.MapFrom(origin => Convert.ToString(origin.Price!.Value, CultureInfo.InvariantCulture)))
        .ForMember(d => d.Total,
            opt => opt.MapFrom(origin => Convert.ToString(origin.Total!.Value, CultureInfo.InvariantCulture)));

    }
}