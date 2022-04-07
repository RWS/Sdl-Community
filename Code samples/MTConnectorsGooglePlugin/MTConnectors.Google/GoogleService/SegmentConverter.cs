using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    public class SegmentConverter
    {
        private string _returnedText;
        private readonly Segment _sourceSegment;
        private Dictionary<string, TagInfo> _mappedTags;


        public SegmentConverter(Segment sourceSegment)
        {
            _sourceSegment = sourceSegment ?? throw new ArgumentNullException();
        }


        private string DecodeReturnedText(string strInput)
        {
            strInput = HttpUtility.HtmlDecode(strInput);
            //HtmlDecode takes care of everything we are doing now
            return strInput;
        }


        public string ConvertSourceSegmentToText()
        {
            _mappedTags = new Dictionary<string, TagInfo>(); //try this
            var tagsInfo = new Dictionary<string, TagInfo>();
            var preparedSourceText = new StringBuilder();
            for (var i = 0; i < _sourceSegment.Elements.Count; i++)
            {
                if (_sourceSegment.Elements[i] is Tag)
                {
                    var coreTag = (Tag)_sourceSegment.Elements[i].Duplicate();
                    var workingTag = new TagInfo(coreTag);

                    var matchingTag = GetMatchingTag(tagsInfo, coreTag);

                    if (workingTag.TagType == TagType.End && matchingTag != null)
                    {
                        matchingTag.IsClosed = true;
                    }

                    var tagText = CreateTagText(workingTag, matchingTag);

                    preparedSourceText.Append(tagText);
                    RememberAdjacentWhiteSpaces(i, workingTag);
                    _mappedTags.Add(tagText, workingTag); //add our new tag code to the dict with the corresponding tag
                }
                else
                {
                    preparedSourceText.Append(_sourceSegment.Elements[i].ToString());
                }
            }

            return preparedSourceText.ToString();
        }

        private void RememberAdjacentWhiteSpaces(int i, TagInfo workingTag)
        {
            //now we have to figure out whether this tag is preceded and/or followed by whitespace
            var isPreviousElementAText = i > 0 && !(_sourceSegment.Elements[i - 1] is Tag);
            if (isPreviousElementAText)
            {
                var prevText = _sourceSegment.Elements[i - 1].ToString();
                if (!string.IsNullOrEmpty(prevText.Trim())) //and not just whitespace
                {
                    //get number of trailing spaces for that segment
                    var whitespace = prevText.Length - prevText.TrimEnd().Length;
                    //add that trailing space to our tag as leading space
                    workingTag.PadLeft = prevText.Substring(prevText.Length - whitespace);
                }
            }

            var isNextElementAText = i < _sourceSegment.Elements.Count - 1 && !(_sourceSegment.Elements[i + 1] is Tag);
            if (isNextElementAText)
            {
                //here we don't care whether it is only whitespace
                //get number of leading spaces for that segment
                var nextText = _sourceSegment.Elements[i + 1].ToString();
                var whitespace = nextText.Length - nextText.TrimStart().Length;
                //add that trailing space to our tag as leading space
                workingTag.PadRight = nextText.Substring(0, whitespace);
            }
        }

        private string CreateTagText(TagInfo workingTag, TagInfo matchingTag)
        {
            if (matchingTag == null)
            {
                return string.Empty;
            }

            var tagText = string.Empty;
            if (workingTag.TagType == TagType.Start)
            {
                tagText = matchingTag.OpeningTag;
            }

            if (workingTag.TagType == TagType.End)
            {
                tagText = matchingTag.ClosingTag;
            }

            var isStandaloneTag = workingTag.TagType == TagType.Standalone
                                  || workingTag.TagType == TagType.TextPlaceholder
                                  || workingTag.TagType == TagType.LockedContent;
            if (isStandaloneTag)

            {
                tagText = matchingTag.StandaloneTag;
            }

            return tagText;
        }

        private TagInfo GetMatchingTag(Dictionary<string, TagInfo> tagsInfo, Tag coreTag)
        {
            var key = coreTag.TagID + "-" + coreTag.Anchor;
            if (!tagsInfo.ContainsKey(key))
            {
                tagsInfo.Add(key, new TagInfo(coreTag));
            }

            return tagsInfo[key];
        }

        /// <summary>
        /// Returns a tagged segments from a target string containing markup, where the target string represents the translation of the class instance's source segment
        /// </summary>
        /// <param name="translatedText"></param>
        /// <returns></returns>
        public Segment ConvertTargetTextToSegment(string translatedText)
        {
            if (string.IsNullOrEmpty(translatedText))
            {
                return new Segment();
            }

            translatedText = DecodeReturnedText(translatedText);

            //for some reason, GT is sometimes adding zero-width spaces, aka "bom", aka char(8203)
            //so we need to remove it
            _returnedText = ScrubBomCharacters(translatedText);

            //our dictionary, dict, is already built
            var targetElements = GetTargetElements();//get our array of elements..it will be array of tagtexts and text in the order received from google

            return AddElementsToSegment(targetElements); ; //this will return a tagged segment
        }

        private Segment AddElementsToSegment(string[] targetElements)
        {
            var segment = new Segment(); //our segment to return

            foreach (var textElement in targetElements)
            {
                if (_mappedTags.ContainsKey(textElement)) //if our text in question is in the tagtext list
                {
                    PadAndAddAsSegment(textElement, segment);
                }
                else
                {
                    //if it is not in the list of tagtexts then the element is just the text
                    if (!string.IsNullOrEmpty(textElement.Trim())
                    ) //if the element is something other than whitespace, i.e. some text in addition
                    {
                        segment.Add(textElement.Trim()); //add to the segment
                    }
                }
            }

            return segment;
        }

        private void PadAndAddAsSegment(string textElement, Segment segment)
        {
            var padleft = _mappedTags[textElement].PadLeft;
            if (padleft.Length > 0)
            {
                segment.Add(padleft); //add leading space if applicable in the source text
            }

            segment.Add(_mappedTags[textElement].SdlTag); //add the actual tag element after casting it back to a Tag

            var padright = _mappedTags[textElement].PadRight;
            if (padright.Length > 0)
            {
                segment.Add(padright); //add trailing space if applicable in the source text
            }
        }


        /// <summary>
        /// puts returned string into an array of elements
        /// </summary>
        /// <returns></returns>
        private string[] GetTargetElements()
        {
            //first create a regex to put our array separators around the tags
            var str = _returnedText;
            var pattern = @"(<tg.*?\>)|(<\/tg.*?\>)|(\<tg.*?/\>)";
            var rgx = new Regex(pattern);
            var matches = rgx.Matches(_returnedText);

            foreach (Match myMatch in matches)
            {
                str = str.Replace(myMatch.Value, "```" + myMatch.Value + "```"); //puts our separator around tagtexts
            }
            var stringSeparators = new[] { "```" }; //split at our inserted marker....is there a better way?
            var strAr = str.Split(stringSeparators, StringSplitOptions.None);
            return strAr;
        }

        private string ScrubBomCharacters(string input)
        {
            //this is to deal with google putting in zero-width spaces for some reason, i.e. char(8203)
            //convert returned text to char array
            char[] chars = input.ToCharArray();
            //remove all char8203 using linq                 
            chars = chars.Where(val => val != (char)8203).ToArray();
            //convert back to a string
            return new string(chars);
        }
    }
}
