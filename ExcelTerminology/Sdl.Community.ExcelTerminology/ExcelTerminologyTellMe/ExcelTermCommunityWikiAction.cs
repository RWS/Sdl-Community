using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExcelTerminology.ExcelTerminologyTellMe
{
	public class ExcelTermCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Excel Terminology results";
		public override Icon Icon => PluginResources.ForumIcon;

		public ExcelTermCommunityWikiAction()
		{
			Name = "SDL Community Excel Terminology / Term Excelerator wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5067/excel-terminology-provider-termexcelerator");
		}
	}
}