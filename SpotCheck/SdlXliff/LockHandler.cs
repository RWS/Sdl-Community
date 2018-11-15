using System.Collections.Generic;
using System.Xml;

#region comments
/*
 lock markers are stored in the TU trans-unit/sdl:seg-defs/ - walk up from the mrk seg to the TU
  
    <sdl:seg id="1" origin-system="SDL BeGlobal Enterprise" origin="mt" conf="Draft" locked="true">
        ... lots of stuff to leave untouched 
    </sdl:seg>
 
 * there is no locked=false, just remove it
 * where the id is equivalent to the segment mrk mid, like so:
    <mrk mid="2" mtype="seg">
        If the condition persists, please contact your service representative.
    </mrk>
 */
#endregion

namespace Sdl.Studio.SpotCheck.SdlXliff
{
    class LockHandler
    {
        #region variables

        private XmlDocument _doc;
        private List<XmlNode> _existingLocks;
        private XmlNamespaceManager _nsmgr;
        private CommentHandler _commenter;
        private HashSet<string> _segments;

        #endregion 

        #region public functions

        public LockHandler(XmlDocument doc, XmlNamespaceManager nsmgr, CommentHandler commenter)
        {
            _nsmgr = nsmgr;
            _doc = doc;
            _commenter = commenter;
            _existingLocks = new List<XmlNode>();
            RefreshExistingLocks();
            _segments = new HashSet<string>();
        }

        public bool IsLocked(XmlNode segment)
        {
            XmlNode defNode = GetDefForSegment(segment);
            return (defNode.Attributes.GetNamedItem("locked") != null);
        }

        public bool Is100(XmlNode segment)
        {
            bool is100;
            XmlNode defNode = GetDefForSegment(segment);
            XmlNode attribute = defNode.Attributes.GetNamedItem("percent");
            is100 = attribute != null && attribute.Value == "100";
            attribute = defNode.Attributes.GetNamedItem("text-match");
            is100 &= attribute == null || attribute.Value != "SourceAndTarget";
            return is100;
        }

        public bool IsCm(XmlNode segment)
        {
            bool isCm;
            XmlNode defNode = GetDefForSegment(segment);
            XmlNode attribute =  defNode.Attributes.GetNamedItem("percent");
            isCm = attribute != null && attribute.Value == "100";
            attribute = defNode.Attributes.GetNamedItem("text-match");
            isCm &= attribute != null && attribute.Value == "SourceAndTarget";
            return isCm;
        }

        public bool IsRepetition(XmlNode segment)
        {
            if (_segments.Contains(segment.InnerText.ToLower()))
                return true;

            _segments.Add(segment.InnerText.ToLower());
            return false;
            /* earlier attempt, already caught by 100%
            XmlNode defNode = GetDefForSegment(segment);
            XmlNode attribute = defNode.Attributes.GetNamedItem("origin");
            isRepetition = attribute != null && attribute.Value == "auto-propagated";
            return isRepetition;
            */
        }

        public void LockSegment(XmlNode segment)
        {
            XmlNode defNode = GetDefForSegment(segment);
            XmlAttribute newAttr = _doc.CreateAttribute("locked");
            newAttr.Value = "true";
            defNode.Attributes.SetNamedItem(newAttr);
            _existingLocks.Add(segment);
        }

        public void UnlockSegment(XmlNode segment)
        {
            XmlNode defNode = GetDefForSegment(segment);
            defNode.Attributes.RemoveNamedItem("locked");
            _existingLocks.Remove(segment);
        }

        public void RemoveLocks()
        {
            int count = _existingLocks.Count;
            for (int i = 0; i < count; ++i)
            {
                XmlNode segment = _existingLocks[0];
                UnlockSegment(segment);
            }
        }

        #endregion

        #region private functions

        private void RefreshExistingLocks()
        {
            _existingLocks.Clear();

            XmlNodeList allSegments = _doc.SelectNodes("/o:xliff/o:file/o:body//o:trans-unit/o:seg-source//o:mrk[@mtype='seg']", _nsmgr);

            foreach (XmlNode segment in allSegments)
            {
                if (!IsLocked(segment))
                    continue;

                string content;
                if (!_commenter.ContainsComment(segment, out content))
                    continue;

                if (content == "Lock Protected")
                    continue;

                _existingLocks.Add(segment);
            }
        }

        private XmlNode GetDefForSegment(XmlNode segment)
        {
            string id = segment.Attributes.GetNamedItem("mid").Value;
            XmlNode parentTu = segment.ParentNode;
            while (parentTu.Name != "trans-unit")
                parentTu = parentTu.ParentNode;
            XmlNode defNode = parentTu.SelectSingleNode(string.Format("sdl:seg-defs/sdl:seg[@id='{0}']", id), _nsmgr);
            return defNode;
        }
        #endregion

    }
}
