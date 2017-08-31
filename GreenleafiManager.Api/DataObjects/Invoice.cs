using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenleafiManager.Api.DataObjects {
    public class Invoice : EntityData {
        public string Number { get; set; }
        public string Date { get; set; }
        public decimal OrderTotal { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsMailOrder { get; set; }

        public string LocationId { get; set; }
        public virtual Location Location { get; set; }

        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}