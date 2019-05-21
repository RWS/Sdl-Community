using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsProject:BaseModel
	{
		private string _name;
		private bool _isSelected;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
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

		public string SourceLanguageFlagUri { get; set; }
		public ObservableCollection<TargetLanguageFlag> TargetLanguageFlagsUri { get; set; }

		public string Status { get; set; }
		public string DueDate { get; set; }
	}
}
