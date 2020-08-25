using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.MtEnhancedProvider.Model.Interface;

namespace Sdl.Community.MtEnhancedProvider.Model
{
	//public class ModelBase : IModelBase
	//{
	//	public event PropertyChangedEventHandler PropertyChanged;

	//	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	//	{
	//		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	//	}
	//}
	public class ModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
