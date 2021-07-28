namespace Trados.TargetRenamer.Interfaces
{
	public interface ITargetRenamerSettings
    {
        bool AppendAsPrefix { get; set; }
        bool AppendAsSuffix { get; set; }
        bool AppendCustomString { get; set; }
        bool AppendTargetLanguage { get; set; }
        string CustomLocation { get; set; }
        string CustomString { get; set; }
        string Delimiter { get; set; }
        string RegularExpressionReplaceWith { get; set; }
        string RegularExpressionSearchFor { get; set; }
        bool UseCustomLocation { get; set; }
        bool UseRegularExpression { get; set; }
        bool UseShortLocales { get; set; }
    }
}