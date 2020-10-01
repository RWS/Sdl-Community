using System;
using DocumentFormat.OpenXml;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model
{
	public class Token
	{
		public enum TokenType { TagOpen, TagClose, TagPlaceholder, Text, LockedContent, CommentStart, CommentEnd, SpecialType, RevisionMarker };

		public enum RevisionMarkerType { DeleteStart, DeleteEnd, InsertStart, InsertEnd };

		public Token(TokenType type)
		{
			Type = type;
		}

		public Token(string tokenContent, TokenType type)
		{
			Content = tokenContent;
			Type = type;
		}

		public Token(OpenXmlElement specialContent, TokenType type)
		{
			Type = type;
			SpecialContent = specialContent;
		}

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
