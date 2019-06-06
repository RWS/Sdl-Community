namespace Sdl.Community.SdiViewer
{
	public class SdiModel: ModelBase
	{
		private string _displayName;
		private string _code;
		private string _description;
		public string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value)
				{
					return;
				}
				_displayName = value;
				OnPropertyChanged(nameof(DisplayName));
			}
		}
		public string Code
		{
			get => _code;
			set
			{
				if (_code == value)
				{
					return;
				}
				_code = value;
				OnPropertyChanged(nameof(Code));
			}
		}
		public string Description
		{
			get => _description;
			set
			{
				if (_description == value)
				{
					return;
				}
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}
	}
}
