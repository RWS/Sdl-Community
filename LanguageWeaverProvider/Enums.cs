namespace LanguageWeaverProvider
{
	public enum PluginVersion
	{
		None,
		LanguageWeaverCloud,
		LanguageWeaverEdge
	}

	public enum AuthenticationType
	{
		None,
		CloudCredentials,
		CloudAPI,
		CloudSSO,
		EdgeCredentials,
		EdgeApiKey,
		EdgeSSO
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