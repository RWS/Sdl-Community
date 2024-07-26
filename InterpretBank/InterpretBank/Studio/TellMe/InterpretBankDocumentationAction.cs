using System.Diagnostics;
using System.Drawing;

namespace InterpretBank.Studio.TellMe
{
    public class InterpretBankDocumentationAction : InterpretBankTellMeAction
    {
        public InterpretBankDocumentationAction()
        {
            Name = "InterpretBank Documentation";
            Keywords = ["interpret", "bank", "documentation", "interpretbank"];
        }

        public override Icon Icon => PluginResources.TellmeDocumentation;

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/243?tab=documentation");
        }
    }
}