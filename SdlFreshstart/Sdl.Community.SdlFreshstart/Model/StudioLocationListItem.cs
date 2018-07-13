using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class StudioLocationListItem: INotifyPropertyChanged
	{
		private bool _isSelected;
		public string DisplayName { get; set; }
	    public string Name { get; set; }
	    public string Path { get; set; }
	    public string Description { get; set; }
		public string Alias { get; set; }

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
