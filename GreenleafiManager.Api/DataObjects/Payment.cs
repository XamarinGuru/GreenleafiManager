using System;
using Microsoft.Azure.Mobile.Server;

namespace GreenleafiManager.Api.DataObjects {
    public class Payment : EntityData {
        public decimal PayedAmount { get; set; }
        public string Date { get; set; }
        public byte[] ReceiptImage { get; set; }
        public string ApprovalCode { get; set; }
        
        public string PaymentTypeId { get; set; }
        public virtual PaymentType PaymentType { get; set; }

        public string InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}