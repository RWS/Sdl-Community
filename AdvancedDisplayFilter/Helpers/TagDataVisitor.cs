using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class TagDataVisitor:IMarkupDataVisitor
	{
		private List<string>_colorCodeList;

		public TagDataVisitor()
		{
			_colorCodeList = new List<string>();
		}

		public List<string> GetTagsColorCode(ISegment segment)
		{
			_colorCodeList.Clear();
			VisitChildren(segment);
			return _colorCodeList;
		}
		public void VisitTagPair(ITagPair tagPair)
		{
			foreach (var formatingProperty in tagPair.StartTagProperties.Formatting)
			{
				var key = formatingProperty.Key;
				if (key.Equals("TextColor"))
				{
					var color = formatingProperty.Value.StringValue;
					var colors = color.Split(',');
					var red = colors[0];
					var green = colors[1];
					var blue = colors[2];
					var hexCode =GetHexCode(byte.Parse(red), byte.Parse(green), byte.Parse(blue));
					_colorCodeList.Add(hexCode);
				}
				
			}
			VisitChildren(tagPair);
		}

		public static string GetHexCode(byte red, byte green, byte blue)
		{
			var hexCode = string.Format("{0:X2}{1:X2}{2:X2}", red, green, blue);

			return hexCode;
		}

		//public List<string> GetTags(ISegment segment)
		//{
		//	_colorCodeList.Clear();
		//	VisitChildren(segment);
		//	return _colorCodeList;
		//}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			//foreach (var VARIABLE in segment.AllSubItems)
			//{
				
			//}
			VisitChildren(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{

		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
				return;
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
