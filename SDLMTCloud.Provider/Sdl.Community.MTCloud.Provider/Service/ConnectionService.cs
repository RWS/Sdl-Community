using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using Auth0Service.ViewModel;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using IWin32Window = System.Windows.Forms.IWin32Window;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class ConnectionService : IConnectionService
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly IHttpClient _httpClient;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private string _currentWorkingPortalAddress;
		private ITranslationProviderCredentialStore _credentialStore;

		public void CheckConnection([CallerMemberName] string caller = null)
		{
			if (Credential.ValidTo >= DateTime.UtcNow)
				return;

			// attempt one connection
			var success = Connect(Credential);
			if (success.Item1)
				return;

			_logger.Error(caller + $"{PluginResources.Message_Connection_token_has_expired}\n {Credential.Token}");

			throw new Exception(PluginResources.Message_Connection_token_has_expired);
		}

		public ConnectionService(IWin32Window owner, VersionService versionService, IHttpClient httpClient)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_httpClient = httpClient;
			_httpClient.SetLogger(_logger);

			Owner = owner;
			VersionService = versionService;

			IsSignedIn = false;

			PluginVersion = VersionService?.GetPluginVersion();
			StudioVersion = VersionService?.GetStudioVersion();
		}

		private void ClearStudioCredentials()
		{
			Credential.Token = null;
			Credential.RefreshToken = null;
			Credential.ValidTo = default;
			SaveCredential();
		}

		public virtual ICredential Credential { get; private set; }

		public string CurrentWorkingPortalAddress
		{
			get
			{
				_currentWorkingPortalAddress = WorkingPortalsAddress.GetWorkingPortalAddress(Credential.AccountRegion);
				return _currentWorkingPortalAddress;
			}
			set { _currentWorkingPortalAddress = value; }
		}

		public bool IsSignedIn { get; private set; }
		public IWin32Window Owner { get; set; }
		public string PluginVersion { get; }
		public string StudioVersion { get; }
		public VersionService VersionService { get; }

		public void AddTraceHeaders(HttpRequestMessage request)
		{
			request.Headers.Add(Constants.TraceId,
				$"{Constants.LanguageWeaver} {PluginVersion} - {StudioVersion}.{Guid.NewGuid()}");
			request.Headers.Add("Trace-App", Constants.LanguageWeaver);
			request.Headers.Add("Trace-App-Version", PluginVersion);
			//request.Headers.Add("Trace-App-Meta-Info", "Optional data");
		}

		public (bool, string) Connect(ICredential credential = null, bool showDialog = false)
		{
			if (credential is not null)
			{
				SwitchAuthType(credential);
			}

			var message = "";
			if (Credential.Type == Authentication.AuthenticationType.Studio)
			{
				if (showDialog)
				{
					var (credentials, authMessage) = StudioSignIn(true);

					Credential.Token = credentials?.AccessToken;
					Credential.ValidTo = GetTokenValidTo();
					Credential.Name = credentials?.Email;
					Credential.Password = null;
					Credential.RefreshToken = credentials?.RefreshToken;
					message = authMessage;
				}

				IsSignedIn = !string.IsNullOrEmpty(Credential?.Token);
				if (IsSignedIn)
				{
					var userDetailsResult =
						Task.Run(async () => await GetUserDetails(Constants.MTCloudUriResourceUserDetails)).Result;
					IsSignedIn = userDetailsResult.Item1 != null;
					Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
					Credential.Name = userDetailsResult.Item1?.Email;
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
						Credential.ValidTo = GetTokenValidTo();
						message = signInResult.Item2;

						if (IsSignedIn)
						{
							var userDetailsResult = Task.Run(async () => await GetUserDetails(Constants.MTCloudUriResourceUserDetails)).Result;
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
						Credential.ValidTo = GetTokenValidTo();

						message = signInResult.Item2;

						if (IsSignedIn)
						{
							var userDetailsResult = Task.Run(async () => await GetUserDetails(Constants.MTCloudUriResourceClientDetails)).Result;
							IsSignedIn = userDetailsResult.Item1 != null;
							Credential.AccountId = userDetailsResult.Item1?.AccountId.ToString();
							message = userDetailsResult.Item2;
						}
					}
				}
			}

			return (IsSignedIn, message);
		}

		private void SwitchAuthType(ICredential credential)
		{
			Credential.Type = credential.Type;
			switch (credential.Type)
			{
				case Authentication.AuthenticationType.Studio:
					if (!string.IsNullOrEmpty(credential.Token) && !(string.IsNullOrEmpty(credential.RefreshToken)))
						Credential = credential;
					break;
				default:
					Credential = credential;
					break;
			}
		}

		public string CredentialToString()
		{
			return "Type=" + Credential.Type + "; Name=" + Credential.Name + "; Password=" + Credential.Password + "; Token=" + Credential.Token + "; AccountId=" + Credential.AccountId + "; ValidTo=" + Credential.ValidTo.ToBinary() + "; AccountRegion=" + Credential.AccountRegion + "; RefreshToken=" + Credential.RefreshToken;
		}

		public (bool, string) EnsureSignedIn(ITranslationProviderCredentialStore credentialStore, bool alwaysShowWindow = false)
		{
			Credential = SetCredential(credentialStore);

			if (Credential == null)
			{
				IsSignedIn = false;
				return (IsSignedIn, PluginResources.Message_Invalid_credentials);
			}

			CurrentWorkingPortalAddress = WorkingPortalsAddress.GetWorkingPortalAddress(Credential.AccountRegion);
			var result = Connect(Credential);
			if (result.Item1 && !alwaysShowWindow)
			{
				return result;
			}

			GetCredentialsWindow(out var credentialsWindow);
			var viewModel = (CredentialsViewModel)credentialsWindow.DataContext;

			Mouse.OverrideCursor = Cursors.Arrow;

			var message = string.Empty;
			credentialsWindow.UserPasswordBox.Password = viewModel.UserPassword;
			credentialsWindow.ClientSecretBox.Password = viewModel.ClientSecret;
			credentialsWindow.ClientIdBox.Password = viewModel.ClientId;

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

			return (IsSignedIn, message);
		}

		private void GetCredentialsWindow(out CredentialsWindow credentialsWindow)
		{
			credentialsWindow = GetCredentialsWindow(Owner);
			credentialsWindow.Closing -= CredentialsWindow_Closing;
			credentialsWindow.Closing += CredentialsWindow_Closing;

			Auth0ViewModel = credentialsWindow.AuthControl.Auth0Service;

			var viewModel = new CredentialsViewModel(credentialsWindow, this);
			credentialsWindow.DataContext = viewModel;
		}

		private void CredentialsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_cancellationTokenSource.Cancel();
		}

		private Auth0ControlViewModel Auth0ViewModel { get; set; }

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
			var refreshToken = string.Empty;
			var accountId = string.Empty;
			var validTo = DateTime.MinValue;
			var accountRegion = WorkingPortal.UEPortal;

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
				
				if (string.Compare(itemName, "RefreshToken", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					refreshToken = itemValue;
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
				if (string.Compare(itemName, "AccountRegion", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var success = Enum.TryParse(itemValue, out WorkingPortal value);
					if (success)
					{
						accountRegion = value;
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
					RefreshToken = refreshToken,
					AccountId = accountId,
					ValidTo = validTo,
					AccountRegion = accountRegion
				};

				if (!string.IsNullOrEmpty(token) && credential.ValidTo == DateTime.MinValue)
				{
					credential.ValidTo = GetTokenValidTo(token);
				}

				return credential;
			}

			return null;
		}

		private ICredential SetCredential(ITranslationProviderCredentialStore credentialStore)
		{
			_credentialStore = credentialStore;
			var uri = new Uri($"{Constants.MTCloudUriScheme}://");

			ICredential credential = null;
			var credentials = credentialStore.GetCredential(uri);
			if (!string.IsNullOrEmpty(credentials?.Credential))
			{
				credential = GetCredential(credentials.Credential);
			}

			return credential ?? new Credential();
		}

		public virtual async Task<(UserDetails, string)> GetUserDetails(string resource)
		{
			if (string.IsNullOrEmpty(Credential.Token))
			{
				return (null, PluginResources.Message_The_token_cannot_be_null);
			}

			if (string.IsNullOrEmpty(resource))
			{
				return (null, PluginResources.Message_The_resource_path_cannot_be_null);
			}

			// there is a known issues with a timeout interfering with the credential authentication from the server side
			// to mitigate this issue, we make two attempts to signin.
			var userDetails = await GetUserDetailsAttempt(resource);
			if (!(userDetails.Item1?.AccountId <= 0)) return userDetails;

			_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + PluginResources.Message_Second_Attempt + $" {resource}");
			userDetails = await GetUserDetailsAttempt(resource);

			return userDetails;
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

		public void SaveCredential(bool persist = true)
		{
			var uri = new Uri($"{Constants.MTCloudUriScheme}://");

			var credentials = new TranslationProviderCredential(CredentialToString(), persist);
			_credentialStore.RemoveCredential(uri);
			_credentialStore.AddCredential(uri, credentials);
		}

		public virtual async Task<(AuthorizationResponse, string)> SignIn(string resource, string content)
		{
			// there is a known issues with a timeout interfering with the credential authentication from the server side
			// to mitigate this issue, we make two attempts to signin.
			var signIn = await SignInAttempt(resource, content);
			if (string.IsNullOrEmpty(signIn.Item1?.AccessToken))
			{
				_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + PluginResources.Message_Second_Attempt + $" {resource}");
				signIn = await SignInAttempt(resource, content);
			}

			return signIn;
		}

		public void SignOut()
		{
			Auth0ViewModel.Logout();
			ClearStudioCredentials();
		}

		public (LanguageCloudCredentials, string) StudioSignIn(bool showDialog = false)
		{
			var auth0Credential = Credential is {Token: { }} ?
				new Auth0Service.Model.Credential(Credential.Token, Credential.RefreshToken) : null;

			var (authenticationMessage, credentials) = Auth0ViewModel.TryLogin(_cancellationTokenSource.Token, showDialog, auth0Credential);

		   	if (authenticationMessage.IsSuccessful)
		   	{
		   		var model = new LanguageCloudCredentials
		   		{
		   			AccessToken = credentials.Token,
					RefreshToken = credentials.RefreshToken
		   		};

		   		return (model, authenticationMessage.Message);
		   	}

		   	return (default, authenticationMessage.Message);
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

		public HttpRequestMessage GetRequestMessage(HttpMethod httpMethod, Uri uri)
		{
			var request = new HttpRequestMessage(httpMethod, uri);
			request.Headers.Add("Authorization", $"Bearer {Credential.Token}");
			AddTraceHeaders(request);
			return request;
		}

		private DateTime GetTokenValidTo(string token = null)
		{
			var tokenModel = ReadToken(token ?? Credential.Token);
			return tokenModel?.ValidTo ?? DateTime.MinValue;
		}

		private async Task<(UserDetails, string)> GetUserDetailsAttempt(string resource)
		{
			var uri = new Uri($"{CurrentWorkingPortalAddress}/v4" + resource);
			var request = GetRequestMessage(HttpMethod.Get, uri);

			var responseMessage = await _httpClient.SendRequest(request);
			var userDetails = await _httpClient.GetResult<UserDetails>(responseMessage);

			return (userDetails, responseMessage.ReasonPhrase);
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

		private async Task<(AuthorizationResponse, string)> SignInAttempt(string resource, string content)
		{
			var uri = new Uri($"{CurrentWorkingPortalAddress}/v4" + resource);

			var request = GetRequestMessage(HttpMethod.Post, uri);
			request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

			var responseMessage = await _httpClient.SendRequest(request);
			var authorizationResponse = await _httpClient.GetResult<AuthorizationResponse>(responseMessage);

			return (authorizationResponse, responseMessage.ReasonPhrase);
		}
	}
}