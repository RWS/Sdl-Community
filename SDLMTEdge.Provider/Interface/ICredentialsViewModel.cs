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
		
		bool UseRwsCredentials { get; set; }

		bool PersistsCredentials { get; set; }
	}
}