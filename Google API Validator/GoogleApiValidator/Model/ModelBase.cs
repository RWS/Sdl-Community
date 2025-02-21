using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.GoogleApiValidator.Annotations;

namespace Sdl.Community.GoogleApiValidator.Model
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
