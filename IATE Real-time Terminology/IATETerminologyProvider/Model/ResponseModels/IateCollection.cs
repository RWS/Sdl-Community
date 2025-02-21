using Newtonsoft.Json;

namespace Sdl.Community.IATETerminologyProvider.Model.ResponseModels
{
	public class IateCollection
	{
		[JsonProperty("name")]
		public IateName Name { get; set; }
		
		[JsonProperty("code")]
		public string Code { get; set; }
	}
}