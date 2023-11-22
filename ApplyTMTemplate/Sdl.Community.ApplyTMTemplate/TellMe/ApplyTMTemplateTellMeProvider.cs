using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	[TellMeProvider]
	public class ApplyTMTemplateTellMeProvider : ITellMeProvider
	{
		public string Name => "Apply TM Template Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ApplyTMTemplateStoreAction
			{
				Keywords = new[] {  "ApplyTm", "applytm","apply", "tm", "template", "apply tm template", "apply tm store", "apply tm download"}
			},
			new ApplyTMCommunitySupportAction
			{
				Keywords = new[] { "ApplyTm", "applytm", "apply", "tm", "template", "apply tm template","forum", "community", "support"}
			},
			new ApplyTmTemplateAction
			{
				Keywords = new[] { "ApplyTm", "applytm", "apply", "tm", "template", "apply tm template", "start"}
			},
			new ApplyTMTemplateHelpAction()
			{
				Keywords = new []{ "ApplyTm", "applytm", "apply", "tm", "template", "apply tm template", "wiki", "help"}
			}
		};
	}
}