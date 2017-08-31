using Newtonsoft.Json;

namespace ShopifyClient.Models {
    public class AddressRoot {
        [JsonProperty ( "address" )]
        public Address UpdateAddress { get; set; }

        [JsonProperty ( "customer_address" )]
        public Address CustomerAddress { get; set; }
    }

    public class Address {
        [JsonProperty ( "id" )]
        public long? AddressOriginalId { get; set; }

        [JsonProperty ( "address1" )]
        public string Address1 { get; set; }

        [JsonProperty ( "address2" )]
        public string Address2 { get; set; }

        [JsonProperty ( "city" )]
        public string City { get; set; }

        [JsonProperty ( "province" )]
        public string Province { get; set; }

        [JsonProperty ( "country" )]
        public string Country { get; set; }

        [JsonProperty ( "zip" )]
        public string Zip { get; set; }

        [JsonProperty ( "phone" )]
        public string Phone { get; set; }
    }
}