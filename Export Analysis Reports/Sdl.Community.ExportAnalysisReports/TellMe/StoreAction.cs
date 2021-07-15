﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download Trados Export Analysis Reports from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/trados-studio-export-analysis-reports/3/");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados Export Analysis Reports results";
		public override Icon Icon => PluginResources.Download;
	}
}