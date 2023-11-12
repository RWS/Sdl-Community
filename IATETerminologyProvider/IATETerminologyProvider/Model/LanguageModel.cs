using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class LanguageModel : ILanguage
	{
		public string Name { get; set; }
		public CultureCode Locale { get; set; }
	}
}