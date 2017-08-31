using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenLeafMobileService.DataObjects {
    public class Location : EntityData {
        public long OriginalId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Tax { get; set; }
      //  public decimal? GlCost { get; set; }

       // public virtual ICollection<Invoice> Invoices { get; set; } 
       // public virtual ICollection<Item> Items { get; set; } 
    }
}