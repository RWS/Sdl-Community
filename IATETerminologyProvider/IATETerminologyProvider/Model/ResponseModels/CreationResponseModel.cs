using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class CreationResponseModel
	{
		[JsonProperty("user_code")]
		public string UserCode { get; set; }

		[JsonProperty("division_code")]
		public string DivisionCode { get; set; }

		[JsonProperty("institution_code")]
		public string InstitutionCode { get; set; }

		public string TimeStamp { get; set; }
	}
}