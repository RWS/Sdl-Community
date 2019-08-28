namespace Sdl.Community.DtSearch4Studio.Provider.Helpers
{
	public static class Constants
	{
		public static readonly string AppUpperName = "DtSearch4Studio";
		public static readonly string AppLowerName = "dtSearch4Studio";
		public static readonly string NoIndexSelected = "No index was selected";
		public static readonly string ProviderScheme = "dtsearch";
		public static readonly string NoSettingsMessage = "Provider settings cannot be null!";
		public static readonly string JsonPath = @"SDL Community\DtSearch4Studio\DtSearch4Studio.json";
		public static readonly string EmptyProvider = "Provider couldn't be set up, because no index was selected!";
		public static readonly string CancelProviderMessage = "When you cancel the changes, please remove the provider, because the setup is not finished and the provider will not work as expected";

		// Logging messages
		public static readonly string Browse = "Browse method";
		public static readonly string WriteToFile = "WriteToFile method";
		public static readonly string CreateTranslationProvider = "CreateTranslationProvider method";
	}
}