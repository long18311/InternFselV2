using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Model.CommandModel.ProductCmd;
using InternFselV2.Model.CommandModel.UserCmd;
using InternFselV2.Model.EnityModel;

namespace InternFselV2.Maps
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductCommandModel, Product>();
            CreateMap<UpdateProductCommandModel, Product>();
            CreateMap<Product, ProductModel>().MaxDepth(1);
        }
    }
}
