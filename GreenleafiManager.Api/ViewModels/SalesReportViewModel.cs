using System;

namespace GreenleafiManager.Api.ViewModels {
    public class SalesReportViewModel {
        public string LocationName { get; set; }
        public string CustomerFullName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public long ItemsSold { get; set; }
        public decimal TotalCost { get; set; }
    }
}