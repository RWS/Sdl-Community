using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TMLifting.Helpers
{
	public class SortableBindingList<T> : BindingList<T>
	{
		private bool _isSorted;
		private ListSortDirection _sortDirection;
		private PropertyDescriptor _sortProperty;
		private readonly PropertyDescriptorCollection _propertyDescriptors =
						 TypeDescriptor.GetProperties(typeof(T));


		public SortableBindingList(IEnumerable<T> enumerable) : base(enumerable.ToList()){ }

		public SortableBindingList() { }


		protected override bool SupportsSortingCore { get { return true; } }

		protected override bool IsSortedCore { get { return _isSorted; } }

		protected override ListSortDirection SortDirectionCore { get { return _sortDirection; } }

		protected override PropertyDescriptor SortPropertyCore { get { return _sortProperty; } }

		protected override void ApplySortCore(PropertyDescriptor prop,
											  ListSortDirection direction)
		{
			_isSorted = true;
			_sortDirection = direction;
			_sortProperty = prop;

			Func<T, object> predicate = n => n.GetType().GetProperty(prop.Name)
														.GetValue(n, null);

			ResetItems(_sortDirection == ListSortDirection.Ascending
						   ? Items.AsParallel().OrderBy(predicate)
						   : Items.AsParallel().OrderByDescending(predicate));
		}

		protected override void RemoveSortCore()
		{
			_isSorted = false;
			_sortDirection = base.SortDirectionCore;
			_sortProperty = base.SortPropertyCore;

			ResetBindings();
		}

		private void ResetItems(IEnumerable<T> items)
		{
			RaiseListChangedEvents = false;
			var tempList = items.ToList();
			ClearItems();

			foreach (var item in tempList) Add(item);

			RaiseListChangedEvents = true;
			ResetBindings();
		}
	}
}
