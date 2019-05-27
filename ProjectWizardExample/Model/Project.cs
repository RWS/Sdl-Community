using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectWizardExample.Model
{
	public class Project: INotifyPropertyChanged
    {
	    public string Name { get; set; }

	    public string Description { get; set; }

	    public string ClientName { get; set; }

	    public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
