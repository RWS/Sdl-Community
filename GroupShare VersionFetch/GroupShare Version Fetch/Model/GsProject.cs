using System.Collections.ObjectModel;
using System.Drawing;

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

		public string ProjectId { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public Image Image { get; set; }
		public ObservableCollection<TargetLanguageFlag> TargetLanguageFlags { get; set; }
		public string Status { get; set; }
		public string DueDate { get; set; }
		public string SourceLanguage { get; set; }
	}
}
