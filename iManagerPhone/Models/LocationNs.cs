using System;
using Foundation;

namespace GreenleafiManagerPhone
{
	public class LocationNs : NSObject
	{
		public LocationNs(Location l)
		{
			if (l != null)
			{
				Id = l.Id;
				Name = l.Name;
				Address1 = l.Address1;
				Address2 = l.Address2;
				Zip = l.Zip;
				City = l.City;
				Province = l.Province;
				Country = l.Country;
				Phone = l.Phone;
			}
		}
		[Export("Id")]
		public string Id { get; set; }
		[Export("Name")]
		public string Name { get; set; }
		[Export("Address1")]
		public string Address1 { get; set; }
		[Export("Address2")]
		public string Address2 { get; set; }
		[Export("Zip")]
		public string Zip { get; set; }
		[Export("City")]
		public string City { get; set; }
		[Export("Province")]
		public string Province { get; set; }
		[Export("Country")]
		public string Country { get; set; }
		[Export("Phone")]
		public string Phone { get; set; }
	}
}

