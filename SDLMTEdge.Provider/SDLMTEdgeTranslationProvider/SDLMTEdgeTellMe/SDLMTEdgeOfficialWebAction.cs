using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class SDLMTEdgeOfficialWebAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => Properties.WeaverEdgeResource.WeaverEdge_CategoryResults;
		public override Icon Icon => PluginResources.ForumIcon;

		public SDLMTEdgeOfficialWebAction()
		{
			Name = "Official SDL Machine Translation Edge documentation";
		}

		public override void Execute()
		{
			Process.Start("https://docs.sdl.com/LiveContent/web/pub.xql?action=home&pub=SDL%20ETS%20Documentation-v4&lang=en-US#docid=GUID-6FD063FF-0947-4966-8190-5CAE05EF60CB&addHistory=true&query=&scope=&tid=&filename=&resource=&inner_id=&toc=false&eventType=lcContent.loadDocGUID-6FD063FF-0947-4966-8190-5CAE05EF60CB");
		}
	}
}