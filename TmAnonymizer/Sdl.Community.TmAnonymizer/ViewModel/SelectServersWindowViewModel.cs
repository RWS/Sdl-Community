using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class SelectServersWindowViewModel : ViewModelBase
	{
		private List<TmFile> _translationMemories;
		private readonly SettingsService _settingsService;
		private readonly Login _login;

		public SelectServersWindowViewModel(SettingsService settingsService, Login login)
		{
			_translationMemories = new List<TmFile>();
			_settingsService = settingsService;
			_login = login;
		}

		public List<TmFile> TranslationMemories
		{
			get
			{
				return _translationMemories;
			}
			set
			{
				if (_translationMemories == value)
				{
					return;
				}

				_translationMemories = value;

				OnPropertyChanged(nameof(TranslationMemories));
			}
		}

		/// <summary>
		/// Connects to GS and set the list of TMS
		/// </summary>
		/// <param name="login"></param>
		private void GetServerTms(Login login)
		{
			var uri = new Uri(login.Url);
			var translationProviderServer = new TranslationProviderServer(uri, false, login.UserName, login.Password);
			var translationMemories = translationProviderServer.GetTranslationMemories(TranslationMemoryProperties.None);

			foreach (var tm in translationMemories)
			{
				var tmPath = tm.ParentResourceGroupPath == "/" ? "" : tm.ParentResourceGroupPath;
				var path = tmPath + "/" + tm.Name;
				var tmAlreadyExist = _translationMemories.Any(t => t.Path.Equals(path));

				if (!tmAlreadyExist)
				{
					var serverTm = new TmFile
					{
						Path = path,
						Name = tm.Name,
						IsServerTm = true
					};

					_translationMemories.Add(serverTm);
				}
			}
		}
	}
}
