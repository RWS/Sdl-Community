using System.Collections.Generic;
using System.Drawing;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public class TagDataVisitor : IMarkupDataVisitor
	{
		private List<string> _colors;
		private Stack<ITagPair> _tagPairStack;
		private ISegment _segment;
		private IParagraph _paragraph;
		private bool _segmentOpen;

		public List<string> GetTagsColorCode(IParagraph paragraph, ISegment segment)
		{
			_tagPairStack = new Stack<ITagPair>();
			_colors = new List<string>();
			_paragraph = paragraph;
			_segment = segment;

			AddHexColorFromParagraphContext(paragraph);

			VisitChildren(paragraph);

			return _colors;
		}
	
		public List<string> GetTagsColorCode(ISegment segment)
		{
			_tagPairStack = new Stack<ITagPair>();
			_colors = new List<string>();
			_paragraph = null;
			_segment = segment;

			VisitChildren(segment);

			return _colors;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_tagPairStack.Push(tagPair);

			VisitChildren(tagPair);

			var tag = _tagPairStack.Pop();

			if (_segmentOpen && tag.StartTagProperties.Formatting != null)
			{
				foreach (var formatting in tag.StartTagProperties.Formatting)
				{
					AddHexColor(formatting);
				}
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			// ignore; not used for this implementation
		}

		public void VisitText(IText text)
		{
			// ignore; not used for this implementation
		}

		public void VisitSegment(ISegment segment)
		{
			_segmentOpen = true;
			
			var processSegment = !(_paragraph != null && segment?.Properties.Id.Id != _segment?.Properties.Id.Id);
			if (processSegment)
			{
				VisitChildren(segment);

				foreach (var tag in _tagPairStack)
				{
					if (tag.StartTagProperties.Formatting == null)
					{
						continue;
					}

					foreach (var formatting in tag.StartTagProperties.Formatting)
					{
						AddHexColor(formatting);
					}
				}
			}

			_segmentOpen = false;
		}

		private void AddHexColor(KeyValuePair<string, IFormattingItem> formatting)
		{
			try
			{
				var key = formatting.Key;
				if (!key.Equals("TextColor") && !key.Equals("BackgroundColor"))
				{
					return;
				}

				var hexCode = GetHexCode(formatting);
				if (hexCode != null && !_colors.Contains(hexCode))
				{
					_colors.Add(hexCode);
				}
			}
			catch
			{
				// catch all; ignore
			}
		}

		private void AddHexColorFromParagraphContext(IParagraph paragraph)
		{
			var contextInfoList = paragraph?.Parent?.Properties?.Contexts?.Contexts;
			if (contextInfoList == null)
			{
				return;
			}

			foreach (var contextInfo in contextInfoList)
			{
				if (contextInfo.DefaultFormatting == null)
				{
					continue;
				}

				foreach (var formattingItem in contextInfo.DefaultFormatting)
				{
					AddHexColor(formattingItem);
				}
			}
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// ignore; not used for this implementation
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			// ignore; not used for this implementation
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			// ignore; not used for this implementation
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			// ignore; not used for this implementation
		}

		private static string GetHexCode(KeyValuePair<string, IFormattingItem> formatingProperty)
		{
			try
			{
				var color = formatingProperty.Value.StringValue;
				var colors = color.Split(',');
			
				if (colors.Length.Equals(1))
				{
					var htmlColor = ColorTranslator.FromHtml(color);
					return ColorPickerHelper.GetHexCode(htmlColor.R, htmlColor.G, htmlColor.B);
				}

				if (colors.Length.Equals(4))
				{					
					return ColorPickerHelper.GetHexCode(byte.Parse(colors[1]), byte.Parse(colors[2]), byte.Parse(colors[3]));
				}

				if (colors.Length.Equals(3))
				{					
					return ColorPickerHelper.GetHexCode(byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2]));
				}
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
			{
				return;
			}

			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
