using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.Comparison
{
    public class TextComparer
    {


        public enum ComparisonType
        {
            Words = 0,
            Characters
        }
        public ComparisonType Type { get; set; }

        public List<ComparisonUnit> GetComparisonTextUnits(List<ContentSection> targetOriginal, List<ContentSection> targetUpdated, bool groupChanges)
        {
            return _getComparisonTextUnits(targetOriginal, targetUpdated, groupChanges);

        }
        private List<ComparisonUnit> _getComparisonTextUnits(IEnumerable<ContentSection> targetOriginal, IEnumerable<ContentSection> targetUpdated, bool groupChanges)
        {
            TextParser.ComparisonType = Type;


            var targetOriginal1 = new List<ContentSection>();
            foreach (var tcrs in targetOriginal)
            {
                var tcrsClone = (ContentSection)tcrs.Clone();

                if (tcrsClone.RevisionMarker != null && tcrsClone.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else
                {
                    if (tcrsClone.CntType == ContentSection.ContentType.Text)
                    {
                        if (targetOriginal1.Count > 0 && targetOriginal1[targetOriginal1.Count - 1].CntType == ContentSection.ContentType.Text)
                        {
                            targetOriginal1[targetOriginal1.Count - 1].Content += tcrsClone.Content;
                        }
                        else
                        {
                            targetOriginal1.Add(tcrsClone);
                        }
                    }
                    else
                    {
                        targetOriginal1.Add(tcrsClone);
                    }
                }

            }
            var targetContentSectionsUpdated = new List<ContentSection>();
            foreach (var tcrs in targetUpdated)
            {
                var tcrsClone = (ContentSection)tcrs.Clone();

                if (tcrsClone.RevisionMarker != null && tcrsClone.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else
                {
                    if (tcrsClone.CntType == ContentSection.ContentType.Text)
                    {
                        if (targetContentSectionsUpdated.Count > 0 && targetContentSectionsUpdated[targetContentSectionsUpdated.Count - 1].CntType == ContentSection.ContentType.Text)
                        {
                            targetContentSectionsUpdated[targetContentSectionsUpdated.Count - 1].Content += tcrsClone.Content;
                        }
                        else
                        {
                            targetContentSectionsUpdated.Add(tcrsClone);
                        }
                    }
                    else
                    {
                        targetContentSectionsUpdated.Add(tcrsClone);
                    }
                }
            }



            var merger = new Merger(targetOriginal1, targetContentSectionsUpdated);
            var comparisonTextUnits = merger.Merge();

            foreach (var t in comparisonTextUnits)
            {
                t.Section = GetSdlXliffCompareMarkupTagSections(t.Text);
                t.Text = remove_sdlXliffCompareMarkupTag(t.Text);
            }
            if (!groupChanges) return comparisonTextUnits;
            var comparisonTextUnitsIdentical = new List<ComparisonUnit>();
            var comparisonTextUnitsRemoved = new List<ComparisonUnit>();
            var comparisonTextUnitsNew = new List<ComparisonUnit>();

            var comparisonTextUnitsOut = new List<ComparisonUnit>();

            var previousType = ComparisonUnit.ComparisonType.None;



            for (var i = 0; i < comparisonTextUnits.Count; i++)
            {
                switch (comparisonTextUnits[i].Type)
                {
                    case ComparisonUnit.ComparisonType.Identical:
                        {
                            if (previousType == ComparisonUnit.ComparisonType.Identical)
                            {

                                if (comparisonTextUnits[i].Text.Trim() == string.Empty && i + 1 < comparisonTextUnits.Count)
                                {
                                    if (comparisonTextUnits[i + 1].Type == ComparisonUnit.ComparisonType.Identical)
                                    {
                                        comparisonTextUnitsIdentical.Add((ComparisonUnit)comparisonTextUnits[i].Clone());

                                        previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.Identical;
                                    }
                                    else
                                    {
                                        comparisonTextUnitsNew.Add((ComparisonUnit)comparisonTextUnits[i].Clone());
                                        comparisonTextUnitsNew[comparisonTextUnitsNew.Count - 1].Type = ComparisonUnit.ComparisonType.New;


                                        comparisonTextUnitsRemoved.Add((ComparisonUnit)comparisonTextUnits[i].Clone());
                                        comparisonTextUnitsRemoved[comparisonTextUnitsRemoved.Count - 1].Type = ComparisonUnit.ComparisonType.Removed;

                                        previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.None;
                                    }
                                }
                                else
                                {
                                    comparisonTextUnitsIdentical.Add((ComparisonUnit)comparisonTextUnits[i].Clone());

                                    previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.Identical;
                                }



                                var merge = comparisonTextUnitsIdentical.Any(t => t.Text.Trim() != string.Empty);
                                if (merge)
                                {

                                    comparisonTextUnitsOut.AddRange(comparisonTextUnitsRemoved);
                                    comparisonTextUnitsOut.AddRange(comparisonTextUnitsNew);
                                    comparisonTextUnitsOut.AddRange(comparisonTextUnitsIdentical);


                                    comparisonTextUnitsIdentical = new List<ComparisonUnit>();
                                    comparisonTextUnitsRemoved = new List<ComparisonUnit>();
                                    comparisonTextUnitsNew = new List<ComparisonUnit>();
                                }

                            }
                            else
                            {

                                if (comparisonTextUnits[i].Text.Trim() == string.Empty)
                                {
                                    comparisonTextUnitsNew.Add((ComparisonUnit)comparisonTextUnits[i].Clone());
                                    comparisonTextUnitsNew[comparisonTextUnitsNew.Count - 1].Type = ComparisonUnit.ComparisonType.New;

                                    comparisonTextUnitsRemoved.Add((ComparisonUnit)comparisonTextUnits[i].Clone());
                                    comparisonTextUnitsRemoved[comparisonTextUnitsRemoved.Count - 1].Type = ComparisonUnit.ComparisonType.Removed;

                                    previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.None;
                                }
                                else
                                {
                                    comparisonTextUnitsIdentical.Add((ComparisonUnit)comparisonTextUnits[i].Clone());

                                    previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.Identical;
                                }


                                var merge = comparisonTextUnitsIdentical.Any(t => t.Text.Trim() != string.Empty);

                                if (merge)
                                {
                                    comparisonTextUnitsOut.AddRange(comparisonTextUnitsRemoved);
                                    comparisonTextUnitsOut.AddRange(comparisonTextUnitsNew);
                                    comparisonTextUnitsOut.AddRange(comparisonTextUnitsIdentical);

                                    comparisonTextUnitsIdentical = new List<ComparisonUnit>();
                                    comparisonTextUnitsRemoved = new List<ComparisonUnit>();
                                    comparisonTextUnitsNew = new List<ComparisonUnit>();
                                }

                            }
                        } break;
                    case ComparisonUnit.ComparisonType.Removed:
                        {
                          
                            comparisonTextUnitsRemoved.Add((ComparisonUnit)comparisonTextUnits[i].Clone());
                            previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.Removed;


                        } break;
                    case ComparisonUnit.ComparisonType.New:
                        {

                            comparisonTextUnitsNew.Add((ComparisonUnit)comparisonTextUnits[i].Clone());
                            previousType = comparisonTextUnits[i].Type = ComparisonUnit.ComparisonType.New;
  

                        } break;
                    case ComparisonUnit.ComparisonType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



            }

            comparisonTextUnitsOut.AddRange(comparisonTextUnitsRemoved);
            comparisonTextUnitsOut.AddRange(comparisonTextUnitsNew);
            comparisonTextUnitsOut.AddRange(comparisonTextUnitsIdentical);

            comparisonTextUnits = comparisonTextUnitsOut;


            return comparisonTextUnits;

        }



        #region   |  Segment tag markup  |

        private const string MarkupTag = "sdlXliffCompareMarkupTag";
        private readonly Regex _regexSdlXliffCompareMarkupTag = new Regex(@"\<" + MarkupTag + @"\>(?<xTag>.*?)\<\/" + MarkupTag + @"\>"
            , RegexOptions.IgnoreCase | RegexOptions.Singleline);


        private List<ContentSection> GetSdlXliffCompareMarkupTagSections(string str)
        {
            var segmentSectionsMarkUp = new List<ContentSection>();

            var mcRegexSdlXliffCompareMarkupTag = _regexSdlXliffCompareMarkupTag.Matches(str);


            var previousStart = 0;
            foreach (Match mRegexSdlXliffCompareMarkupTag in mcRegexSdlXliffCompareMarkupTag)
            {
                if (mRegexSdlXliffCompareMarkupTag.Index > previousStart)
                {
                    var startText = str.Substring(previousStart, mRegexSdlXliffCompareMarkupTag.Index - previousStart);
                    if (startText.Length > 0)
                        segmentSectionsMarkUp.Add(new ContentSection(ContentSection.LanguageType.TargetUpdated, ContentSection.ContentType.Text, string.Empty, startText));
                }


                var tagText = mRegexSdlXliffCompareMarkupTag.Groups["xTag"].Value;
                if (tagText.Length > 0)
                {
                    ContentSection.ContentType ct;

                    var tagTextTest = tagText.Replace(" ", "");
                    if (tagTextTest.StartsWith("</"))
                    {
                        ct = ContentSection.ContentType.End;
                    }
                    else if (tagTextTest.EndsWith("/>"))
                    {
                        ct = ContentSection.ContentType.Standalone;
                    }
                    else if (tagTextTest.StartsWith("<"))
                    {
                        ct = ContentSection.ContentType.Start;
                    }
                    else
                    {
                        ct = ContentSection.ContentType.Standalone;
                    }
                    segmentSectionsMarkUp.Add(new ContentSection(ContentSection.LanguageType.TargetUpdated, ct, string.Empty, tagText));
                }


                previousStart = mRegexSdlXliffCompareMarkupTag.Index + mRegexSdlXliffCompareMarkupTag.Length;

            }

            var endText = str.Substring(previousStart);
            if (endText.Length > 0)
                segmentSectionsMarkUp.Add(new ContentSection(ContentSection.LanguageType.TargetUpdated, ContentSection.ContentType.Text, string.Empty, endText));


            return segmentSectionsMarkUp;
        }
        private string remove_sdlXliffCompareMarkupTag(string str)
        {
            return _regexSdlXliffCompareMarkupTag.Replace(str, "${xTag}");
        }

        #endregion

        #region  |  Data types  |

        private enum SequenceStatus
        {
            Deleted = 0,
            Inserted,
            NoChange
        }

        private class Sequence
        {

            internal Sequence()
            {
            }

            internal Sequence(int startIndex, int endIndex)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
            }

            internal int StartIndex;
            internal int EndIndex;
        }

        private class MiddleSnake
        {
            public MiddleSnake()
            {
                Source = new Sequence();
                Destination = new Sequence();
            }

            internal readonly Sequence Source;


            internal readonly Sequence Destination;


            internal int SesLength;
        }

        private class IntVector
        {
            private readonly int[] _data;
            private readonly int _n;

            public IntVector(int n)
            {
                _data = new int[2 * n];
                _n = n;
            }

            public int this[int index]
            {
                get { return _data[_n + index]; }
                set
                {
                    _data[_n + index] = value;
                }
            }
        }


        #endregion

        #region  |  Word and Words Collection  |


        public class Word : IComparable
        {
            private string _word;
            private string _prefix;
            private string _suffix;


            internal Word()
            {
                _word = string.Empty;
                _prefix = string.Empty;
                _suffix = string.Empty;
            }


            internal Word(string word, string prefix, string suffix)
            {
                _word = word;
                _prefix = prefix;
                _suffix = suffix;
            }


            internal string word
            {
                get { return _word; }
                set { _word = value; }
            }


            internal string Prefix
            {
                get { return _prefix; }
                set { _prefix = value; }
            }


            internal string Suffix
            {
                get { return _suffix; }
                set { _suffix = value; }
            }


            internal string Reconstruct()
            {
                return _prefix + _word + _suffix;
            }


            internal string Reconstruct(string beginTag, string endTag)
            {
                return _prefix + beginTag + _word + endTag + _suffix;
            }

            #region IComparable Members


            public int CompareTo(object obj)
            {
                var word1 = obj as Word;
                if (word1 != null)
                    return string.Compare(_word, word1.word, StringComparison.Ordinal);
                throw new ArgumentException("The obj is not a Word", obj.ToString());
            }

            #endregion
        }



        internal class WordsCollection : CollectionBase
        {

            internal WordsCollection()
            {
            }


            internal WordsCollection(IEnumerable list)
            {
                foreach (var item in list)
                {
                    if (item is Word)
                        List.Add(item);
                }
            }


            internal int Add(Word item)
            {
                return List.Add(item);
            }


            internal void Insert(int index, Word item)
            {
                List.Insert(index, item);
            }


            internal void Remove(Word item)
            {
                List.Remove(item);
            }


            internal bool Contains(Word item)
            {
                return List.Contains(item);
            }


            internal int IndexOf(Word item)
            {
                return List.IndexOf(item);
            }


            internal Word this[int index]
            {
                get { return (Word)List[index]; }
                set { List[index] = value; }
            }


            internal void CopyTo(WordsCollection col, int index)
            {
                for (var i = index; i < List.Count; i++)
                {
                    col.Add(this[i]);
                }
            }


            internal void CopyTo(WordsCollection col)
            {
                CopyTo(col, 0);
            }
        }
        #endregion

        #region  |  Text Paser  |


        internal class TextParser
        {
            internal static ComparisonType ComparisonType { get; set; }

            internal static WordsCollection Parse(List<ContentSection> xSegmentSections)
            {
                var prefix = string.Empty;
                var words = new WordsCollection();

                foreach (var xSegmentSection in xSegmentSections)
                {
                    string suffix;
                    if (xSegmentSection.CntType != ContentSection.ContentType.Text)
                    {
                        prefix = string.Empty;
                        suffix = string.Empty;
                        words.Add(new Word("<" + MarkupTag + ">" + xSegmentSection.Content + "</" + MarkupTag + ">", prefix, suffix));
                        prefix = string.Empty;
                    }
                    else
                    {
                        var curPos = 0;
                        while (curPos < xSegmentSection.Content.Length)
                        {
                            var prevPos = curPos;
                            while (curPos < xSegmentSection.Content.Length &&
                               (char.IsControl(xSegmentSection.Content[curPos])
                               || char.IsWhiteSpace(xSegmentSection.Content[curPos])))
                            {
                                curPos++;
                            }
                            prefix += xSegmentSection.Content.Substring(prevPos, curPos - prevPos);

                            if (curPos == xSegmentSection.Content.Length)
                            {

                                if (prefix != string.Empty)
                                {
                                    words.Add(new Word(string.Empty, prefix, string.Empty));
                                }
                                break;
                            }

                            prevPos = curPos;
                            while (curPos < xSegmentSection.Content.Length &&
                                !char.IsControl(xSegmentSection.Content[curPos]) &&
                                !char.IsWhiteSpace(xSegmentSection.Content[curPos]))
                            {
                                curPos++;
                            }
                            var word = xSegmentSection.Content.Substring(prevPos, curPos - prevPos);


                            prevPos = curPos;
                            while (curPos < xSegmentSection.Content.Length &&
                                (char.IsControl(xSegmentSection.Content[curPos]) ||
                                char.IsWhiteSpace(xSegmentSection.Content[curPos])))
                            {
                                curPos++;
                            }
                            suffix = xSegmentSection.Content.Substring(prevPos, curPos - prevPos);
                            ProcessWord(words, prefix, word, suffix);
                            prefix = string.Empty;
                        }
                    }
                }
                return words;
            }

            private static void ProcessWord(WordsCollection words, string prefix, string word, string suffix)
            {

                var length = word.Length;

                if (length == 1)
                {
                    AddWordsCollection(words, prefix, word, suffix);
                }
                else if (!char.IsLetterOrDigit(word[0]))
                {
                    AddWordsCollection(words, prefix, word[0].ToString(), string.Empty);
                    AddWordsCollection(words, string.Empty, word.Substring(1), suffix);
                }
                else if (char.IsPunctuation(word[length - 1]))
                {
                    AddWordsCollection(words, prefix, word.Substring(0, length - 1), string.Empty);
                    AddWordsCollection(words, string.Empty, word[length - 1].ToString(), suffix);
                }
                else
                {
                    AddWordsCollection(words, prefix, word, suffix);
                }
            }

            private static void AddWordsCollection(WordsCollection words, string prefix, string word, string suffix)
            {
                #region  |  prefix  |
                if (prefix != string.Empty)
                {
                    if (prefix.Trim() == string.Empty && prefix.Length > 1)
                    {
                        //check for double spaces
                        var chars = prefix.ToCharArray();
                        for (var i = 0; i < chars.Length; i++)
                        {
                            words.Add(i == 0
                                ? new Word(string.Empty, chars[i].ToString(), string.Empty)
                                : new Word(((char) 160).ToString(), string.Empty, string.Empty));
                        }
                    }
                    else
                    {
                        words.Add(new Word("", prefix, ""));
                    }
                }
                #endregion

                #region  |  word  |
                var wortTmp = string.Empty;
                foreach (var _char in word)
                {
                    if (ComparisonType == ComparisonType.Characters) //every character is a treated as a word
                    {
                        words.Add(new Word(_char.ToString(), string.Empty, string.Empty));
                    }
                    else
                    {
                        if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) //aisian characters?
                        {
                            if (wortTmp != string.Empty)
                                words.Add(new Word(wortTmp, string.Empty, string.Empty));
                            wortTmp = string.Empty;

                            words.Add(new Word(_char.ToString(), string.Empty, string.Empty));
                        }
                        else
                            wortTmp += _char.ToString();
                    }

                }
                if (wortTmp != string.Empty)
                    words.Add(new Word(wortTmp, string.Empty, string.Empty));
                #endregion

                #region  |  suffix  |

                if (suffix == string.Empty) return;
                {
                    if (suffix.Trim() == string.Empty && suffix.Length > 1)
                    {
                        //check for double spaces
                        var chars = suffix.ToCharArray();
                        for (var i = 0; i < chars.Length; i++)
                        {
                            words.Add(i == 0
                                ? new Word(string.Empty, string.Empty, chars[i].ToString())
                                : new Word(((char) 160).ToString(), string.Empty, string.Empty));
                        }
                    }
                    else
                    {
                        words.Add(new Word(string.Empty, string.Empty, suffix));
                    }
                }

                #endregion
            }
        }


        #endregion

        #region |  Merge Engine  |

        internal class Merger
        {
            private readonly WordsCollection _original;
            private readonly WordsCollection _modified;
            private readonly IntVector _fwdVector;
            private readonly IntVector _bwdVector;

            internal Merger(List<ContentSection> original, List<ContentSection> modified)
            {
                _original = TextParser.Parse(original);
                _modified = TextParser.Parse(modified);

                _fwdVector = new IntVector(_original.Count + _modified.Count + 2);
                _bwdVector = new IntVector(_original.Count + _modified.Count + 2);
            }


            internal int WordsInOriginalFile
            {
                get { return _original.Count; }
            }


            internal int WordsInModifiedFile
            {
                get { return _modified.Count; }
            }


            private MiddleSnake FindMiddleSnake(Sequence src, Sequence des)
            {
                int d;
                var midSnake = new MiddleSnake();

                // the range of diagonal values
                var minDiag = src.StartIndex - des.EndIndex;
                var maxDiag = src.EndIndex - des.StartIndex;

                // middle point of forward searching
                var fwdMid = src.StartIndex - des.StartIndex;
                // middle point of backward searching
                var bwdMid = src.EndIndex - des.EndIndex;

                // forward seaching range 
                var fwdMin = fwdMid;
                var fwdMax = fwdMid;

                // backward seaching range 
                var bwdMin = bwdMid;
                var bwdMax = bwdMid;

                var odd = ((fwdMin - bwdMid) & 1) == 1;

                _fwdVector[fwdMid] = src.StartIndex;
                _bwdVector[bwdMid] = src.EndIndex;


                for (d = 1; ; d++)
                {
                    // extend or shrink the search range
                    if (fwdMin > minDiag)
                        _fwdVector[--fwdMin - 1] = -1;
                    else
                        ++fwdMin;

                    if (fwdMax < maxDiag)
                        _fwdVector[++fwdMax + 1] = -1;
                    else
                        --fwdMax;

                    // top-down search
                    int k;
                    int x;
                    int y;
                    for (k = fwdMax; k >= fwdMin; k -= 2)
                    {
                        if (_fwdVector[k - 1] < _fwdVector[k + 1])
                        {
                            x = _fwdVector[k + 1];
                        }
                        else
                        {
                            x = _fwdVector[k - 1] + 1;
                        }
                        y = x - k;
                        midSnake.Source.StartIndex = x;
                        midSnake.Destination.StartIndex = y;

                        while (x < src.EndIndex &&
                            y < des.EndIndex &&
                            _original[x].CompareTo(_modified[y]) == 0)
                        {
                            x++;
                            y++;
                        }

                        // update forward vector
                        _fwdVector[k] = x;

                        if (!odd || k < bwdMin || k > bwdMax || x < _bwdVector[k]) continue;
                        // this is the snake we are looking for
                        // and set the end indeses of the snake 
                        midSnake.Source.EndIndex = x;
                        midSnake.Destination.EndIndex = y;
                        midSnake.SesLength = 2 * d - 1;

                        return midSnake;
                    }

                    // extend the search range
                    if (bwdMin > minDiag)
                        _bwdVector[--bwdMin - 1] = int.MaxValue;
                    else
                        ++bwdMin;

                    if (bwdMax < maxDiag)
                    {
                        //test the index rage before assigning it
                        var iBwdMaxTry = ++bwdMax + 1;

                        //if (bwdVector.data.Length > (bwdVector.N + iBwdMaxTry))
                        _bwdVector[iBwdMaxTry] = int.MaxValue;
                        //else
                        //    --bwdMax;
                    }
                    else
                        --bwdMax;

                    // bottom-up search
                    for (k = bwdMax; k >= bwdMin; k -= 2)
                    {
                        if (_bwdVector[k - 1] < _bwdVector[k + 1])
                        {
                            x = _bwdVector[k - 1];
                        }
                        else
                        {
                            x = _bwdVector[k + 1] - 1;
                        }
                        y = x - k;
                        midSnake.Source.EndIndex = x;
                        midSnake.Destination.EndIndex = y;

                        while (x > src.StartIndex &&
                            y > des.StartIndex &&
                            _original[x - 1].CompareTo(_modified[y - 1]) == 0)
                        {
                            x--;
                            y--;
                        }
                        // update backward Vector
                        _bwdVector[k] = x;


                        if (odd || k < fwdMin || k > fwdMax || x > _fwdVector[k]) continue;
                        // this is the snake we are looking for
                        // and set the started indexes of the snake 
                        midSnake.Source.StartIndex = x;
                        midSnake.Destination.StartIndex = y;
                        midSnake.SesLength = 2 * d;
                        return midSnake;
                    }
                }
            }


            private List<ComparisonUnit> DoMerge(Sequence src, Sequence des)
            {
                var comparisonTextUnits = new List<ComparisonUnit>();

                Sequence s;

                List<ComparisonUnit> tail = null;

                var y = des.StartIndex;

                while (src.StartIndex < src.EndIndex &&
                    des.StartIndex < des.EndIndex &&
                    _original[src.StartIndex].CompareTo(_modified[des.StartIndex]) == 0)
                {
                    src.StartIndex++;
                    des.StartIndex++;
                }

                if (des.StartIndex > y)
                {
                    s = new Sequence(y, des.StartIndex);
                    comparisonTextUnits.AddRange(ConstructText(s, SequenceStatus.NoChange));
                }

                y = des.EndIndex;

                while (src.StartIndex < src.EndIndex &&
                    des.StartIndex < des.EndIndex &&
                    _original[src.EndIndex - 1].CompareTo(_modified[des.EndIndex - 1]) == 0)
                {
                    src.EndIndex--;
                    des.EndIndex--;
                }

                if (des.EndIndex < y)
                {
                    s = new Sequence(des.EndIndex, y);
                    tail = ConstructText(s, SequenceStatus.NoChange);
                }

                var n = src.EndIndex - src.StartIndex;
                var m = des.EndIndex - des.StartIndex;

                if (n < 1 && m < 1)
                {
                    if (tail != null && tail.Count > 0)
                        comparisonTextUnits.AddRange(tail);
                    return comparisonTextUnits;
                }
                if (n < 1)
                {
                    comparisonTextUnits.AddRange(ConstructText(des, SequenceStatus.Inserted));
                    if (tail != null && tail.Count > 0)
                        comparisonTextUnits.AddRange(tail);
                    return comparisonTextUnits;
                }
                if (m < 1)
                {
                    comparisonTextUnits.AddRange(ConstructText(src, SequenceStatus.Deleted));
                    if (tail != null && tail.Count > 0)
                        comparisonTextUnits.AddRange(tail);
                    return comparisonTextUnits;
                }
                if (m == 1 && n == 1)
                {
                    comparisonTextUnits.AddRange(ConstructText(src, SequenceStatus.Deleted));
                    comparisonTextUnits.AddRange(ConstructText(des, SequenceStatus.Inserted));
                    if (tail != null && tail.Count > 0)
                        comparisonTextUnits.AddRange(tail);
                    return comparisonTextUnits;
                }
                var snake = FindMiddleSnake(src, des);

                if (snake.SesLength > 1)
                {
                    var leftSrc = new Sequence(src.StartIndex, snake.Source.StartIndex);
                    var leftDes = new Sequence(des.StartIndex, snake.Destination.StartIndex);
                    var rightSrc = new Sequence(snake.Source.EndIndex, src.EndIndex);
                    var rightDes = new Sequence(snake.Destination.EndIndex, des.EndIndex);

                    comparisonTextUnits.AddRange(DoMerge(leftSrc, leftDes));

                    if (snake.Source.StartIndex < snake.Source.EndIndex)
                    {
                        comparisonTextUnits.AddRange(ConstructText(snake.Destination, SequenceStatus.NoChange));
                    }

                    comparisonTextUnits.AddRange(DoMerge(rightSrc, rightDes));
                    if (tail != null && tail.Count > 0)
                        comparisonTextUnits.AddRange(tail);
                    return comparisonTextUnits;
                }

                // N and M can't be equal!
                if (n > m)
                {
                    if (src.StartIndex != snake.Source.StartIndex)
                    {
                        // case 1
                        var leftSrc = new Sequence(src.StartIndex, snake.Source.StartIndex);

                        comparisonTextUnits.AddRange(ConstructText(leftSrc, SequenceStatus.Deleted));
                        comparisonTextUnits.AddRange(ConstructText(snake.Destination, SequenceStatus.NoChange));

                    }
                    else
                    {
                        // case 2
                        var rightSrc = new Sequence(snake.Source.StartIndex, src.EndIndex);

                        comparisonTextUnits.AddRange(ConstructText(rightSrc, SequenceStatus.Deleted));
                        comparisonTextUnits.AddRange(ConstructText(snake.Destination, SequenceStatus.NoChange));

                    }
                }
                else
                {
                    if (des.StartIndex != snake.Destination.StartIndex)
                    {
                        // case 3
                        var upDes = new Sequence(des.StartIndex, snake.Destination.StartIndex);

                        comparisonTextUnits.AddRange(ConstructText(upDes, SequenceStatus.Inserted));
                        comparisonTextUnits.AddRange(ConstructText(snake.Destination, SequenceStatus.NoChange));

                    }
                    else
                    {
                        // case 4
                        var bottomDes = new Sequence(snake.Destination.EndIndex, des.EndIndex);

                        comparisonTextUnits.AddRange(ConstructText(bottomDes, SequenceStatus.Inserted));
                        comparisonTextUnits.AddRange(ConstructText(snake.Destination, SequenceStatus.NoChange));

                    }
                }

                if (tail != null && tail.Count > 0)
                    comparisonTextUnits.AddRange(tail);
                return comparisonTextUnits;
            }


            private List<ComparisonUnit> ConstructText(Sequence seq, SequenceStatus status)
            {

                var comparisonTextUnits = new List<ComparisonUnit>();

                switch (status)
                {
                    case SequenceStatus.Deleted:
                        for (var i = seq.StartIndex; i < seq.EndIndex; i++)
                        {
                            if (_original[i].Prefix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_original[i].Prefix, ComparisonUnit.ComparisonType.Removed));
                            if (_original[i].word != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_original[i].word, ComparisonUnit.ComparisonType.Removed));
                            if (_original[i].Suffix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_original[i].Suffix, ComparisonUnit.ComparisonType.Removed));
                        }
                        break;
                    case SequenceStatus.Inserted:
                        for (var i = seq.StartIndex; i < seq.EndIndex; i++)
                        {
                            if (_modified[i].Prefix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_modified[i].Prefix, ComparisonUnit.ComparisonType.New));
                            if (_modified[i].word != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_modified[i].word, ComparisonUnit.ComparisonType.New));
                            if (_modified[i].Suffix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_modified[i].Suffix, ComparisonUnit.ComparisonType.New));

                        }
                        break;
                    case SequenceStatus.NoChange:
                        for (var i = seq.StartIndex; i < seq.EndIndex; i++)
                        {
                            if (_modified[i].Prefix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_modified[i].Prefix, ComparisonUnit.ComparisonType.Identical));
                            if (_modified[i].word != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_modified[i].word, ComparisonUnit.ComparisonType.Identical));
                            if (_modified[i].Suffix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonUnit(_modified[i].Suffix, ComparisonUnit.ComparisonType.Identical));

                        }
                        break;                    
                }
                return comparisonTextUnits;

            }


            internal List<ComparisonUnit> Merge()
            {
                var src = new Sequence(0, _original.Count);
                var des = new Sequence(0, _modified.Count);

                return DoMerge(src, des);
            }
        }


        #endregion


        public int GetTotalCharCount(List<ContentSection> tcr, out List<string> items, bool includeTags)
        {
            var count = 0;
            items = new List<string>();
            foreach (var section in tcr)
            {
                if (section.RevisionMarker != null && section.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else
                {
                    if (section.CntType == ContentSection.ContentType.Text)
                    {
                        var strLists = GetTextSections(section.Content);

                        foreach (var part in strLists)
                        {
                            foreach (var letter in part)
                            {
                                items.Add(letter.ToString());
                                count++;
                            }
                        }
                    }
                    else if (includeTags)
                    {
                        items.Add(section.Content);
                        count++;
                    }
                }
            }

            return count;
        }

        public int DamerauLevenshteinDistance_fromObject(List<ContentSection> source, List<ContentSection> target, out int charsSource, out int charsTarget, bool includeTags)
        {
            return _DamerauLevenshteinDistance_fromObject(source, target, out charsSource, out charsTarget, includeTags);
        }
        private int _DamerauLevenshteinDistance_fromObject(List<ContentSection> source, List<ContentSection> target, out int charsSource, out int charsTarget, bool includeTags)
        {

            var sourceLen = 0;
            List<string> sourceItems;
            sourceLen = GetTotalCharCount(source, out sourceItems, includeTags);

            var targetLen = 0;
            List<string> targetItems;
            targetLen = GetTotalCharCount(target, out targetItems, includeTags);


            charsSource = sourceLen;
            charsTarget = targetLen;

            if (sourceLen == 0)
            {
                return targetLen == 0 ? 0 : targetLen;
            }
            if (targetLen == 0)
            {
                return sourceLen;
            }

            var score = new int[sourceLen + 2, targetLen + 2];

            var inf = sourceLen + targetLen;
            score[0, 0] = inf;
            for (var i = 0; i <= sourceLen; i++) { score[i + 1, 1] = i; score[i + 1, 0] = inf; }
            for (var j = 0; j <= targetLen; j++) { score[1, j + 1] = j; score[0, j + 1] = inf; }

            var sd = new SortedDictionary<string, int>();

            var fullList = new List<string>();
            fullList.AddRange(sourceItems);
            fullList.AddRange(targetItems);

            foreach (var item in fullList)
            {
                if (!sd.ContainsKey(item))
                    sd.Add(item, 0);
            }


            for (var i = 1; i <= sourceLen; i++)
            {
                var db = 0;
                for (var j = 1; j <= targetLen; j++)
                {
                    var i1 = sd[targetItems[j - 1]];
                    var j1 = db;

                    if (sourceItems[i - 1] == targetItems[j - 1])
                    {
                        score[i + 1, j + 1] = score[i, j];
                        db = j;
                    }
                    else
                    {
                        score[i + 1, j + 1] = Math.Min(score[i, j], Math.Min(score[i + 1, j], score[i, j + 1])) + 1;
                    }

                    score[i + 1, j + 1] = Math.Min(score[i + 1, j + 1], score[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[sourceItems[i - 1]] = i;
            }

            return score[sourceLen + 1, targetLen + 1];
        }
        private static IEnumerable<string> GetTextSections(string str)
        {
            var strList = new List<string>();

            var regexDoubleSpaces = new Regex(@"\s{2,}", RegexOptions.Singleline);
            var mcRegexDoubleSpaces = regexDoubleSpaces.Matches(str);


            var previousStart = 0;
            foreach (Match mRegexDoubleSpaces in mcRegexDoubleSpaces)
            {
                if (mRegexDoubleSpaces.Index > previousStart)
                {
                    var startText = str.Substring(previousStart, mRegexDoubleSpaces.Index - previousStart);
                    if (startText.Length > 0)
                        strList.Add(startText);
                }


                var tagText = mRegexDoubleSpaces.Value.Replace(" ", ((char)160).ToString());
                if (tagText.Length > 0)
                    strList.Add(tagText);


                previousStart = mRegexDoubleSpaces.Index + mRegexDoubleSpaces.Length;

            }

            var endText = str.Substring(previousStart);
            if (endText.Length > 0)
                strList.Add(endText);


            return strList;
        }
    }
}
