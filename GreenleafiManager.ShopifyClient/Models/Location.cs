using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShopifyClient.Models {
    public class LocationsRoot {
        [JsonProperty ( "locations" )]
        public List<Location> Locations { get; set; }
    }

    public class Location {
        [JsonProperty ( "id" )]
        public long Id { get; set; }

        [JsonProperty ( "name" )]
        public string Name { get; set; }

        [JsonProperty ( "address1" )]
        public string Address1 { get; set; }

        [JsonProperty ( "address2" )]
        public string Address2 { get; set; }

        [JsonProperty ( "city" )]
        public string City { get; set; }

        [JsonProperty ( "zip" )]
        public string Zip { get; set; }

        [JsonProperty ( "province" )]
        public string Province { get; set; }

        [JsonProperty ( "country" )]
        public string Country { get; set; }

        [JsonProperty ( "phone" )]
        public string Phone { get; set; }
    }
}