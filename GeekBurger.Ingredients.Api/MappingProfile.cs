using AutoMapper;
using GeekBurger.Ingredients.Contract.Response;
using GeekBurger.Ingredients.DomainModel;
using GeekBurger.Products.Contract;
using System;
using System.Linq;

namespace GeekBurger.Ingredients.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<LabelImageAddedMessage, Ingredient>()
                .ForMember(destiny => destiny.Name, src => src.MapFrom(l => l.ItemName))
                .ForMember(destiny => destiny.Composition, src => src.MapFrom(l => l.Ingredients))
                .ReverseMap();

            this.CreateMap<ItemToGet, Ingredient>()
                .ForMember(destiny => destiny.Name, src => src.MapFrom(itg => itg.Name))
                .ReverseMap();

            this.CreateMap<ProductChangedMessage, ProductWithIngredients>()
                .ForMember(destiny => destiny.StoreId, src=> src.MapFrom(pcm => pcm.Product.StoreId))
                .ForMember(destiny => destiny.Id, src => src.MapFrom(pcm => pcm.Product.ProductId))
                .ForMember(destiny => destiny.Ingredients, src => src.MapFrom(pcm => pcm.Product.Items))
                .ReverseMap();

            this.CreateMap<ProductToGet, ProductWithIngredients>().ReverseMap();
            //.ForMember(destiny => destiny.ProductId, src => src.MapFrom(ptg => ptg.ProductId))
            //.ForMember(destiny => destiny.StoreId, src => src.MapFrom(ptg => ptg.StoreId));

            this.CreateMap<ProductWithIngredients, IngredientsToUpsert>()
                .ForMember(destiny => destiny.ProductId, src => src.MapFrom(pwi => pwi.Id))
                .ForMember(destiny => destiny.Ingredients, src => src.MapFrom(pwi => pwi.Ingredients.Select(i => String.Join(",",i.Composition)).ToHashSet()))
                .ReverseMap();
        }
    }
}