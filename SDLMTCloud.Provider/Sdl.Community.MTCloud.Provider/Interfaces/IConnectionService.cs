using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.LanguageCloud.IdentityApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IConnectionService
	{
		ICredential Credential { get; }
		string CurrentWorkingPortalAddress { get; set; }
		bool IsSignedIn { get; }
		LanguageCloudIdentityApi LanguageCloudIdentityApi { get; }
		IWin32Window Owner { get; set; }
		string PluginVersion { get; }

		string StudioVersion { get; }

		void AddTraceHeaders(HttpRequestMessage request);

		(bool, string) Connect(ICredential credential);

		string CredentialToString();

		(bool, string) EnsureSignedIn(ICredential credential, bool alwaysShowWindow = false);

		ICredential GetCredential(string credentialString);

		ICredential GetCredential(ITranslationProviderCredentialStore credentialStore);

		Task<(UserDetails, string)> GetUserDetails(string resource);

		bool IsValidCredential(out string message);

		bool IsValidStudioCredential(out string message);

		void SaveCredential(ITranslationProviderCredentialStore credentialStore, bool persist = true);

		Task<(AuthorizationResponse, string)> SignIn(string resource, string content);

		(LanguageCloudIdentityApiModel, string) StudioSignIn();
	}
}