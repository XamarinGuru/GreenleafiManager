using System;
using Foundation;

namespace GreenleafiManager
{
	public class CustomerNs: NSObject
	{
		public CustomerNs (Customer cm)
		{
			Id = cm.Id;
			firstName = cm.FirstName;
			lastName = cm.LastName;
			address = cm.Address;
			email = cm.Email;
			phoneNumber = cm.Phone;
			SetSearchString ();
		}
		[Export("Id")]
		public string Id {
			get;
			set;
		}
		[Export("Name")]
		public string firstName {get;set;}

		[Export("Last_Name")]
		public string lastName
		{
			get;
			set;
		}
		[Export("LastNameInitial")]
		public string lastNameInitial
		{
			get
			{
				var i = lastName == null ? "" : lastName.Substring(0, 1);
				return i;
			}
		}
		[Export("Address")]
		public string address
		{
			get;
			set;
		}
		[Export("City")]
		public string city
		{
			get;
			set;
		}
		[Export("Country")]
		public string country
		{
			get;
			set;
		}
		[Export("Province")]
		public string province
		{
			get;
			set;
		}
		[Export("Zip")]
		public string zip
		{
			get;
			set;
		}
		[Export("Email")]
		public string email {
			get;
			set;
		}
		[Export("Phone")]
		public string phoneNumber {
			get;
			set;
		}
		[Export("SearchString")]
		public string SearchString { get; set; }


		private void SetSearchString()
		{
			this.SearchString = String.Format ("{0} {1} {2} {3} {4} {5} {6} {7}", 
				firstName != null ? firstName : string.Empty, 
				lastName != null ? lastName : string.Empty,
				email != null ? email : string.Empty,
				phoneNumber != null ? phoneNumber : string.Empty,
               city != null ? city : string.Empty,
               address != null ? address : string.Empty,
               province != null ? province : string.Empty,
			   country != null ? country : string.Empty,
               zip != null ? zip : string.Empty

			);
			//return SearchString;
		}

	}
}

