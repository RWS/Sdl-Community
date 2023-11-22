using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChromiumBrowserSupport.ViewModel
{
	public class BaseViewModel: INotifyPropertyChanged
	{
		private string _name;

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
				{
					return;
				}

				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
