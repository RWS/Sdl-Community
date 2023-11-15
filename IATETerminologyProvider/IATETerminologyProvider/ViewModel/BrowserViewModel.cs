using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.IATETerminologyProvider.Commands;
using System.Web;
using System.Windows.Input;
using System.Xml.Linq;

namespace Sdl.Community.IATETerminologyProvider.ViewModel
{
	public class BrowserViewModel : BaseViewModel
	{

		private bool _needsClearingCookies;
		private string _uri;
		private string _redirectUri;
		public BrowserViewModel()
		{
			Name = PluginResources.Plugin_Name;
		}

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

	}
}
