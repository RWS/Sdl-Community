using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.Community.XliffCompare.Core.Comparer.TextComparer
{
    internal partial class TextComparer
    {

        internal List<ComparisonTextUnit> GetComparisonTextUnits(List<SDLXLIFF.SegmentSection> targetOriginal, List<SDLXLIFF.SegmentSection> targetUpdated)
        {
            var merger = new Merger(targetOriginal, targetUpdated);
            var comparisonTextUnits = merger.Merge();

            foreach (ComparisonTextUnit textUnit in comparisonTextUnits)
            {
                switch (textUnit.ComparisonTextUnitType)
                {
                    case ComparisonTextUnitType.Identical:
                        {
                            textUnit.TextSections = GetSdlXliffCompareMarkupTagSections(textUnit.Text);
                            textUnit.Text = remove_sdlXliffCompareMarkupTag(textUnit.Text);
                        } break;
                    case ComparisonTextUnitType.Removed:
                        {
                            textUnit.TextSections = GetSdlXliffCompareMarkupTagSections(textUnit.Text);
                            textUnit.Text = remove_sdlXliffCompareMarkupTag(textUnit.Text);
                        } break;
                    case ComparisonTextUnitType.New:
                        {
                            textUnit.TextSections = GetSdlXliffCompareMarkupTagSections(textUnit.Text);
                            textUnit.Text = remove_sdlXliffCompareMarkupTag(textUnit.Text);
                        } break;
                }
            }


            return comparisonTextUnits;

        }



        #region   |  Segment tag markup  |

        private const string MarkupTag = "sdlXliffCompareMarkupTag";
        private readonly Regex _regexSdlXliffCompareMarkupTag = new Regex(@"\<" + MarkupTag + @"\>(?<xTag>.*?)\<\/" + MarkupTag + @"\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private List<SDLXLIFF.SegmentSection> GetSdlXliffCompareMarkupTagSections(string str)
        {
            var segmentSectionsMarkUp = new List<SDLXLIFF.SegmentSection>();

            var mcRegexSdlXliffCompareMarkupTag = _regexSdlXliffCompareMarkupTag.Matches(str);


            var previousStart = 0;
            foreach (Match mRegexSdlXliffCompareMarkupTag in mcRegexSdlXliffCompareMarkupTag)
            {
                if (mRegexSdlXliffCompareMarkupTag.Index > previousStart)
                {
                    var startText = str.Substring(previousStart, mRegexSdlXliffCompareMarkupTag.Index - previousStart);
                    if (startText.Length > 0)
                        segmentSectionsMarkUp.Add(new SDLXLIFF.SegmentSection(true, startText));
                }


                var tagText = mRegexSdlXliffCompareMarkupTag.Groups["xTag"].Value;
                if (tagText.Length > 0)
                    segmentSectionsMarkUp.Add(new SDLXLIFF.SegmentSection(false, tagText));


                previousStart = mRegexSdlXliffCompareMarkupTag.Index + mRegexSdlXliffCompareMarkupTag.Length;

            }

            var endText = str.Substring(previousStart);
            if (endText.Length > 0)
                segmentSectionsMarkUp.Add(new SDLXLIFF.SegmentSection(true, endText));


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
            internal MiddleSnake()
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

            internal IntVector(int n)
            {
                _data = new int[2 * n];
                _n = n;
            }

            internal int this[int index]
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


        internal class Word : IComparable
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


            internal WordsCollection(ArrayList list)
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


            public bool Contains(Word item)
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


            public void CopyTo(WordsCollection col, int index)
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
            internal static WordsCollection Parse(List<SDLXLIFF.SegmentSection> xSegmentSections)
            {
                var curPos = 0;
                var prefix = string.Empty;
                var words = new WordsCollection();

                foreach (var xSegmentSection in xSegmentSections)
                {
                    string suffix;
                    if (!xSegmentSection.IsText)
                    {
                        prefix = string.Empty;
                        suffix = string.Empty;
                        words.Add(new Word("<" + MarkupTag + ">" + xSegmentSection.Content + "</" + MarkupTag + ">", prefix, suffix));
                        prefix = string.Empty;

                        curPos = 0;
                    }
                    else
                    {

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
                foreach (var _char in word.ToCharArray())
                {
                    if (Processor.Settings.CompareType == Settings.ComparisonType.Characters) //every character is a treated as a word
                    {
                        words.Add(new Word(_char.ToString(), string.Empty, string.Empty));
                    }
                    else
                    {
                        if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) //aisian characters
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
                if (suffix != string.Empty)
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

            internal Merger(List<SDLXLIFF.SegmentSection> original, List<SDLXLIFF.SegmentSection> modified)
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
                    int x;
                    int k;
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

                        if (odd && k >= bwdMin && k <= bwdMax && x >= _bwdVector[k])
                        {
                            // this is the snake we are looking for
                            // and set the end indeses of the snake 
                            midSnake.Source.EndIndex = x;
                            midSnake.Destination.EndIndex = y;
                            midSnake.SesLength = 2 * d - 1;

                            return midSnake;
                        }
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


                        if (!odd && k >= fwdMin && k <= fwdMax && x <= _fwdVector[k])
                        {
                            // this is the snake we are looking for
                            // and set the start indexes of the snake 
                            midSnake.Source.StartIndex = x;
                            midSnake.Destination.StartIndex = y;
                            midSnake.SesLength = 2 * d;
                            return midSnake;
                        }
                    }
                }
            }


            private List<ComparisonTextUnit> DoMerge(Sequence src, Sequence des)
            {
                var comparisonTextUnits = new List<ComparisonTextUnit>();

                Sequence s;

                List<ComparisonTextUnit> tail = null;

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


            private List<ComparisonTextUnit> ConstructText(Sequence seq, SequenceStatus status)
            {

                var comparisonTextUnits = new List<ComparisonTextUnit>();

                switch (status)
                {
                    case SequenceStatus.Deleted:
                        for (var i = seq.StartIndex; i < seq.EndIndex; i++)
                        {
                            if (_original[i].Prefix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_original[i].Prefix, ComparisonTextUnitType.Removed));
                            if (_original[i].word != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_original[i].word, ComparisonTextUnitType.Removed));
                            if (_original[i].Suffix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_original[i].Suffix, ComparisonTextUnitType.Removed));
                        }
                        break;
                    case SequenceStatus.Inserted:
                        for (var i = seq.StartIndex; i < seq.EndIndex; i++)
                        {
                            if (_modified[i].Prefix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_modified[i].Prefix, ComparisonTextUnitType.New));
                            if (_modified[i].word != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_modified[i].word, ComparisonTextUnitType.New));
                            if (_modified[i].Suffix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_modified[i].Suffix, ComparisonTextUnitType.New));

                        }
                        break;
                    case SequenceStatus.NoChange:
                        for (var i = seq.StartIndex; i < seq.EndIndex; i++)
                        {
                            if (_modified[i].Prefix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_modified[i].Prefix, ComparisonTextUnitType.Identical));
                            if (_modified[i].word != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_modified[i].word, ComparisonTextUnitType.Identical));
                            if (_modified[i].Suffix != string.Empty)
                                comparisonTextUnits.Add(new ComparisonTextUnit(_modified[i].Suffix, ComparisonTextUnitType.Identical));

                        }
                        break;
                }
                return comparisonTextUnits;

            }


            internal List<ComparisonTextUnit> Merge()
            {
                var src = new Sequence(0, _original.Count);
                var des = new Sequence(0, _modified.Count);

                return DoMerge(src, des);
            }
        }


        #endregion
    }
}
