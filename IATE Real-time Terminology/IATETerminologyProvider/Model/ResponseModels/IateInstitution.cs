using Newtonsoft.Json;

namespace Sdl.Community.IATETerminologyProvider.Model.ResponseModels
{
	public class IateInstitution
	{
		public string Code { get; set; }
		public string Name { get; set; }

		[JsonProperty("parent")]
		public string ParentCode { get; set; }
	}
}