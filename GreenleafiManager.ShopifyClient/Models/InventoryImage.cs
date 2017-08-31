using Newtonsoft.Json;

namespace ShopifyClient.Models
{
    public class InventoryImage
    {
        [JsonProperty("attachment")]
        public string attachment { get; set; }
        [JsonProperty("filename")]
        public string filename { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("product_id")]
        public string product_id { get; set; }
        [JsonProperty("position")]
        public string position { get; set; }
    }

    public class InventoryUpdateImage
    {
        [JsonProperty("position")]
        public string position { get; set; }
    }
}