using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

#region comments
/*
 * Comments are stored in /xliff/doc-info/cmt-defs
    <cmt-def id="a15f0943-7272-4c96-b4e1-b0b8909b60b2">
        <Comments>
            <Comment version="1.0" date="2014-10-29T15:16:15.3366393+00:00" user="gkordmann" severity="Low">context</Comment>
        </Comments>
    </cmt-def>
 * 
 * and they are wrapped around content as markup tags:
    <mrk mtype="x-sdl-comment" sdl:cid="a15f0943-7272-4c96-b4e1-b0b8909b60b2">
        Tube Warm up konnte nicht abgeschlossen, blabla...
    </mrk>
 */

//using Sdl.FileTypeSupport.Framework.BilingualApi;

/* early experiment to do this properly, not by xml hacks, but the framework is not allowed in plugins
FileTypeSupport.Framework.Native.Comment c = new FileTypeSupport.Framework.Native.Comment("batman", "1", "magic");
FileTypeSupport.Framework.Bilingual.CommentMarker d = new FileTypeSupport.Framework.Bilingual.CommentMarker();
d.Comments.Add(c);

IEnumerator<ISegmentPair> enu = _editorController.ActiveDocument.SegmentPairs.GetEnumerator();
enu.MoveNext();
enu.Current.Target.Add(d);
 */

#endregion

namespace Sdl.Studio.SpotCheck.SdlXliff
{
    class CommentHandler
    {
        #region variables

        private XmlDocument _doc;
        private XmlNode _commentStore;
        private Dictionary<string, XmlNode> _existingComments;
        private XmlNamespaceManager _nsmgr;

        #endregion

        #region init

        public CommentHandler(XmlDocument doc, XmlNamespaceManager nsmgr)
        {
            _nsmgr = nsmgr;
            _doc = doc;

            CreateCommentStore(nsmgr);

            _existingComments = new Dictionary<string, XmlNode>();
            RefreshExistingComments();
        }

        public void RefreshExistingComments()
        {
            _existingComments.Clear();
            foreach (XmlNode cmt in _commentStore)
            {
                XmlNode comment = cmt.SelectSingleNode("s:Comments/s:Comment", _nsmgr);
                if (comment.Attributes.GetNamedItem("user").Value == "SpotCheck Plugin")
                    _existingComments.Add(cmt.Attributes.GetNamedItem("id").Value, cmt);
            }
        }

        #endregion

        #region public functions

        public static bool ContainsComments(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    if (line.Contains("user=\"SpotCheck Plugin\""))
                        return true;
            }
            return false;
        }

        public bool ContainsComment(XmlNode segment)
        {
            string dummy;
            return ContainsComment(segment, out dummy);
        }

        public bool ContainsComment(XmlNode segment, out string value)
        {
            value = null;
            XmlNode child = segment.SelectSingleNode("o:mrk[@mtype='x-sdl-comment']", _nsmgr);
            if (child == null) return false;
            string id = child.Attributes.GetNamedItem("sdl:cid").Value;

            bool result = false;
            if (_existingComments.ContainsKey(id))
            {
                result = true;
                XmlNode cmtDef = _existingComments[id];
                value = cmtDef.InnerText;
            }
            return result;
        }

        public void CreateComment(XmlNode segment, string label)
        {
            string guid = CreateCommentInList(label);

            XmlNode marker = _doc.CreateElement("mrk", _nsmgr.LookupNamespace("o"));
            AddAttribute(marker, "mtype", "x-sdl-comment");
            AddAttribute(marker, "sdl", "cid", guid);

            segment.InsertBefore(marker, segment.FirstChild);
            int count = segment.ChildNodes.Count;
            for (int i = 1; i < count; ++i)
            {
                XmlNode removed = segment.RemoveChild(segment.ChildNodes[1]);
                marker.AppendChild(removed);
            }
        }

        public void RemoveComments()
        {
            XmlNodeList markers = _doc.SelectNodes("/o:xliff/o:file/o:body//o:trans-unit//o:mrk[@mtype='x-sdl-comment']", _nsmgr);
            foreach (XmlNode marker in markers)
                RemoveComment(marker);
            _existingComments.Clear();
            RemoveCommentStore();
        }

        #endregion

        #region private functions

        private void CreateCommentStore(XmlNamespaceManager nsmgr)
        {
            _commentStore = _doc.SelectSingleNode("/o:xliff/s:doc-info/s:cmt-defs", nsmgr);
            if (_commentStore == null)
            {
                XmlNode parent = _doc.SelectSingleNode("/o:xliff/s:doc-info", nsmgr);
                if (parent == null)
                {
                    parent = _doc.SelectSingleNode("/o:xliff", nsmgr);
                    parent = parent.InsertBefore(_doc.CreateElement("doc-info", "http://sdl.com/FileTypes/SdlXliff/1.0"), _doc.SelectSingleNode("/o:xliff/o:file", nsmgr));
                }
                _commentStore = parent.AppendChild(_doc.CreateElement("cmt-defs", _nsmgr.LookupNamespace("sdl")));
            }
        }

        // can't have empty cmt-defs for some reason
        private void RemoveCommentStore()
        {
            _commentStore = _doc.SelectSingleNode("/o:xliff/s:doc-info/s:cmt-defs", _nsmgr);
            if (_commentStore != null)
            {
                XmlNodeList comments = _doc.SelectNodes("/o:xliff/s:doc-info/s:cmt-defs/s:cmt-def", _nsmgr);
                if (comments.Count > 0)
                    return;

                _doc.SelectSingleNode("/o:xliff/s:doc-info", _nsmgr).RemoveChild(_commentStore);
            }
        }

        private string CreateCommentInList(string label)
        {
            string id = Guid.NewGuid().ToString();
            XmlNode cmtDef = _doc.CreateElement("cmt-def", _nsmgr.LookupNamespace("sdl"));
            AddAttribute(cmtDef, "id", id);
            _commentStore.AppendChild(cmtDef);

            XmlNode comments = _doc.CreateElement("Comments", _nsmgr.LookupNamespace("sdl"));
            cmtDef.AppendChild(comments);

            XmlNode comment = _doc.CreateElement("Comment", _nsmgr.LookupNamespace("sdl"));
            comments.AppendChild(comment);
            AddAttribute(comment, "version", "1.0");
            AddAttribute(comment, "date", DateTime.Now.ToString("O"));
            AddAttribute(comment, "user", "SpotCheck Plugin");
            AddAttribute(comment, "severity", "Low");
            comment.InnerText = label;

            _existingComments.Add(id, cmtDef);

            return id;
        }

        private void AddAttribute(XmlNode node, string name, string val)
        {
            AddAttribute(node, null, name, val);
        }

        private void AddAttribute(XmlNode node, string prefix, string name, string val)
        {
            XmlAttribute attr;
            if (prefix == null)
                attr = _doc.CreateAttribute(name);
            else
                attr = _doc.CreateAttribute(prefix, name, _nsmgr.LookupNamespace(prefix));
            attr.Value = val;
            node.Attributes.Append(attr);
        }

        private void RemoveComment(XmlNode commentMarker)
        {
            string id = commentMarker.Attributes.GetNamedItem("sdl:cid").Value;
            if (!_existingComments.ContainsKey(id))
                return; // it's not ours, keep it

            int count = commentMarker.ChildNodes.Count;
            for (int i = 0; i < count; ++i )
            {
                XmlNode removed = commentMarker.ChildNodes[0];
                commentMarker.ParentNode.InsertBefore(removed, commentMarker);
            }
            commentMarker.ParentNode.RemoveChild(commentMarker);

            XmlNode existingStored = _commentStore.SelectSingleNode(string.Format("//sdl:cmt-def[@id='{0}']", id), _nsmgr);
            if (existingStored != null)
                _commentStore.RemoveChild(existingStored);
        }

        #endregion

    }
}
