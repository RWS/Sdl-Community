﻿using System.Drawing;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.TellMe.Actions
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction() => Name = $"{PluginResources.Plugin_Name} Settings";

		public override string Category =>
			string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Settings2;
		public override bool IsAvailable => true;

		public override void Execute() => SdlTradosStudio.Application.ExecuteAction<QualitivityConfiguration>();
	}
}