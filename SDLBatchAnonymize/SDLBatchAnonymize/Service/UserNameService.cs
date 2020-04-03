using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
	public class UserNameService:IUserNameService
	{
		public void AnonymizeModifiedBy(ISegment segment, string value)
		{
			throw new NotImplementedException();
		}

		public void AnonymizeLastEditedBy(ISegment segment, string value)
		{
			throw new NotImplementedException();
		}

		public void AnonymizeCommentAuthor(ISegmentPair segmentPair, string value)
		{
			throw new NotImplementedException();
		}
	}
}
