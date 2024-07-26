using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	public class ExcelTermCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Term Excelerator results";
		public override Icon Icon => PluginResources.Question;

		public ExcelTermCommunityWikiAction()
		{
			Name = "Term Excelerator wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5067/excel-terminology-provider-termexcelerator");
		}
	}
}