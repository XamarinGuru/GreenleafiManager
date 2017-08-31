using System;
using SQLite;
using Newtonsoft.Json;

namespace GreenleafiManager
{
	public class Location
	{
		//public Location ()
		//{
		//	if (Id == null)
		//		Id = Guid.NewGuid ().ToString();
		//}

		[PrimaryKey, Unique]
		[JsonProperty(PropertyName = "Id")]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "Name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "Address1")]
		public string Address1 { get; set; }
		[JsonProperty(PropertyName = "Address2")]
		public string Address2 { get; set; }
		[JsonProperty(PropertyName = "Zip")]
		public string Zip { get; set; }
		[JsonProperty(PropertyName = "City")]
		public string City { get; set; }
		[JsonProperty(PropertyName = "Province")]
		public string Province { get; set; }
		[JsonProperty(PropertyName = "Country")]
		public string Country { get; set; }
		[JsonProperty(PropertyName = "Phone")]
		public string Phone { get; set; }
		[JsonProperty(PropertyName = "Tax")]
		public string Tax { get; set; }

		//[JsonProperty(PropertyName = "GlCost")]
	////	public decimal? GlCost { get; set; }

		public override string ToString ()
		{
			return Name.ToString ();
		}
	}
}

