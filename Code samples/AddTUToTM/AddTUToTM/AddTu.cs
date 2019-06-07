using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace AddTUToTM
{
	[ApplicationInitializer]
	public class AddTu:IApplicationInitializer
	{
		public void Execute()
		{
			var tm = new FileBasedTranslationMemory(@"tm path");

			var tu = new TranslationUnit
			{
				SourceSegment = new Segment(tm.LanguageDirection.SourceLanguage),
				TargetSegment = new Segment(tm.LanguageDirection.TargetLanguage)
			};

			tu.SourceSegment.Add("source text");
			tu.TargetSegment.Add("target text");

			tm.LanguageDirection.AddTranslationUnit(tu, GetImportSettings());
			tm.Save();
		}

		private ImportSettings GetImportSettings()
		{
			var settings = new ImportSettings
			{
				CheckMatchingSublanguages = true,
				ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge
			};

			return settings;
		}
	}
}
