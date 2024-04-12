using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class ConnectionProvider : INotifyPropertyChanged, IDisposable
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly System.Timers.Timer _timer;
		
		private DateTime? _expireDate;

		private bool _accessTokenExpired;
		private bool _refreshTokenExpired;

		private string _accessToken;
		private string _refreshToken;

		private string _userName;
		private string _password;

		private HttpClient _httpClient;

		public ConnectionProvider()
		{
			ResetProperties();

			_timer = new System.Timers.Timer
			{
				// every 10 seconds
				Interval = 10000
			};

			_timer.Elapsed += Timer_Elapsed;
			_timer.Enabled = true;
			_timer.AutoReset = true;
			_timer.Start();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public HttpClient HttpClient
		{
			get => _httpClient;
			private set
			{
				if (_httpClient == value)
				{
					return;
				}

				_httpClient = value;
				OnPropertyChanged(nameof(HttpClient));
			}
		}
		
		public bool AccessTokenExpired
		{
			get => _accessTokenExpired;
			private set
			{
				if (_accessTokenExpired == value)
				{
					return;
				}

				_accessTokenExpired = value;
				OnPropertyChanged(nameof(AccessTokenExpired));
			}
		}

		public bool RefreshTokenExpired
		{
			get => _refreshTokenExpired;
			private set
			{
				if (_refreshTokenExpired == value)
				{
					return;
				}

				_refreshTokenExpired = value;
				OnPropertyChanged(nameof(RefreshTokenExpired));
			}
		}

		public string AccessToken
		{
			get => _accessToken;
			private set
			{
				if (_accessToken == value)
				{
					return;
				}

				_accessToken = value;
				OnPropertyChanged(nameof(AccessToken));
			}
		}

		public string RefreshToken
		{
			get => _refreshToken;
			private set
			{
				if (_refreshToken == value)
				{
					return;
				}

				_refreshToken = value;
				OnPropertyChanged(nameof(RefreshToken));
			}
		}

		public DateTime? ExpireDate
		{
			get => _expireDate;
			private set
			{
				if (_expireDate == value)
				{
					return;
				}

				_expireDate = value;
				OnPropertyChanged(nameof(ExpireDate));
			}
		}

		public bool Login(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName)
			    || string.IsNullOrEmpty(password))
			{
				return false;
			}

			_userName = userName;
			_password = password;
			
			ResetProperties();

			var response = GetAccessTokenResponse(userName, password);
			if (response != null && response.Tokens.Count > 0)
			{
				_accessToken = response.Tokens[0];
				_refreshToken = response.RefreshToken;

				_accessTokenExpired = false;
				_refreshTokenExpired = false;

				_expireDate = ReadTokenExpiryDate(_accessToken);

				_httpClient = GetDefaultHttpClient();
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

				OnPropertyChanged(nameof(AccessToken));
				OnPropertyChanged(nameof(ExpireDate));
				OnPropertyChanged(nameof(HttpClient));

				return true;
			}

			throw new Exception("Failed login!");
		}

		public bool ReLogin()
		{
			return Login(_userName, _password);
		}

		public bool ExtendLogin()
		{
			if (_refreshTokenExpired ||
			    string.IsNullOrEmpty(_refreshToken) ||
			    _expireDate == DateTime.MinValue || 
			    _expireDate == DateTime.MaxValue)
			{
				return false;
			}

			var response = GetExtendAccessTokenResponse();
			if (response != null && response.Tokens.Count > 0)
			{
				_accessToken = response.Tokens[0];
				_refreshToken = response.RefreshToken;

				_accessTokenExpired = true;
				_refreshTokenExpired = false;

				_expireDate = ReadTokenExpiryDate(_accessToken);
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

				OnPropertyChanged(nameof(AccessToken));
				OnPropertyChanged(nameof(ExpireDate));
				OnPropertyChanged(nameof(HttpClient));

				return true;
			}

			return true;
		}

		public bool EnsureConnection()
		{
			if (HttpClient == null)
			{
				return ReLogin();
			}
			
			if (AccessTokenExpired)
			{
				var success = RefreshTokenExpired
					? ReLogin()
					: ExtendLogin();

				return success;
			}

			return true;
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
			
			if (_timer is null)
			{
				return;
			}
			
			_timer.Stop();
			_timer.Elapsed -= Timer_Elapsed;
			_timer.Close();
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private JsonAccessTokenModel GetResponse(HttpClient httpClient)
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get
			};

			try
			{
				var httpResponse = httpClient.SendAsync(httpRequest);
				var httpResponseAsString = httpResponse?.Result?.Content?.ReadAsStringAsync().Result;
				var accessTokenRespose = JsonConvert.DeserializeObject<JsonAccessTokenModel>(httpResponseAsString);

				return accessTokenRespose;
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
			}

			return null;
		}

		private JsonAccessTokenModel GetAccessTokenResponse(string userName, string password)
		{
			var httpClient = GetDefaultHttpClient();
			httpClient.BaseAddress = new Uri(ApiUrls.GetAccessTokenUri(userName, password));

			return GetResponse(httpClient);
		}

		private JsonAccessTokenModel GetExtendAccessTokenResponse()
		{
			var httpClient = GetDefaultHttpClient();
			httpClient.BaseAddress = new Uri(ApiUrls.GetExtendAccessTokenUri(RefreshToken));
				
			return GetResponse(httpClient);
		}

		private DateTime? ReadTokenExpiryDate(string token)
		{
			try
			{
				return JwtHelper.GetTokenExpiryDate(token);
			}
			catch (Exception e)
			{
				
			}

			return null;
		}

		private HttpClient GetDefaultHttpClient()
		{
			var httpClient = new HttpClient();

			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Connection.Add("Keep-Alive");
			httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
			httpClient.DefaultRequestHeaders.Add("Origin", "https://iate.europa.eu");
			httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
			httpClient.DefaultRequestHeaders.Add("Host", "iate.europa.eu");
			httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			httpClient.Timeout = TimeSpan.FromMinutes(2);

			return httpClient;
		}

		private void ResetProperties()
		{
			_httpClient = null;
			
			_accessTokenExpired = false;
			_refreshTokenExpired = false;

			_accessToken = string.Empty;
			_refreshToken = string.Empty;

			_expireDate = DateTime.MinValue;
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_expireDate == DateTime.MinValue || _expireDate == DateTime.MaxValue)
			{
				return;
			}

			// if the expire date will expire within the next minute
			if (_expireDate < DateTime.UtcNow.AddMinutes(1))
			{
				if (!_accessTokenExpired)
				{
					// refresh the token before it expires using the RefreshToken
					AccessTokenExpired = true;
					EnsureConnection();
				}
				else if (!_refreshTokenExpired)
				{
					// relogin is required if the refresh token expires
					RefreshTokenExpired = true;
					EnsureConnection();
				}
			}
		}
	}
}