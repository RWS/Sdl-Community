using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class Credentials : ModelBase
	{
		public string Url { get; set; }

		public string UserName { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public string Password { get; set; }
		
	}
}
