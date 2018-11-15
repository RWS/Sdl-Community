using System;
using System.IO;
using System.Xml;

namespace Sdl.Studio.SpotCheck
{
    class SpotCheckProcessor
    {
        #region variables

        private XmlNodeList _segments;
        private Random _rnd = new Random();
        private const string _marker = "~~~spotcheck~~~";
        private XmlDocument _doc;

        #endregion 

        #region add/remove markers

        public void AddMarkers(string path, int percentage)
        {
            Open(path);

            _segments = _doc.SelectNodes("//seg-source/mrk[@mtype='seg']");
            int percent = (int)((_segments.Count / 100.0) * percentage);
            if (percent < 1) percent = 1;
            for (int i = 0; i < percent; ++i)
            {
                AddMarker();
            }

            Save(path);
        }

        public void RemoveMarkers(string path)
        {
            Open(path);

            _segments = _doc.SelectNodes("//seg-source/mrk[@mtype='seg']");
            foreach (XmlNode node in _segments)
            {
                XmlNode textNode = GetTextNode(node);
                if (textNode.InnerText.StartsWith(_marker))
                    textNode.InnerText = textNode.InnerText.Substring(_marker.Length);
            }

            Save(path);
        }

        private void Open(string path)
        {
            _doc = new XmlDocument();
            using (XmlTextReader tr = new XmlTextReader(path))
            {
                tr.Namespaces = false;
                _doc.Load(tr);
            }
        }

        private void Save(string path)
        {
            using (XmlTextWriter tr = new XmlTextWriter(path, System.Text.Encoding.UTF8))
            {
                tr.Namespaces = false;
                _doc.Save(tr);
            }
        }

        private void AddMarker()
        {
            string text = "";
            XmlNode node = null;
            do
            {
                int position = _rnd.Next(_segments.Count);
                node = _segments[position];
                node = GetTextNode(node);
                text = node.InnerText;
            } while (text.StartsWith(_marker));
            node.InnerText = _marker + node.InnerText;
        }

        private XmlNode GetTextNode(XmlNode node)
        {
            // in case the segment starts with a tag, move to first text node
            foreach (XmlNode child in node.ChildNodes)
                if (child.NodeType == XmlNodeType.Text)
                    return child;
            
            return null;
        }

        #endregion

        #region verifying markers

        public bool ContainsMarkers(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    if (line.Contains(_marker))
                        return true;
            }
            return false;
        }
        #endregion
    }
}
