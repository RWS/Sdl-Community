using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace ExportToExcel
{
    public class DataExtractor : IMarkupDataVisitor
    {
        private StringBuilder _plainText;
        private readonly List<string> _comments;
        private readonly List<Token> _tokens;

        public DataExtractor()
        {
            _plainText = new StringBuilder();
            _comments = new List<string>();
            _tokens = new List<Token>();
        }

        public GeneratorSettings Settings
        {
            get;
            set;
        }

        public List<string> Comments
        {
            get
            {
                return _comments;
            }
        }

        public StringBuilder PlainText
        {
            get
            {
                return _plainText;
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return _tokens;
            }
        }

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
                    _tokens.Add(new Token(textToProcess.Substring(currentPosition, (item.Index - currentPosition)), Token.TokenType.Text));
                }
                //add special token
                _tokens.Add(new Token(new DocumentFormat.OpenXml.Wordprocessing.Break(), Token.TokenType.SpecialType));
                //reset position
                currentPosition = item.Index + item.Length;
            }
            //process rest of the string
            if (currentPosition < textToProcess.Length)
            {
                _tokens.Add(new Token(textToProcess.Substring(currentPosition), Token.TokenType.Text));
            }

        }

        #region IMarkupDataVisitor Members

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            if (Settings.ExtractComments)
            {
                for (int i = 0; i < commentMarker.Comments.Count; i++)
                {
                    IComment comment = commentMarker.Comments.GetItem(i);
                    var startComment = new Token(comment.Text, Token.TokenType.CommentStart)
                    {
                        Author = comment.Author,
                        Date = comment.Date
                    };
                    _tokens.Add(startComment);

                    Comments.Add("[" + comment.Author + " " + comment.Date + "] " + comment.Text);
                }
            }
            VisitChildren(commentMarker);
            if (Settings.ExtractComments)
            {
                for (var i = 0; i < commentMarker.Comments.Count; i++)
                {
                    _tokens.Add(new Token("commentend", Token.TokenType.CommentEnd));
                }
            }
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            //not necessary
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            var lockedText = RemoveTags(lockedContent.ToString());
            _tokens.Add(new Token(lockedText, Token.TokenType.LockedContent));
            PlainText.Append(lockedText);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            _tokens.Add(new Token(tag.Properties.TagId.Id, Token.TokenType.TagPlaceholder));
            PlainText.Append(tag.Properties.TextEquivalent);
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            var revisionMarkerTokenStart = new Token(Token.TokenType.RevisionMarker);
            revisionMarkerTokenStart.Author = revisionMarker.Properties.Author;
            if (revisionMarker.Properties.Date != null)
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

            var revisionMarkerTokenEnd = new Token(Token.TokenType.RevisionMarker);
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
            _tokens.Add(new Token(tagPair.StartTagProperties.TagId.Id, Token.TokenType.TagOpen));
            VisitChildren(tagPair);
            _tokens.Add(new Token(tagPair.StartTagProperties.TagId.Id, Token.TokenType.TagClose));
        }

        public void VisitText(IText text)
        {
            TokenizeSpecialCharacters(text.Properties.Text);
            PlainText.Append(text.Properties.Text);
        }

        #endregion
    }
}
