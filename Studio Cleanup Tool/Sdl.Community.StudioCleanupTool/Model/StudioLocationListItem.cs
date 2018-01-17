using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Helpers;

namespace Sdl.Community.StudioCleanupTool.Model
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
