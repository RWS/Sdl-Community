using System;
using System.ComponentModel.DataAnnotations;
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
using Sdl.LanguageCloud.IdentityApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class ConnectionService : IConnectionService
	{
		public ConnectionService(IWin32Window owner, VersionService versionService, LanguageCloudIdentityApi languageCloudIdentityApi)
		{
			Owner = owner;
			VersionService = versionService;

			IsSignedIn = false;

			PluginVersion = VersionService?.GetPluginVersion();
			StudioVersion = VersionService?.GetStudioVersion();

			LanguageCloudIdentityApi = languageCloudIdentityApi;

			Credential = new Credential();
		}

		public IWin32Window Owner { get; set; }

		public LanguageCloudIdentityApi LanguageCloudIdentityApi { get; }

		public VersionService VersionService { get; }

		public bool IsSignedIn { get; private set; }

		public virtual ICredential Credential { get; private set; }

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
			var user = string.Empty;
			var password = string.Empty;
			var token = string.Empty;
			var accountId = string.Empty;
			var validTo = DateTime.MinValue;

			var regex = new Regex(@";");
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

				if (string.Compare(itemName, "user", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					user = itemValue;
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

			if (string.IsNullOrEmpty(name) &&
			    !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
			{
				if (string.Compare(type, "CustomUser", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					var emailAddress = new EmailAddressAttribute();
					var isvalid = emailAddress.IsValid(user);
					type = isvalid 
						? Authentication.AuthenticationType.User.ToString() 
						: Authentication.AuthenticationType.Client.ToString();
				}
				else
				{
					type = Authentication.AuthenticationType.Studio.ToString();
				}

				name = user;
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

			string message;
			if (Credential.Type == Authentication.AuthenticationType.Studio)
			{
				//IsSignedIn = IsValidStudioCredential(out message);
				var signInResult = StudioSignIn();

				IsSignedIn = !string.IsNullOrEmpty(signInResult.Item1?.AccessToken);
				Credential.Token = signInResult.Item1?.AccessToken;
				Credential.ValidTo = GetTokenValidTo(Credential.Token);
				Credential.Name = signInResult.Item1?.Email;
				Credential.Password = null;
				message = signInResult.Item2;

				if (IsSignedIn)
				{
					var userDetailsResult =
						Task.Run(async () => await GetUserDetails(Credential.Token, Constants.MTCloudUriResourceUserDetails)).Result;
					IsSignedIn = userDetailsResult.Item1 != null;
					Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
					message = userDetailsResult.Item2;
				}
			}
			else
			{
				IsSignedIn = IsValidCredential(out message);
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

						var signInResult = Task.Run(async () => await SignIn(Constants.MTCloudUriResourceUserToken, content)).Result;
						IsSignedIn = !string.IsNullOrEmpty(signInResult.Item1?.AccessToken);
						Credential.Token = signInResult.Item1?.AccessToken;
						Credential.ValidTo = GetTokenValidTo(Credential.Token);
						message = signInResult.Item2;

						if (IsSignedIn)
						{
							var userDetailsResult = Task.Run(async () => await GetUserDetails(Credential.Token, Constants.MTCloudUriResourceUserDetails)).Result;
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

						var signInResult = Task.Run(async () => await SignIn(Constants.MTCloudUriResourceClientToken, content)).Result;
						IsSignedIn = !string.IsNullOrEmpty(signInResult.Item1?.AccessToken);
						Credential.Token = signInResult.Item1?.AccessToken;
						Credential.ValidTo = GetTokenValidTo(Credential.Token);

						message = signInResult.Item2;

						if (IsSignedIn)
						{
							var userDetailsResult = Task.Run(async () => await GetUserDetails(Credential.Token, Constants.MTCloudUriResourceClientDetails)).Result;
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
				return new Tuple<bool, string>(IsSignedIn, PluginResources.Message_Invalid_credentials);
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
				IsSignedIn = viewModel.IsSignedIn;
			}
			else
			{
				IsSignedIn = false;
				Credential.Token = string.Empty;
				Credential.AccountId = string.Empty;
			}

			return new Tuple<bool, string>(IsSignedIn, message);
		}

		public virtual Tuple<LanguageCloudIdentityApiModel, string> StudioSignIn()
		{
			if (LanguageCloudIdentityApi == null)
			{
				return new Tuple<LanguageCloudIdentityApiModel, string>(null, string.Empty);
			}

			var success = LanguageCloudIdentityApi.TryLogin(out var message);			
			if (success)
			{
				var model = new LanguageCloudIdentityApiModel
				{
					AccessToken = LanguageCloudIdentityApi.AccessToken,
					ActiveAccountId = LanguageCloudIdentityApi.ActiveAccountId,
					ActiveUserId = LanguageCloudIdentityApi.ActiveUserId,
					ActiveTenantId = LanguageCloudIdentityApi.ActiveTenantId,
					ApiKey = LanguageCloudIdentityApi.ApiKey,
					StudioApplicationKey = LanguageCloudIdentityApi.StudioApplicationKey,
					StudioClientId = LanguageCloudIdentityApi.StudioClientId,
					AccountName = LanguageCloudIdentityApi.LanguageCloudCredential?.AccountName,
					Email = LanguageCloudIdentityApi.LanguageCloudCredential?.Email,
				};

				return new Tuple<LanguageCloudIdentityApiModel, string>(model, message);
			}

			return new Tuple<LanguageCloudIdentityApiModel, string>(null, message);
		}

		public virtual async Task<Tuple<AuthorizationResponse, string>> SignIn(string resource, string content)
		{
			// there is a known issues with a timeout interfering with the credential authentication from the server side
			// to mitigate this issue, we make two attempts to signin.
			var signIn = await SignInAttempt(resource, content);
			if (string.IsNullOrEmpty(signIn.Item1?.AccessToken))
			{
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + PluginResources.Message_Second_Attempt + $" {resource}");
				signIn = await SignInAttempt(resource, content);
			}

			return signIn;
		}

		public virtual async Task<Tuple<UserDetails, string>> GetUserDetails(string token, string resource)
		{
			if (string.IsNullOrEmpty(token))
			{
				return new Tuple<UserDetails, string>(null, PluginResources.Message_The_token_cannot_be_null);
			}

			if (string.IsNullOrEmpty(resource))
			{
				return new Tuple<UserDetails, string>(null, PluginResources.Message_The_resource_path_cannot_be_null);
			}

			// there is a known issues with a timeout interfering with the credential authentication from the server side
			// to mitigate this issue, we make two attempts to signin.
			var userDetails = await GetUserDetailsAttempt(token, resource);
			if (userDetails.Item1?.AccountId <= 0)
			{
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + PluginResources.Message_Second_Attempt + $" {resource}");
				userDetails = await GetUserDetailsAttempt(token, resource);
			}

			return userDetails;
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

		public virtual bool IsValidStudioCredential(out string message)
		{
			if (LanguageCloudIdentityApi == null)
			{
				message = PluginResources.Message_The_LanguageCloudIdentityApi_is_not_implemented;
				return false;
			}

			var languageCloudCredential = LanguageCloudIdentityApi?.LanguageCloudCredential;

			message = string.Empty;

			var isNullOrEmpty = string.IsNullOrEmpty(languageCloudCredential?.Email)
				   || string.IsNullOrEmpty(LanguageCloudIdentityApi?.AccessToken);

			if (isNullOrEmpty)
			{
				message = PluginResources.Message_Credential_criteria_is_not_valid;
				return false;
			}


			var validTo = GetTokenValidTo(LanguageCloudIdentityApi?.AccessToken);
			if (validTo < DateTime.UtcNow)
			{
				message = PluginResources.Message_Credential_has_expired;
				return false;
			}

			return true;
		}

		public bool IsValidCredential(out string message)
		{
			message = string.Empty;

			var isNullOrEmpty = string.IsNullOrEmpty(Credential.AccountId)
								|| string.IsNullOrEmpty(Credential.Token)
								|| string.IsNullOrEmpty(Credential.Name)
								|| string.IsNullOrEmpty(Credential.Password);

			if (isNullOrEmpty)
			{
				message = PluginResources.Message_Credential_criteria_is_not_valid;
				return false;
			}

			// if the ValidTo value is available, then validate against the current date/time
			if (Credential.ValidTo < DateTime.UtcNow)
			{
				message = PluginResources.Message_Credential_has_expired;
				return false;
			}

			return true;
		}

		private JwtSecurityToken ReadToken(string token)
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

		private DateTime GetTokenValidTo(string token)
		{
			var tokenModel = ReadToken(token);

			return tokenModel?.ValidTo ?? DateTime.MinValue;
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
