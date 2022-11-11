using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace GoogleTranslatorProvider.TellMe
{
	public class CommunityForumAction : AbstractTellMeAction
	{
		public CommunityForumAction()
		{
			Name = $"{Constants.GooglePluginName} - Forum";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.ForumIcon;

		public override string Category => Constants.GooglePluginName;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_CommunityForumUrl);
		}
	}
}