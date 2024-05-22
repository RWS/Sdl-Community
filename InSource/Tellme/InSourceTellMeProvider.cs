using Sdl.Community.InSource.Tellme.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InSource.Tellme
{
	[TellMeProvider]
	public class InSourceTellMeProvider : ITellMeProvider
	{
		public string Name => "Trados InSource! Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ForumAction
			{
				Keywords = new[] {"insource", "insource community", "documentation docs"}
			},
			new DocumentationAction
			{
				Keywords = new[] {"insource", "insource help", "insource guide"}
			},
			new SourceCodeAction
			{
				Keywords = new[] {"insource", "insource help", "source code"}
			},
			new InSourceViewAction
			{
				Keywords = new[] {"insource", "insource view"}
			},
			new CheckProjectRequestsAction
			{
				Keywords = new[] {"insource", "check project requests"}
			},
			new CreateProjectsTellMeAction
			{
				Keywords = new[] {"insource", "create projects"}
			}
		};
	}
}
