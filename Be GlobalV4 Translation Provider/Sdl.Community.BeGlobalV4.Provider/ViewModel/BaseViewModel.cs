using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	[DataContract]
	public class BaseViewModel:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;	 
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
