using System;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class Rule : ModelBase, ICloneable
	{
		private bool _isSelected;
		private string _name;
		private string _description;
		private int _order;


		public Rule()
		{
			Id = Guid.NewGuid().ToString();
			_isSelected = false;
			_name = string.Empty;
			_description = string.Empty;
			_order = 0;
		}

		public string Id { get; set; }

		public int Order
		{
			get => _order;
			set
			{
				if (_order == value)
				{
					return;
				}

				_order = value;
				OnPropertyChanged(nameof(Order));
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		public object Clone()
		{
			return new Rule
			{
				Id = Id,
				Name = Name,
				Description = Description,
				IsSelected = IsSelected,
				Order = Order
			};
		}
	}
}
