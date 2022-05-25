﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	public class IATEStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Download;

		public IATEStoreAction()
		{
			Name = "Download IATE Terminology Provider from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/iate-terminology/950/");
		}
	}
}