using System.Collections.Generic;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class SubdomainsResponseModel
	{
		public string Code { get; set; }
		[JsonProperty("eurovoc_code	")]
		public string EurovocCode { get; set; }
		public string Name { get; set; }
		public string Level { get; set; }
		public List<string> Lookups { get; set; }
		public List<SubdomainsResponseModel> Subdomains { get; set; }
	}
}