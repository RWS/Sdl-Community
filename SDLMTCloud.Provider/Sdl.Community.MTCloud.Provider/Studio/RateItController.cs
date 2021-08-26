﻿using System;
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
			SwitchVisibility();
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

		private void SwitchVisibility()
		{
			if (!MtCloudApplicationInitializer.IsStudioRunning()) return;

			if (!GetTpStatus())
			{
				SwitchVisibility(false);
				return;
			}

			var currentProvider = MtCloudApplicationInitializer.GetCurrentProjectProvider();
			if (currentProvider is null) return;

			var visibility = currentProvider.Options.SendFeedback;

			SwitchVisibility(visibility);

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

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			SwitchVisibility();
		}

		private void Settings_TranslationProviderStatusChanged(TranslationProviderStatusChanged tpInfo)
		{
			if (!tpInfo.TpUri.ToString().Contains(PluginResources.SDLMTCloudUri)) return;
			SwitchVisibility(tpInfo.NewStatus ?? false);
		}

		private void SwitchVisibility(bool onOffSwitch)
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
			SwitchVisibility();
		}
	}
}