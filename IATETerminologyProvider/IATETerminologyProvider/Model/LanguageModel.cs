using System.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class LanguageModel : ILanguage
	{
		public string Name { get; set; }
		public CultureInfo Locale { get; set; }
	}
}