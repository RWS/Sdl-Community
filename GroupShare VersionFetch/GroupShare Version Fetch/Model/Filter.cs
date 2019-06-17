using Newtonsoft.Json;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class Filter
	{
		[JsonProperty(PropertyName = "orgPath")]
		public string OrgPath { get; set; }
		[JsonProperty(PropertyName = "projectName")]
		public string ProjectName { get; set; }

		[JsonProperty(PropertyName = "includeSubOrgs")]
		public bool IncludeSubOrgs { get; set; }

		[JsonProperty(PropertyName = "status")]
		public int Status { get; set; }
	}
}
