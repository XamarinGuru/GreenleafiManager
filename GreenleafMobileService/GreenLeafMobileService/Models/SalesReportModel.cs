using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLeafMobileService.Models
{
    public class SalesReportModel
    {
        public SalesReportModel(decimal totalValueItems, int totalItemsSold)
        {
            this.TotalItemsSold = totalItemsSold;
            this.TotalValueItems = totalValueItems;
        }
        public decimal TotalValueItems { get; set; }
        public int TotalItemsSold { get; set; }
    }
}