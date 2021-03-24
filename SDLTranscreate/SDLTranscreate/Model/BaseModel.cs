using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Trados.Transcreate.Model
{
	public class BaseModel: INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			try
			{
				if (propertyName != null)
				{
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
				}
			}
			catch
			{
				// ignore; catch all;
			}
		}
	}
}
