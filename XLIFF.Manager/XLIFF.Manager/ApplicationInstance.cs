using System.IO;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.View;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Application = System.Windows.Application;

namespace Sdl.Community.XLIFF.Manager
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		private static PathInfo SettingsPathInfo { get; set; }

		public static Form GetActiveForm()
		{
			var allForms = System.Windows.Forms.Application.OpenForms;
			var activeForm = allForms[allForms.Count - 1];
			foreach (Form form in allForms)
			{
				if (form.GetType().Name == "StudioWindowForm")
				{
					activeForm = form;
					break;
				}
			}
			return activeForm;
		}

		public static void ShowSettingsWindow()
		{
			var settings = GetSettings();
			var view = new SettingsWindow();
			var viewModel = new SettingsViewModel(view, settings, SettingsPathInfo);
			view.DataContext = viewModel;
			view.ShowDialog();
		}

		public void Execute()
		{
			SetApplicationShutdownMode();
			SettingsPathInfo = new PathInfo();
		}

		private static Settings GetSettings()
		{
			if (File.Exists(SettingsPathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(SettingsPathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private static void SetApplicationShutdownMode()
		{
			if (Application.Current == null)
			{
				// initialize the enviornments application instance
				new Application();
			}

			if (Application.Current != null)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			}
		}
	}
}