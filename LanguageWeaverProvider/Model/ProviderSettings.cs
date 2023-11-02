namespace LanguageWeaverProvider.Model
{
	public class ProviderSettings
	{
		public bool IncludeTags { get; set; } = true;

		public bool ResendDrafts { get; set; } = true;

		public bool UseCustomName { get; set; }

		public string CustomName { get; set; }
	}
}