using System;
using System.Collections.Generic;

namespace GreenleafiManager
{
	public static class PaymentPaiedByLogic
	{
		public static string Calculate(List<Payment> list)
		{
			var returnValue = string.Empty;
			foreach (var item in list)
			{
				if (item.PaymentType != null)
				{
					if (returnValue.Equals(string.Empty))
						returnValue = item.PaymentType.ShortName;
					else
						returnValue = returnValue + " / " + item.PaymentType.ShortName;
				}

			}
			return returnValue;
		}
	}
}
