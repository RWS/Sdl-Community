using System.Collections.ObjectModel;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class OrganizationTest:BaseModel
	{
		private string _title;
		private ObservableCollection<OrganizationTest> _children;
		private OrganizationTest _selectedItem;

		public string Title
		{
			get => _title;
			set
			{
				if (_title == value)
				{
					return;
				}
				_title = value;
				OnPropertyChanged(nameof(Title));
			}
		}

		public ObservableCollection<OrganizationTest> Children
		{
			get => _children;
			set
			{
				_children = value;
				OnPropertyChanged(nameof(Children));
			}
		}
		public OrganizationTest SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}
	}
}
