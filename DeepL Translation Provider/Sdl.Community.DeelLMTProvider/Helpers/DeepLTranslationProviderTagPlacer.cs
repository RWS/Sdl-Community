using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using NLog;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
	public class DeepLTranslationProviderTagPlacer
	{
		private readonly Logger _logger = Log.GetLogger(nameof(DeepLTranslationProviderTagPlacer));
		private string _preparedSourceText;
		private string _returnedText;
		private Segment _sourceSegment;
		private Dictionary<string, DeepLTag> _tagsDictionary;

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

		public List<TagInfo> TagsInfo { get; set; }

        /// <summary>
        /// Returns a tagged segments from a target string containing markup, where the target string represents the translation of the class instance's source segment
        /// </summary>
        /// <param name="returnedText"></param>
        /// <param name="decodeFromHtmlOrUrl"></param>
        /// <returns></returns>
        public Segment GetTaggedSegment(string returnedText)
		{
			//decode the returned text
            _returnedText = returnedText;
			//our dictionary, dict, is already built
			var segment = new Segment(); //our segment to return
			try
			{
				//tag texts and text pieces, in the order DeepL returned them
				foreach (var text in GetTargetElements())
				{
					if (_tagsDictionary.TryGetValue(text, out var tag))
						segment.Add(tag.SdlTag);
					else if (text.Length > 0) //text keeps DeepL's whitespace, including space-only pieces between tags
						segment.Add(text);
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{e.Message}\n {e.StackTrace}");
			}

			return segment; //this will return a tagged segment
		}

		private string AddSeparators(string text, MatchCollection matches)
		{
			foreach (Match match in matches)
			{
				text = text.Replace(match.Value, "```" + match.Value + "```"); //puts our separator around tagtexts
			}
			return text;
		}

		

		private Dictionary<string, DeepLTag> GetSourceTagsDict()
		{
			_tagsDictionary = new Dictionary<string, DeepLTag>();
			try
			{
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
						if (theTag.SdlTag.Type == TagType.Standalone || theTag.SdlTag.Type == TagType.TextPlaceholder || theTag.SdlTag.Type == TagType.LockedContent)
						{
							tagText = "<tg" + tagInfo.TagId + "/>";
						}

						_preparedSourceText += tagText;

						//add our new tag code to the dict with the corresponding tag if it's not already there
						if (!_tagsDictionary.ContainsKey(tagText))
						{
							_tagsDictionary.Add(tagText, theTag);
						}
					}
					else
					{//if not a tag
						var str = HttpUtility.HtmlEncode(_sourceSegment.Elements[i].ToString()); //HtmlEncode our plain text to be better processed by google and add to string
						_preparedSourceText += _sourceSegment.Elements[i].ToString();
					}
				}
				TagsInfo.Clear();
			}
			catch (Exception e)
			{
				_logger.Error($"{e.Message}\n {e.StackTrace}");
			}
			return _tagsDictionary;
		}

		/// <summary>
		/// puts returned string into an array of elements
		/// </summary>
		/// <returns></returns>
		private string[] GetTargetElements()
		{
			//first create a regex to put our array separators around the tags
			var str = _returnedText;
			const string aplhanumericPattern = @"</?([a-z]*)[0-9]*/?>";

			var alphaRgx = new Regex(aplhanumericPattern, RegexOptions.IgnoreCase);
			var alphaMatches = alphaRgx.Matches(str);
			if (alphaMatches.Count > 0)
			{
				str = AddSeparators(str, alphaMatches);
			}

			var stringSeparators = new[] { "```" };
			var strAr = str.Split(stringSeparators, StringSplitOptions.None);
			return strAr;
		}
	}
}