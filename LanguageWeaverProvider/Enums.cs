namespace LanguageWeaverProvider
{
	public enum PluginVersion
	{
		None = 0,
		LanguageWeaverCloud = 1,
		LanguageWeaverEdge = 2
	}

	public enum AuthenticationType
	{
		None = 0,
		CloudCredentials = 1,
		CloudAPI = 2,
		CloudSSO = 3,
		EdgeCredentials = 4,
		EdgeApiKey = 5,
		EdgeSSO = 6
	}

	public enum Direction
	{
		Inverted
	}

	public enum QualityEstimations
	{
		None,
		Poor,
		Adequate,
		Good
	}
}