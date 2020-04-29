using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.AhkPlugin.Annotations;

namespace Sdl.Community.AhkPlugin.Model
{
	public class ModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
