using System;

namespace Sdl.Community.StudioViews.Model
{
	public class TokenInfo
	{
		public enum TokenType { TagOpen, TagClose, TagPlaceholder, Text, LockedContent, CommentStart, CommentEnd, SpecialType, RevisionMarker };

		public enum RevisionMarkerType { DeleteStart, DeleteEnd, InsertStart, InsertEnd };

		public string Content
		{
			get;
			set;
		}

		public TokenType Type
		{
			get;
			set;
		}


		public string Author
		{
			get;
			set;
		}

		public DateTime Date
		{
			get;
			set;
		}

		public RevisionMarkerType RevisionType
		{
			get;
			set;
		}
	}
}
