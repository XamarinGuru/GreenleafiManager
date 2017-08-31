using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenLeafMobileService.DataObjects
{
    public class Customer : EntityData
    {
        public string OriginalId { get; set; }
        public string AddressOriginalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool AcceptMarketing { get; set; }
        public string LastOrderDate { get; set; }
        public string Notes { get; set; }

        //public virtual ICollection<Invoice> Invoices { get; set; }
    }
}