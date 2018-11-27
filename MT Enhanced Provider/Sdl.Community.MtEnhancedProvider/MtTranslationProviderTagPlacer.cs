using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider
{
    /// <summary>
    /// Holds data on a source segment and the tags it contains, which can be used to insert the tags in the target segment
    /// </summary>
    public class MtTranslationProviderTagPlacer
    {
        private string _returnedText;
        private string _preparedSourceText;
        private readonly Segment _sourceSegment;
        private Dictionary<string, MtTag> dict;

	    public List<TagInfo> TagsInfo { get; set; }
        public MtTranslationProviderTagPlacer(Segment sourceSegment)
        {
            _sourceSegment = sourceSegment;
	        TagsInfo = new List<TagInfo>();
			dict = GetSourceTagsDict(); //fills the dictionary and populates our string to send to google
        }

        /// <summary>
        /// Returns the source text with markup replacing the tags in the source segment
        /// </summary>
        public string PreparedSourceText => _preparedSourceText;

        private string DecodeReturnedText(string strInput)
        {
            strInput = HttpUtility.HtmlDecode(strInput);
            //HtmlDecode takes care of everything we are doing now
            return strInput;
        }


        private Dictionary<string, MtTag> GetSourceTagsDict()
        {
            dict = new Dictionary<string, MtTag>(); //try this
													//build dict
            for (var i = 0; i < _sourceSegment.Elements.Count; i++)
            {
                var elType = _sourceSegment.Elements[i].GetType();

                if (elType.ToString() == "Sdl.LanguagePlatform.Core.Tag") //if tag, add to dictionary
                {
                    var theTag = new MtTag((Tag)_sourceSegment.Elements[i].Duplicate());
					var tagText = string.Empty;

	                var tagInfo = new TagInfo
	                {
		                TagType = theTag.SdlTag.Type,
		                Index = i,
		                IsClosed = false,
		                TagId = theTag.SdlTag.TagID
	                };
	                if (!TagsInfo.Any(n => n.TagId.Equals(tagInfo.TagId)))
	                {
		                TagsInfo.Add(tagInfo);
					}

	                var tag = GetCorrespondingTag(theTag.SdlTag.TagID);
					if (theTag.SdlTag.Type == TagType.Start)
					{
						if (tag != null)
						{
							tagText = "<tg" + tag.TagId + ">";
						}
					}
					if(theTag.SdlTag.Type == TagType.End)
					{
						if (tag != null)
						{
							tag.IsClosed = true;
							tagText = "</tg" + tag.TagId + ">";
						}
					}
					if(theTag.SdlTag.Type == TagType.Standalone || theTag.SdlTag.Type == TagType.TextPlaceholder)
					{
						if (tag != null)
						{
							tagText = "<tg" + tag.TagId + "/>";
						}
					}
	                _preparedSourceText += tagText;
                    //now we have to figure out whether this tag is preceded and/or followed by whitespace
                    if (i > 0 && !_sourceSegment.Elements[i - 1].GetType().ToString().Equals("Sdl.LanguagePlatform.Core.Tag"))
                    {
                        var prevText = _sourceSegment.Elements[i - 1].ToString();
                        if (!prevText.Trim().Equals(""))//and not just whitespace
                        {
                            //get number of trailing spaces for that segment
                            var whitespace = prevText.Length - prevText.TrimEnd().Length;
                            //add that trailing space to our tag as leading space
                            theTag.PadLeft = prevText.Substring(prevText.Length - whitespace);
                        }
                    }
                    if (i < _sourceSegment.Elements.Count - 1 && !_sourceSegment.Elements[i + 1].GetType().ToString().Equals("Sdl.LanguagePlatform.Core.Tag"))
                    {
                        //here we don't care whether it is only whitespace
                        //get number of leading spaces for that segment
                        var nextText = _sourceSegment.Elements[i + 1].ToString();
                        var whitespace = nextText.Length - nextText.TrimStart().Length;
                        //add that trailing space to our tag as leading space
                        theTag.PadRight = nextText.Substring(0, whitespace);
                    }
                    dict.Add(tagText, theTag); //add our new tag code to the dict with the corresponding tag
                }
                else
				{
					var str = HttpUtility.HtmlEncode(_sourceSegment.Elements[i].ToString()); //HtmlEncode our plain text to be better processed by google and add to string
																							 //_preparedSourceText += str;
					_preparedSourceText += _sourceSegment.Elements[i].ToString();
				}
            }
			TagsInfo.Clear();
            return dict;
        }

	    private TagInfo GetCorrespondingTag(string tagId)
	    {
		   return TagsInfo.FirstOrDefault(t => t.TagId.Equals(tagId));

	    }
        /// <summary>
        /// Returns a tagged segments from a target string containing markup, where the target string represents the translation of the class instance's source segment
        /// </summary>
        /// <param name="returnedText"></param>
        /// <returns></returns>
        public Segment GetTaggedSegment(string returnedText)
        {
            //decode the returned text
            _returnedText = DecodeReturnedText(returnedText);

            //our dictionary, dict, is already built
            var segment = new Segment(); //our segment to return
            var targetElements = GetTargetElements();//get our array of elements..it will be array of tagtexts and text in the order received from google
            
			//build our segment looping through elements
            for (var i = 0; i < targetElements.Length; i++)
            {
                var text = targetElements[i]; //the text to be compared/added
                if (dict.ContainsKey(text)) //if our text in question is in the tagtext list
                {
                    try
                    {
                        var padleft = dict[text].PadLeft;
                        var padright = dict[text].PadRight;
                        if (padleft.Length > 0) segment.Add(padleft); //add leading space if applicable in the source text
                        segment.Add(dict[text].SdlTag); //add the actual tag element after casting it back to a Tag
                        if (padright.Length > 0) segment.Add(padright); //add trailing space if applicable in the source text
                    }
                    catch(Exception e)
                    { }
                }
                else
                {   //if it is not in the list of tagtexts then the element is just the text
                    if (text.Trim().Length > 0) //if the element is something other than whitespace, i.e. some text in addition
                    {
                        text = text.Trim(); //trim out extra spaces, since they are dealt with by associating them with the tags
                        segment.Add(text); //add to the segment
                    }
                }
            }
            //Microsoft sends back closing tags that need to be removed
         //   segment = RemoveTrailingClosingTags(segment);

            return segment; //this will return a tagged segment
        }

        /// <summary>
        /// Microsoft always adds closing tags, but we don't keep track of our tags that way..so the segments always have garbage text at the end with the closing tag markup...this method removes them
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public Segment RemoveTrailingClosingTags(Segment segment)
        {
            #region RemoveTrailingClosingTags
            var element = segment.Elements[segment.Elements.Count - 1]; //get last element
            var str = element.ToString();
            var pattern = @"\</tg[0-9]*\>"; //we want to find "</tg" + {any number} + ">"
            var rgx = new Regex(pattern);
            var elType = element.GetType();
            var matches = rgx.Matches(str);
            if (elType.ToString().Equals("Sdl.LanguagePlatform.Core.Text") && matches.Count > 0) //if a text element containing matches
            {
                foreach (Match myMatch in matches)
                {
                    str = str.Replace(myMatch.Value, ""); //puts our separator around tagtexts
                }
                segment.Elements.Remove(element);
                segment.Add(str.TrimStart());
            }
            #endregion
            return segment;
        }

        /// <summary>
        /// puts returned string into an array of elements
        /// </summary>
        /// <returns></returns>
        private string[] GetTargetElements()
        {
            //first create a regex to put our array separators around the tags
            var str = _returnedText;
	        const string tagsPattern = @"(<tg[0-9]*\>)|(<\/tg[0-9]*\>)|(\<tg[0-9]*/\>)";
	        const string aplhanumericPattern = @"(<tgpt[0-9]*\>)|(<\/tgpt[0-9]*\>)|(\<tgpt[0-9]*/\>)";

			var tagRgx = new Regex(tagsPattern);
            var tagMatches = tagRgx.Matches(_returnedText);

	        if (tagMatches.Count > 0)
	        {
		        str = AddSeparators(str, tagMatches);
	        }

	        var alphaRgx = new Regex(aplhanumericPattern);
	        var alphaMatches = alphaRgx.Matches(str);
	        if (alphaMatches.Count > 0)
	        {
		        str = AddSeparators(str, alphaMatches);

	        }

	        var stringSeparators = new[] { "```" };
	        var strAr = str.Split(stringSeparators, StringSplitOptions.None);
	        return strAr;
		}

	    private string AddSeparators(string text, MatchCollection matches)
	    {
		    foreach (Match match in matches)
		    {
			    text = text.Replace(match.Value, "```" + match.Value + "```"); //puts our separator around tagtexts
		    }
		    return text;
	    }
	}
}
