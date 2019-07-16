using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public class AccessTokenService : INotifyPropertyChanged, IDisposable
	{
		private readonly System.Timers.Timer _timer;
		private DateTime _requestedAccessToken;
		private DateTime _extendedRefreshToken;

		private bool _accessTokenExpired;
		private bool _refreshTokenExpired;
		private bool _accessTokenExtended;

		private TimeSpan _accessTokenLifespan;
		private TimeSpan _refreshTokenLifespan;

		private string _accessToken;
		private string _refreshToken;
		private string _userName;
		private string _password;

		public AccessTokenService(TimeSpan accessTokenLifespan, TimeSpan refreshTokenLifespan)
		{
			Reset();

			_accessTokenLifespan = accessTokenLifespan.TotalSeconds > 0 ? accessTokenLifespan : new TimeSpan(0, 2, 45, 0);
			_refreshTokenLifespan = refreshTokenLifespan.TotalSeconds > 0 ? refreshTokenLifespan : new TimeSpan(0, 8, 45, 0);

			_timer = new System.Timers.Timer
			{
				Interval = 60000
			};

			_timer.Elapsed += Timer_Elapsed;
			_timer.Enabled = true;
			_timer.AutoReset = true;
			_timer.Start();
		}

		public AccessTokenService() : this(new TimeSpan(0), new TimeSpan(0))
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public bool AccessTokenExpired
		{
			get { return _accessTokenExpired; }
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
			get { return _refreshTokenExpired; }
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

		public bool AccessTokenExtended
		{
			get { return _accessTokenExtended; }
			private set
			{
				if (_accessTokenExtended == value)
				{
					return;
				}

				_accessTokenExtended = value;
				OnPropertyChanged(nameof(AccessTokenExtended));
			}
		}

		public string AccessToken
		{
			get { return _accessToken; }
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
			get { return _refreshToken; }
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

		public DateTime RequestedAccessToken
		{
			get { return _requestedAccessToken; }
			private set
			{
				if (_requestedAccessToken == value)
				{
					return;
				}

				_requestedAccessToken = value;
				OnPropertyChanged(nameof(RequestedAccessToken));
			}
		}

		public DateTime ExtendedRefreshToken
		{
			get { return _extendedRefreshToken; }
			private set
			{
				if (_extendedRefreshToken == value)
				{
					return;
				}

				_extendedRefreshToken = value;
				OnPropertyChanged(nameof(ExtendedRefreshToken));
			}
		}

		public TimeSpan AccessTokenLifespan
		{
			get { return _accessTokenLifespan; }
			private set
			{
				if (_accessTokenLifespan == value)
				{
					return;
				}

				_accessTokenLifespan = value;
				OnPropertyChanged(nameof(AccessTokenLifespan));
			}
		}

		public TimeSpan RefreshTokenLifespan
		{
			get { return _refreshTokenLifespan; }
			private set
			{
				if (_refreshTokenLifespan == value)
				{
					return;
				}

				_refreshTokenLifespan = value;
				OnPropertyChanged(nameof(RefreshTokenLifespan));
			}
		}

		public bool GetAccessToken(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName)
				|| string.IsNullOrEmpty(password))
			{
				return false;
			}

			Reset();

			_userName = userName;
			_password = password;

			var response = GetAccessTokenResponse(userName, password);
			if (response != null && response.Tokens.Count > 0)
			{
				_accessToken = response.Tokens[0];
				_refreshToken = response.RefreshToken;

				_accessTokenExpired = false;
				_accessTokenExtended = false;
				_refreshTokenExpired = false;

				_requestedAccessToken = DateTime.Now;
				_extendedRefreshToken = DateTime.MinValue;

				OnPropertyChanged(nameof(AccessToken));
				OnPropertyChanged(nameof(RequestedAccessToken));

				return true;
			}

			return false;
		}

		public bool ExtendAccessToken()
		{
			if (_accessTokenExtended ||
				string.IsNullOrEmpty(_accessToken) ||
				string.IsNullOrEmpty(RefreshToken) ||
				_requestedAccessToken == DateTime.MinValue)
			{
				return false;
			}

			var response = GetExtendAccessTokenResponse();
			if (response != null && response.Tokens.Count > 0)
			{
				_accessToken = response.Tokens[0];
				_refreshToken = response.RefreshToken;

				_accessTokenExpired = true;
				_accessTokenExtended = true;
				_refreshTokenExpired = false;

				_extendedRefreshToken = DateTime.Now;

				OnPropertyChanged(nameof(ExtendedRefreshToken));

				return true;
			}

			return true;
		}

		public void Dispose()
		{
			if (_timer != null)
			{
				_timer.Stop();
				_timer.Elapsed -= Timer_Elapsed;
				_timer.Close();
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private static JsonAccessTokenModel GetResponse(HttpClient httpClient)
		{
			Utils.AddDefaultParameters(httpClient);

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get
			};

			var httpResponse = httpClient.SendAsync(httpRequest);

			var httpResponseAsString = httpResponse.Result.Content.ReadAsStringAsync().Result;

			var accessTokenRespose = JsonConvert.DeserializeObject<JsonAccessTokenModel>(httpResponseAsString);
			return accessTokenRespose;
		}

		private JsonAccessTokenModel GetAccessTokenResponse(string userName, string password)
		{
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri(ApiUrls.GetAccessTokenUri(userName, password))
			};
			return GetResponse(httpClient);
		}

		private JsonAccessTokenModel GetExtendAccessTokenResponse()
		{
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri(ApiUrls.GetExtendAccessTokenUri(RefreshToken))
			};
			return GetResponse(httpClient);
		}

		private void Reset()
		{
			_accessTokenExpired = false;
			_accessTokenExtended = false;
			_refreshTokenExpired = false;

			_accessToken = string.Empty;
			_refreshToken = string.Empty;

			_requestedAccessToken = DateTime.MinValue;
			_extendedRefreshToken = DateTime.MinValue;

			_userName = string.Empty;
			_password = string.Empty;
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!_accessTokenExpired && _requestedAccessToken > DateTime.MinValue)
			{
				var expireTime = _requestedAccessToken.AddSeconds(_accessTokenLifespan.TotalSeconds);
				if (expireTime < DateTime.Now.AddMinutes(1))
				{
					_accessTokenExpired = true;
				}
			}
			else if (!_refreshTokenExpired && _extendedRefreshToken > DateTime.MinValue)
			{
				var expireTime = _extendedRefreshToken.AddSeconds(_refreshTokenLifespan.TotalSeconds);
				if (expireTime < DateTime.Now.AddMinutes(1))
				{
					_refreshTokenExpired = true;
				}
			}
		}
	}
}