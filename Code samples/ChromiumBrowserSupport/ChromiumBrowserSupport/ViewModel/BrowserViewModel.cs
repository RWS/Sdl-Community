using System;
using System.Web;
using System.Windows;
using System.Windows.Input;
using ChromiumBrowserSupport.Commands;

namespace ChromiumBrowserSupport.ViewModel
{
	public class BrowserViewModel: BaseViewModel
	{
		
		private bool _needsClearingCookies;
		private string _uri;
		private string _redirectUri;

		private ICommand _saveCommand;

		public BrowserViewModel()
		{
			Name = PluginResources.Plugin_Name;
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(Save));

		public bool NeedsClearingCookies
		{
			get => _needsClearingCookies;
			set
			{
				if (_needsClearingCookies == value)
				{
					return;
				}

				_needsClearingCookies = value;
				OnPropertyChanged(nameof(NeedsClearingCookies));
			}
		}

		public string Uri
		{
			get => _uri;
			set
			{
				if (_uri == value)
				{
					return;
				}

				_uri = value;
				OnPropertyChanged(nameof(Uri));
			}
		}

		public string RedirectUri
		{
			get => _redirectUri;
			set
			{
				if (_redirectUri == value)
				{
					return;
				}

				_redirectUri = value;
				OnPropertyChanged(nameof(RedirectUri));
			}
		}

		public void ExtractAuthorizationCode(Uri redirectUri)
		{
			var query = HttpUtility.ParseQueryString(redirectUri.Query);
			var authorizationCode = query.Get("code");

			// Do something...
		}

		private void Save(object obj)
		{
			if (obj is Window window)
			{
				//Do something...
				window.DialogResult = true;
			}
		}
	}
}
