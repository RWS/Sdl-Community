using System;
using System.Collections.Generic;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileTypeSupport.TXML
{
    class TxmlWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {

        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private XmlDocument _targetFile;
        private TxmlTextExtractor _textExtractor;

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
        {
            _originalFileProperties = fileProperties;
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            _nativeFileProperties = properties;
        }


        public void SetFileProperties(IFileProperties fileInfo)
        {
            _targetFile = new XmlDocument();
            _targetFile.PreserveWhitespace = true;
            _targetFile.Load(_originalFileProperties.OriginalFilePath);
        }


        public void Initialize(IDocumentProperties documentInfo)
        {
            _textExtractor = new TxmlTextExtractor();
        }



        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            string unitId = paragraphUnit.Properties.Contexts.Contexts[1].GetMetaData("UnitID");
            XmlNode xmlUnit = _targetFile.SelectSingleNode("//translatable[@blockId='" + unitId + "']");

            CreateParagraphUnit(paragraphUnit, xmlUnit);
        }



        private void CreateParagraphUnit(IParagraphUnit paragraphUnit, XmlNode xmlUnit)
        {            
            int i = 0;            

            // iterate all segment pairs
            foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
            {
                var xmlNodeList = xmlUnit.SelectNodes("segment");
                if (xmlNodeList != null && xmlNodeList[i] != null)
                {
                    XmlNode source = xmlNodeList[i].SelectSingleNode("source");

                    if (source != null)
                    {
                        source.InnerXml = _textExtractor.GetPlainText(segmentPair.Source);
                    }

                }

                Byte matchPercent;
                if (segmentPair.Properties.TranslationOrigin != null)
                    matchPercent = segmentPair.Properties.TranslationOrigin.MatchPercent;
                else
                    matchPercent = 0;
                
                XmlNode target;
                var segmentNodes = xmlUnit.SelectNodes("segment");
                if (segmentNodes != null && segmentNodes[i]!=null)
                {
                    if (segmentNodes[i].SelectSingleNode("target") == null)
                    {
                        if (segmentNodes[i].SelectSingleNode("target") == null)
                        {
                            XmlDocument segDoc = new XmlDocument();
                            string nodeContent = segmentNodes[i].OuterXml;
                            segDoc.LoadXml(nodeContent);
                            XmlNode trgNode = segDoc.CreateNode(XmlNodeType.Element, "target", null);

                            XmlNode importNode = segmentNodes[i].OwnerDocument.ImportNode(trgNode, true);
                            segmentNodes[i].AppendChild(importNode);
                            segmentNodes[i].SelectSingleNode("target").InnerXml =
                                _textExtractor.GetPlainText(segmentPair.Target);

                        }
                    }
                    else
                    {
                        if (segmentNodes[i] != null)
                        {
                            target = segmentNodes[i].SelectSingleNode("target");
                            target.InnerXml = _textExtractor.GetPlainText(segmentPair.Target);
                        }
                    }


                    //add comments (if applicable)
                    List<string> comments = _textExtractor.GetSegmentComment(segmentPair.Target);
                    if (comments.Count > 0 && segmentNodes[i].SelectSingleNode("comments") == null)
                    {
                        XmlElement commentsElement = _targetFile.CreateElement("comments");
                        XmlElement commentElement = _targetFile.CreateElement("comments");
                        segmentNodes[i].AppendChild(commentElement);
                    }

                    foreach (string comment in _textExtractor.GetSegmentComment(segmentPair.Target))
                    {
                        AddComment(segmentNodes[i].SelectSingleNode("comments"), comment);
                    }


                    //update score value
                    if (segmentNodes[i].SelectSingleNode("target").Attributes["score"] != null)
                    {
                        segmentNodes[i].SelectSingleNode("target").Attributes["score"].Value = matchPercent.ToString();
                    }
                    else
                    {
                        segmentNodes[i].SelectSingleNode("target")
                            .Attributes.Append(xmlUnit.OwnerDocument.CreateAttribute("score"));
                        segmentNodes[i].SelectSingleNode("target").Attributes["score"].Value = matchPercent.ToString();
                    }


                    //update modified status
                    if (xmlUnit.Attributes["modified"] != null)
                    {
                        segmentNodes[i].Attributes["modified"].Value =
                            UpdateEditedStatus(segmentPair.Properties.ConfirmationLevel);
                    }
                    else
                    {
                        segmentNodes[i].Attributes.Append(xmlUnit.OwnerDocument.CreateAttribute("modified"));
                        segmentNodes[i].Attributes["modified"].Value =
                            UpdateEditedStatus(segmentPair.Properties.ConfirmationLevel);
                    }
                }
                i++;
            }
        }



        private string UpdateEditedStatus(ConfirmationLevel unitLevel)
        {
            string status;

            switch (unitLevel)
            {
                case ConfirmationLevel.Translated:
                    status = "true";
                    break;
                case ConfirmationLevel.Draft:
                    status = "true";
                    break;
                case ConfirmationLevel.Unspecified:
                    status = "false";
                    break;
                case ConfirmationLevel.ApprovedTranslation:
                    status = "true";
                    break;
                case ConfirmationLevel.ApprovedSignOff:
                    status = "true";
                    break;
                case ConfirmationLevel.RejectedSignOff:
                    status = "true";
                    break;
                case ConfirmationLevel.RejectedTranslation:
                    status = "true";
                    break;
                default:
                    status = "true";
                    break;
            }

            return status;
        }





        private void AddComment(XmlNode xmlUnit, string commentText)
        {
            string[] chunk = commentText.Split(';');

            XmlElement commentElement = _targetFile.CreateElement("comment");
            
            XmlAttribute creationid = _targetFile.CreateAttribute("creationid");
            XmlAttribute creationdate = _targetFile.CreateAttribute("creationdate");
            XmlAttribute type = _targetFile.CreateAttribute("type");

            string commentDate = this.GetCommentDate();

            creationdate.Value = commentDate;
            creationid.Value = chunk[2];

            switch (chunk[3])
            {
                case "Medium":
                    type.Value = "text";
                    break;

                case "High":
                    type.Value = "important";
                    break;

                default:
                    type.Value = "text";
                    break;
            }           

            commentElement.Attributes.Append(creationid);
            commentElement.Attributes.Append(creationdate);
            commentElement.Attributes.Append(type);

            commentElement.InnerText = chunk[0];
            xmlUnit.AppendChild(commentElement);
        }

        private string GetCommentDate()
        {
            string day;
            string month;

            if (DateTime.UtcNow.Month.ToString().Length == 1)
                month = "0" + DateTime.UtcNow.Month;
            else
                month = "0" + DateTime.UtcNow.Month;

            if (DateTime.UtcNow.Day.ToString().Length == 1)
                day = "0" + DateTime.UtcNow.Day;
            else
                day = "0" + DateTime.UtcNow.Day;

            return DateTime.UtcNow.Year + month + day + "T" +
                DateTime.UtcNow.Hour + DateTime.UtcNow.Minute +
                DateTime.UtcNow.Second + "Z";
        }



        public void FileComplete()
        {
            _targetFile.Save(_nativeFileProperties.OutputFilePath);
            _targetFile = null;
        }

        public void Complete()
        {

        }



        public void Dispose()
        {
            //don't need to dispose of anthing
        }

    }
}
