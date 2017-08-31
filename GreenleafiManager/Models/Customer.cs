using System;
using Newtonsoft.Json;
using Foundation;
using SQLite;

namespace GreenleafiManager
{
	public class Customer
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "AddressOriginalId")]
		public string AddressOriginalId { get; set; }
		[JsonProperty(PropertyName = "FirstName")]
		public string FirstName { get; set; }
		[JsonProperty(PropertyName = "LastName")]
		public string LastName { get; set; }
		[JsonProperty(PropertyName = "Address")]
		public string Address { get; set; }
		[JsonProperty(PropertyName = "City")]
		public string City { get; set; }
		[JsonProperty(PropertyName = "Country")]
		public string Country { get; set; }
		[JsonProperty(PropertyName = "Province")]
		public string Province { get; set; }
		[JsonProperty(PropertyName = "Zip")]
		public string Zip { get; set; }
		[JsonProperty(PropertyName = "Email")]
		public string Email { get; set; }
		[JsonProperty(PropertyName = "Phone")]
		public string Phone { get; set; }
		[JsonProperty(PropertyName = "AcceptMarketing")]
		public bool AcceptMarketing { get; set; }
		[JsonProperty(PropertyName = "LastOrderDate")]
		public string LastOrderDate { get; set; }
		[JsonProperty(PropertyName = "Notes")]
		public string Notes { get; set; }

		[JsonIgnore]
		public string FullName
		{
			get { return $"{FirstName} {LastName}"; }
		}

		[JsonIgnore]
		public string SearchString
		{
			get
			{
				var ss = String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
					FirstName != null ? FirstName : string.Empty,
					LastName != null ? LastName : string.Empty,
					Email != null ? Email : string.Empty,
					Phone != null ? Phone : string.Empty,
				   City != null ? City : string.Empty,
				   Address != null ? Address : string.Empty,
				   Province != null ? Province : string.Empty,
				   Country != null ? Country : string.Empty,
								   Zip != null ? Zip : string.Empty);
				return ss;
			}
		}
	}
}

