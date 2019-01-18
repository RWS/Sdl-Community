namespace Sdl.Community.TMOptimizer.Control
{
    public interface IWizardPageControl
    {
        bool Next();

        bool Previous();

        void Help();

        void Finish();

        void Cancel();
    }
}