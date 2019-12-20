using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public class XmlReportGenerator : IXmlReportGenerator
    {
        private const string File = "File";
        private const string FullPath = "FullPath";
        private const string Name = "Name";
        private readonly XDocument document = new XDocument(new XElement("CleanupReport"));
        private readonly string logFolder = null;
        private int conversionCount = 0;
        private int segmentLockCount = 0;
        private int tagRemoveCount = 0;

        public XmlReportGenerator(string logFolder)
        {
            this.logFolder = logFolder;
        }

        public void AddConversionItem(string segmentNumber, string before, string after, string searchText, string replaceText = "")
        {
            SimpleLog.Info(string.Join(Environment.NewLine,
                           $"{ Environment.NewLine }\tCHANGED TEXT",
                           $"\tId: { segmentNumber }",
                           $"\tBefore: { before }",
                           $"\tAfter: { after }",
                           $"\tSearched Text: { searchText }",
                           $"\tReplaced Text: { replaceText + Environment.NewLine }"));

            conversionCount++;
        }

        public void AddFile(string fileName)
        {
            var file = Path.GetFileName(fileName);
            SimpleLog.SetLogFile(logDir: logFolder, prefix: file + "_", writeText: false, check: false);

            document.Root.Add(new XElement(File,
                     new XAttribute(Name, file),
                     new XAttribute(FullPath, new Uri(SimpleLog.FileName).AbsoluteUri)));
        }

        public void AddLockItem(string segmentNumber, string lockedContent, string lockReason)
        {
            SimpleLog.Info(string.Join(Environment.NewLine,
                           $"{ Environment.NewLine }\tLOCKED SEGMENT",
                           $"\tId: { segmentNumber }",
                           $"\tContent: { lockedContent }",
                           $"\tReason: { lockReason + Environment.NewLine }"));

            segmentLockCount++;
        }

        public void AddTagItem(string segmentNumber, string removedTag)
        {
            SimpleLog.Info(string.Join(Environment.NewLine,
                           $"{ Environment.NewLine }\tREMOVED TAG",
                           $"\tId: { segmentNumber }",
                           $"\tRemoved Tag: { removedTag + Environment.NewLine }"));

            tagRemoveCount++;
        }

        public override string ToString()
        {
            var file = GetLastFileElement();

            file.Add(new XElement("LockCount", segmentLockCount),
                     new XElement("TagRemoveCount", tagRemoveCount),
                     new XElement("ConversionCount", conversionCount));

            Reset();

            return document.ToStringWithDeclaration();
        }

        private XElement GetLastFileElement()
        {
            return document.Root.Elements(File).Last();
        }

        private void Reset()
        {
            segmentLockCount = 0;
            tagRemoveCount = 0;
            conversionCount = 0;
        }
    }
}