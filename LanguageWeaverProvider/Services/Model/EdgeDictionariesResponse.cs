using System.Collections.Generic;

namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeDictionariesResponse
	{
		public List<EdgeDictionary> Dictionaries { get; set; }

		public int Page { get; set; }

		public int PerPage { get; set; }

		public int TotalPages { get; set; }

		public int TotalItems { get; set; }
	}
}