using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	[ContractClass(typeof(ICleanUpMessageReporterContract))]
    public interface ICleanUpMessageReporter
    {
        void Report(ISegmentHandler handler, ErrorLevel errorLevel, string message, string locationDescription);

        void Report(ISegmentHandler handler, ErrorLevel errorLevel, string message, TextLocation from, TextLocation to);
    }
}