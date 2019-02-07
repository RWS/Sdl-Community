using System.Diagnostics.Contracts;
using Sdl.Community.TargetWordCount.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.TargetWordCount
{
	//[ContractClass(typeof(ISegmentWordCounterContract))]
    public interface ISegmentWordCounter
    {
        FileCountInfo FileCountInfo { get; }

        string FileName { get; }

        void FileComplete();

        void Initialize(IDocumentProperties documentInfo);

        void ProcessParagraphUnit(IParagraphUnit paragraphUnit);
    }
}