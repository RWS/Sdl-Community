namespace Sdl.Community.SignoffVerifySettings.Model
{
	public class LanguageFileXmlNodeModel
	{
		// .sdlproj LanguageFile node properties
		public string LanguageFileGUID { get; set; }
		public string SettingsBundleGuid { get; set; }
		public string LanguageCode { get; set; }

		// QAVerification "Verify Files" report properties
		public string RunAt { get; set; }
		public string FileName { get; set; }
	}
}