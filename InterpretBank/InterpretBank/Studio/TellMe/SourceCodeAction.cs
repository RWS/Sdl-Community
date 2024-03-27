using System.Diagnostics;
using System.Drawing;

namespace InterpretBank.Studio.TellMe
{
    public class SourceCodeAction : InterpretBankTellMeAction
    {
        public SourceCodeAction()
        {
            Name = "InterpretBank Source Code";
            Keywords = ["interpret", "bank", "code", "source", "interpretbank"];
        }

        public override Icon Icon => PluginResources.SourceCode;

        public override void Execute()
        {
            Process.Start("https://github.com/RWS/Sdl-Community/tree/Studio_2022_SR2/InterpretBank");
        }
    }
}