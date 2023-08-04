using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Model
{
	public class TranslationResponse
	{
		public string SourceLanguageId { get; set; }
		public string TargetLanguageId { get; set; }
		public string Model { get; set; }
		public List<string> Translation { get; set; }
	}
}