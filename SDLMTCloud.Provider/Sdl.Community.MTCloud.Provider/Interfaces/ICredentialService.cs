using System;
using System.Net.Http;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ICredentialService
	{
		IWin32Window Owner { get; set; }

		bool IsSignedIn { get; }

		ICredential Credential { get; }

		string PluginVersion { get; }

		string StudioVersion { get; }

		string CredentialToString();

		ICredential GetCredential(string credentialString);

		Tuple<bool, string> Connect(ICredential credential);

		Tuple<bool, string> EnsureSignedIn(ICredential credential, bool alwaysShowWindow = false);
		
		bool IsSignedInCredentialsAuthentication(Authentication.AuthenticationType type, out string name);

		void AddTraceHeader(HttpRequestMessage request);

		void SaveCredential(ITranslationProviderCredentialStore credentialStore, bool persist = true);

		ICredential GetCredential(ITranslationProviderCredentialStore credentialStore);
	}
}
