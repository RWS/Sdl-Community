using Sdl.TellMe.ProviderApi;

namespace InterpretBank.Studio.TellMe
{
    public abstract class InterpretBankTellMeAction : AbstractTellMeAction
    {
        public override string Category => "InterpretBank results";
        public override bool IsAvailable => true;

        public abstract override void Execute();
    }
}