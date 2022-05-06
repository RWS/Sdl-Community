﻿using Sdl.FileTypeSupport.Framework.BilingualApi;
using SDLCopyTags.Helpers;
using System;
using System.Collections.Generic;
using NLog;

namespace SDLCopyTags
{
	internal class TagVisitor : IMarkupDataVisitor
	{
		public IList<IAbstractMarkupData> Tags { get; private set; } = new List<IAbstractMarkupData>();
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			Tags.Add(tag);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);

			foreach (var tag in Tags)
			{
				if (tag is ITagPair)
				{
					ClearContentInTagPairs((ITagPair)tag);
				}
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			try
			{
				Tags.Add(tagPair);
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

		public void VisitText(IText text)
		{
		}

		private static bool IsRemovableItem(IAbstractMarkupData item)
		{
			return typeof(IText).IsAssignableFrom(item.GetType()) ||
				   typeof(ICommentMarker).IsAssignableFrom(item.GetType()) ||
				   typeof(ILocationMarker).IsAssignableFrom(item.GetType()) ||
				   typeof(ILockedContent).IsAssignableFrom(item.GetType()) ||
				   typeof(IOtherMarker).IsAssignableFrom(item.GetType()) ||
				   typeof(IRevisionMarker).IsAssignableFrom(item.GetType());
		}

		private void ClearContentInTagPairs(ITagPair tagPair)
		{
			try
			{
				for (int i = tagPair.Count - 1; i >= 0; --i)
				{
					var item = tagPair[i];

					if (IsRemovableItem(item))
					{
						item.RemoveFromParent();
					}
					else if (item is ITagPair)
					{
						ClearContentInTagPairs((ITagPair)item);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			try
			{
				foreach (var item in container)
				{
					item.AcceptVisitor(this);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}
	}
}