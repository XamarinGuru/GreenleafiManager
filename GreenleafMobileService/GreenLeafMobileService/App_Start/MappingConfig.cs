using System.Linq;
using AutoMapper;
using GreenLeafMobileService.DataObjects;
using ShopifyClient.Models;
using Customer = GreenLeafMobileService.DataObjects.Customer;
using Location = GreenLeafMobileService.DataObjects.Location;

namespace GreenLeafMobileService {
    public class MappingConfig {
        
        public static MapperConfiguration RegisterMappings () {
            var config = new MapperConfiguration( cfg => {
                cfg.CreateMap<Location, ShopifyClient.Models.Location>()
                    .ForMember( l => l.Id, x => x.MapFrom( l => l.OriginalId ) )
                    .ReverseMap();

                cfg.CreateMap<Customer, Address>()
                    .ForMember( a => a.Address1, x => x.MapFrom( c => c.Address ) )
                    .ReverseMap();

                cfg.CreateMap<Customer, ShopifyClient.Models.Customer>()
                    .ForMember( c => c.Address, x => x.Ignore() );

                cfg.CreateMap<ShopifyClient.Models.Customer, Customer>()
                    .ForMember( c => c.Address, x => x.MapFrom( c => c.Address != null ? c.Address.Address1 : string.Empty ) )
                    .ForMember( c => c.AddressOriginalId, x => x.MapFrom( c => c.Address != null ? c.Address.AddressOriginalId : 0 ) )
                    .ForMember( c => c.City, x => x.MapFrom( c => c.Address != null ? c.Address.City : string.Empty ) )
                    .ForMember( c => c.Province, x => x.MapFrom( c => c.Address != null ? c.Address.Province : string.Empty ) )
                    .ForMember( c => c.Country, x => x.MapFrom( c => c.Address != null ? c.Address.Country : string.Empty ) )
                    .ForMember( c => c.Zip, x => x.MapFrom( c => c.Address != null ? c.Address.Zip : string.Empty ) )
                    .ForMember( c => c.Phone, x => x.MapFrom( c => c.Address != null ? c.Address.Phone : string.Empty ) );

                cfg.CreateMap<Item, Inventory>()
                    .ForMember( i => i.Location, x => x.Ignore() )
                    .ForMember( i => i.GlItemCode, x => x.MapFrom( i => i.GlItemCode ) );

                cfg.CreateMap<Item, Variant>()
                    .ForMember( v => v.Price, x => x.MapFrom( i => i.TagPrice ) )
                    .ReverseMap();

                cfg.CreateMap<Inventory, Item>()
                    //.ForMember( item => item.Location, x => x.Ignore() )
                    .ForMember( item => item.GlItemCode, x => x.MapFrom( inv => inv.GlItemCode ) )
                    .ForMember( item => item.TagPrice, x => x.MapFrom( inv => inv.Variants != null && inv.Variants.Any() ? inv.Variants.First().Price : 0 ) )
                    .ForMember(item => item.VariantId, x => x.MapFrom(inv => inv.Variants != null && inv.Variants.Any() ? inv.Variants.First().VariantId : 0))
                    .ForMember( item => item.Sku, x => x.MapFrom( inv => inv.Variants != null && inv.Variants.Any() ? inv.Variants.First().Sku : string.Empty ) );
            } );
            return config;
        }
    }
}