using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.ViewModel;
using System.Windows.Input;

namespace MicrosoftTranslatorProvider.Model
{
	public class UrlMetadata : BaseViewModel
	{
		public UrlMetadata()
		{
			ClearCommand = new RelayCommand(Clear);
		}

		public string Key { get; set; }

		public string Value { get; set; }

		public bool IsSelected { get; set; }

		public bool IsReadOnly { get; set; }

		public ICommand ClearCommand { get; private set; }

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case "key":
					Key = string.Empty;
					OnPropertyChanged(nameof(Key));
					break;

				case "value":
					Value = string.Empty;
					OnPropertyChanged(nameof(Value));
					break;

				default:
					break;
			}
		}
	}
}