using System;
using System.Diagnostics.Contracts;
using Sdl.Community.TargetWordCount.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.TargetWordCount
{
	[ContractClassFor(typeof(ISegmentWordCounter))]
    internal abstract class ISegmentWordCounterContract : ISegmentWordCounter
    {
        public FileCountInfo FileCountInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<FileCountInfo>() != null);
                Contract.Ensures(Contract.Result<FileCountInfo>().SourceInfo != null);
                Contract.Ensures(Contract.Result<FileCountInfo>().TargetInfo != null);
                return default(FileCountInfo);
            }
        }

        public string FileName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
                return default(string);
            }
        }

        public void FileComplete()
        {
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
            Contract.Requires<ArgumentNullException>(documentInfo != null);
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            Contract.Requires<ArgumentNullException>(paragraphUnit != null);
        }
    }
}