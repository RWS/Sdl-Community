using System.ComponentModel;
using System.Runtime.CompilerServices;
using Rws.MultiSelectComboBox.API;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class FilterItemGroup: IItemGroup, INotifyPropertyChanged
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
