using Sdl.Community.StyleSheetVerifier.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StyleSheetVerifier.TellMe
{
    [TellMeProvider]
    public class StyleSheetVerifierProvider : ITellMeProvider
    {
        public string Name => "StyleSheet Verifier Tell Me provider";

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new DocumentationAction
            {
                Keywords = new[] {"stylesheetverifier", "stylesheet verifier", "stylesheetverifier community", "stylesheet verifier community", "stylesheetverifier support",
                    "stylesheet verifier support", "stylesheetverifier wiki", "stylesheet verifier documentation docs" }
            },
            new StyleSheetVerifierCommunityAction
            {
                Keywords = new[] { "stylesheetverifier", "stylesheet verifier", "stylesheetverifier community", "stylesheet verifier community", "stylesheetverifier support",
                    "stylesheet verifier support", "stylesheetverifier forum", "stylesheet verifier forum" }
            },
            new SourceCodeAction
            {
                Keywords = new[] { "stylesheetverifier", "stylesheet verifier", "source code" }
            },
            new VerifyAction
            {
                Keywords = new[] { "stylesheetverifier", "stylesheet verifier" }
            }
        };
    }
}