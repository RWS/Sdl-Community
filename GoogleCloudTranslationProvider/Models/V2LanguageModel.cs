using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GoogleCloudTranslationProvider.Models
{
	public class V2LanguageModel
	{
		public string LanguageName { get; set; }
		public string LanguageCode { get; set; }
	}
}
