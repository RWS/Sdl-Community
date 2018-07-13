using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class StudioVersionListItem : INotifyPropertyChanged
	{
		private bool _isSelected;
		public string DisplayName { get; set; }
		public string MajorVersionNumber { get; set; }
		public string MinorVersionNumber { get; set; }
		public string FolderName { get; set; }
		public string CacheFolderName { get; set; }

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
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
