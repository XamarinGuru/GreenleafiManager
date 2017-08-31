using System;
using Foundation;

namespace GreenleafiManager
{
	public class UserNs : NSObject
	{
		public UserNs (User u)
		{
			Id = u.Id;
			UserName = u.UserName;
			Address = u.Address;
			Email = u.Email;
			Phone = u.Phone;
			FirstName = u.FirstName;
			LastName = u.LastName;

			SetSearchString ();
		}

		[Export("Id")]
		public string Id { get; set; }
		public string CreatedAt { get; set; }

		public string UpdatedAt { get; set; }

		[Export("DisplayName")]
		public string DisplayName {get;set;}

		public string FirstName {get;set;}
		public string LastName {get;set;}

		[Export("UserName")]
		public string UserName {
			get;
			set;
		}
		[Export("Address")]
		public string Address {
			get;
			set;
		}
		[Export("Email")]
		public string Email {
			get;
			set;
		}
		[Export("Phone")]
		public string Phone {
			get;
			set;
		}
		[Export("RoleId")]
		public string RoleId {
			get;
			set;
		}
		[Export("RoleName")]
		public string RoleName {
			get;
			set;
		}
		[Export("SearchString")]
		public string SearchString { get; set; }

		private void SetSearchString()
		{
			this.SearchString = String.Format ("{0} {1} {2} {3}", 
				UserName != null ? UserName : string.Empty, 
				Phone != null ? Phone : string.Empty,
				Email != null ? Email : string.Empty,
				RoleName != null ? RoleName : string.Empty
			);
		}
	}
}

