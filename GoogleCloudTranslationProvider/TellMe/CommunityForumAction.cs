using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace GoogleCloudTranslationProvider.TellMe
{
	public class CommunityForumAction : AbstractTellMeAction
	{
		public CommunityForumAction()
		{
			Name = $"{Constants.GoogleNaming_FullName} - Forum";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.ForumIcon;

		public override string Category => Constants.GoogleNaming_FullName;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_CommunityForumUrl);
		}
	}
}