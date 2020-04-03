using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class ConnectionService : ICredentialService
	{
		public ConnectionService(IWin32Window owner)
		{
			Owner = owner;

			IsSignedIn = false;
			PluginVersion = VersionHelper.GetPluginVersion();
			StudioVersion = VersionHelper.GetStudioVersion();
			Credential = new Credential();
		}

		public IWin32Window Owner { get; set; }

		public bool IsSignedIn { get; private set; }

		public ICredential Credential { get; private set; }

		public string PluginVersion { get; }

		public string StudioVersion { get; }

		public string CredentialToString()
		{
			return "Type=" + Credential.Type + "; Name=" + Credential.Name + "; Password=" + Credential.Password + "; Token=" + Credential.Token + "; AccountId=" + Credential.AccountId + "; ValidTo=" + Credential.ValidTo.ToBinary();
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
			var validTo = DateTime.MinValue;

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

				if (string.Compare(itemName, "ValidTo", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var success = long.TryParse(itemValue, out var value);
					if (success)
					{
						validTo = DateTime.FromBinary(value);
					}
				}
			}

			if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(name))
			{
				var credential = new Credential
				{
					Type = (Authentication.AuthenticationType)Enum.Parse(typeof(Authentication.AuthenticationType), type, true),
					Name = name,
					Password = password,
					Token = token,
					AccountId = accountId,
					ValidTo = validTo
				};

				if (!string.IsNullOrEmpty(token) && credential.ValidTo == DateTime.MinValue)
				{
					credential.ValidTo = GetTokenValidTo(token);
				}

				return credential;
			}

			return null;
		}

		public Tuple<bool, string> Connect(ICredential credential)
		{
			Credential = credential;

			var message = string.Empty;
			if (Credential.Type == Authentication.AuthenticationType.Studio)
			{
				IsSignedIn = IsSignedInStudioAuthentication(out var user);
				if (!IsSignedIn)
				{
					var currentCursor = Mouse.OverrideCursor;
					try
					{
						Mouse.OverrideCursor = Cursors.Arrow;
						IsSignedIn = StudioInstance.GetLanguageCloudIdentityApi.TryLogin(out message);
					}
					finally
					{
						Mouse.OverrideCursor = currentCursor;
					}
				}

				Credential.Token = StudioInstance.GetLanguageCloudIdentityApi.AccessToken;				
				Credential.ValidTo = GetTokenValidTo(Credential.Token);
				Credential.Name = StudioInstance.GetLanguageCloudIdentityApi.LanguageCloudCredential?.Email;
				Credential.Password = null;

				if (IsSignedIn)
				{
					var resource = "/accounts/users/self";
					var userDetailsResult = Task.Run(async () => await GetUserDetails(Credential.Token, resource)).Result;
					IsSignedIn = userDetailsResult.Item1 != null;
					Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
					message = userDetailsResult.Item2;
				}
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

						var signInResult = Task.Run(async () => await SignIn(resource, content)).Result;
						IsSignedIn = signInResult.Item1 != null;
						Credential.Token = signInResult.Item1?.AccessToken;
						Credential.ValidTo = GetTokenValidTo(Credential.Token);
						message = signInResult.Item2;

						if (IsSignedIn)
						{
							resource = "/accounts/users/self";
							var userDetailsResult = Task.Run(async () => await GetUserDetails(Credential.Token, resource)).Result;
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

						var signInResult = Task.Run(async () => await SignIn(resource, content)).Result;
						IsSignedIn = signInResult.Item1 != null;
						Credential.Token = signInResult.Item1?.AccessToken;
						Credential.ValidTo = GetTokenValidTo(Credential.Token);
						
						message = signInResult.Item2;

						if (IsSignedIn)
						{
							resource = "/accounts/api-credentials/self";
							var userDetailsResult = Task.Run(async () => await GetUserDetails(Credential.Token, resource)).Result;
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

			var result = Connect(Credential);
			if (result.Item1 && !alwaysShowWindow)
			{
				return result;
			}

			Mouse.OverrideCursor = Cursors.Arrow;
			var credentialsWindow = GetCredentialsWindow(Owner);
			var viewModel = new CredentialsViewModel(credentialsWindow, this);
			credentialsWindow.DataContext = viewModel;
			var message = string.Empty;
			var result1 = credentialsWindow.ShowDialog();
			if (result1.HasValue && result1.Value)
			{
				message = viewModel.ExceptionMessage;
				IsSignedIn = viewModel.StudioSignedIn;
			}
			else
			{
				IsSignedIn = false;
				Credential.Token = string.Empty;
				Credential.AccountId = string.Empty;
			}

			return new Tuple<bool, string>(IsSignedIn, message);
		}

		public async Task<Tuple<AuthorizationResponse, string>> SignIn(string resource, string content)
		{
			// there is a known issues with a timeout interfering with the credential authentication from the server side.W
			// make two attempts to signin to mitiate this issue.
			var signIn = await SignInAttempt(resource, content);
			if (string.IsNullOrEmpty(signIn.Item1?.AccessToken))
			{
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + PluginResources.Message_Second_Attempt + $" {resource}");
				signIn = await SignInAttempt(resource, content);
			}

			return signIn;
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

		private static JwtSecurityToken ReadToken(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}

			try
			{
				var jwtHandler = new JwtSecurityTokenHandler();

				//Check if readable token (string is in a JWT format)
				var readableToken = jwtHandler.CanReadToken(token);
				if (!readableToken)
				{
					return null;
				}

				return jwtHandler.ReadJwtToken(token);
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private static DateTime GetTokenValidTo(string token)
		{
			var tokenModel = ReadToken(token);

			return tokenModel?.ValidTo ?? DateTime.MinValue;
		}	

		public bool IsSignedInCredentialsAuthentication(Authentication.AuthenticationType type, out string name)
		{
			if (type == Authentication.AuthenticationType.Studio || Credential.Type == Authentication.AuthenticationType.Studio)
			{
				return IsSignedInStudioAuthentication(out name);
			}

			name = Credential.Name;
			return IsSignedInCredentialsAuthentication();
		}

		private bool IsSignedInStudioAuthentication(out string name)
		{
			var languageCloudCredential = StudioInstance.GetLanguageCloudIdentityApi.LanguageCloudCredential;
			name = languageCloudCredential?.Email;

			var validTo = GetTokenValidTo(StudioInstance.GetLanguageCloudIdentityApi?.AccessToken);
			if (validTo < DateTime.UtcNow)
			{
				return false;
			}

			return !string.IsNullOrEmpty(languageCloudCredential?.Email)
				   && !string.IsNullOrEmpty(StudioInstance.GetLanguageCloudIdentityApi?.AccessToken);
		}
	

		private bool IsSignedInCredentialsAuthentication()
		{
			// if the ValidTo value is available, then validate against the current date/time
			if (Credential.ValidTo < DateTime.UtcNow)
			{
				return false;
			}

			var isNullOrEmpty = string.IsNullOrEmpty(Credential.AccountId)
								|| string.IsNullOrEmpty(Credential.Token)
								|| string.IsNullOrEmpty(Credential.Name)
								|| string.IsNullOrEmpty(Credential.Password);
		
			return !isNullOrEmpty;
		}

		private async Task<Tuple<AuthorizationResponse, string>> SignInAttempt(string resource, string content)
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
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{ex.Message}\n {ex.StackTrace}");
				return new Tuple<AuthorizationResponse, string>(null, ex.Message);
			}
		}

		public async Task<Tuple<UserDetails, string>> GetUserDetails(string token, string resource)
		{
			// there is a known issues with a timeout interfering with the credential authentication from the server side.W
			// make two attempts to signin to mitiate this issue.
			var userDetails = await GetUserDetailsAttempt(token, resource);
			if (userDetails.Item1?.AccountId <= 0)
			{
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + PluginResources.Message_Second_Attempt + $" {resource}");
				userDetails = await GetUserDetailsAttempt(token, resource);
			}

			return userDetails;
		}

		private async Task<Tuple<UserDetails, string>> GetUserDetailsAttempt(string token, string resource)
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
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{ex.Message}\n {ex.StackTrace}");
				return new Tuple<UserDetails, string>(null, ex.Message);
			}
		}

		private CredentialsWindow GetCredentialsWindow(IWin32Window owner)
		{
			var credentialsWindow = new CredentialsWindow();
			var helper = new WindowInteropHelper(credentialsWindow);
			if (owner != null)
			{
				helper.Owner = owner.Handle;
			}

			return credentialsWindow;
		}
	}
}
