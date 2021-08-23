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
		Id = "SDL MT Cloud Rate Translations",
		Name = "SDL MT Cloud Rate Translations",
		Description = "SDL MT Cloud Rate Translations",
		Icon = "rating")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class RateItController : AbstractViewPartController
	{
		private Lazy<View.RateItControl> _control;

		public RateItController()
		{
			MtCloudApplicationInitializer.ProjectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;
			MtCloudApplicationInitializer.EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			MtCloudApplicationInitializer.Subscribe<TranslationProviderStatusChanged>(Settings_TranslationProviderStatusChanged);
			MtCloudApplicationInitializer.Subscribe<TranslationProviderRateItOptionsChanged>(TranslationProviderRateItOptionsChanged);
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
		}

		private static bool? GetTpStatus()
		{
			var tpStatus =
				Application.Current.Dispatcher.Invoke(
					() =>
						MtCloudApplicationInitializer.GetProjectInProcessing()?.GetTranslationProviderConfiguration().Entries
							.FirstOrDefault(
								e => e.MainTranslationProvider.Uri.ToString().Contains(PluginResources.SDLMTCloudUri))?.MainTranslationProvider
							.Enabled);
			return tpStatus;
		}

		private void ActivateRatingController()
		{
			if (!MtCloudApplicationInitializer.IsStudioRunning()) return;

			var tpStatus = GetTpStatus();
			if (tpStatus is null)
			{
				SwitchRateTranslationsControllerVisibility(false);
				return;
			}

			try
			{
				Application.Current?.Dispatcher?.Invoke(
					() => RateIt.SetTranslationService(MtCloudApplicationInitializer.TranslationService));
			}
			catch
			{
				// catch all; unable to locate the controller
			}

			SwitchRateTranslationsControllerVisibility(tpStatus.Value);
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			ActivateRatingController();
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			ActivateRatingController();
		}

		private void Settings_TranslationProviderStatusChanged(TranslationProviderStatusChanged tpInfo)
		{
			if (!tpInfo.TpUri.ToString().Contains(PluginResources.SDLMTCloudUri)) return;
			SwitchRateTranslationsControllerVisibility(tpInfo.NewStatus ?? false);
		}

		private void SwitchRateTranslationsControllerVisibility(bool onOffSwitch)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				if (!onOffSwitch)
				{
					Hide();
				}
				if (onOffSwitch)
				{
					Activate();
				}
			});
		}

		private void TranslationProviderRateItOptionsChanged(TranslationProviderRateItOptionsChanged options)
		{
			SwitchRateTranslationsControllerVisibility(options.SendFeedback);
		}
	}
}