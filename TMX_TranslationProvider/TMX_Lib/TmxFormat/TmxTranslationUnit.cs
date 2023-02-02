using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Utils;

namespace TMX_Lib.TmxFormat
{
	public class TmxText
	{
		public string Language;
		public string LocaseText;
		public string FormattedText;
	}

	public class TmxTranslationUnit
	{
		public DateTime? CreationTime;
		public string CreationAuthor;
		public DateTime? ChangeTime;
		public string ChangeAuthor;

		public DateTime? TranslateTime => ChangeTime ?? CreationTime;

		public string XmlProperties = "";
		public string TuAttributes = "";

		// Example: <prop type="x-Domain:SinglePicklist">Construction</prop>
		public string Domain = "";

        public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;

        public List<TmxText> Texts = new List<TmxText>();
	}
}
