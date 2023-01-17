using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	public class CommunityForumAction : AbstractTellMeAction
	{
		public CommunityForumAction()
		{
			Name = $"{Constants.MicrosoftNaming_FullName} - Forum";
		}

		public override bool IsAvailable => true;

		public override string Category => $"{Constants.MicrosoftNaming_FullName}";

		public override Icon Icon => PluginResources.ForumIcon;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_CommunityForumUrl);
		}
	}
}