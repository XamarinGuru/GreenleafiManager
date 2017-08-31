using GreenLeafMobileService.DataObjects;
using ShopifyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Customer = GreenLeafMobileService.DataObjects.Customer;
using Location = GreenLeafMobileService.DataObjects.Location;

namespace GreenLeafMobileService.Helper
{
    public static class Converter
    {
        public static ShopifyClient.Models.Location Convert(Location model)
        {
            return new ShopifyClient.Models.Location()
            {
                Id = model.OriginalId,
                Name = model.Name,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Country = model.City,
                Phone = model.Phone,
                Province = model.Province,
                Zip = model.Zip
            };
        }

        public static Customer Convert(Customer customer, ShopifyClient.Models.Address model)
        {
            customer.Address = model.Address1;
            customer.City = model.City;
            customer.Country = model.Country;
            customer.Phone = model.Phone;
            customer.Province = model.Province;
            customer.Zip = model.Zip;

            return customer;
        }

        public static ShopifyClient.Models.Customer Convert(Customer model)
        {
            return new ShopifyClient.Models.Customer()
            {
                //AcceptMarketing = model.AcceptMarketing,
                FirstName = model.FirstName,
                Email = model.Email,
                LastName = model.LastName,
                //Id = model.OriginalId,
                Notes = model.Notes
            };
        }

        public static Customer Convert(ShopifyClient.Models.Customer model)
        {
            //source -> destination
            return new Customer()
            {
                //AcceptMarketing = model.AcceptMarketing,
                FirstName = model.FirstName,
                Email = model.Email,
                LastName = model.LastName,
                //Id = model.OriginalId,
                Notes = model.Notes,
                Address = model.Address != null ? model.Address.Address1 : string.Empty,
                AddressOriginalId = model.Address != null ? model.Address.AddressOriginalId.ToString() : "0",
                City = model.Address != null ? model.Address.City : string.Empty,
                Province = model.Address != null ? model.Address.Province : string.Empty,
                Country = model.Address != null ? model.Address.Country : string.Empty,
                Zip = model.Address != null ? model.Address.Zip : string.Empty,
                Phone = model.Address != null ? model.Address.Phone : string.Empty,
            };
        }

        //Item, Inventory
        //source -> destination
        public static Inventory Convert(Item model)
        {
            //source -> destination
            var result = new Inventory()
            {
                Title = model.ShopifyTitle,
                Description = model.ShopifyDescription,
                GlCode = model.GlCode,
                GlCost = model.GlCost.HasValue ? decimal.Parse(model.GlCost.Value.ToString()) : 0,
                GlItemCode = model.GlItemCode,
                Info1 = model.Info1,
                Info2 = model.Info2,
                Info3 = model.Info3,
                IsSold = model.IsSold,
                //Location = model.Location != null ? model.Location.
                MetalCode = model.MetalCode,
                SecretCode = model.SecretCode,
                OriginalId = model.OriginalId == null ? 0 : long.Parse(model.OriginalId),
                VendorCode = model.VendorCode,
                published = model.ShowOnWebsite
            };
            if (model.ShowOnWebsite)
                result.published_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            else
                result.published_at = null;
            return result;
        }
        public static Item Convert(Inventory model)
        {
            //source -> destination
            return new Item()
            {
                ShopifyDescription = model.Description,
                ShopifyTitle = model.Title,
                GlCode = model.GlCode,
                GlCost = model.GlCost,
                GlItemCode = model.GlItemCode,
                Info1 = model.Info1,
                Info2 = model.Info2,
                Info3 = model.Info3,
                IsSold = model.IsSold,
                // Location = model.Location,
                MetalCode = model.MetalCode,
                SecretCode = model.SecretCode,
                OriginalId = model.OriginalId.ToString(),//This is the shopify ID
                VendorCode = model.VendorCode,
                ShopifyPrice = model.Variants != null ? model.Variants.First().Price : 0,
                Sku = model.Variants != null ? model.Variants.First().Sku : "",
                VariantId = model.Variants != null ? model.Variants.First().VariantId.ToString() : ""

            };
        }
        public static Variant ConvertToVariant(Item model)
        {
            long vId = 0;
            long.TryParse(model.VariantId, out vId);
            return new Variant()
            {
                Price = model.ShopifyPrice,
                Sku = model.Sku,
                VariantId = vId,
                inventory_management = "shopify",
                inventory_quantity = 1

            };
        }
    }
}