using Sdl.Community.TargetWordCount.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.TargetWordCount
{
	internal abstract class ISegmentWordCounterContract : ISegmentWordCounter
    {
        public FileCountInfo FileCountInfo
        {
            get
            {
                return default(FileCountInfo);
            }
        }

        public string FileName
        {
            get
            {
                return default(string);
            }
        }

        public void FileComplete()
        {
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
        }
    }
}