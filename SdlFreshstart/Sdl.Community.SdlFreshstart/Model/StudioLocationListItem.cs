using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class StudioLocationListItem : BaseModel
	{
		private bool _isSelected;
		public string Alias { get; set; }
		public string Description { get; set; }
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
	}
}