using System;
using System.Collections.Generic;
using System.Linq;
using Multilingual.Excel.FileType.Models;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class QuickInsertWriter
	{
		private readonly List<TagMap> _tagMapping;
		private Stack<IStartTagProperties> _openedTags = new Stack<IStartTagProperties>();

		public QuickInsertWriter(List<TagMap> tagMapping)
		{
			_tagMapping = tagMapping;
		}

		public IStartTagProperties InlineStartTag(IStartTagProperties tagInfo)
		{
			_openedTags.Push(tagInfo);

			if (TryParseQuickInsertId(tagInfo.TagId.Id, out var quickId))
			{
				var replacementTag = _tagMapping.FirstOrDefault(a => quickId == a.QuickInsertId && a.TagPair?.StartTag != null);
				if (replacementTag != null)
				{
					return replacementTag.TagPair.StartTag;
				}
			}

			return null;
		}

		public IEndTagProperties InlineEndTag(IEndTagProperties tagInfo)
		{
			var matchingStartTag = _openedTags.Pop();

			if (TryParseQuickInsertId(matchingStartTag.TagId.Id, out var quickId))
			{
				var replacementTag = _tagMapping.FirstOrDefault(a => quickId == a.QuickInsertId && a.TagPair?.EndTag != null);
				if (replacementTag != null)
				{
					return replacementTag.TagPair.EndTag;
				}
			}

			return null;
		}

		public void ParagraphUnitStart()
		{
			_openedTags = new Stack<IStartTagProperties>();
		}


		private static bool TryParseQuickInsertId(string tagId, out QuickInsertIds quickTag)
		{
			foreach (var enumValue in Enum.GetValues(typeof(QuickInsertIds)))
			{
				if (BuildQuickTagId((QuickInsertIds)enumValue) == tagId)
				{
					quickTag = (QuickInsertIds)enumValue;
					return true;
				}
			}

			quickTag = QuickInsertIds.None;
			return false;
		}

		private static string BuildQuickTagId(QuickInsertIds id)
		{
			return id.ToString();
		}
	}
}
