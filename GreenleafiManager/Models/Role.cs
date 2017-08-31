using System;
using SQLite;
using Newtonsoft.Json;

namespace GreenleafiManager
{
	public class Role
	{
		public string Id { get; set; }
		[JsonProperty(PropertyName = "Name")]
		public string Name {get;set;}
	}
}

