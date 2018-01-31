using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class UserVisitor: IMarkupDataVisitor
	{
		private string _createdBy;
		private string _modifiedBy;


		public bool ModifiedBy(ISegment segment, string modifiedBy)
		{
			return CheckModifiedBy(modifiedBy);
		}
		public bool CreatedBy(ISegment segment, string createdBy)
		{
			VisitSegment(segment);

			return CheckCreatedBy(createdBy);
		}

		private bool CheckCreatedBy(string userName)
		{
			var usersNameList = userName.Split('|').ToList();
			foreach (var name in usersNameList)
			{
				if (_createdBy.Contains(name))
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckModifiedBy(string userName)
		{
			var usersNameList = userName.Split('|').ToList();
			foreach (var name in usersNameList)
			{
				if (_modifiedBy.Contains(name))
				{
					return true;
				}
			}
			return false;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			
		}

		public void VisitSegment(ISegment segment)
		{
			if (segment.Properties.TranslationOrigin?.MetaData != null)
			{
				var metadatas = segment.Properties.TranslationOrigin?.MetaData;
				foreach (var metadata in metadatas)
				{
					if (metadata.Key.Equals("created_by"))
					{
						_createdBy = metadata.Value;
					}
					if (metadata.Key.Equals("last_modified_by"))
					{
						_modifiedBy = metadata.Value;
					}
				}
			}
			//here I'll set created by name and modify by
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
