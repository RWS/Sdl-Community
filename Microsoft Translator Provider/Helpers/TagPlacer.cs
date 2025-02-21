using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MicrosoftTranslatorProvider.Model;
using NLog;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Helpers
{
	public class TagPlacer
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly Segment _sourceSegment;

		private Dictionary<string, MicrosoftTag> _tagsDictionary;
		private string _returnedText;
		private MicrosoftTag _currentTag;

		public TagPlacer(Segment sourceSegment)
		{
			_sourceSegment = sourceSegment;
			TagsInfo = new List<TagInfo>();
			GetSourceTagsDictionary();
		}

		public List<TagInfo> TagsInfo { get; set; } = new();

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

		private string AddSeparators(string text, MatchCollection matches)
		{
			foreach (Match match in matches)
			{
				text = text.Replace(match.Value, $"```{match.Value}```");
			}

			return text;
		}

		public string MarkTags(string translation, string pattern)
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

		private Segment TryGetTaggedSegment(string returnedText)
		{
			_returnedText = new HtmlUtil().HtmlDecode(returnedText);
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
			const string SimplePattern = "tg[0-9]*";
			const string SimpleTagRegex = $@"(<{SimplePattern}\>)|(<\/{SimplePattern}\>)|(\<{SimplePattern}/\>)";

			const string GuidPattern = "tg[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}";
			const string GuidTagIdRegex = $@"(<{GuidPattern}/>)|(<\\/{GuidPattern}\\>)|(<{GuidPattern}>)";

			const string AlphanumericPattern = "tgpt[0-9]*";
			const string AlphanumericTagRegex = $@"(<{AlphanumericPattern}\>)|(<\/{AlphanumericPattern}\>)|(\<{AlphanumericPattern}/\>)";

			const string DecimalsPattern = @"tg[0-9,\.]*";
			const string DecimalsTagRegex = $@"(<{DecimalsPattern}\>)|(<\/{DecimalsPattern}\>)|(\<{DecimalsPattern}/\>)";

			var translation = _returnedText;
			translation = MarkTags(translation, SimpleTagRegex);
			translation = MarkTags(translation, GuidTagIdRegex);
			translation = MarkTags(translation, AlphanumericTagRegex);
			translation = MarkTags(translation, DecimalsTagRegex);

			return translation.Split(new[] { "```" }, StringSplitOptions.None);
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
				TagsInfo.Clear();
			}
			catch (Exception e)
			{
				LogException(e);
			}
		}

		private void TryGetSourceTagsDictionary()
		{
			_tagsDictionary = new Dictionary<string, MicrosoftTag>();
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

				_currentTag = new MicrosoftTag((Tag)elements[i].Duplicate());
				UpdateTagsInfo(i);
				var tagText = ConvertTagToString();
				PreparedSourceText += tagText;
				SetWhiteSpace(elements, i);
				_tagsDictionary.Add(tagText, _currentTag);
			}
		}

		private string ConvertTagToString()
		{
			if (GetCorrespondingTag(_currentTag.SdlTag.TagID) is not TagInfo tagInfo)
			{
				return string.Empty;
			}

			return _currentTag.SdlTag.Type switch
			{ // Undefined, UnmatchedStart, UnmatchedEnd ?
				TagType.Start => $"<tg{tagInfo.TagId}>",
				TagType.End => $"</tg{tagInfo.TagId}>",
				TagType.Standalone => $"<tg{tagInfo.TagId}/>",
				TagType.TextPlaceholder => $"<tg{tagInfo.TagId}/>",
				TagType.LockedContent => $"<tg{tagInfo.TagId}/>",
				_ => string.Empty,
			};
		}

		private TagInfo GetCorrespondingTag(string tagId)
		{
			return TagsInfo.FirstOrDefault(t => t.TagId.Equals(tagId));
		}

		private void SetWhiteSpace(List<SegmentElement> elements, int currentIndex)
		{
			SetTrailingWhitespaces(elements, currentIndex - 1);
			SetLeadingWhitespaces(elements, currentIndex + 1);
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

		private void UpdateTagsInfo(int index)
		{
			if (TagsInfo.Any(n => n.TagId.Equals(_currentTag.SdlTag.TagID)))
			{
				return;
			}

			TagsInfo.Add(new TagInfo
			{
				Index = index,
				IsClosed = _currentTag.SdlTag.Type == TagType.End,
				TagId = _currentTag.SdlTag.TagID,
				TagType = _currentTag.SdlTag.Type,
			});
		}

		private void LogException(Exception e)
			=> _logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n {e.StackTrace}");
	}
}