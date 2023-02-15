using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class LinguisticOptions
	{
		[JsonProperty("linguisticOptions")]
		public List<LinguisticOption> AvailableLinguisticOptions { get; set; }
	}
}