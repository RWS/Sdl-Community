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

	public enum QualityEstimation
	{
		None,
		VeryPoor,
		Poor,
		Adequate,
		Good,
		VeryGood
	}
}