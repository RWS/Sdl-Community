namespace LanguageWeaverProvider.Model
{
	public class ProviderSettings
	{
		public bool UseCustomName { get; set; } = false;

		public string CustomName { get; set; } = null;

		public bool IncludeTags { get; set; } = true;

		public bool ResendDrafts { get; set; } = true;

		public bool AutosendFeedback { get; set; } = true;

		public bool UsePrelookup { get; set; } = false;

		public string PreLookupFilePath { get; set; } = null;

		public bool UsePostLookup { get; set; } = false;

		public string PostLookupFilePath { get; set; }
	}
}