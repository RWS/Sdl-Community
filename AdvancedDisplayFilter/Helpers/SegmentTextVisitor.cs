using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class SegmentTextVisitor : IMarkupDataVisitor
	{
		private readonly StringBuilder _textBuilder = new StringBuilder();
	
		public void VisitText(IText text)
		{
			_textBuilder.Append(text.Properties.Text);
			
		}

		public string GetText(ISegment segment)
		{
			_textBuilder.Clear();
			VisitChildren(segment);

			return _textBuilder.ToString();
		}
		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
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

		public void VisitTagPair(ITagPair tagPair)
		{
			//foreach (dynamic subItem in tagPair.AllSubItems)
			//{
			//	var property = subItem.Properties;
			//	if (property != null)
			//	{
			//		var text = property.Text;
			//		_textBuilder.Append(text);
			//	}

			//}

			foreach (dynamic subItem in tagPair.AllSubItems)
			{
				if (IsPropertyExist(subItem, "AllSubItems"))
				{
					foreach (dynamic innerSubitem in subItem.AllSubItems)
					{
						AppendText(innerSubitem);
					}
				}
				else
				{
					AppendText(subItem);
				}

			}

		}

		private  void AppendText(dynamic tag)
		{
			var property = tag.Properties;
			if (property != null)
			{
				var text = property.Text;
				_textBuilder.Append(text);
			}
		}
		/// <summary>
		/// Check if tag pair contains specified property
		/// </summary>
		/// <param name="tagPair"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool IsPropertyExist(dynamic tagPair, string name)
		{
			var tP = tagPair as ExpandoObject;
			if (tP != null)
				return ((IDictionary<string, object>)tagPair).ContainsKey(name);

			return tagPair.GetType().GetProperty(name) != null;
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{

		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			
		}

		

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			
		}
		
	}
}
