using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks.Contracts
{
	internal abstract class ICleanUpMessageReporterContract : ICleanUpMessageReporter
    {
        public void Report(ISegmentHandler handler, ErrorLevel errorLevel, string message, string locationDescription)
        {
        }

        public void Report(ISegmentHandler handler, ErrorLevel errorLevel, string message, TextLocation from, TextLocation to)
        {
        }
    }
}