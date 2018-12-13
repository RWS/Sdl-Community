using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BaseViewModel:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;	 
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
