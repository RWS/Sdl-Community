using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;

using Sdl.Studio.SpotCheck.Helpers;

namespace Sdl.Studio.SpotCheck.SdlXliff
{
    class SegmentSelector
    {
        #region variables

        private Random _rnd;
        private XmlDocument _doc;
        private CommentHandler _commenter;
        private LockHandler _locker;
        private XmlNamespaceManager _nsmgr;
        private List<XmlNode> _segments;
        private Dictionary<XmlNode, int> _segmentMap;
        private ApplicationSettings _settings;
        public bool GetContext;
        #endregion

        #region public functions

        public SegmentSelector(XmlDocument doc, XmlNamespaceManager nsmgr, CommentHandler commenter, LockHandler locker)
        {
            _nsmgr = nsmgr;
            _doc = doc;
            _locker = locker;
            _commenter = commenter;

            _rnd = new Random();
        }

        public int GetSegmentLimit(ApplicationSettings settings)
        {
            if (settings.LimitByWords)
            {
                return _segments.Count;
            }
            else
            {
                int requiredSegments = (int)(_segments.Count / 100.0 * settings.Percentage + .5);
                if (requiredSegments == 0)
                    requiredSegments = 1;
                return requiredSegments;
            }
        }

        public List<XmlNode> GetAvailableSegments(ApplicationSettings settings, out int words)
        {
            words = 0;
            _settings = settings;

            XmlNodeList _segmentNodesList;

            _segmentNodesList = _doc.SelectNodes("/o:xliff/o:file/o:body//o:trans-unit/o:seg-source//o:mrk[@mtype='seg']", _nsmgr);
            _segments = new List<XmlNode>(_segmentNodesList.Cast<XmlNode>());
            _segmentMap = new Dictionary<XmlNode, int>(_segments.Count);

            int requiredSegments = GetSegmentLimit(settings);
            List<XmlNode> remaining = _segments.Where(n => WordCount(n.InnerText) >= settings.MinWords && WordCount(n.InnerText) <= settings.MaxWords).ToList();

            // need to preserve original positions while remaining shrinks
            Dictionary<XmlNode, int> segmentIndexMap = new Dictionary<XmlNode, int>(_segments.Count);
            for (int i = 0; i < _segments.Count; ++i)
                segmentIndexMap.Add(_segments[i], i);

            while (remaining.Count > 0 && _segmentMap.Count < requiredSegments)
            {
                int position = _rnd.Next(remaining.Count);
                XmlNode segment = remaining[position];

                position = segmentIndexMap[segment];
                if (IsValidSegment(segment))
                    _segmentMap.Add(segment, position);
                remaining.Remove(segment);
                if (_settings.LimitByWords)
                {
                    int segmentWordCount = WordCounter.Count(segment.InnerText);
                    words += segmentWordCount;
                    if (words > settings.TotalWords)
                        break;
                }
            }
            return _segmentMap.Keys.ToList<XmlNode>();
        }

        private bool IsValidSegment(XmlNode segment)
        {
            bool valid = !(_settings.SkipLocked && _locker.IsLocked(segment));
            valid &= !(_settings.SkipCm && _locker.IsCm(segment));
            valid &= !(_settings.Skip100 && _locker.Is100(segment));
            valid &= !(_settings.SkipRepetition && _locker.IsRepetition(segment));
            return valid;
        }

        public void GetContextNodes(XmlNode segment, out XmlNode before, out XmlNode after)      
        {
            before = null;
            after = null;

            int index = _segmentMap[segment];

            while (index > 0)
            {
                if (FindContext(--index, out before))
                    break;
            }

            index = _segmentMap[segment];
            while (index < _segments.Count - 1)
            {
                if (FindContext(++index, out after))
                    break;
            }
        }
        #endregion

        #region private functions
        
        private bool FindContext(int index, out XmlNode context)
        {
            bool doBreak = false;
            context = null;

            XmlNode parent = _segments[index];
            string content;
            if (!_commenter.ContainsComment(parent, out content) && !_locker.IsLocked(parent))
            {
                context = parent;
                doBreak = true; // it's a valid context node, return
            }
            if (content == "SpotCheck Context")
            {
                doBreak = true; // it already is context, don't do anything
            }

            return doBreak;
        }

        private int WordCount(string text)
        {
            string[] words = text.Split(new char[] {' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        #endregion

    }
}
