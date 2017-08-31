using System;
using Newtonsoft.Json;

namespace GreenleafiManager {
    public class Payment {
		public Payment()
		{
			if (string.IsNullOrEmpty(Id))
				Id = Guid.NewGuid().ToString();

			Update = false;
			Remove = false;
			Add = false;
		}
		public string Id { get; set; }
        public decimal PaidAmount { get; set; }
        public string Date { get; set; }
        public byte[] ReceiptImage { get; set; }
        public string ApprovalCode { get; set; }
        
        public string PaymentTypeId { get; set; }
		[JsonIgnore]
        public virtual PaymentType PaymentType { get; set; }

        public string InvoiceId { get; set; }
		[JsonIgnore]
        public virtual Invoice Invoice { get; set; }

		[JsonIgnore]
		public bool Update { get; set; }
		[JsonIgnore]
		public bool Remove { get; set; }
		[JsonIgnore]
		public bool Add { get; set; }

    }
}