using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IConnectionService
	{
		IWin32Window Owner { get; set; }

		bool IsSignedIn { get; }

		ICredential Credential { get; }

		string PluginVersion { get; }

		string StudioVersion { get; }

		string CredentialToString();

		ICredential GetCredential(string credentialString);

		(bool, string) Connect(ICredential credential = null, bool showDialog = false);

		(bool, string) EnsureSignedIn(ITranslationProviderCredentialStore credentialStore, bool alwaysShowWindow = false);

		bool IsValidCredential(out string message);

		(LanguageCloudCredentials, string) StudioSignIn(bool showDialog);

		Task<(AuthorizationResponse, string)> SignIn(string resource, string content);

		Task<(UserDetails, string)> GetUserDetails(string resource);

		void AddTraceHeaders(HttpRequestMessage request);

		void SaveCredential(bool persist = true);

		//ICredential GetCredential(ITranslationProviderCredentialStore credentialStore);
		string CurrentWorkingPortalAddress { get; set; }
		void SignOut();
		void CheckConnection([CallerMemberName] string caller = null);
		HttpRequestMessage GetRequestMessage(HttpMethod httpMethod, Uri uri);
	}
}
