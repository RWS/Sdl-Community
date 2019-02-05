using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FileTypeSupport.MXLIFF
{
    internal class MXLIFFWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private INativeOutputFileProperties nativeFileProperties;
        private XmlNamespaceManager nsmgr;
        private IPersistentFileConversionProperties originalFileProperties;
        private XmlDocument targetFile;
        private MXLIFFTextExtractor textExtractor;
        private Dictionary<string, string> users = new Dictionary<string, string>();
        private int workflowLevel = 0;

        public void Complete()
        {
        }

        public void Dispose()
        {
            // Don't need to dispose of anything
        }

        public void FileComplete()
        {
            using (XmlTextWriter wr = new XmlTextWriter(nativeFileProperties.OutputFilePath, Encoding.UTF8))
            {
                wr.Formatting = Formatting.None;
                targetFile.Save(wr);
                targetFile = null;
            }
        }

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
        {
            originalFileProperties = fileProperties;
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
            textExtractor = new MXLIFFTextExtractor();
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            string id = paragraphUnit.Properties.Contexts.Contexts[0].GetMetaData("ID");
            XmlNode xmlUnit = targetFile.SelectSingleNode("//x:trans-unit[@id='" + id + "']", nsmgr);

            CreateParagraphUnit(paragraphUnit, xmlUnit);
        }

        public void SetFileProperties(IFileProperties fileInfo)
        {
            targetFile = new XmlDocument();
            targetFile.PreserveWhitespace = false;
            targetFile.Load(originalFileProperties.OriginalFilePath);
            nsmgr = new XmlNamespaceManager(targetFile.NameTable);
            nsmgr.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");
            nsmgr.AddNamespace("m", "http://www.memsource.com/mxlf/2.0");

            var level = targetFile.DocumentElement.Attributes["m:level"];

            if (level != null)
            {
                workflowLevel = Int32.Parse(level.Value);
            }

            // Acquire users
            var memsourceUsers = targetFile.SelectNodes("//m:user", nsmgr);

            if (memsourceUsers != null)
            {
                foreach (XmlElement user in memsourceUsers)
                {
                    var id = user.Attributes["id"]?.Value;
                    var username = user.Attributes["username"]?.Value;

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(id) && users.ContainsKey(username))
                    {
                        users.Add(username, id);
                    }
                }
            }
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            nativeFileProperties = properties;
        }

        private static void UpdateConfirmedAttribute(XmlNode transUnit, ISegmentPair segmentPair)
        {
            if (segmentPair.Target != null && segmentPair.Target.Properties != null)
            {
                switch (segmentPair.Target.Properties.ConfirmationLevel)
                {
                    case ConfirmationLevel.Unspecified:
                        transUnit.Attributes["m:confirmed"].Value = "0";
                        break;

                    case ConfirmationLevel.Draft:
                        transUnit.Attributes["m:confirmed"].Value = "0";
                        break;

                    case ConfirmationLevel.Translated:
                        transUnit.Attributes["m:confirmed"].Value = "1";
                        break;

                    case ConfirmationLevel.RejectedTranslation:
                        transUnit.Attributes["m:confirmed"].Value = "0";
                        break;

                    case ConfirmationLevel.ApprovedTranslation:
                        transUnit.Attributes["m:confirmed"].Value = "1";
                        break;

                    case ConfirmationLevel.RejectedSignOff:
                        transUnit.Attributes["m:confirmed"].Value = "0";
                        break;

                    case ConfirmationLevel.ApprovedSignOff:
                        transUnit.Attributes["m:confirmed"].Value = "1";
                        break;

                    default:
                        break;
                }
            }
        }

        private void AddComments(XmlNode xmlUnit, List<IComment> comments)
        {
            var text = string.Empty;
            var comment = comments.First();

            // We concatenate all comment text if there are multiple
            foreach (var c in comments)
            {
                text += c.Text + " ";
            }

            var createdat = targetFile.CreateAttribute("created-at");
            var createdby = targetFile.CreateAttribute("created-by");
            var modifiedat = targetFile.CreateAttribute("modified-at");
            modifiedat.Value = "0";

            var modifiedby = targetFile.CreateAttribute("modified-by");

            var resolved = targetFile.CreateAttribute("resolved");
            resolved.Value = "false";

            string commentDate = this.GetCommentDate();

            // Convert DateTime to Unix timestamp in milliseconds
            long milliseconds = (long)(TimeZoneInfo.ConvertTimeToUtc(comment.Date) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            createdat.Value = milliseconds.ToString();

            // Try to find the user id by author name
            // If it fails, just leave it blank
            if (users.ContainsKey(comment.Author))
            {
                createdby.Value = users[comment.Author].ToString();
            }
            else
            {
                createdby.Value = "";
            }

            xmlUnit.Attributes.Append(createdat);
            xmlUnit.Attributes.Append(createdby);
            xmlUnit.Attributes.Append(modifiedat);
            xmlUnit.Attributes.Append(modifiedby);
            xmlUnit.Attributes.Append(resolved);

            xmlUnit.InnerText = text;
        }

        private XmlNode CreateNewNode(XmlNode parent, XmlNode transUnit)
        {
            XmlDocument segDoc = new XmlDocument();
            string nodeContent = parent.OuterXml;
            segDoc.LoadXml(nodeContent);
            XmlNode tuNode = segDoc.CreateNode(XmlNodeType.Element, "trans-unit", nsmgr.LookupNamespace("x"));

            XmlNode importNode = parent.OwnerDocument.ImportNode(tuNode, true);
            parent.InsertAfter(importNode, transUnit);

            return importNode;
        }

        private void CreateParagraphUnit(IParagraphUnit paragraphUnit, XmlNode transUnit)
        {
            if (transUnit == null)
            {
                return;
            }

            if (paragraphUnit.SegmentPairs.Count() == 1)
            {
                UpdateSegment(transUnit, paragraphUnit.SegmentPairs.First());
            }
            else if (paragraphUnit.SegmentPairs.Count() > 1)
            {
                UpdateSegment(transUnit, paragraphUnit.SegmentPairs.First());

                string id = paragraphUnit.Properties.Contexts.Contexts[0].GetMetaData("ID");

                transUnit.Attributes["id"].Value = UpdateTopId(transUnit.Attributes["id"].Value);

                var topId = transUnit.Attributes["id"].Value;
                int count = (int)char.GetNumericValue(topId.Last()) + 1;
                // Iterate all segment pairs
                foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs.Skip(1).Reverse())
                {
                    XmlNode parent = transUnit.ParentNode;

                    XmlNode newNode = CreateNewNode(parent, transUnit);
                    AddAttributes(transUnit, newNode);
                    AddElements(transUnit, newNode);

                    newNode.Attributes["id"].Value = UpdateId(topId, count);

                    UpdateSegment(newNode, segmentPair);

                    count++;
                }
            }

        }

        private string UpdateTopId(string value)
        {
            if (Regex.IsMatch(value, @".*?(:\d+?:\d+)"))
            {
                return value;
            }
            else
            {
                return value + ":0";
            }
        }

        private string UpdateId(string id, int count)
        {
            return id.Remove(id.Length - 1) + count;
        }

        private void AddElements(XmlNode transUnit, XmlNode newNode)
        {
            XmlDocument segDoc = new XmlDocument();
            string nodeContent = transUnit.OuterXml;
            segDoc.LoadXml(nodeContent);
            XmlNode source = segDoc.CreateNode(XmlNodeType.Element, "source", nsmgr.LookupNamespace("x"));
            XmlNode target = segDoc.CreateNode(XmlNodeType.Element, "target", nsmgr.LookupNamespace("x"));
            
            XmlNode sourceNode = transUnit.OwnerDocument.ImportNode(source, true);
            XmlNode targetNode = transUnit.OwnerDocument.ImportNode(target, true);

            newNode.AppendChild(sourceNode);
            newNode.AppendChild(targetNode);

            // alt-trans
            XmlNode altTrans = segDoc.CreateNode(XmlNodeType.Element, "alt-trans", nsmgr.LookupNamespace("x"));

            XmlNode altTransNode = transUnit.OwnerDocument.ImportNode(altTrans, true);
            altTransNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("origin"));
            altTransNode.Attributes["origin"].Value = "machine-trans";
            altTransNode.AppendChild(targetNode.Clone());

            newNode.AppendChild(altTransNode);

            // m:tunit-metadata
            // m:tunit-target-metadata
            XmlNode meta1 = segDoc.CreateNode(XmlNodeType.Element, "m:tunit-metadata", nsmgr.LookupNamespace("m"));
            XmlNode meta2 = segDoc.CreateNode(XmlNodeType.Element, "m:tunit-target-metadata", nsmgr.LookupNamespace("m"));

            XmlNode m1 = transUnit.OwnerDocument.ImportNode(meta1, true);
            XmlNode m2 = transUnit.OwnerDocument.ImportNode(meta2, true);

            newNode.AppendChild(m1);
            newNode.AppendChild(m2);

            // m:editing-stats
            XmlNode mstats = segDoc.CreateNode(XmlNodeType.Element, "m:editing-stats", nsmgr.LookupNamespace("m"));
            XmlNode thinkingtime = segDoc.CreateNode(XmlNodeType.Element, "m:thinking-time", nsmgr.LookupNamespace("m"));
            thinkingtime.InnerText = "0";
            XmlNode editingtime = segDoc.CreateNode(XmlNodeType.Element, "m:editing-time", nsmgr.LookupNamespace("m"));
            editingtime.InnerText = "0";

            mstats.AppendChild(thinkingtime);
            mstats.AppendChild(editingtime);

            XmlNode mstatsNode = transUnit.OwnerDocument.ImportNode(mstats, true);

            newNode.AppendChild(mstatsNode);
        }

        private void AddAttributes(XmlNode transUnit, XmlNode newNode)
        {
            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("id"));

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:para-id", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:para-id"].Value = transUnit.Attributes["m:para-id"].Value;

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("xml:space"));
            newNode.Attributes["xml:space"].Value = "preserve";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:trans-origin", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:trans-origin"].Value = "null";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:score", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:score"].Value = "0";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:gross-score", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:gross-score"].Value = "0";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:confirmed", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:confirmed"].Value = "0";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:locked", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:locked"].Value = "false";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:level-edited", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:level-edited"].Value = "true";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:created-by", nsmgr.LookupNamespace("m")));
            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:created-at", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:created-at"].Value = "0";

            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:modified-by", nsmgr.LookupNamespace("m")));
            newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:modified-at", nsmgr.LookupNamespace("m")));
            newNode.Attributes["m:modified-at"].Value = "0";
        }

        private void FillSegment(ISegmentPair segmentPair, XmlNode node, bool source)
        {
            ISegment seg;
            if (source)
            {
                seg = segmentPair.Source;
            }
            else
            {
                seg = segmentPair.Target;
            }

            node.InnerText = textExtractor.GetPlainText(seg);
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

        private void UpdateSegment(XmlNode transUnit, ISegmentPair segmentPair)
        {
            Byte matchPercent = 0;

            XmlNode source = transUnit.SelectSingleNode("x:source", nsmgr);

            if (source != null)
            {
                FillSegment(segmentPair, source, true);
            }

            if (segmentPair.Properties.TranslationOrigin != null)
            {
                matchPercent = segmentPair.Properties.TranslationOrigin.MatchPercent;
            }

            if (transUnit.SelectSingleNode("x:target", nsmgr) == null)
            {
                XmlDocument segDoc = new XmlDocument();
                string nodeContent = transUnit.OuterXml;
                segDoc.LoadXml(nodeContent);
                XmlNode trgNode = segDoc.CreateNode(XmlNodeType.Element, "target", nsmgr.LookupNamespace("x"));
                trgNode.InnerText = "";

                XmlNode importNode = transUnit.OwnerDocument.ImportNode(trgNode, true);
                transUnit.AppendChild(importNode);

                XmlNode target = transUnit.SelectSingleNode("x:target", nsmgr);

                FillSegment(segmentPair, target, false);
            }
            else
            {
                XmlNode target = transUnit.SelectSingleNode("x:target", nsmgr);
                FillSegment(segmentPair, target, false);
            }

            // Add comments (if applicable)
            var comments = textExtractor.GetSegmentComment(segmentPair.Target);
            if (comments.Count > 0 && transUnit.SelectSingleNode("m:comment", nsmgr) == null)
            {
                XmlElement commentElement = targetFile.CreateElement("m:comment", nsmgr.LookupNamespace("m"));

                var tunitMetaData = transUnit.SelectSingleNode("m:tunit-metadata", nsmgr);
                if (tunitMetaData != null)
                {
                    transUnit.InsertBefore(commentElement, tunitMetaData);
                    AddComments(transUnit.SelectSingleNode("m:comment", nsmgr), comments);
                }
            }

            // Update score value
            var dbl = matchPercent / 100.0;
            if (transUnit.Attributes["m:score"] != null && transUnit.Attributes["m:gross-score"] != null
                && transUnit.Attributes["m:trans-origin"] != null)
            {
                transUnit.Attributes["m:score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
                transUnit.Attributes["m:gross-score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                transUnit.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:score"));
                transUnit.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:gross-score"));
                transUnit.Attributes["m:score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
                transUnit.Attributes["m:gross-score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
            }

            var transOrigin = segmentPair?.Target?.Properties?.TranslationOrigin;
            if (transOrigin != null)
            {
                var memSourceTransOrigin = transUnit.Attributes["m:trans-origin"];

                if (memSourceTransOrigin != null)
                {
                    if (transOrigin.OriginType == DefaultTranslationOrigin.TranslationMemory)
                    {
                        memSourceTransOrigin.Value = DefaultTranslationOrigin.TranslationMemory;
                    }
                    else if (transOrigin.OriginType == DefaultTranslationOrigin.MachineTranslation)
                    {
                        memSourceTransOrigin.Value = DefaultTranslationOrigin.MachineTranslation;
                    }
                }
            }

            // Update m:locked
            if (transUnit.Attributes["m:locked"] != null)
            {
                var isLocked = segmentPair.Target?.Properties?.IsLocked.ToString();
                transUnit.Attributes["m:locked"].Value = isLocked != null ? isLocked.ToLower() : "false";
            }

            // Update m:confirmed
            if (transUnit.Attributes["m:confirmed"] != null)
            {
                UpdateConfirmedAttribute(transUnit, segmentPair);
            }
        }
    }
}