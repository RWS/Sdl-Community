using System.Collections.Generic;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class ItemsResponseModel
	{
		public string Code { get; set; }

		[JsonProperty("eurovoc_code	")]
		public string EurovocCode { get; set; }
		public string Name { get; set; }
		public string Level { get; set; }
		public List<SubdomainsResponseModel> Subdomains { get; set; }
		public List<TermsDomainResponseModel> Domains { get; set; }
		public string Id { get; set; }
	}
}