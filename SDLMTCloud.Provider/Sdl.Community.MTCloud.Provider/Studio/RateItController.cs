using System;
using System.Linq;
using System.Windows;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Settings.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[ViewPart(
		Id = "RateTranslations_Name",
		Name = "RateTranslations_Name",
		Description = "RateTranslations_Name",
		Icon = "rating")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class RateItController : AbstractViewPartController
	{
		private Lazy<View.RateItControl> _control;

		public RateItController()
		{
			MtCloudApplicationInitializer.EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			MtCloudApplicationInitializer.Subscribe<TranslationProviderStatusChanged>(Settings_TranslationProviderStatusChanged);
			MtCloudApplicationInitializer.Subscribe<TranslationProviderRateItOptionsChanged>(TranslationProviderRateItOptionsChanged);
			MtCloudApplicationInitializer.Subscribe<TranslationProviderAdded>(TranslationProviderAdded);
		}

		public IRatingService RateIt => _control?.Value.RatingService;

		public void FocusFeedbackTextBox()
		{
			_control.Value.FocusFeedbackTextBox();
		}

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
			_control = new Lazy<View.RateItControl>(() => new View.RateItControl());
			Setup();
		}

		private static bool GetTpStatus()
		{
			var tpStatus =
				MtCloudApplicationInitializer.GetProjectInProcessing()?.GetTranslationProviderConfiguration().Entries
					.FirstOrDefault(
						e => e.MainTranslationProvider.Uri.ToString().Contains(PluginResources.SDLMTCloudUri))?.MainTranslationProvider
					.Enabled;
			return tpStatus ?? false;
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			Setup();
		}

		private void Settings_TranslationProviderStatusChanged(TranslationProviderStatusChanged tpInfo)
		{
			if (!tpInfo.TpUri.ToString().Contains(PluginResources.SDLMTCloudUri)) return;
			SwitchVisibility(tpInfo.NewStatus ?? false);
		}

		private void Setup(bool providerAdded = false)
		{
			if (MtCloudApplicationInitializer.CurrentViewDetector.View != Helpers.CurrentViewDetector.CurrentView.EditorView)
				return;

			var projectInProcessing = MtCloudApplicationInitializer.GetProjectInProcessing();
			if (MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project !=
			    projectInProcessing) return;

			var currentProvider = MtCloudApplicationInitializer.GetCurrentProjectProvider();

			var visibility = currentProvider != null && currentProvider.Options.SendFeedback && (GetTpStatus() || providerAdded);
			SwitchVisibility(visibility);

			if (currentProvider is null) return;
			try
			{
				Application.Current?.Dispatcher?.Invoke(
					() => RateIt.SetTranslationService(currentProvider.TranslationService));
			}
			catch
			{
				// catch all; unable to locate the controller
			}
		}

		private void SwitchVisibility(bool onOffSwitch)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				if (!onOffSwitch)
				{
					Hide();
				}
				else
				{
					Show();
				}
			});
		}

		private void TranslationProviderAdded(TranslationProviderAdded obj)
		{
			Setup(true);
		}

		private void TranslationProviderRateItOptionsChanged(TranslationProviderRateItOptionsChanged options)
		{
			Setup();
		}
	}
}