using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
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

			if (_segmentOpen)
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
				if (!key.Equals("TextColor"))
				{
					return;
				}

				var hexCode = GetHexCode(formatting);

				if (!_colors.Contains(hexCode))
				{
					_colors.Add(hexCode);
				}
			}
			catch
			{
				// catch all; ignore
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
			var color = formatingProperty.Value.StringValue;
			var colors = color.Split(',');
			var red = string.Empty;
			var green = string.Empty;
			var blue = string.Empty;

			//for files which color code is like this "0,12,12,12"
			if (colors.Length.Equals(4))
			{
				red = colors[1];
				green = colors[2];
				blue = colors[3];
			}

			//"0,12,12,12"
			if (colors.Length.Equals(3))
			{
				red = colors[0];
				green = colors[1];
				blue = colors[2];
			}

			var hexCode = ColorPickerHelper.GetHexCode(byte.Parse(red), byte.Parse(green), byte.Parse(blue));
			return hexCode;
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
