﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public  class ContactAction: AbstractTellMeAction
	{
		public override void Execute()
		{
			Process.Start("https://translate.sdlbeglobal.com/");
		}
		public ContactAction()
		{
			Name = $"{PluginResources.SDLMTCloud_Provider_Name} web interface";
		}

		public override bool IsAvailable => true;
		public override string Category => $"{PluginResources.SDLMTCloud_Provider_Name} results";
		public override Icon Icon => PluginResources.global;
	}
}
