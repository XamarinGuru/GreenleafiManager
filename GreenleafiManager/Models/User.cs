using System;
using SQLite;
using Newtonsoft.Json;
using System.Linq;

namespace GreenleafiManager
{
	public class User
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "FirstName")]
		public string FirstName { get; set; }
		[JsonProperty(PropertyName = "LastName")]
		public string LastName { get; set; }
		[JsonProperty(PropertyName = "UserName")]
		public string UserName { get; set; }
		[JsonProperty(PropertyName = "address")]
		public string Address { get; set; }
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }
		[JsonProperty(PropertyName = "phone")]
		public string Phone { get; set; }
		[JsonProperty(PropertyName = "RoleId")]
		public string RoleId { get; set; }
		public string RoleName
		{
			get
			{
				var service = AzureService.UserService;
				var role = service.Roles.Where(x => x.Id == RoleId).FirstOrDefault();
				return role.Name;
			}
		}

		[JsonIgnore]
		public string FullName
		{
			get { return $"{FirstName} {LastName}"; }
		}
	}
}

