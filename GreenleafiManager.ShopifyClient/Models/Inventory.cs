using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;

namespace ShopifyClient.Models {
    public class InventoryPictureRoot
    {
        [JsonProperty("image")]
        public InventoryImage image { get; set; }
    }
    public class InventoryRoot {
        [JsonProperty ( "products" )]
        public List<Inventory> Inventories { get; set; }

        [JsonProperty ( "product" )]
        public Inventory Inventory { get; set; }
    }

    public class Inventory {
        
     
        [JsonProperty ( "id" )]
        public long? OriginalId { get; set; }

        [JsonProperty ( "body_html" )]
        public string Description { get; set; }

        [JsonProperty ( "title" )]
        public string Title { get; set; }

        [JsonProperty ( "tags" )]
        public string Tags { get; set; }

        [JsonProperty ( "vendor" )]
        public string Vendor { get; set; }

        [JsonProperty ( "product_type" )]
        public string ProductType { get; set; }

        [JsonProperty("published_at")]
        public string published_at { get; set; }

        [JsonProperty("published")]
        public bool published { get; set; }

        [JsonProperty ( "variants" )]
        public List<Variant> Variants { get; set; }


        public List<ProductImage> Images { get; set; }

        [JsonIgnore]
        public string GlCode { get; set; }

        [JsonIgnore]
        public decimal GlCost { get; set; }

        [JsonIgnore]
        public string VendorCode { get; set; }

        [JsonIgnore]
        public string GlItemCode { get; set; }

        [JsonIgnore]
        public string SecretCode { get; set; }

        [JsonIgnore]
        public string MetalCode { get; set; }

        [JsonIgnore]
        public string Info1 { get; set; }

        [JsonIgnore]
        public string Info2 { get; set; }

        [JsonIgnore]
        public string Info3 { get; set; }

        [JsonIgnore]
        public bool IsSold { get; set; }

        [JsonIgnore]
        public string Location { get; set; }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    public class VariantRoot
    {
        [JsonProperty("variant")]
        public Variant variant { get; set; }
    }
    public class Variant {
        [JsonProperty ( "id" )]
        public long? VariantId { get; set; }

        [JsonProperty ( "price" )]
        public decimal Price { get; set; }

        [JsonProperty ( "sku" )]
        public string Sku { get; set; }

        [JsonProperty ( "barcode" )]
        public string Barcode { get; set; }

        [JsonProperty("inventory_management")]
        public string inventory_management { get; set; }

        [JsonProperty("inventory_quantity")]
        public long inventory_quantity { get; set; }
    }

    public class ImagesRoot
    {
        [JsonProperty("images")]
        public List<ProductImage> Images { get; set; }
    }

    public class ProductImage
    {
        //[JsonIgnore]
        public string azure_blob_scr
        {
            get
            {
                if (!string.IsNullOrEmpty(src))
                    return "http://greenleafpictures.blob.core.windows.net/greenleafitempictures/" + id + "__" + product_id + ".png";
                else
                    return string.Empty;
            }
        }
        public string id
        {
            get;
            set;
        }
        public string position
        {
            get;
            set;
        }
        public string product_id
        {
            get;
            set;
        }
        public string src
        {
            get;
            set;
        }
        public DateTime updated_at
        {
            get;
            set;
        }
        public DateTime created_at
        {
            get;
            set;
        }
    }
}