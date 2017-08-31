using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLeafMobileService.Models
{
    public class CurrentStockReportModel
    {
        public CurrentStockReportModel(int totalofitems, decimal totalvalueofitems)
        {
            this.TotalOfItems = totalofitems;
            this.TotalValueOfItems = totalvalueofitems;
        }
        public int TotalOfItems { get; set; }
        public decimal TotalValueOfItems { get; set; }
    }
}