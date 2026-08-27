using System.Xml.Serialization;

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
		EdgeSSO = 6,

		/// <summary>
		/// Reuses the RWS ID session Trados Studio already holds, instead of running a second interactive sign-in.
		/// The token is owned and renewed by Studio, so the plug-in re-reads it rather than refreshing it itself.
		/// </summary>
		CloudStudio = 7
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

	public enum EditItemType
	{
		[XmlEnum(Name = "plain_text")]
		PlainText,
		[XmlEnum(Name = "regular_expression")]
		RegularExpression
	};

	public enum CloudResources
	{
		LanguagePairs,
		Dictionaries
	}
}