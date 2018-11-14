using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class MetadataResponseModel
	{
		public CreationResponseModel Creation { get; set; }
		public ModificationResponseModel Modification { get; set; }

		[JsonProperty("institution_code")]
		public string InstitutionCode { get; set; }

		[JsonProperty("own_by_creator")]
		public bool OwnByCreator { get; set; }

		public int Confidentiality { get; set; }
		public int Primarity { get; set; }
	}
}