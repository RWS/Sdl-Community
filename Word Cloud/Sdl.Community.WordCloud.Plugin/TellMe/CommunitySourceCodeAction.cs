using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.WordCloud.Plugin
{
	public class CommunitySourceCodeAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.TellMe_SourceCode;

		public CommunitySourceCodeAction()
		{
			Name = string.Format(PluginResources.TellMe_Source_Code, PluginResources.Plugin_Name); 
		}

		public override void Execute()
		{			
			Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Word%20Cloud");
		}
	}
}
