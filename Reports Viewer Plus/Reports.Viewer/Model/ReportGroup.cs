using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reports.Viewer.Plus.Model
{
	public class ReportGroup : INotifyPropertyChanged
	{
		private string _name;

		private List<GroupItem> _groupItems;

		private bool _isSelected;

		private bool _isExtended;

		public ReportGroup()
		{
			GroupItems = new List<GroupItem>();
		}

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
				{
					return;
				}

				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public List<GroupItem> GroupItems
		{
			get => _groupItems;
			set
			{
				if (_groupItems == value)
				{
					return;
				}

				_groupItems = value;
				OnPropertyChanged(nameof(GroupItems));
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected == value)
				{
					return;
				}

				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public bool IsExpanded
		{
			get => _isExtended;
			set
			{
				if (_isExtended == value)
				{
					return;
				}

				_isExtended = value;
				OnPropertyChanged(nameof(IsExpanded));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
