using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShopifyClient.Models {
    public class CustomerRoot {
        [JsonProperty("customers")]
        public List<Customer> Customers { get; set; }

        [JsonProperty ( "customer" )]
        public Customer Customer { get; set; }
    }

    public class Customer {
        [JsonProperty ( "id" )]
        public long? OriginalId { get; set; }

        [JsonProperty ( "first_name" )]
        public string FirstName { get; set; }

        [JsonProperty ( "last_name" )]
        public string LastName { get; set; }

        [JsonProperty ( "email" )]
        public string Email { get; set; }

        [JsonProperty ( "verified_email" )]
        public bool? VerifiedEmail { get; set; } = true;

        [JsonProperty ( "default_address" )]
        public Address Address { get; set; }

        [JsonProperty ( "addresses" )]
        public List<Address> Addresses { get; set; }

        [JsonProperty ( "last_order_id" )]
        public long? LastOrderId { get; set; }

        [JsonIgnore]
        public DateTime? LastOrderDate { get; set; }

        [JsonProperty ( "accepts_marketing" )]
        public bool? AcceptMarketing { get; set; }

        [JsonProperty ( "note" )]
        public string Notes { get; set; }
    }
}