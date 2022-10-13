using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Multilingual.Excel.FileType.TellMe
{
	public class CommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Question;

		public CommunityWikiAction()
		{
			Name = string.Format("{0} plugin wiki", PluginResources.Plugin_Name);
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6290/multilingual-excel-filetype");
		}
	}
}
