namespace Sdl.Community.SignoffVerifySettings.Model
{
	public class NumberVerifierSettingsModel
	{
		// FileFullPath used to compare with the target file location.
		// cannot use the target FileGuid because in NumberVerifier it is not exposed
		public string FileFullPath { get; set; }
		public string ExecutedDateTime { get; set; }
	}
}