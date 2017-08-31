using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenLeafMobileService.DataObjects {
    public class Invoice : EntityData {
        public string Number { get; set; }
        public string Date { get; set; }
        public decimal OrderTotal { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsMailOrder { get; set; }
        public string LocationId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        
        public string Notes { get; set; }
        public bool ShowOnWebsite { get; set; }
    }
}