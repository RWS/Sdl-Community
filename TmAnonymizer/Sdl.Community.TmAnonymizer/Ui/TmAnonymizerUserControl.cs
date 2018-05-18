using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.Ui
{
	public partial class TmAnonymizerUserControl : UserControl
	{
		public TmAnonymizerUserControl()
		{
			InitializeComponent();
			//var tm = new ServerBasedTranslationMemory(translationProviderServer);

			if (!Directory.Exists(Constants.SettingsFolderPath))
			{
				Directory.CreateDirectory(Constants.SettingsFolderPath);
			}
			if (!File.Exists(Constants.SettingsFilePath))
			{
				var settingsFile = File.Create(Constants.SettingsFilePath);
				settingsFile.Close();
			}
			if (!AgreementMethods.UserAgreed())
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

		private void LoadTmView()
		{
			var wpfMainWindow = new MainViewControl();
			wpfMainWindow.InitializeComponent();
			elementHost.Child = wpfMainWindow;
		}

		private void AcceptWindow_Closing(object sender, CancelEventArgs e)
		{
			if (AgreementMethods.UserAgreed())
			{
				LoadTmView();
			}
			
		}
	}
}
