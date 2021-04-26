namespace Trados.TargetRenamer.Interfaces
{
	public interface ITargetRenamerSettings
	{
		bool OverwriteTargetFiles { get; set; }
		bool AppendAsPrefix { get; set; }
		bool AppendAsSuffix { get; set; }
		bool UseCustomLocation { get; set; }
		string CustomLocation { get; set; }
		bool UseRegularExpression { get; set; }
		string RegularExpressionSearchFor { get; set; }
		string RegularExpressionReplaceWith { get; set; }
		string Delimitator { get; set; }
		bool UseShortLocales { get; set; }
		bool AppendTargetLanguage { get; set; }
		bool AppendCustomString { get; set; }
		string CustomString { get; set; }
	}
}
