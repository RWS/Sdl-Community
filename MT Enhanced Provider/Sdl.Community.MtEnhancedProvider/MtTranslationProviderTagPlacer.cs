/* Copyright 2015 Patrick Porter

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider
{
    /// <summary>
    /// Holds data on a source segment and the tags it contains, which can be used to insert the tags in the target segment
    /// </summary>
    public class MtTranslationProviderTagPlacer
    {

        private string returnedText;
        private string preparedSourceText;
        private Segment sourceSegment;
        private Dictionary<string, MtTag> dict;

        public MtTranslationProviderTagPlacer(Segment _sourceSegment)
        {
            sourceSegment = _sourceSegment;
            dict = GetSourceTagsDict(); //fills the dictionary and populates our string to send to google

        }

        /// <summary>
        /// Returns the source text with markup replacing the tags in the source segment
        /// </summary>
        public string PreparedSourceText => preparedSourceText;

        private string DecodeReturnedText(string strInput)
        {
            strInput = HttpUtility.HtmlDecode(strInput);
            //HtmlDecode takes care of everything we are doing now
            //add others found in testing if necessary
            return strInput;
        }


        private Dictionary<string, MtTag> GetSourceTagsDict()
        {
            dict = new Dictionary<string, MtTag>(); //try this
													//build dict
			var startTagIndex = 0;
            for (var i = 0; i < sourceSegment.Elements.Count; i++)
            {
                var elType = sourceSegment.Elements[i].GetType();

                if (elType.ToString() == "Sdl.LanguagePlatform.Core.Tag") //if tag, add to dictionary
                {
                    var theTag = new MtTag((Tag)sourceSegment.Elements[i].Duplicate());
					var tagText = string.Empty;
					if (theTag.SdlTag.Type == TagType.Start)
					{
						startTagIndex = i;
						tagText = "<tg" + i + ">";
					}
					if(theTag.SdlTag.Type == TagType.End)
					{
						tagText = "</tg" + startTagIndex + ">";
					}
					if(theTag.SdlTag.Type == TagType.Standalone || theTag.SdlTag.Type == TagType.TextPlaceholder)
					{
						tagText = "</tg" + i + ">";
					}
                    //var tagNumber = sourceSegment.Elements.IndexOf(theTag.SdlTag);//this is a number we will assign the tag
                    //var tagText = "<tg" + tagNumber + ">"; //create our abbreviated tag to send to google
                    preparedSourceText += tagText;




                    //now we have to figure out whether this tag is preceded and/or followed by whitespace
                    if (i > 0 && !sourceSegment.Elements[i - 1].GetType().ToString().Equals("Sdl.LanguagePlatform.Core.Tag"))
                    {
                        var prevText = sourceSegment.Elements[i - 1].ToString();
                        if (!prevText.Trim().Equals(""))//and not just whitespace
                        {
                            //get number of trailing spaces for that segment
                            var whitespace = prevText.Length - prevText.TrimEnd().Length;
                            //add that trailing space to our tag as leading space
                            theTag.PadLeft = prevText.Substring(prevText.Length - whitespace);
                        }
                    }
                    if (i < sourceSegment.Elements.Count - 1 && !sourceSegment.Elements[i + 1].GetType().ToString().Equals("Sdl.LanguagePlatform.Core.Tag"))
                    {
                        //here we don't care whether it is only whitespace
                        //get number of leading spaces for that segment
                        var nextText = sourceSegment.Elements[i + 1].ToString();
                        var whitespace = nextText.Length - nextText.TrimStart().Length;
                        //add that trailing space to our tag as leading space
                        theTag.PadRight = nextText.Substring(0, whitespace);
                    }
                    dict.Add(tagText, theTag); //add our new tag code to the dict with the corresponding tag
                }
                else
                {//if not a tag
                    var str = HttpUtility.HtmlEncode(sourceSegment.Elements[i].ToString()); //HtmlEncode our plain text to be better processed by google and add to string
                    preparedSourceText += str;
                }
            }

            return dict;
        }

        /// <summary>
        /// Returns a tagged segments from a target string containing markup, where the target string represents the translation of the class instance's source segment
        /// </summary>
        /// <param name="returnedText"></param>
        /// <returns></returns>
        public Segment GetTaggedSegment(string _returnedText)
        {
            //decode the returned text
            returnedText = DecodeReturnedText(_returnedText);
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
                        //segment.Add(" ");//add a space after each tag
                    }
                    catch
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
            segment = RemoveTrailingClosingTags(segment);

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

            var tagsCount = segment.GetTagCount();
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
            var str = returnedText;
			/*var pattern = @"\<tg[0-9]*\>";*/ //do we need to be more exlusive, i.e. only \<tg.*?\>

			var pattern = @"(<tg[0-9]*\>)|(<\/tg[0-9]*\>)";

			var rgx = new Regex(pattern);
            var matches = rgx.Matches(returnedText);

            foreach (Match myMatch in matches)
            {
                str = str.Replace(myMatch.Value, "```" + myMatch.Value + "```"); //puts our separator around tagtexts
            }

            var stringSeparators = new string[] { "```" }; //split at our inserted marker....is there a better way?
            var strAr = str.Split(stringSeparators, StringSplitOptions.None);
            return strAr;
        }
    }
}
