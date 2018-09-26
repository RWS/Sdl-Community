using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.Services;
using PathInfo = Sdl.Community.SdlTmAnonymizer.Model.PathInfo;

namespace Sdl.Community.SdlTmAnonymizer.Ui
{
	public partial class TmAnonymizerUserControl : UserControl
	{
		private readonly SettingsService _settingsService;
		private readonly AcceptWindow _acceptWindow;

		public TmAnonymizerUserControl()
		{
			InitializeComponent();
			
			_settingsService = new SettingsService(new PathInfo());		

			InitializeWpfApplicationSettings();

			if (!_settingsService.UserAgreed())
			{
				// TODO: confirm if this should rather be a model dialog?
				_acceptWindow = new AcceptWindow(_settingsService);
				_acceptWindow.InitializeComponent();
				_acceptWindow.Show();
				_acceptWindow.Closing += AcceptWindow_Closing;
			}
			else
			{
				LoadTmView();
			}
		}

		private void InitializeWpfApplicationSettings()
		{
			UpdateApplicationResources();

			_settingsService.AddDefaultRules();
		}

		private static void UpdateApplicationResources()
		{
			if (System.Windows.Application.Current == null)
			{
				// TODO: confirm is this required?
				new System.Windows.Application();
			}

			if (System.Windows.Application.Current != null)
			{
				System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				var controlsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.Absolute)
				};
				var colorsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.Absolute)
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
		}

		private void LoadTmView()
		{
			var wpfMainWindow = new MainViewControl(_settingsService);

			wpfMainWindow.InitializeComponent();
			elementHost.Child = wpfMainWindow;
		}

		private void AcceptWindow_Closing(object sender, CancelEventArgs e)
		{
			_acceptWindow.Closing -= AcceptWindow_Closing;

			if (_settingsService.UserAgreed())
			{
				LoadTmView();
			}
		}
	}
}
