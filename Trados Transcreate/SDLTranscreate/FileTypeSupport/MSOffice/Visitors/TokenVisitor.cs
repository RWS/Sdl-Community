using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Visitors
{
	public class TokenVisitor : IMarkupDataVisitor
	{
		private StringBuilder _plainText;
		private readonly List<string> _comments;
		private readonly List<Token> _tokens;

		public TokenVisitor()
		{
			_plainText = new StringBuilder();
			_comments = new List<string>();
			_tokens = new List<Token>();
		}

		public List<string> Comments => _comments;

		public StringBuilder PlainText => _plainText;

		public List<Token> Tokens => _tokens;

		public void Process(ISegment segment)
		{
			_plainText = new StringBuilder();
			_comments.Clear();
			_tokens.Clear();
			VisitChildren(segment);
		}

		/// <summary>
		/// Iterates all sub items of container (IAbstractMarkupDataContainer)
		/// </summary>
		/// <param name="container"></param>
		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		private string RemoveTags(string input)
		{
			return Regex.Replace(input, @"</?[a-z][a-z0-9]*[^<>]*>", "");
		}


		/// <summary>
		/// Some special characters cannot be written as string - so convert them into special type of Token
		/// Only soft line break is handled at the moment.
		/// </summary>
		/// <param name="textToProcess"></param>
		private void TokenizeSpecialCharacters(string textToProcess)
		{
			var matches = Regex.Matches(textToProcess, "\n");

			//process all matches
			var currentPosition = 0;
			foreach (Match item in matches)
			{
				//process text 
				if (item.Index != currentPosition)
				{
					_tokens.Add(new Token { Content = textToProcess.Substring(currentPosition, (item.Index - currentPosition)), Type = Token.TokenType.Text });
				}

				//add special token
				_tokens.Add(new Token { SpecialContent = new DocumentFormat.OpenXml.Wordprocessing.Break(), Type = Token.TokenType.SpecialType });

				//reset position
				currentPosition = item.Index + item.Length;
			}

			//process rest of the string
			if (currentPosition < textToProcess.Length)
			{
				_tokens.Add(new Token { Content = textToProcess.Substring(currentPosition), Type = Token.TokenType.Text });
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			for (var i = 0; i < commentMarker.Comments.Count; i++)
			{
				var comment = commentMarker.Comments.GetItem(i);
				var startComment = new Token { Content = comment.Text, Type = Token.TokenType.CommentStart };
				startComment.Author = comment.Author;
				startComment.Date = comment.Date;
				_tokens.Add(startComment);

				Comments.Add("[" + comment.Author + " " + comment.Date + "] " + comment.Text);
			}

			VisitChildren(commentMarker);

			for (int i = 0; i < commentMarker.Comments.Count; i++)
			{
				_tokens.Add(new Token { Content = "commentend", Type = Token.TokenType.CommentEnd });
			}
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			//not necessary
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			var lockedText = RemoveTags(lockedContent.ToString());
			_tokens.Add(new Token { Content = lockedText, Type = Token.TokenType.LockedContent });
			PlainText.Append(lockedText);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			_tokens.Add(new Token { Content = tag.Properties.TagId.Id, Type = Token.TokenType.TagPlaceholder });
			PlainText.Append(tag.Properties.TextEquivalent);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			var revisionMarkerTokenStart = new Token { Type = Token.TokenType.RevisionMarker };
			revisionMarkerTokenStart.Author = revisionMarker.Properties.Author;
			revisionMarkerTokenStart.Date = (DateTime)revisionMarker.Properties.Date;

			switch (revisionMarker.Properties.RevisionType)
			{
				case RevisionType.Delete:
					revisionMarkerTokenStart.RevisionType = Token.RevisionMarkerType.DeleteStart;
					_tokens.Add(revisionMarkerTokenStart);
					break;
				case RevisionType.Insert:
					revisionMarkerTokenStart.RevisionType = Token.RevisionMarkerType.InsertStart;
					_tokens.Add(revisionMarkerTokenStart);
					break;
				case RevisionType.Unchanged:
				default:
					break;
			}

			VisitChildren(revisionMarker);

			var revisionMarkerTokenEnd = new Token { Type = Token.TokenType.RevisionMarker };
			switch (revisionMarker.Properties.RevisionType)
			{
				case RevisionType.Delete:
					revisionMarkerTokenEnd.RevisionType = Token.RevisionMarkerType.DeleteEnd;
					_tokens.Add(revisionMarkerTokenEnd);
					break;
				case RevisionType.Insert:
					revisionMarkerTokenEnd.RevisionType = Token.RevisionMarkerType.InsertEnd;
					_tokens.Add(revisionMarkerTokenEnd);
					break;
				case RevisionType.Unchanged:
				default:
					break;
			}
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_tokens.Add(new Token { Content = tagPair.StartTagProperties.TagId.Id, Type = Token.TokenType.TagOpen });
			VisitChildren(tagPair);
			_tokens.Add(new Token { Content = tagPair.StartTagProperties.TagId.Id, Type = Token.TokenType.TagClose });
		}

		public void VisitText(IText text)
		{
			TokenizeSpecialCharacters(text.Properties.Text);
			PlainText.Append(text.Properties.Text);
		}
	}
}
