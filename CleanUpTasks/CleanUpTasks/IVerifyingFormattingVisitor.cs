using Sdl.FileTypeSupport.Framework.Formatting;

namespace Sdl.Community.CleanUpTasks
{
	public interface IVerifyingFormattingVisitor : IFormattingVisitor
    {
        bool ShouldRemoveTag();

        void ResetVerifier();
    }
}