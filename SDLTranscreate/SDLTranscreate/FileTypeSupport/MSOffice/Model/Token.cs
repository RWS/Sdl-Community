using System;
using DocumentFormat.OpenXml;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
	public class Token
	{
		public enum TokenType { TagOpen, TagClose, TagPlaceholder, Text, LockedContent, CommentStart, CommentEnd, SpecialType, RevisionMarker };

		public enum RevisionMarkerType { DeleteStart, DeleteEnd, InsertStart, InsertEnd };

		public string Content
		{
			get;
			set;
		}

		/// <summary>
		/// Used for storing special contents like SoftLineBreak
		/// </summary>
		public OpenXmlElement SpecialContent
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
