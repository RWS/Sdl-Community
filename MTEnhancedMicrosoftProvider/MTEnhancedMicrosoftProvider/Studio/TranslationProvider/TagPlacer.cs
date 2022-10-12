using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Service;
using NLog;
using Sdl.LanguagePlatform.Core;

namespace MTEnhancedMicrosoftProvider.Studio.TranslationProvider
{
	public class TagPlacer
	{
		private string _returnedText;
		private readonly Segment _sourceSegment;
		private readonly HtmlUtil _htmlUtil;
		private Dictionary<string, MTETag> _dict;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		public readonly string SimpleTagRegex;
		public readonly string GuidTagIdRegex;
		private readonly string[] _tagsSeparators;

		public TagPlacer(Segment sourceSegment, HtmlUtil htmlUtil):this(htmlUtil)
		{
			_sourceSegment = sourceSegment;
			TagsInfo = new List<TagInfo>();
			this._dict = GetSourceTagsDict();
		}

		public TagPlacer(HtmlUtil htmlUtil)
		{
			_htmlUtil = htmlUtil;

			const string simpleTagId = "tg[0-9]*";
			const string guidTagId = "tg[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}";

			SimpleTagRegex = $"(<{simpleTagId}\\>)|(<\\/{simpleTagId}\\>)|(\\<{simpleTagId}/\\>)";
			GuidTagIdRegex = $"(<{guidTagId}/>)|(<\\/{guidTagId}\\>)|(<{guidTagId}>)";
			_tagsSeparators = new[] { "```" };
		}

		public List<TagInfo> TagsInfo { get; set; }
		public string PreparedSourceText { get; private set; }

		public Segment GetTaggedSegment(string returnedText)
		{
			try
			{
				_returnedText = _htmlUtil.HtmlDecode(returnedText);
				var segment = new Segment();
				var targetElements = GetTargetElements();//get our array of elements..it will be array of tagtexts and text in the order received from google
				for (var i = 0; i < targetElements.Length; i++)
				{
					var text = targetElements[i];
					if (!_dict.ContainsKey(text))
					{
						if (text.Trim().Length > 0)
						{
							text = text.Trim();
							segment.Add(text);
						}

						continue;
					}

					var (padLeft, padRight) = (_dict[text].PadLeft, _dict[text].PadRight);
					if (padLeft.Length > 0)
					{
						segment.Add(padLeft);
					}

					segment.Add(_dict[text].SdlTag);
					if (padRight.Length > 0)
					{
						segment.Add(padRight);
					}
				}

				return segment;
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
				return new Segment();
			}
		}

		private string[] GetTargetElements()
		{
			const string alphanumericRegex = @"(<tgpt[0-9]*\>)|(<\/tgpt[0-9]*\>)|(\<tgpt[0-9]*/\>)";
			const string decimalsRegex = @"(<tg[0-9,\.]*\>)|(<\/tg[0-9,\.]*\>)|(\<tg[0-9,\.]*/\>)";

			var translation = _returnedText;
			translation = MarkTags(translation, SimpleTagRegex);
			translation = MarkTags(translation, GuidTagIdRegex);
			translation = MarkTags(translation, alphanumericRegex);
			translation = MarkTags(translation, decimalsRegex);

			var strAr = translation.Split(_tagsSeparators, StringSplitOptions.None);
			return strAr;
		}

		public string MarkTags(string translation, string pattern)
		{
			try
			{
				var tagMatches = new Regex(pattern).Matches(translation);
				if (tagMatches.Count > 0)
				{
					return AddSeparators(translation, tagMatches);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
			}

			return translation;
		}

		private string AddSeparators(string text, MatchCollection matches)
		{
			foreach (Match match in matches)
			{
				text = text.Replace(match.Value, $"```{match.Value}```");
			}

			return text;
		}

		private Dictionary<string, MTETag> GetSourceTagsDict()
		{
			//build dict by adding the new tag which is used for translation process and the actual tag from segment that will be used to display the translation in editor
			_dict = new Dictionary<string, MTETag>();
			var languagePlatformDllName = "Sdl.LanguagePlatform.Core.Tag";
			try
			{
				for (var i = 0; i < _sourceSegment.Elements.Count; i++)
				{
					var elType = _sourceSegment.Elements[i].GetType();

					if (elType.ToString() == languagePlatformDllName) //if tag, add to dictionary
					{
						var theTag = new MTETag((Tag)_sourceSegment.Elements[i].Duplicate());
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
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
			}
			return _dict;
		}

		private TagInfo GetCorrespondingTag(string tagId)
		{
			return TagsInfo.FirstOrDefault(t => t.TagId.Equals(tagId));
		}


	}
}