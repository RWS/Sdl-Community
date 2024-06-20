using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	[TellMeProvider]
	public class ApplyTMTemplateTellMeProvider : ITellMeProvider
	{
		public string Name => "applyTM Template Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ApplyTMDocumentationAction
            {
				Keywords = new[] {  "applyTm", "applytm","apply", "tm", "template", "apply tm template", "apply tm documentation", "documentation"}
			},
			new ApplyTMCommunitySupportAction
			{
				Keywords = new[] { "applyTm", "applytm", "apply", "tm", "template", "apply tm template","forum", "community", "support"}
			},
            new ApplyTMSourceCodeAction()
            {
                Keywords = new []{ "applyTm", "applytm", "apply", "tm", "template", "apply tm template", "wiki", "help"}
            },
            new ApplyTMSettingsAction
			{
				Keywords = new[] { "applyTm", "applytm", "apply", "tm", "template", "apply tm template", "settings"}
			}
        };
	}
}