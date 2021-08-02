using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.IATETerminologyProvider.Model.ResponseModels
{
	public class ItemsResponseModel
	{
		public string Code { get; set; }

		[JsonProperty("eurovoc_code")]
		public string EurovocCode { get; set; }
		
		public string Name { get; set; }
		
		public string Level { get; set; }
		
		public List<SubdomainsResponseModel> Subdomains { get; set; }
		
		public List<TermsDomainResponseModel> Domains { get; set; }
		
		public string Id { get; set; }
		public bool? Deprecated { get; set; }

		[JsonProperty("cjeu_code")]
		public string CjeuCode { get; set; }
	}
}