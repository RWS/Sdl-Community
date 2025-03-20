using System;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.ViewModel;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class PairModel : BaseViewModel
    {
		//ICommand _clearCommand;

		string _sourceLanguageCode;
		string _targetLanguageCode;
		string _model;

		public string DisplayName { get; set; }

		public bool IsSupported { get; set; }

		public string SourceLanguageName { get; set; }

		public string TargetLanguageName { get; set; }

		public LanguagePair TradosLanguagePair { get; set; }

		public string SourceLanguageCode
		{
			get => _sourceLanguageCode;
			set
			{
				_sourceLanguageCode = value;
				OnPropertyChanged();
			}
		}

		public string TargetLanguageCode
		{
			get => _targetLanguageCode;
			set
			{
				_targetLanguageCode = value;
				OnPropertyChanged();
			}
		}

		public string Model
		{
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand => new RelayCommand(Clear);

		public PairModel Clone()
		{
			return MemberwiseClone() as PairModel;
		}

		private void Clear(object parameter)
		{
			if (parameter is not string target)
			{
				return;
			}

			switch (target)
			{
				case nameof(Model):
					Model = string.Empty;
					break;

			}
		}
	}
}