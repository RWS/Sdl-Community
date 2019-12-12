using Sdl.FileTypeSupport.Framework.Formatting;

namespace SDLCommunityCleanUpTasks
{
	public interface IVerifyingFormattingVisitor : IFormattingVisitor
    {
        bool ShouldRemoveTag();
        void ResetVerifier();
    }
}