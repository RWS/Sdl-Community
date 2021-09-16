﻿using Rws.MultiSelectComboBox.API;
using Sdl.Community.StudioViews.ViewModel;

namespace Sdl.Community.StudioViews.Model
{
	public class FilterItemGroup : BaseModel, IItemGroup
	{
		private int _order;

		public FilterItemGroup(int index, string name)
		{
			Order = index;
			Name = name;
		}

		public int Order
		{
			get => _order;
			set
			{
				if (_order.Equals(value))
				{
					return;
				}

				_order = value;
				OnPropertyChanged(nameof(Order));
			}
		}

		public string Name { get; }

		public override string ToString()
		{
			return Name;
		}
	}
}
