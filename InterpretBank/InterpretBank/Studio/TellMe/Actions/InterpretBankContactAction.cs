using System.Diagnostics;
using System.Drawing;

namespace InterpretBank.Studio.TellMe.Actions
{
    public class InterpretBankContactAction : InterpretBankTellMeAction
    {
        public InterpretBankContactAction()
        {
            Name = "InterpretBank Official Website";
            Keywords = ["interpret bank interpretbank contact"];
        }

        public override Icon Icon => PluginResources.IB;

        public override void Execute()
        {
            Process.Start("https://interpretbank.com/site/");
        }
    }
}