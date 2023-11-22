using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.Models;


namespace Multilingual.XML.FileType.FileType.ViewModels
{
	public class WriterViewModel : BaseModel
	{

		private bool _monoLanguage;

		public WriterViewModel(WriterSettings settings)
		{
			Settings = settings;
			MonoLanguage = Settings.LanguageMappingMonoLanguage;
		}

		public WriterSettings Settings { get; set; }

		public bool MonoLanguage
		{
			get => _monoLanguage;
			set
			{
				if (_monoLanguage == value)
				{
					return;
				}

				_monoLanguage = value;
				OnPropertyChanged(nameof(MonoLanguage));

				Settings.LanguageMappingMonoLanguage = _monoLanguage;
			}
		}

		public WriterSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			MonoLanguage = Settings.LanguageMappingMonoLanguage;
			return Settings;
		}

	}
}
