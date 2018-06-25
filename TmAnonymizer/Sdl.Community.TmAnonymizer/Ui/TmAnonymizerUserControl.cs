using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Newtonsoft.Json;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;


namespace Sdl.Community.TmAnonymizer.Ui
{
	public partial class TmAnonymizerUserControl : UserControl
	{
		public TmAnonymizerUserControl()
		{
			InitializeComponent();
			InitializeWpfApplicationSettings();
			
			if (!SettingsMethods.UserAgreed())
			{
				var acceptWindow = new AcceptWindow();
				acceptWindow.InitializeComponent();
				acceptWindow.Show();
				acceptWindow.Closing += AcceptWindow_Closing;
			}
			else
			{
				LoadTmView();
			}
		}

		private void InitializeWpfApplicationSettings()
		{
			if (System.Windows.Application.Current == null)
			{
				new System.Windows.Application();
			}
			if (System.Windows.Application.Current != null)
			{
				System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				var colorsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml",UriKind.Absolute),
				};
				System.Windows.Application.Current.Resources.MergedDictionaries.Add(colorsResources);
			}

			//create settings folder
			if (!Directory.Exists(Constants.SettingsFolderPath))
			{
				Directory.CreateDirectory(Constants.SettingsFolderPath);
			}
		
			var settings = SettingsMethods.GetSettings();
			if (!settings.AlreadyAddedDefaultRules)
			{
				AddDefaultRules(settings);
			}
		}

		private void AddDefaultRules(Settings settings)
		{
			settings.AlreadyAddedDefaultRules = true;
			settings.Rules = Constants.GetDefaultRules();
			File.WriteAllText(Constants.SettingsFilePath, JsonConvert.SerializeObject(settings));
		}

		private void LoadTmView()
		{
			var wpfMainWindow = new MainViewControl();
			wpfMainWindow.InitializeComponent();
			elementHost.Child = wpfMainWindow;
		}

		private void AcceptWindow_Closing(object sender, CancelEventArgs e)
		{
			if (SettingsMethods.UserAgreed())
			{
				LoadTmView();
			}
		}
	}
}
