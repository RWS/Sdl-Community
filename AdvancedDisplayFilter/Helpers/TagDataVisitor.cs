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
				try
				{
					var key = formatingProperty.Key;
					if (key.Equals("TextColor"))
					{
						var color = formatingProperty.Value.StringValue;
						var colors = color.Split(',');
						var red = string.Empty;
						var green = string.Empty;
						var blue = string.Empty;
						//for  files which color code is like this "0,12,12,12"
						if (colors.Count().Equals(4))
						{
							red = colors[1];
							green = colors[2];
							blue = colors[3];
						}
						//"0,12,12,12"
						if (colors.Count().Equals(3))
						{
							red = colors[0];
							green = colors[1];
							blue = colors[2];
						}

						var hexCode = GetHexCode(byte.Parse(red), byte.Parse(green), byte.Parse(blue));
						if (!_colorCodeList.Contains(hexCode))
						{
							_colorCodeList.Add(hexCode);
						}

					}
				}
				catch (Exception e)
				{
					
				}
			}
			VisitChildren(tagPair);
		}

		public static string GetHexCode(byte red, byte green, byte blue)
		{
			var hexCode = string.Format("{0:X2}{1:X2}{2:X2}", red, green, blue);

			return hexCode;
		}

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
