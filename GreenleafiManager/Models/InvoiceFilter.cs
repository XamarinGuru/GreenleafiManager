using System;
using System.Collections.Generic;

namespace GreenleafiManager
{
	public class InvoiceFilter
	{
		public InvoiceFilter()
		{
			Numbers = new List<string>();
			OrderDates = new List<DateTime>();
			Locations = new List<string>();
		}
		public IList<string> Numbers
		{
			get;
			set;
		}
		public IList<DateTime> OrderDates
		{
			get;
			set;
		}

		public IList<string> Locations
		{
			get;
			set;
		}
		public string Item
		{
			get;
			set;
		}
		public decimal OrderTotal
		{
			get;
			set;
		}
		public string Sku
		{
			get;
			set;
		}
	}
}
