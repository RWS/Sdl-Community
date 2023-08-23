using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GoogleCloudTranslationProvider.Models;
using NLog;
using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.Helpers
{
	public class TagPlacer
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly List<TagInfo> _tagsInfo;
		private readonly Segment _sourceSegment;
		private readonly HtmlUtil _htmlUtil;

		private Dictionary<string, GoogleTag> _tagsDictionary;
		private string _returnedText;
		private GoogleTag _currentTag;

		public TagPlacer(Segment sourceSegment, HtmlUtil htmlUtil)
		{
			_sourceSegment = sourceSegment;
			_htmlUtil = htmlUtil;
			_tagsInfo = new List<TagInfo>();
			GetSourceTagsDictionary();
		}

		public string PreparedSourceText { get; private set; }

		public Segment GetTaggedSegment(string returnedText)
		{
			try
			{
				return TryGetTaggedSegment(returnedText);
			}
			catch (Exception e)
			{
				LogException(e);
			}

			return new Segment();
		}

		private Segment TryGetTaggedSegment(string returnedText)
		{
			_returnedText = _htmlUtil.HtmlDecode(returnedText);
			var segment = new Segment();
			var targetElements = GetTargetElements();
			for (var i = 0; i < targetElements.Length; i++)
			{
				var text = targetElements[i];
				if (_tagsDictionary.ContainsKey(text))
				{
					AddTagPadding(segment, text);
					continue;
				}

				if (text.Trim().Length > 0)
				{
					segment.Add(text.Trim());
				}
			}

			return segment;
		}

		private string[] GetTargetElements()
		{
			const string SimplePattern = "0tg[0-9]*";
			const string SimpleTagRegex = $@"(<{SimplePattern}\>)|(<\/{SimplePattern}\>)|(\<{SimplePattern}/\>)";

			const string GuidPattern = "0tg[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}";
			const string GuidTagIdRegex = $@"(<{GuidPattern}/>)|(<\\/{GuidPattern}\\>)|(<{GuidPattern}>)";

			const string AlphanumericPattern = "0tgpt[0-9]*";
			const string AlphanumericTagRegex = $@"(<{AlphanumericPattern}\>)|(<\/{AlphanumericPattern}\>)|(\<{AlphanumericPattern}/\>)";

			const string DecimalsPattern = @"0tg[0-9,\.]*";
			const string DecimalsTagRegex = $@"(<{DecimalsPattern}\>)|(<\/{DecimalsPattern}\>)|(\<{DecimalsPattern}/\>)";

			var translation = _returnedText;
			translation = MarkTags(translation, SimpleTagRegex);
			translation = MarkTags(translation, GuidTagIdRegex);
			translation = MarkTags(translation, AlphanumericTagRegex);
			translation = MarkTags(translation, DecimalsTagRegex);

			return translation.Split(new[] { "```" }, StringSplitOptions.None);
		}

		private string MarkTags(string translation, string pattern)
		{
			try
			{
				var matches = new Regex(pattern).Matches(translation);
				if (matches.Count > 0)
				{
					return AddSeparators(translation, matches);
				}
			}
			catch (Exception e)
			{
				LogException(e);
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

		private void AddTagPadding(Segment segment, string text)
		{
			var (padLeft, padRight) = (_tagsDictionary[text].PadLeft, _tagsDictionary[text].PadRight);
			if (padLeft.Length > 0)
			{
				segment.Add(padLeft);
			}

			segment.Add(_tagsDictionary[text].SdlTag);
			if (padRight.Length > 0)
			{
				segment.Add(padRight);
			}
		}

		private void GetSourceTagsDictionary()
		{
			try
			{
				TryGetSourceTagsDictionary();
				_tagsInfo.Clear();
			}
			catch (Exception e)
			{
				LogException(e);
			}
		}

		private void TryGetSourceTagsDictionary()
		{
			_tagsDictionary = new Dictionary<string, GoogleTag>();
			var elements = _sourceSegment?.Elements;
			if (elements is null || !elements.Any())
			{
				return;
			}

			for (var i = 0; i < elements.Count; i++)
			{    
				if (elements[i].GetType() != typeof(Tag))
				{
					PreparedSourceText += elements[i].ToString();
					continue;
				}

				_currentTag = new GoogleTag((Tag)elements[i].Duplicate());
				UpdateTagsInfo(i);
				var tagText = ConvertTagToString();
				PreparedSourceText += tagText;
				SetWhiteSpace(elements, i);
				_tagsDictionary.Add(tagText, _currentTag);
			}
		}

		private void UpdateTagsInfo(int index)
		{
			if (_tagsInfo.Any(n => n.TagId.Equals(_currentTag.SdlTag.TagID)))
			{
				return;
			}

			_tagsInfo.Add(new TagInfo
			{
				Index = index,
				IsClosed = _currentTag.SdlTag.Type == TagType.End,
				TagId = _currentTag.SdlTag.TagID,
				TagType = _currentTag.SdlTag.Type,
			});
		}

		private string ConvertTagToString()
		{
			if (GetCorrespondingTag(_currentTag.SdlTag.TagID) is not TagInfo tagInfo)
			{
				return string.Empty;
			}

			return _currentTag.SdlTag.Type switch
			{ // Undefined, UnmatchedStart, UnmatchedEnd ?
				TagType.Start => $"<0tg{tagInfo.TagId}>",
				TagType.End => $"</0tg{tagInfo.TagId}>",
				TagType.Standalone => $"<0tg{tagInfo.TagId}/>",
				TagType.TextPlaceholder => $"<0tg{tagInfo.TagId}/>",
				TagType.LockedContent => $"<0tg{tagInfo.TagId}/>",
				_ => string.Empty,
			};
		}

		private TagInfo GetCorrespondingTag(string tagId)
		{
			return _tagsInfo.FirstOrDefault(t => t.TagId.Equals(tagId));
		}

		private void SetWhiteSpace(List<SegmentElement> elements, int currentIndex)
		{
			SetTrailingWhitespaces(elements, currentIndex - 1);
			SetLeadingWhitespaces(elements, currentIndex + 1);
		}

		private void SetTrailingWhitespaces(List<SegmentElement> elements, int previousIndex)
		{
			if (previousIndex < 0 || elements[previousIndex].GetType() == typeof(Tag))
			{
				return;
			}

			var previousElement = elements[previousIndex].ToString();
			if (previousElement.Trim().Equals(""))
			{
				return;
			}

			var whitespace = previousElement.Length - previousElement.TrimEnd().Length;
			_currentTag.PadLeft = previousElement.Substring(previousElement.Length - whitespace);
		}

		private void SetLeadingWhitespaces(List<SegmentElement> elements, int nextIndex)
		{
			if (nextIndex >= elements.Count || elements[nextIndex].GetType() == typeof(Tag))
			{
				return;
			}

			var nextElement = elements[nextIndex].ToString();
			var whitespace = nextElement.Length - nextElement.TrimStart().Length;
			_currentTag.PadRight = nextElement.Substring(0, whitespace);
		}

		private void LogException(Exception e)
			=> _logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n {e.StackTrace}");
	}
}