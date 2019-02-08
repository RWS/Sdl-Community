using Sdl.Community.CleanUpTasks.Utilities;

namespace Sdl.Community.CleanUpTasks.Contracts
{
    internal abstract class IXmlReportGeneratorContract : IXmlReportGenerator
    {
        public void AddConversionItem(string segmentNumber, string before, string after, string searchText, string replaceText)
        {
        }

        public void AddFile(string fileName)
        {
        }

        public void AddLockItem(string segmentNumber, string lockedContent, string lockReason)
        {

        }

        public void AddTagItem(string segmentNumber, string removedTag)
        {
        }
    }
}