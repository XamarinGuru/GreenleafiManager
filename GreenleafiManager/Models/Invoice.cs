using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GreenleafiManager
{
	public class Invoice
	{
		public string Id { get; set; }
		public string Number { get; set; }
		public string Date { get; set; }
		public decimal OrderTotal { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsMailOrder { get; set; }

		public string LocationId { get; set; }

		public string Notes { get; set; }
		public bool ShowOnWebsite { get; set; }

		[JsonIgnore]
		private string _LocationName;
		[JsonIgnore]
		//public virtual Location Location { get; set; }
		public string LocationName
		{
			get
			{
				if (String.IsNullOrEmpty(_LocationName))
				{
					var loc = AzureService.LocationService.Locations.Where(x => x.Id == LocationId).FirstOrDefault();
					if (loc == null)
						_LocationName = "";
					else
						_LocationName = loc.Name;
				}
				return _LocationName;
			}
			
		}
		[JsonIgnore]
		private Location _Location;
		[JsonIgnore]
		//public virtual Location Location { get; set; }
		public Location Location
		{
			get
			{
				var loc = AzureService.LocationService.Locations.Where(x => x.Id == LocationId).FirstOrDefault();
				if (loc == null)
					_Location = new Location();
				else
					_Location = loc;
				return _Location;
			}

		}
		[JsonIgnore]
		private string _LocationTax;
		[JsonIgnore]
		//public virtual Location Location { get; set; }
		public decimal LocationTax
		{
			get
			{
				var loc = AzureService.LocationService.Locations.Where(x => x.Id == LocationId).FirstOrDefault();
				if (loc == null)
					_LocationTax = "0.0";
				else
				{
					if (this.IsMailOrder)
						_LocationTax = "0.0";
					else
						_LocationTax = loc.Tax;
				}
				return Convert.ToDecimal(_LocationTax);
			}
		}
		public string CustomerId { get; set; }
		[JsonIgnore]
		public virtual Customer Customer { get; set; }
		public string UserId { get; set; }
		[JsonIgnore]
		public virtual User User { get; set; }
		[JsonIgnore]
		public virtual ICollection<Item> Items { get; set; }
		[JsonIgnore]
		public virtual ICollection<Payment> Payments { get; set; }

		[JsonIgnore]
		public double BalanceDue { get { return TotalPrice - PaidAmount; } }
		[JsonIgnore]
		public double PaidAmount { get; set; }
		[JsonIgnore]
		public double TotalPrice { get; set; }
		[JsonIgnore]
		public string SearchString
		{
			get
			{
				var ss = String.Format("{0} {1} {2} {3} {4} {5}", Number, Date, OrderTotal.ToString(), LocationName, Customer.FullName, User.FullName);
				return ss;
			}
		}
	}
}