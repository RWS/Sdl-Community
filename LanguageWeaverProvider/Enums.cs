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
		CloudSecret,
		EdgeCredentials,
		EdgeApiKey
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