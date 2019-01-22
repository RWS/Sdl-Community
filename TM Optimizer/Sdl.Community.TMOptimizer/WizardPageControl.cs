namespace Sdl.Community.TMOptimizer
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