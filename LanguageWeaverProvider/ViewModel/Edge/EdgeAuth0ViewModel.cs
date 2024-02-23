using System.Net.Http;
using System.Threading.Tasks;
using System;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model;
using Microsoft.Web.WebView2.Wpf;
using LanguageWeaverProvider.Services;

namespace LanguageWeaverProvider.ViewModel.Edge
{
	public class EdgeAuth0ViewModel : BaseViewModel
	{
		private string _authUrl;
		private ITranslationOptions _translationOptions;
		private EdgeCredentials _edgeCredentials;
		private bool _success;

		public EdgeAuth0ViewModel(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
		{
			EdgeCredentials = edgeCredentials;
			TranslationOptions = translationOptions;
		}

		public string AuthUrl
		{
			get => _authUrl;
			set
			{
				if (_authUrl != value)
				{
					_authUrl = value;
					OnPropertyChanged(nameof(AuthUrl));
				}
			}
		}

		public ITranslationOptions TranslationOptions
		{
			get => _translationOptions;
			set
			{
				_translationOptions = value;
				OnPropertyChanged();
			}
		}

		public EdgeCredentials EdgeCredentials
		{
			get => _edgeCredentials;
			set
			{
				_edgeCredentials = value;
				OnPropertyChanged();
			}
		}

		public bool Success
		{
			get => _success;
			set
			{
				_success = value;
				OnPropertyChanged();
			}
		}

		public async Task Connect(WebView2 webView)
		{
			Success = await EdgeService.SignInAuthAsync(EdgeCredentials, TranslationOptions, webView);
		}
	}
}