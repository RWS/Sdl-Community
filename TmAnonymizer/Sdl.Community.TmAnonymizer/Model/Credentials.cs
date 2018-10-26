using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class Credentials : ModelBase
	{
		public Credentials()
		{
			CanAuthenticate = true;
		}

		public string Url { get; set; }

		public string UserName { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public string Password { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public bool CanAuthenticate { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public bool IsAuthenticated { get; set; }
	}
}
