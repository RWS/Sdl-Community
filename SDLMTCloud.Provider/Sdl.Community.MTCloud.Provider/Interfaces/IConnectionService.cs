using System;
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
		IWin32Window Owner { get; set; }

		LanguageCloudIdentityApi LanguageCloudIdentityApi { get; }

		bool IsSignedIn { get; }

		ICredential Credential { get; }

		string PluginVersion { get; }

		string StudioVersion { get; }

		string CredentialToString();

		ICredential GetCredential(string credentialString);

		(bool, string) Connect(ICredential credential);

		(bool, string) EnsureSignedIn(ICredential credential, bool alwaysShowWindow = false);

		bool IsValidStudioCredential(out string message);

		bool IsValidCredential(out string message);

		(LanguageCloudIdentityApiModel, string) StudioSignIn();

		Task<(AuthorizationResponse, string)> SignIn(string resource, string content);

		Task<(UserDetails, string)> GetUserDetails(string token, string resource);

		void AddTraceHeader(HttpRequestMessage request);

		void SaveCredential(ITranslationProviderCredentialStore credentialStore, bool persist = true);

		ICredential GetCredential(ITranslationProviderCredentialStore credentialStore);
	}
}
