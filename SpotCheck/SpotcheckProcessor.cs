using System.Collections.Generic;
using System.Xml;

using Sdl.Studio.SpotCheck.SdlXliff;
using Sdl.Studio.SpotCheck.Helpers;

namespace Sdl.Studio.SpotCheck
{
    class SpotCheckProcessor
    {
        #region variables
        private string _path;
        private XmlDocument _doc;
        private XmlNamespaceManager _nsmgr;

        private SegmentSelector _selector;
        private CommentHandler _commenter;
        private LockHandler _locker;

        #endregion

        public int Segments { get; set; }
        public int Words { get; set; }

        #region public functions

        public void Open(string path)
        {
            _path = path;
            _doc = new XmlDocument();
            _doc.Load(path);
            _doc.PreserveWhitespace = true;

            _nsmgr = new XmlNamespaceManager(_doc.NameTable);
            _nsmgr.AddNamespace("o", "urn:oasis:names:tc:xliff:document:1.2");
            _nsmgr.AddNamespace("s", "http://sdl.com/FileTypes/SdlXliff/1.0");
            _nsmgr.AddNamespace("sdl", "http://sdl.com/FileTypes/SdlXliff/1.0");

            _commenter = new CommentHandler(_doc, _nsmgr);
            _locker = new LockHandler(_doc, _nsmgr, _commenter);
            _selector = new SegmentSelector(_doc, _nsmgr, _commenter, _locker);
            _selector.GetContext = true;
        }

        public void Close()
        {
            _doc.Save(_path);
        }

        public bool AddMarkers(ApplicationSettings settings)
        {
            int words;

            List<XmlNode> segments = _selector.GetAvailableSegments(settings, out words);
            Words = words;
            Segments = segments.Count;

            int requestedSegments = _selector.GetSegmentLimit(settings);
            if (settings.LimitByWords)
            {
                if (words < settings.TotalWords)
                    System.Windows.Forms.MessageBox.Show(
                        _path +
                        "\nThere are not enough segments meeting the specified word length limits.\n" +
                        string.Format("{0} words were requested, but only {1} words are available in valid segments.", 
                        settings.TotalWords, words),
                        "Not enough matches",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            else
            {
                if (segments.Count < requestedSegments)
                    System.Windows.Forms.MessageBox.Show(
                        _path +
                        "\nThere are not enough segments meeting the specified word length limits.\n" +
                        string.Format("{0} segments were requested, but only {1} segments are valid.", requestedSegments, segments.Count),
                        "Not enough matches",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }

            if (segments.Count == 0)
                return false;

            foreach (XmlNode segment in segments)
                _commenter.CreateComment(segment, "SpotCheck Segment");

            if (settings.LockContext)
            {
                foreach (XmlNode segment in segments)
                {
                    XmlNode before;
                    XmlNode after;
                    _selector.GetContextNodes(segment, out before, out after);
                    LockContext(before);
                    LockContext(after);
                }
            }

            PluginInitializer.FilesWithSpotcheckMarkers.Add(_path);

            return true;
        }

        public void RemoveMarkers(ApplicationSettings settings)
        {
            _commenter.RemoveComments();
            if (settings.LockContext)
                _locker.RemoveLocks();
            PluginInitializer.FilesWithSpotcheckMarkers.Remove(_path);
        }

        #endregion

        #region private functions

        private void LockContext(XmlNode node)
        {
            if (node != null)
            {
                if (!_commenter.ContainsComment(node))
                    _commenter.CreateComment(node, "SpotCheck Context");
                if (_locker.IsLocked(node))
                {
                    // if its not a spotchecker lock, mark, so it won't be removed during cleanup
                    string content;
                    if (!_commenter.ContainsComment(node, out content) || content != "Lock Protected")
                        _commenter.CreateComment(node, "Lock Protected");
                }
                else
                    _locker.LockSegment(node);
            }
        }

        #endregion

    }
}
