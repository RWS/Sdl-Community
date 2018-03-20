using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    class SplitInfoWriter
    {
        private string _infoFileExt = Properties.StringResource.extSplitInfo;
        
        private XmlTextWriter _writer;
        private int _filesCount;
        private string _srcFile;
        private string _srcFileCRC;

        public SplitInfoWriter(string srcFile, string srcFileCRC, string outPath)
        {
            _filesCount = 0;
            _srcFile = srcFile;
            _srcFileCRC = srcFileCRC;

            _writer = new XmlTextWriter(string.Format(@"{0}\{1}.{2}", outPath, Path.GetFileNameWithoutExtension(_srcFile), _infoFileExt), null);
            WriteStart();
        }

        public void WriteFileTag(string fNameAttr)
        {
            if (_filesCount > 0)
                _writer.WriteEndElement(); // close file

            _writer.WriteStartElement(SplitInfoTag.getTag(SplitInfo.File));
            _writer.WriteAttributeString(SplitInfoTag.getTag(SplitInfo.FileNameAttr), fNameAttr);
            _filesCount++;
        }
        public void WriteTransUnitsCountTag(int tusCount)
        {
            _writer.WriteElementString(SplitInfoTag.getTag(SplitInfo.TransUnits), tusCount.ToString());
        }
        public void WriteWordsCountTag(int wordsCount)
        {
            _writer.WriteElementString(SplitInfoTag.getTag(SplitInfo.Words), wordsCount.ToString());
        }
        //public void WriteTransUnitTag(int tusCount)
        //{
        //    _writer.WriteStartElement(SplitInfoTag.getTag(SplitInfo.TransUnit));
        //    _writer.WriteAttributeString(SplitInfoTag.getTag(SplitInfo.TransUnitIDAttr), tuIDAttr);
        //    _writer.WriteEndElement();
        //}
        public void CloseWrite(int flsCount)
        {
            _writer.WriteEndElement(); // close file
            _writer.WriteEndElement(); // close index

            _writer.WriteElementString(SplitInfoTag.getTag(SplitInfo.FilesCount), flsCount.ToString());

            WriteEnd();
        }


        private void WriteStart()
        {
            _writer.WriteStartDocument();

            _writer.WriteStartElement(SplitInfoTag.getTag(SplitInfo.SplitInfo));

            _writer.WriteElementString(SplitInfoTag.getTag(SplitInfo.FileName), Path.GetFileNameWithoutExtension(_srcFile));
            _writer.WriteElementString(SplitInfoTag.getTag(SplitInfo.FileCRC), Path.GetFileNameWithoutExtension(_srcFileCRC));
            _writer.WriteElementString(SplitInfoTag.getTag(SplitInfo.DateCreated), DateTime.Now.ToString());

            _writer.WriteStartElement(SplitInfoTag.getTag(SplitInfo.TransUnitIndex));
        }
        private void WriteEnd()
        {
            _writer.WriteEndDocument();

            _writer.Close();
        }
    }

    public enum SplitInfo
    {
        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        SplitInfo,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        FileName,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        FileCRC,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        DateCreated,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        FilesCount,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        TransUnitIndex,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        File,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        TransUnits,

        /// <summary>
        /// Represents SplitInfo file Tag
        /// </summary>
        Words,

        /// <summary>
        /// Represents SplitInfo file Tag Attr
        /// </summary>
        FileNameAttr
}

    public static class SplitInfoTag
    {

        public static string getTag(SplitInfo type)
        {
            switch (type)
            {
                case SplitInfo.SplitInfo:
                    return "split-info";
                case SplitInfo.FileName:
                    return "file-name";
                case SplitInfo.FileCRC:
                    return "file-crc";
                case SplitInfo.DateCreated:
                    return "cr-date";
                case SplitInfo.FilesCount:
                    return "files-count";
                case SplitInfo.TransUnitIndex:
                    return "index";
                case SplitInfo.File:
                    return "file";
                case SplitInfo.TransUnits:
                    return "trans-units";
                case SplitInfo.Words:
                    return "words";
                case SplitInfo.FileNameAttr:
                    return "name";
                default:
                    return "";
            }
        }
    }
}
