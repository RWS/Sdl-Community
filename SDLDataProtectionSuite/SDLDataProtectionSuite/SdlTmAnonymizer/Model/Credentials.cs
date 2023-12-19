using System.Xml.Serialization;
using Newtonsoft.Json;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
	public class Credentials : ModelBase
	{
		public CredentialKind CredentialType { get; set; }

		public string Url { get; set; }

		public string UserName { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public string Password { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public bool CanAuthenticate { get; set; } = true;

		[JsonIgnore]
		[XmlIgnore]
		public bool IsAuthenticated { get; set; }
	}
}
