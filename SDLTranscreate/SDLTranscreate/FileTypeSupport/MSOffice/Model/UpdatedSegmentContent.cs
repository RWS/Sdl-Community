using System.Collections.Generic;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model
{
	internal class UpdatedSegmentContent
	{

		public UpdatedSegmentContent()
		{
			Tokens = new List<Token>();
		}

		public List<Token> Tokens
		{
			get;
			set;
		}

		public bool SegmentHasTrackedChanges
		{
			get;
			set;
		}

	}
}
