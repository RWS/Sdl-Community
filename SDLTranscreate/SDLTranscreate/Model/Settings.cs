using Sdl.Core.Globalization;

namespace Trados.Transcreate.Model
{
	public class Settings
	{
		public Settings()
		{
			ConvertOptions = new ConvertOptions();

			ExportOptions = new ExportOptions();

			ImportOptions = new ImportOptions();		
			ImportOptions.StatusTranslationUpdatedId = ConfirmationLevel.Translated.ToString();

			BackTranslationOptions = new BackTranslationOptions();
		}

		public ConvertOptions ConvertOptions { get; set; }

		public ExportOptions ExportOptions { get; set; }

		public ImportOptions ImportOptions { get; set; }
		
		public BackTranslationOptions BackTranslationOptions { get; set; }
	}
}
