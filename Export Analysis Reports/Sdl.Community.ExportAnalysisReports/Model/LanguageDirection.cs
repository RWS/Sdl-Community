using System.Globalization;

namespace Sdl.Community.ExportAnalysisReports.Model
{
	public class LanguageDirection
	{
		public CultureInfo TargetLang { get; set; }
		public string Guid { get; set; }
		public string PathToReport { get; set; }

		public override string ToString()
		{
			return TargetLang.EnglishName;
		}
	}
}