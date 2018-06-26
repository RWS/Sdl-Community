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
				var controlsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml",UriKind.Absolute)
				};
				var colorsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml",UriKind.Absolute)
				};
				var fontsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.Absolute)
				};
				var greenResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml", UriKind.Absolute)
				};
				var baseLightResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.Absolute)
				};
				var flatButtonsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml", UriKind.Absolute)
				};

				//System.Windows.Application.Current.Resources.MergedDictionaries.Add(fontsResources);
				System.Windows.Application.Current.Resources.MergedDictionaries.Add(colorsResources);
				System.Windows.Application.Current.Resources.MergedDictionaries.Add(greenResources);
				System.Windows.Application.Current.Resources.MergedDictionaries.Add(baseLightResources);
				System.Windows.Application.Current.Resources.MergedDictionaries.Add(flatButtonsResources);
				System.Windows.Application.Current.Resources.MergedDictionaries.Add(controlsResources);
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
