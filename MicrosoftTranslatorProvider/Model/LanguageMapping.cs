using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class LanguageMapping
	{
		public LanguagePair LanguagePair { get; set; }

		public string DisplayName { get; set; }

		public string CategoryID { get; set; }

		[JsonIgnore]
		public List<RegionSubscription> Regions { get; set; }

		[JsonIgnore]
		public RegionSubscription Region { get; set; }

		public string RegioKey { get; set; }
	}
}