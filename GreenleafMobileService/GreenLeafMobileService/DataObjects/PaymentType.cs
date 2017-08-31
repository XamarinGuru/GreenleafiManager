using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenLeafMobileService.DataObjects {
    public class PaymentType : EntityData {
        public string ShortName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } 
    }
}