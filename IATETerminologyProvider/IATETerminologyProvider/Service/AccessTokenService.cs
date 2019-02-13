using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;
using RestSharp;

namespace IATETerminologyProvider.Service
{
	public class AccessTokenService : INotifyPropertyChanged, IDisposable
	{
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

		private readonly DispatcherTimer _timer;

		public AccessTokenService(TimeSpan accessTokenLifespan, TimeSpan refreshTokenLifespan)
		{
			Reset();

			_accessTokenLifespan = accessTokenLifespan.TotalSeconds > 0 ? accessTokenLifespan : new TimeSpan(0, 3, 0, 0);
			_refreshTokenLifespan = refreshTokenLifespan.TotalSeconds > 0 ? refreshTokenLifespan : new TimeSpan(0, 12, 0, 0);

			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1000)
			};
			_timer.Tick += Timer_Tick;
			_timer.Start();
		}

		public AccessTokenService() : this(new TimeSpan(0), new TimeSpan(0)) { }

		public bool GetAccessToken(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
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

		public bool GetAccessToken()
		{
			return GetAccessToken("SDL_PLUGIN", "E9KWtWahXs4hvE9z");
		}

		public bool ExtendAccessToken()
		{
			if (_accessTokenExtended || string.IsNullOrEmpty(_accessToken) ||
				string.IsNullOrEmpty(RefreshToken) || _requestedAccessToken == DateTime.MinValue)
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

		private JsonAccessTokenModel GetAccessTokenResponse(string userName, string password)
		{
			var client = new RestClient(ApiUrls.GetAccessTokenUri(userName, password));
			return GetResponse(client);
		}

		private JsonAccessTokenModel GetExtendAccessTokenResponse()
		{
			var client = new RestClient(ApiUrls.GetExtendAccessTokenUri(RefreshToken));
			return GetResponse(client);
		}

		private static JsonAccessTokenModel GetResponse(IRestClient client)
		{
			var request = new RestRequest("", Method.GET);
			request.AddHeader("Connection", "Keep-Alive");
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Pragma", "no-cache");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Origin", "https://iate.europa.eu");
			request.AddHeader("Host", "iate.europa.eu");
			request.AddHeader("Access-Control-Allow-Origin", "*");

			var response = client.Execute(request);

			var accessTokenRespose = JsonConvert.DeserializeObject<JsonAccessTokenModel>(response.Content);
			return accessTokenRespose;
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

		private void Timer_Tick(object sender, System.EventArgs e)
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Dispose()
		{
			if (_timer != null)
			{
				_timer.Tick -= Timer_Tick;
			}
		}
	}
}
