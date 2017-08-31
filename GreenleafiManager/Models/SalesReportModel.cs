using System;
namespace GreenleafiManager
{
	
	public class SalesReportModel
	{
		public string LocationName { get; set; }
		public string SalesPersonName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public long ItemsSold { get; set; }
		public decimal TotalCost { get; set; }
		public decimal TotalSalePrice { get; set; }
		public decimal TotalTax { get; set; }
		public decimal TotalIncludingTax { get; set; }
		public string Errors { get; set;   }
	}
}

