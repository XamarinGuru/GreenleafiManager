using System;

using UIKit;

namespace GreenleafiManager
{
	public partial class ReportViewController : UIViewController
	{
		public ReportViewController() : base("ReportViewController", null)
		{
		}

		public ReportViewController(IntPtr handle) : base (handle)
		{
		}

		public virtual void UpdateLocationCode(string code) {
			throw new NotImplementedException();
		}

		public virtual void UpdateUserCode(string code)
		{
			throw new NotImplementedException();
		}
	}
}


