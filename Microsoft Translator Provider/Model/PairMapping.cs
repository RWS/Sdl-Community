using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.ViewModel;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class PairMapping : BaseViewModel
	{
		private string _categoryId;
		private ICommand _clearCommand;

		public LanguagePair LanguagePair { get; set; }

		public string DisplayName { get; set; }

		public string CategoryID
		{
			get => _categoryId;
			set
			{
				_categoryId = value;
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public PairMapping Clone()
		{
			return MemberwiseClone() as PairMapping;
		}

		private void Clear(object parameter)
		{
			CategoryID = string.Empty;
		}
	}
}