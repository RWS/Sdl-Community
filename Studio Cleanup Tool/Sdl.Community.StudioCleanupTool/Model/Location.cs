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

	public delegate void DescriptionChangedEventHandler(object sender);
	public class Location: INotifyPropertyChanged
	{
		public event DescriptionChangedEventHandler DescriptionChanged;

		public Location()
	    {
		    
	    }
	    private bool _isSelected;
		public string DisplayName { get; set; }
	    public string Name { get; set; }
	    public string Path { get; set; }
	    public string Description { get; set; }
		private static List<string> _selectedLocations = new List<string>();
	    private ICommand _selectCommand;
		public ICommand SelectCommand => _selectCommand ?? (_selectCommand = new CommandHandler(Select, true));

	    private void Select()
	    {
		    if (IsSelected)
		    {
			    if (!_selectedLocations.Contains(DisplayName))
			    {
				    _selectedLocations.Add(DisplayName);
			    }
		    }
		    else
		    {
			    _selectedLocations.Remove(DisplayName);
		    }

	    }
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

	    public static List<string> GetSelectedLocations()
	    {
		    return _selectedLocations;

	    }

		public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		    if (propertyName != null && propertyName.Equals(nameof(IsSelected)))
		    {
			    var descriptionEvent = new DescriptionChangedEventArgs(Description);
			    if (DescriptionChanged != null) DescriptionChanged(this);
		    }
	    }
    }
}
