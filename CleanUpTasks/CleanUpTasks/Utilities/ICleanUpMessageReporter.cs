using System.Diagnostics.Contracts;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks.Utilities
{
	[ContractClass(typeof(ICleanUpMessageReporterContract))]
    public interface ICleanUpMessageReporter
    {
        void Report(ISegmentHandler handler, ErrorLevel errorLevel, string message, string locationDescription);

        void Report(ISegmentHandler handler, ErrorLevel errorLevel, string message, TextLocation from, TextLocation to);
    }
}