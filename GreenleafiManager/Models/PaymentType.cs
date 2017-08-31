using System.Collections.Generic;

namespace GreenleafiManager {
    public class PaymentType {
		public string Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } 
    }
}