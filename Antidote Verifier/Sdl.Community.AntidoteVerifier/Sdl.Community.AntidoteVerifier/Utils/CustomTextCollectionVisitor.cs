using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Sdl.Community.AntidoteVerifier
{
    public class CustomTextCollectionVisitor : IMarkupDataVisitor
    {
        string _collectedText;//text of the segment
        bool _inLockedContent;//the markup visited is locked
        int _startOfRange;//start of the range 
        int _endOfRange;//end of the range
        int _startOffsetOfFirstElemInRange;//offset of the start of the range in the first element in the range
        int _endOffsetOfLastElemInRange;//offset of the end of the range in the last element in the range
        Collection<IText> _markupsListInRange;//list of markups in the range 
        Collection<IAbstractMarkupData> _markupsListVisited;//list of markups visited 
        public Collection<RangeOfCharacterInfos> _listOfLockedRanges;//List of ranges of text locked
        ISegment _segment;//the segment visited

        //Constructors
        public CustomTextCollectionVisitor(ISegment theSegment)
            : base()
        {
            _segment = theSegment;
            _collectedText = "";
            _startOffsetOfFirstElemInRange = 0;
            _endOffsetOfLastElemInRange = 0;
            _startOfRange = -1;
            _endOfRange = -1;
            _markupsListInRange = new Collection<IText>();
            _markupsListVisited = new Collection<IAbstractMarkupData>();
            _listOfLockedRanges = new Collection<RangeOfCharacterInfos>();
        }

        public CustomTextCollectionVisitor(ISegment theSegment, int theStart, int theEnd)
            : base()
        {
            _segment = theSegment;
            _collectedText = "";
            _startOffsetOfFirstElemInRange = 0;
            _endOffsetOfLastElemInRange = 0;
            _startOfRange = theStart;
            _endOfRange = theEnd;
            _markupsListInRange = new Collection<IText>();
            _markupsListVisited = new Collection<IAbstractMarkupData>();
            _listOfLockedRanges = new Collection<RangeOfCharacterInfos>();
        }

        //Destructor
        ~CustomTextCollectionVisitor()
        {
            _markupsListVisited.Clear();
            _listOfLockedRanges.Clear();
        }

        //The text that has been collected by the visitor.
        public string CollectedText
        {
            get { return _collectedText; }
            set { _collectedText = value; }
        }

        //check if the range ( _startOfRange, _endOfRange ) contains locked text
        public bool RangeContainsTextLocked()
        {
            bool contains = false;
            if (_endOfRange > _startOfRange)
            {
                foreach (RangeOfCharacterInfos item in _listOfLockedRanges)
                {
                    int startOfIgnoredRange = item.start;
                    int endOfIgnoredRange = item.start + item.length;
                    //a part of the range contains locked text
                    if ((_startOfRange > startOfIgnoredRange && _startOfRange < endOfIgnoredRange)//The start of the range is inside
                         || (_endOfRange > startOfIgnoredRange && _endOfRange < endOfIgnoredRange))//The end of the range is inside
                    {
                        contains = true;
                    }
                    //all the range is locked
                    else if (_startOfRange <= item.start && _endOfRange > item.start + item.length)
                    {
                        contains = true;
                    }
                }
            }
            return contains;
        }

        //Get the text of the range ( _startOfRange, _endOfRange ) 
        public string GetText()
        {
            string aString = "";
            if (_startOfRange == -1 && _endOfRange == -1)
            {
                aString = _collectedText;
            }
            else if (_endOfRange > _startOfRange && _endOfRange <= _collectedText.Length)
            {
                aString = _collectedText.Substring(_startOfRange, _endOfRange - _startOfRange);
            }
            else
            {
                //empty string
            }
            return aString;
        }

        //Replace the text of the range ( _startOfRange, _endOfRange ) with the text "replacementText"
        public void ReplaceText(string replacementText)
        {
            if (_markupsListInRange.Count == 0)
            {
                return;//nothing to do
            }

            IText firstElem = _markupsListInRange[0];
            IText lastElem = _markupsListInRange[_markupsListInRange.Count - 1];

            int _endOffsetOfFirstElemInRange = _markupsListInRange.Count == 1 ? _endOffsetOfLastElemInRange : firstElem.Properties.Text.Length;
            if (_markupsListInRange.Count > 1)
            {
                bool keepLastElement = _endOffsetOfLastElemInRange < lastElem.Properties.Text.Length;
                //delete some elements
                for (var index = 1; index < _markupsListInRange.Count; index++)
                {
                    if (index < _markupsListInRange.Count - 1)
                    {
                        _markupsListInRange[index].RemoveFromParent();
                    }
                    else
                    {
                        if (!keepLastElement)
                            _markupsListInRange[index].RemoveFromParent();
                    }
                }

                //adjust the text of the last element
                if (keepLastElement)
                {
                    var subString = new StringBuilder(lastElem.ToString());
                    subString.Remove(0, _endOffsetOfLastElemInRange);
                    lastElem.Properties.Text = subString.ToString();
                }
            }

            //adjust the text of the first element
            //Location startLocation = new Location(_segment, firstElem);
            //TextLocation startTextLocation = new TextLocation(startLocation, _startOffsetOfFirstElemInRange);
            var sb = new StringBuilder(firstElem.ToString());
            sb.Remove(_startOffsetOfFirstElemInRange, _endOffsetOfFirstElemInRange - _startOffsetOfFirstElemInRange);
            sb.Insert(_startOffsetOfFirstElemInRange, replacementText);
            firstElem.Properties.Text = sb.ToString();
        }

        //collect the text
        private void AddCollectedText(string theString)
        {
            _collectedText += theString;
        }

        //check if "theAncestor" is an ancestor of "theMarkupData"  
        private bool IsAncestorOfMarkup(IAbstractMarkupData theMarkupData, ISegment theAncestor)
        {
            bool ret = false;
            if (theMarkupData != null && theAncestor != null)
            {
                IAbstractMarkupDataContainer parent = theMarkupData.Parent;
                if (parent != null)
                {
                    if (parent.Equals(theAncestor))
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = theAncestor.Contains(theMarkupData);
                    }
                }
            }
            return ret;
        }

        //check if the markup is in the list of the markups visited
        private bool ListContainsMarkupData(IAbstractMarkupData theMarkupData)
        {
            bool contains = false;
            //Why isn't the text in a PlaceHolder in the segment?
            Location newItemLocation = IsAncestorOfMarkup(theMarkupData, _segment) ? new Location(_segment, theMarkupData) : new Location(theMarkupData.Parent, theMarkupData);

            foreach (IAbstractMarkupData item in _markupsListVisited)
            {
                if (item.Equals(theMarkupData))
                {
                    Location itemLocation = IsAncestorOfMarkup(theMarkupData, _segment) ? new Location(_segment, item) : new Location(item.Parent, item);
                    if (newItemLocation != null && itemLocation != null)
                    {
                        contains = itemLocation.Equals(newItemLocation);
                    }
                }
            }
            return contains;
        }

        //visit functions
        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            // all locked content must be counted, as it is treated
            // as a single unit.
            if (_inLockedContent)
            {
                // we are already iterating recursively through all locked content
                // must not do it again here, as that would cause nested locked
                // content to be counted multiple times.
                return;
            }
            _inLockedContent = true;
            foreach (IAbstractMarkupData item in lockedContent.Content.AllSubItems)
            {
                item.AcceptVisitor(this);
            }
            _inLockedContent = false;
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            if (tag.Properties.DisplayText.Length > 0)
            {
                RangeOfCharacterInfos item = new RangeOfCharacterInfos { start = _collectedText.Length, length = tag.Properties.DisplayText.Length };
                _listOfLockedRanges.Add(item);
                _collectedText += tag.Properties.DisplayText;
            }
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            //don't add delete revisions
            if (revisionMarker.Properties.RevisionType != RevisionType.Delete)
            {
                foreach (IAbstractMarkupData item in revisionMarker.AllSubItems)
                {
                    item.AcceptVisitor(this);
                }
            }
        }

        public void VisitSegment(ISegment segment)
        {
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            foreach (IAbstractMarkupData item in tagPair.AllSubItems)
            {
                item.AcceptVisitor(this);
            }
        }

        public void VisitText(IText text)
        {
            //Exclude text of the revision for deleted text
            if (text.Parent is IRevisionMarker)
            {
                IRevisionMarker parent = text.Parent as IRevisionMarker;
                if (parent != null && parent.Properties.RevisionType == RevisionType.Delete)
                {
                    return;
                }
            }

            if (!ListContainsMarkupData(text))
            {
                _markupsListVisited.Add(text);
                if (text.Properties.Text.Length > 0)
                {
                    //get the information about ranges of locked text
                    if (_inLockedContent)
                    {
                        RangeOfCharacterInfos item = new RangeOfCharacterInfos { start = _collectedText.Length, length = text.Properties.Text.Length };
                        _listOfLockedRanges.Add(item);
                    }

                    //Get information about the range
                    if (_startOfRange != -1 && _endOfRange != -1)
                    {
                        int startOfMarkup = _collectedText.Length;
                        int endOfMarkup = _collectedText.Length + text.Properties.Text.Length;
                        bool isMarkupInRange = false;
                        //set start of the range
                        if (_startOfRange >= startOfMarkup && _startOfRange <= endOfMarkup)
                        {
                            isMarkupInRange = !(_startOfRange != _endOfRange && //exclude not empty range
                                                                 _startOfRange == endOfMarkup);//which begin at the end of the markup data
                            if (isMarkupInRange)
                                _startOffsetOfFirstElemInRange = _startOfRange - startOfMarkup;
                        }

                        //set end of the range
                        if (_endOfRange >= startOfMarkup && _endOfRange <= endOfMarkup)
                        {
                            isMarkupInRange = !(_startOfRange != _endOfRange && //exclude not empty range
                                                                 _endOfRange == startOfMarkup);//which begin at the start of the markup data
                            if (isMarkupInRange)
                                _endOffsetOfLastElemInRange = _endOfRange - startOfMarkup;
                        }

                        //All the markup data is in the range
                        if (startOfMarkup >= _startOfRange && endOfMarkup <= _endOfRange)
                            isMarkupInRange = true;

                        if (isMarkupInRange)
                            _markupsListInRange.Add(text);
                    }

                    //Collect the text
                    _collectedText += text.Properties.Text;
                }
            }
        }
    }
}
