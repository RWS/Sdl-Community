using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	[TellMeProvider]
	public class ApplyTMTemplateTellMeProvider : ITellMeProvider
	{
		public string Name => "Apply TM Template tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ApplyTMTemplateStoreAction
			{
				Keywords = new[] {"applytm", "apply tm store", "apply tm download"}
			},
			new ApplyTMCommunitySupportAction
			{
				Keywords = new[] {"apply tm template", "community", "support"}
			},
			new ApplyTMTemplateAction
			{
				Keywords = new[] {"apply tm template", "start"}
			},
			new ApplyTMTemplateHelpAction()
			{
				Keywords = new []{"apply tm template", "help"}
			}
		};
	}
}