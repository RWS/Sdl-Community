using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class MultiTermVersionListItem : BaseModel
	{
		private bool _isSelected;
		public string CacheFolderName { get; set; }
		public string DisplayName { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		public string MajorVersionNumber { get; set; }
		public string ReleaseNumber { get; set; }
	}
}