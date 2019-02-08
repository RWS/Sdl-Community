using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.CleanUpTasks.Contracts
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