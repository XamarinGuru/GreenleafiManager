using System;
using System.ComponentModel.DataAnnotations;

namespace GreenLeafMobileService.ViewModels {
    public class SalesReportFiltersViewModel {
        //[Required]
        public string LocationId { get; set; }

        //[Required]
        public string CustomerId { get; set; }

        //[Required]
        public DateTime DateFrom { get; set; }

        //[Required]
        public DateTime DateTo { get; set; }
    }
}