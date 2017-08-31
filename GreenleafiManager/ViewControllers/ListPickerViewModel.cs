using System;
using UIKit;
using System.Collections.Generic;

namespace GreenleafiManager
{
	public class ListPickerViewModel<TItem> : UIPickerViewModel
	{
		public TItem SelectedItem { get; private set; }

		IList<TItem> _items;
		public IList<TItem> Items
		{
			get { return _items; }
			set { _items = value; Selected(null, 0, 0); }
		}

		public ListPickerViewModel()
		{
		}

		public ListPickerViewModel(IList<TItem> items)
		{
			Items = items;
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			var IntComponent = Convert.ToInt16 (component);
			if (NoItem())
				return 1;
			return Items.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			var IntRow = Convert.ToInt16 (row);
			var IntComponent = Convert.ToInt16 (component);

			if (NoItem(IntRow))
				return "";
			var item = Items[IntRow];
			return GetTitleForItem(item);
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			var IntRow = Convert.ToInt16 (row);
			var IntComponent = Convert.ToInt16 (component);

			if (NoItem(IntRow))
				SelectedItem = default(TItem);
			else
				SelectedItem = Items[IntRow];
		}

		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public virtual string GetTitleForItem(TItem item)
		{
			return item.ToString();
		}

		bool NoItem(int row = 0)
		{
			return Items == null || row >= Items.Count;
		}
	}
}

