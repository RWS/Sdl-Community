using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Model
{
	public class TemplateTmDetails
	{
		public int Penalty { get; set; }
		public bool IsCreatedFromPlugin { get; set; }
		//For tms which are already on the template and we need to check if the languages pair is compatible with any of the project languages
		public Language SourceLanguage { get; set; }
		public Language TargetLanguage { get; set; }
		public string LocalPath { get; set; }
		public string Name { get; set; }
		public LanguagePair TransitLanguagePairOptions { get; set; }
	}
}
