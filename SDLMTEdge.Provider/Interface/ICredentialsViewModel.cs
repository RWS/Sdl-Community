using System.Windows.Documents;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.Interface
{
	public interface ICredentialsViewModel
	{
		BaseModel ViewModel { get; set; }

		string Port { get; set; }

		string Host { get; set; }
		
		string ApiKey { get; set; }
		
		string UserName { get; set; }
		
		string Password { get; set; }
		
		bool PersistsHost { get; set; }
		
		bool RequiresSecureProtocol { get; set; }
		
		bool PersistsCredentials { get; set; }

		bool PersistsApiKey { get; set; }

		bool UseBasicCredentials { get; set; }

		bool UseApiKey { get; set; }

		bool UseAuth0SSO { get; set; }

		bool CredentialsAreValid(bool isSettingsView = false);

		bool UriIsValid();
	}
}