namespace Sdl.Community.Utilities.TMTool.Task
{
	public interface ISettings
	{
		/// <summary>
		/// performs settings validation
		/// </summary>
		/// <param name="errMsg">error message, empty if validation succeeded</param>
		/// <returns>false - if validation failed</returns>
		bool ValidateSettings(out string errMsg);
		/// <summary>
		/// resets settings to defaults
		/// </summary>
		void ResetToDefaults();
		/// <summary>
		/// target folder to save processed files in
		/// </summary>
		string TargetFolder { get; set; }
	}
}