using System;
namespace GreenleafiManager
{
	public class InvoiceItems
	{
		public InvoiceItems()
		{
			if(string.IsNullOrEmpty(Id))
				Id = Guid.NewGuid().ToString();
		}
		public string Id { get; set; }
		public string Invoice_Id { get; set; }
		public string Item_Id { get; set; }
	}
}
