using System.Collections.Generic;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class ItemsResponseModel
	{
		public MetaResponseModel Meta { get; set; }
		public string Code { get; set; }
		public int Id { get; set; }
		public decimal Score { get; set; }

		[JsonProperty("result_group")]
		public int ResultGroup { get; set; }

		public DomainsResponseModel Domains { get; set; }

		[JsonProperty("anchor_language")]
		public string AnchorLanguage { get; set; }
		
		public List<CrossRefsResponseModel> CrossRefs { get; set; }

		public MetadataResponseModel Metadata { get; set; }

		public LanguageResponseModel Language { get; set; }
	}
}