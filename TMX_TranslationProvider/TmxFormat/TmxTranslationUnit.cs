using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;

namespace TMX_TranslationProvider.TmxFormat
{
	public class TmxTranslationUnit
	{
		public DateTime? CreationDate;
		public string CreationAuthor;
		public DateTime? ChangeDate;
		public string ChangeAuthor;

		// Example: <prop type="x-Domain:SinglePicklist">Construction</prop>
		public string Domain = "";

        public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;

		public List<TmxFormattedTextPart> Source = new List<TmxFormattedTextPart>();
		public List<TmxFormattedTextPart> Target = new List<TmxFormattedTextPart>();

		public string SourceText() => string.Join("", Source.Where(part => part.Text != "").Select(part => part.Text));
		public string TargetText() => string.Join("", Target.Where(part => part.Text != "").Select(part => part.Text));
	}
}
