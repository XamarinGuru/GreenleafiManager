using Microsoft.Azure.Mobile.Server;
using System;

namespace GreenLeafMobileService.DataObjects {
    public class Item : EntityData {
        public string OriginalId { get; set; }
        public string GlItemCode { get; set; }
        public string MetalCode { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal TagPrice { get; set; }
        public string Description { get; set; }

        public string ShopifyDescription { get; set; }
        public string ShopifyTitle { get; set; }
        public decimal ShopifyPrice { get; set; }
        public string SecretCode { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public bool IsSold { get; set; }
        public string GlCode { get; set; }
        public decimal? GlCost { get; set; }
        public string VendorCode { get; set; }
        public string VariantId { get; set; }
        public string InvoiceDescription { get; set; }

        public string LocationId { get; set; }
        public string InvoiceId { get; set; }

        public bool ShowOnWebsite { get; set; }
        public DateTime LastCheckedDate { get; set; }
     



    }
}
