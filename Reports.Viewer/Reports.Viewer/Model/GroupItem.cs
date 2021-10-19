using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Reports.Viewer.Api.Model;

namespace Reports.Viewer.Plus.Model
{
	public class GroupItem: INotifyPropertyChanged
	{
		private string _name;		

		private List<Report> _reports;

		private bool _isSelected;

		private bool _isExtended;

		public GroupItem()
		{
			Reports = new List<Report>();
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

				_name = value ?? string.Empty;
				OnPropertyChanged(nameof(Name));
			}
		}	

		public List<Report> Reports
		{
			get => _reports;
			set
			{
				if (_reports == value)
				{
					return;
				}

				_reports = value;
				OnPropertyChanged(nameof(Reports));
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
