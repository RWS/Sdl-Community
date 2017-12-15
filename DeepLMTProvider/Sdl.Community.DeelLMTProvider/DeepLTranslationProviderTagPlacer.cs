using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderTagPlacer
	{
		private string _returnedText;
		private string _preparedSourceText;
		private Segment _sourceSegment;
		private Dictionary<string, DeepLTag> _tagsDictionary;
		public List<TagInfo> TagsInfo { get; set; }
		public DeepLTranslationProviderTagPlacer(Segment sourceSegment)
		{
			_sourceSegment = sourceSegment;
			TagsInfo = new List<TagInfo>();
			_tagsDictionary = GetSourceTagsDict(); 
		}

		/// <summary>
		/// Returns the source text with markup replacing the tags in the source segment
		/// </summary>
		public string PreparedSourceText => _preparedSourceText;

		private string DecodeReturnedText(string strInput)
		{
			strInput = HttpUtility.HtmlDecode(strInput);
			//HtmlDecode takes care of everything we are doing now
			//add others found in testing if necessary
			return strInput;
		}


		private Dictionary<string, DeepLTag> GetSourceTagsDict()
		{
			_tagsDictionary = new Dictionary<string, DeepLTag>(); //try this
			//build dict
			for (var i = 0; i < _sourceSegment.Elements.Count; i++)
			{
				var elType = _sourceSegment.Elements[i].GetType();

				if (elType.ToString() == "Sdl.LanguagePlatform.Core.Tag") //if tag, add to dictionary
				{
					var theTag = new DeepLTag((Tag)_sourceSegment.Elements[i].Duplicate());
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
					//if (theTag.SdlTag.Type == TagType.Standalone || theTag.SdlTag.Type == TagType.TextPlaceholder)
					//{
					//	if (tag != null)
					//	{
					//		tagText = "</tg" + tag.TagId + ">";
					//	}
					//}

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
					_tagsDictionary.Add(tagText, theTag); //add our new tag code to the dict with the corresponding tag
				}
				else
				{//if not a tag
					var str = HttpUtility.HtmlEncode(_sourceSegment.Elements[i].ToString()); //HtmlEncode our plain text to be better processed by google and add to string
					_preparedSourceText += str;
				}
			}
			TagsInfo.Clear();
			return _tagsDictionary;
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
			var str = _returnedText;
			var pattern = @"(<tg[0-9]*\>)|(<\/tg[0-9]*\>)";

			var rgx = new Regex(pattern);
			var matches = rgx.Matches(_returnedText);

			foreach (Match myMatch in matches)
			{
				str = str.Replace(myMatch.Value, "```" + myMatch.Value + "```"); //puts our separator around tagtexts
			}
			var stringSeparators = new [] { "```" }; //split at our inserted marker....is there a better way?
			var strAr = str.Split(stringSeparators, StringSplitOptions.None);
			return strAr;
		}
	}
}
