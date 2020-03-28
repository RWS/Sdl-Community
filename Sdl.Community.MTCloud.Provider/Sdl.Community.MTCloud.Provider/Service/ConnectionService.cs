using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Interop;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguageCloud.IdentityApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class ConnectionService
	{
		private readonly LanguageCloudIdentityApi _languageCloudIdentityApi;

		public ConnectionService(IWin32Window owner)
		{
			Owner = owner;
			_languageCloudIdentityApi = LanguageCloudIdentityApi.Instance;

			IsSignedIn = false;
			PluginVersion = VersionHelper.GetPluginVersion();
			StudioVersion = VersionHelper.GetStudioVersion();
			Credential = new Credential();
		}

		public IWin32Window Owner { get; internal set; }

		public bool IsSignedIn { get; private set; }

		public ICredential Credential { get; private set; }

		public string PluginVersion { get; }

		public string StudioVersion { get; }

		public string CredentialToString()
		{
			return "Type=" + Credential.Type + "; Name=" + Credential.Name + "; Password=" + Credential.Password + "; Token=" + Credential.Token + "; AccountId=" + Credential.AccountId + "; Created=" + Credential.Created.ToBinary();
		}

		public ICredential GetCredential(string credentialString)
		{
			if (string.IsNullOrEmpty(credentialString) || credentialString.IndexOf(';') <= 0)
			{
				return null;
			}

			var type = string.Empty;
			var name = string.Empty;
			var password = string.Empty;
			var token = string.Empty;
			var accountId = string.Empty;
			var created = DateTime.MinValue;

			var regex = new Regex(@";\s+");
			var items = regex.Split(credentialString);
			foreach (var item in items)
			{
				if (string.IsNullOrEmpty(item) || item.IndexOf('=') <= 0)
				{
					continue;
				}

				var itemName = item.Substring(0, item.IndexOf('=')).Trim();
				var itemValue = item.Substring(item.IndexOf('=') + 1).Trim();

				if (string.Compare(itemName, "Type", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					type = itemValue;
				}

				if (string.Compare(itemName, "Name", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					name = itemValue;
				}

				if (string.Compare(itemName, "Password", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					password = itemValue;
				}

				if (string.Compare(itemName, "Token", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					token = itemValue;
				}

				if (string.Compare(itemName, "AccountId", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					accountId = itemValue;
				}

				if (string.Compare(itemName, "Created", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var success = int.TryParse(itemValue, out var value);
					if (success)
					{
						created = DateTime.FromBinary(value);
					}
				}
			}

			if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(name))
			{
				return new Credential
				{
					Type = (Authentication.AuthenticationType)Enum.Parse(typeof(Authentication.AuthenticationType), type, true),
					Name = name,
					Password = password,
					Token = token,
					AccountId = accountId,
					Created = created == DateTime.MinValue ? DateTime.Now : created
				};
			}

			return null;
		}

		public async Task<Tuple<bool, string>> Connect(ICredential credential)
		{
			Credential = credential;

			var message = string.Empty;
			if (Credential.Type == Authentication.AuthenticationType.Studio)
			{
				var isSignedIn = IsSignedInStudioAuthentication(out var user);
				if (!IsSignedIn || Credential.Created == DateTime.MinValue)
				{
					Credential.Created = DateTime.Now;
				}

				IsSignedIn = isSignedIn || _languageCloudIdentityApi.TryLogin(out message);
				Credential.Token = _languageCloudIdentityApi.AccessToken;
				Credential.Name = _languageCloudIdentityApi.LanguageCloudCredential?.Email;
				Credential.Password = null;

				var resource = "/accounts/users/self";
				var userDetailsResult = await GetUserDetails(Credential.Token, resource);
				IsSignedIn = userDetailsResult.Item1 != null;
				Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
				message = userDetailsResult.Item2;
			}
			else
			{
				IsSignedIn = IsSignedInCredentialsAuthentication();
				if (!IsSignedIn)
				{
					if (Credential.Type == Authentication.AuthenticationType.User)
					{
						var credentials = new UserCredential
						{
							UserName = Credential.Name,
							Password = Credential.Password
						};

						var content = JsonConvert.SerializeObject(credentials);
						var resource = "/token/user";

						var signInResult = await SignIn(Credential.Name, Credential.Password, resource, content);
						IsSignedIn = signInResult.Item1 != null;
						Credential.Token = signInResult.Item1?.AccessToken;
						Credential.Created = DateTime.Now;
						message = signInResult.Item2;

						if (IsSignedIn)
						{
							resource = "/accounts/users/self";
							var userDetailsResult = await GetUserDetails(Credential.Token, resource);
							IsSignedIn = userDetailsResult.Item1 != null;
							Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
							message = userDetailsResult.Item2;
						}
					}
					else if (Credential.Type == Authentication.AuthenticationType.Client)
					{
						var credentials = new ClientCredential
						{
							ClientId = Credential.Name,
							ClientSecret = Credential.Password
						};

						var content = JsonConvert.SerializeObject(credentials);
						var resource = "/token";

						var signInResult = await SignIn(Credential.Name, Credential.Password, resource, content);
						IsSignedIn = signInResult.Item1 != null;
						Credential.Token = signInResult.Item1?.AccessToken;
						Credential.Created = DateTime.Now;
						message = signInResult.Item2;

						if (IsSignedIn)
						{
							resource = "/accounts/api-credentials/self";
							var userDetailsResult = await GetUserDetails(Credential.Token, resource);
							IsSignedIn = userDetailsResult.Item1 != null;
							Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
							message = userDetailsResult.Item2;
						}
					}
				}
			}

			return new Tuple<bool, string>(IsSignedIn, message);
		}

		public Tuple<bool, string> EnsureSignedIn(ICredential credential, bool alwaysShowWindow = false)
		{
			Credential = credential;

			if (Credential == null)
			{
				IsSignedIn = false;
				return new Tuple<bool, string>(IsSignedIn, "Credentials cannot be null!");
			}

			var result = Task.Run(async () => await Connect(Credential)).Result;
			if (result.Item1 && !alwaysShowWindow)
			{
				return result;
			}

			var credentialsWindow = GetCredentialsWindow(Owner);
			var viewModel = new CredentialsViewModel(credentialsWindow, this);
			credentialsWindow.DataContext = viewModel;

			var result1 = credentialsWindow.ShowDialog();
			if (result1.HasValue && result1.Value)
			{
				IsSignedIn = viewModel.StudioSignedIn;
			}
			else
			{
				IsSignedIn = false;
				Credential.Token = string.Empty;
				Credential.AccountId = string.Empty;
			}

			return new Tuple<bool, string>(IsSignedIn, viewModel.ExceptionMessage);
		}

		public async Task<Tuple<AuthorizationResponse, string>> SignIn(string name, string password, string resource, string content)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + resource);
					var request = new HttpRequestMessage(HttpMethod.Post, uri);
					request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

					AddTraceHeader(request);

					var responseMessage = await httpClient.SendAsync(request);
					var response = await responseMessage.Content.ReadAsStringAsync();

					if (responseMessage.IsSuccessStatusCode)
					{
						var authorizationResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(response);
						return new Tuple<AuthorizationResponse, string>(authorizationResponse, responseMessage.ReasonPhrase);
					}

					return new Tuple<AuthorizationResponse, string>(null, responseMessage.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				return new Tuple<AuthorizationResponse, string>(null, ex.Message);
			}
		}

		public async Task<Tuple<UserDetails, string>> GetUserDetails(string token, string resource)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

					var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + resource);
					var request = new HttpRequestMessage(HttpMethod.Get, uri);

					AddTraceHeader(request);

					var responseMessage = await httpClient.SendAsync(request);
					var response = await responseMessage.Content.ReadAsStringAsync();

					if (responseMessage.IsSuccessStatusCode)
					{
						var userDetails = JsonConvert.DeserializeObject<UserDetails>(response);
						return new Tuple<UserDetails, string>(userDetails, responseMessage.ReasonPhrase);
					}

					return new Tuple<UserDetails, string>(null, responseMessage.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				return new Tuple<UserDetails, string>(null, ex.Message);
			}
		}

		public CredentialsWindow GetCredentialsWindow(IWin32Window owner)
		{
			var credentialsWindow = new CredentialsWindow();
			var helper = new WindowInteropHelper(credentialsWindow);
			if (owner != null)
			{
				helper.Owner = owner.Handle;
			}

			return credentialsWindow;
		}

		public bool IsSignedInCredentialsAuthentication()
		{
			var isNullOrEmpty = string.IsNullOrEmpty(Credential.AccountId)
								|| string.IsNullOrEmpty(Credential.Token)
								|| string.IsNullOrEmpty(Credential.Name)
								|| string.IsNullOrEmpty(Credential.Password);

			var isValidDate = Credential.Created.AddHours(6) > DateTime.Now;

			return !isNullOrEmpty && isValidDate;
		}

		public bool IsSignedInStudioAuthentication(out string user)
		{
			var languageCloudCredential = _languageCloudIdentityApi.LanguageCloudCredential;

			user = languageCloudCredential?.Email;

			var success = !string.IsNullOrEmpty(languageCloudCredential?.Email)
				   && !string.IsNullOrEmpty(_languageCloudIdentityApi?.AccessToken);

			return success;
		}

		public void AddTraceHeader(HttpRequestMessage request)
		{
			request.Headers.Add(Constants.TraceId,
				$"{Constants.SDLMachineTranslationCloudProvider} {PluginVersion} - {StudioVersion}.{Guid.NewGuid().ToString()}");
		}

		public void SaveCredential(ITranslationProviderCredentialStore credentialStore, bool persist = true)
		{
			var uri = new Uri($"{Constants.MTCloudUriScheme}://");

			var credentials = new TranslationProviderCredential(CredentialToString(), persist);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
		}

		public ICredential GetCredential(ITranslationProviderCredentialStore credentialStore)
		{
			var uri = new Uri($"{Constants.MTCloudUriScheme}://");

			ICredential credential = null;
			var credentials = credentialStore.GetCredential(uri);
			if (!string.IsNullOrEmpty(credentials?.Credential))
			{
				credential = GetCredential(credentials.Credential);
			}

			return credential ?? new Credential();
		}
	}
}
