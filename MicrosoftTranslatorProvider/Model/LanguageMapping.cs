using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class LanguageMapping : BaseModel
	{
		private string _categoryId;

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
	}
}