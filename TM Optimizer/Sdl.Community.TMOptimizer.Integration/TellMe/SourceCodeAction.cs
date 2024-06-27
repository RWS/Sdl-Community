﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMOptimizer.Integration.TellMe
{
	public class SourceCodeAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.TellMe_SourceCode;

		public SourceCodeAction()
        {
            Name = string.Format("{0} Source Code", PluginResources.Plugin_Name);
        }

		public override void Execute()
		{
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/TM%20Optimizer");
		}
	}
}
