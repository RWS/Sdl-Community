using System.Collections.Generic;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class RequestResponseModel
	{
		public string Source { get; set; }
		public List<string> Targets { get; set; }

		[JsonProperty("query_operator")]
		public int QueryOperator { get; set; }

		public string Query { get; set; }

		[JsonProperty("search_in_fields")]
		public List<int> SearchInFields { get; set; }

		[JsonProperty("search_in_term_types")]
		public List<int> SearchInTermTypes { get; set; }

		// the below property is used when doing the request to server
		[JsonProperty("include_subdomains")]
		public bool IncludeSubdomains { get; set; }

		// the below property is used when doing the request to server
		[JsonProperty("filter_by_entry_collection")]
		public List<int> FilterByEntryCollection { get; set; }

		// the below property is used when doing the request to server
		[JsonProperty("filter_by_domains")]
		public List<int> FilterByDomains { get; set; }
	}
}