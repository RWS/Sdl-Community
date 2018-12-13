using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalTagPlacer
	{
		private string _returnedText;
		private readonly Segment _sourceSegment;
		private Dictionary<string, BeGlobalTag> _tagsDictionary;
		public List<TagInfo> TagsInfo { get; set; }

		public BeGlobalTagPlacer(Segment sourceSegment)
		{
			_sourceSegment = sourceSegment;
			TagsInfo = new List<TagInfo>();
			_tagsDictionary = GetSourceTagsDict();
		}

		/// <summary>
		/// Returns the source text with markup replacing the tags in the source segment
		/// </summary>
		public string PreparedSourceText { get; private set; }

		private string DecodeReturnedText(string strInput)
		{
			strInput = HttpUtility.HtmlDecode(strInput);
			return strInput;
		}

		private Dictionary<string, BeGlobalTag> GetSourceTagsDict()
		{
			_tagsDictionary = new Dictionary<string, BeGlobalTag>(); //try this
			//build dict
			for (var i = 0; i < _sourceSegment.Elements.Count; i++)
			{
				var elType = _sourceSegment.Elements[i].GetType();

				if (elType.ToString() == "Sdl.LanguagePlatform.Core.Tag") //if tag, add to dictionary
				{
					var theTag = new BeGlobalTag((Tag)_sourceSegment.Elements[i].Duplicate());
					var tagText = string.Empty;

					var tagInfo = new TagInfo
					{
						TagType = theTag.SdlTag.Type,
						Index = i,
						IsClosed = false,
						TagId = theTag.SdlTag.TagID
					};

					if (theTag.SdlTag.Type == TagType.Start)
					{
						tagText = "<tg" + tagInfo.TagId + ">";
					}
					if (theTag.SdlTag.Type == TagType.End)
					{
						tagInfo.IsClosed = true;
						tagText = "</tg" + tagInfo.TagId + ">";
					}
					if (theTag.SdlTag.Type == TagType.Standalone || theTag.SdlTag.Type == TagType.TextPlaceholder)
					{
						tagText = "<tg" + tagInfo.TagId + "/>";
					}

					PreparedSourceText += tagText;

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
					_tagsDictionary.Add(tagText, theTag); //add our new tag code to the dict with the corresponding tag
				}
				else
				{//if not a tag
					var str = HttpUtility.HtmlEncode(_sourceSegment.Elements[i].ToString()); //HtmlEncode our plain text to be better processed by google and add to string
					//_preparedSourceText += str;
					PreparedSourceText += _sourceSegment.Elements[i].ToString();
				}
			}
			TagsInfo.Clear();
			return _tagsDictionary;
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
				if (_tagsDictionary.ContainsKey(text)) //if our text in question is in the tagtext list
				{
					try
					{
						var padleft = _tagsDictionary[text].PadLeft;
						var padright = _tagsDictionary[text].PadRight;
						if (padleft.Length > 0) segment.Add(padleft); //add leading space if applicable in the source text
						segment.Add(_tagsDictionary[text].SdlTag); //add the actual tag element after casting it back to a Tag
						if (padright.Length > 0) segment.Add(padright); //add trailing space if applicable in the source text
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}
				else
				{   //if it is not in the list of tagtexts then the element is just the text
					if (text.Trim().Length <= 0) continue;
					text = text.Trim(); //trim out extra spaces, since they are dealt with by associating them with the tags
					segment.Add(text); //add to the segment
				}
			}
			return segment; //this will return a tagged segment
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
			var tagMatches = tagRgx.Matches(str);
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

			var stringSeparators = new[] {"```"};
			var strAr = str.Split(stringSeparators, StringSplitOptions.None);
			return strAr;
		}

		private string AddSeparators(string text,MatchCollection matches)
		{
			foreach (Match match in matches)
			{
				text = text.Replace(match.Value, "```" + match.Value + "```"); //puts our separator around tagtexts
			}
			return text;
		}
	}

}
