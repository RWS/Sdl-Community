namespace LanguageWeaverProvider.Model
{
	public class ProviderSettings
	{
		public bool UseCustomName { get; set; }

		public string CustomName { get; set; }

		public bool IncludeTags { get; set; } = true;

		public bool ResendDrafts { get; set; } = true;

		public bool AutosendFeedback { get; set; } = true;
	}
}