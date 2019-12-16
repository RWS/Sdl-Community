using System.Diagnostics.Contracts;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks.Utilities
{
	[ContractClass(typeof(IXmlReportGeneratorContract))]
    public interface IXmlReportGenerator
    {
        void AddConversionItem(string segmentNumber, string before, string after, string searchText, string replaceText);

        void AddFile(string fileName);

        void AddLockItem(string segmentNumber, string lockedContent, string lockReason);

        void AddTagItem(string segmentNumber, string removedTag);
    }
}