using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using NLog;
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
		private readonly Segment _sourceSegment;
		private Dictionary<string, MtTag> _dict;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		public readonly string SimpleTagRegex;
		public readonly string GuidTagIdRegex;
		private readonly string[] _tagsSeparators;

		public List<TagInfo> TagsInfo { get; set; }

		public MtTranslationProviderTagPlacer(Segment sourceSegment):this()
		{
			_sourceSegment = sourceSegment;
			TagsInfo = new List<TagInfo>();
			_dict = GetSourceTagsDict(); //fills the dictionary and populates our string to send to google
		}

		public MtTranslationProviderTagPlacer()
		{
			const string simpleTagId = "tg[0-9]*";
			const string guidTagId = "tg[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}";

			SimpleTagRegex = $"(<{simpleTagId}\\>)|(<\\/{simpleTagId}\\>)|(\\<{simpleTagId}/\\>)";
			GuidTagIdRegex = $"(<{guidTagId}/>)|(<\\/{guidTagId}\\>)|(<{guidTagId}>)";
			_tagsSeparators  = new[] {"```"};
		}

		/// <summary>
		/// Returns the source text with markup replacing the tags in the source segment
		/// </summary>
		public string PreparedSourceText { get; private set; }

		/// <summary>
		/// Returns a tagged segments from a target string containing markup, where the target string represents the translation of the class instance's source segment
		/// </summary>
		public Segment GetTaggedSegment(string returnedText)
		{
			try
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
					if (_dict.ContainsKey(text)) //if our text in question is in the tagtext list
					{
						var padleft = _dict[text].PadLeft;
						var padright = _dict[text].PadRight;
						if (padleft.Length > 0) segment.Add(padleft); //add leading space if applicable in the source text
						segment.Add(_dict[text].SdlTag); //add the actual tag element after casting it back to a Tag
						if (padright.Length > 0) segment.Add(padright); //add trailing space if applicable in the source text

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
				return segment;
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
				return new Segment();
			}
		}

		private string DecodeReturnedText(string strInput)
		{
			try
			{
				// HtmlDecode takes care of everything we are doing now
				strInput = HttpUtility.HtmlDecode(strInput);
				return strInput;
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
				return string.Empty;
			}
		}

		/// <summary>
		/// Get the corresponding dictionary for the source tags
		/// </summary>
		private Dictionary<string, MtTag> GetSourceTagsDict()
		{			 
			//build dict by adding the new tag which is used for translation process and the actual tag from segment that will be used to display the translation in editor
			_dict = new Dictionary<string, MtTag>();
			var languagePlatformDllName = "Sdl.LanguagePlatform.Core.Tag";
			try
			{
				for (var i = 0; i < _sourceSegment.Elements.Count; i++)
				{
					var elType = _sourceSegment.Elements[i].GetType();

					if (elType.ToString() == languagePlatformDllName) //if tag, add to dictionary
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
								tagText = $"<tg{tag.TagId}>";
							}
						}
						if (theTag.SdlTag.Type == TagType.End)
						{
							if (tag != null)
							{
								tag.IsClosed = true;
								tagText = $"</tg{tag.TagId}>";
							}
						}
						if (theTag.SdlTag.Type.Equals(TagType.Standalone)
							|| theTag.SdlTag.Type.Equals(TagType.TextPlaceholder)
							|| theTag.SdlTag.Type.Equals(TagType.LockedContent))
						{
							if (tag != null)
							{
								tagText = $"<tg{tag.TagId}/>";
							}
						}
						PreparedSourceText += tagText;
						//now we have to figure out whether this tag is preceded and/or followed by whitespace
						if (i > 0 && !_sourceSegment.Elements[i - 1].GetType().ToString().Equals(languagePlatformDllName))
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
						if (i < _sourceSegment.Elements.Count - 1 && !_sourceSegment.Elements[i + 1].GetType().ToString().Equals(languagePlatformDllName))
						{
							//here we don't care whether it is only whitespace
							//get number of leading spaces for that segment
							var nextText = _sourceSegment.Elements[i + 1].ToString();
							var whitespace = nextText.Length - nextText.TrimStart().Length;

							//add that trailing space to our tag as leading space
							theTag.PadRight = nextText.Substring(0, whitespace);
						}
						_dict.Add(tagText, theTag); //add our new tag code to the dict with the corresponding tag
					}
					else
					{
						PreparedSourceText += _sourceSegment.Elements[i].ToString();
					}
				}
				TagsInfo.Clear();
			}
			catch(Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
			}
			return _dict;
		}

		/// <summary>
		/// Select the corresponding tag
		/// </summary>
		/// <param name="tagId"></param>
		/// <returns>TagInfo</returns>
		private TagInfo GetCorrespondingTag(string tagId)
		{
			return TagsInfo.FirstOrDefault(t => t.TagId.Equals(tagId));
		}

		/// <summary>
		/// puts returned string into an array of elements
		/// </summary>
		private string[] GetTargetElements()
		{
			//first create a regex to put our array separators around the tags
			var translation = _returnedText;

			translation = MarkTags(translation,SimpleTagRegex); // simpe tags tg1
			translation = MarkTags(translation, GuidTagIdRegex); // tags with guid as id

			translation = MarkTags(translation, @"(<tgpt[0-9]*\>)|(<\/tgpt[0-9]*\>)|(\<tgpt[0-9]*/\>)"); // Alphanumeric tags
			translation = MarkTags(translation, @"(<tg[0-9,\.]*\>)|(<\/tg[0-9,\.]*\>)|(\<tg[0-9,\.]*/\>)"); // Tags with decimals
			
			var strAr = translation.Split(_tagsSeparators, StringSplitOptions.None);
			return strAr;
		}

		public string MarkTags(string translation,string pattern)
		{
			try
			{
				var tagRgx = new Regex(pattern);
				var tagMatches = tagRgx.Matches(translation);
				if (tagMatches.Count > 0)
				{
					return AddSeparators(translation, tagMatches);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
			}
			return translation;
		}

		private string AddSeparators(string text, MatchCollection matches)
		{
			foreach (Match match in matches)
			{
				text = text.Replace(match.Value, $"```{match.Value}```"); //puts our separator around tagtexts
			}
			return text;
		}
	}
}