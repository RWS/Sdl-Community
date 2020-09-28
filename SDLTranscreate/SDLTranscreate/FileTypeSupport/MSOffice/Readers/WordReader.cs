using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Comment = DocumentFormat.OpenXml.Wordprocessing.Comment;


namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Readers
{
	internal class WordReader
	{
		private readonly List<Token> _comments = new List<Token>();
		private readonly GeneratorSettings _settings;
		private readonly Type[] _trackedRevisionsElements = {
			typeof(CellDeletion),
			typeof(CellInsertion),
			typeof(CellMerge),
			typeof(CustomXmlDelRangeEnd),
			typeof(CustomXmlDelRangeStart),
			typeof(CustomXmlInsRangeEnd),
			typeof(CustomXmlInsRangeStart),
			typeof(Deleted),
			typeof(DeletedFieldCode),
			typeof(DeletedMathControl),
			typeof(DeletedRun),
			typeof(DeletedText),
			typeof(Inserted),
			typeof(InsertedMathControl),
			typeof(InsertedMathControl),
			typeof(InsertedRun),
			typeof(MoveFrom),
			typeof(MoveFromRangeEnd),
			typeof(MoveFromRangeStart),
			typeof(MoveTo),
			typeof(MoveToRangeEnd),
			typeof(MoveToRangeStart),
			typeof(MoveToRun),
			typeof(NumberingChange),
			typeof(ParagraphMarkRunPropertiesChange),
			typeof(ParagraphPropertiesChange),
			typeof(RunPropertiesChange),
			typeof(SectionPropertiesChange),
			typeof(TableCellPropertiesChange),
			typeof(TableGridChange),
			typeof(TablePropertiesChange),
			typeof(TablePropertyExceptionsChange),
			typeof(TableRowPropertiesChange),
		};

		public WordReader(GeneratorSettings settings)
		{
			_settings = settings;
		}

		public Dictionary<string, UpdatedSegmentContent> ReadFile(string filePath)
		{
			var collector = new Dictionary<string, UpdatedSegmentContent>();

			using (var wordDoc = WordprocessingDocument.Open(filePath, false))
			{
				GetComments(wordDoc.MainDocumentPart.WordprocessingCommentsPart);

				var document = wordDoc.MainDocumentPart.Document;
				var content = document.Body.GetFirstChild<Table>();

				if (content != null)
				{
					IterateTableForSideBySide(content, ref collector);
				}

			}
			return collector;
		}

		private void GetComments(WordprocessingCommentsPart commentsPart)
		{
			if (commentsPart?.Comments != null)
			{
				foreach (Comment comment in commentsPart.Comments.Elements<Comment>())
				{
					var commentToken = new Token(comment.InnerText, Token.TokenType.CommentStart);
					commentToken.Author = comment.Author;
					commentToken.Date = comment.Date;
					_comments.Add(commentToken);
				}
			}
		}

		private void IterateTableForSideBySide(Table tableToProcess, ref Dictionary<string, UpdatedSegmentContent> collector)
		{
			foreach (var rowItem in tableToProcess.Elements<TableRow>())
			{
				var segmentIdCell = GetSegmentID(rowItem.Elements<TableCell>().ElementAt(0));
				try
				{
					//process the target segment
					var currentContentList = new UpdatedSegmentContent();
					var columnIndex = _settings.ImportBackTranslations ? 5 : 4;
					var targetCell = rowItem.Elements<TableCell>().ElementAt(columnIndex);
					ProcessTargetCellContent(targetCell, ref currentContentList, ref collector, segmentIdCell);
				}
				catch (ArgumentException)
				{
					throw new Exception(string.Format(StringResources.ErrorSegmenIDNotUnique, GetPlainSegmentID(segmentIdCell)));
				}
				catch (TooManyParagraphsException)
				{
					throw new Exception(string.Format(StringResources.ErrorTooManyParagraphs, GetPlainSegmentID(segmentIdCell)));
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format(StringResources.ErrorReadingSegments, GetPlainSegmentID(segmentIdCell)), ex);
				}
			}
		}

		/// <summary>
		/// Extract the segment ID
		/// </summary>
		/// <param name="segmentIdCell"></param>
		/// <returns></returns>
		private string GetPlainSegmentID(string segmentIdCell)
		{
			return segmentIdCell.Substring(segmentIdCell.IndexOf("_", StringComparison.Ordinal) + 1);
		}

		/// <summary>
		/// Reads the target cell content and add tokens into collector
		/// </summary>
		/// <param name="targetCell"></param>
		/// <param name="currentContentList"></param>
		/// <param name="collector"></param>
		/// <param name="segmentIdCell"></param>
		private void ProcessTargetCellContent(TableCell targetCell, ref UpdatedSegmentContent currentContentList,
			ref Dictionary<string, UpdatedSegmentContent> collector, string segmentIdCell)
		{
			if (targetCell != null)
			{
				CheckParagraphCount(targetCell, segmentIdCell);
				currentContentList.SegmentHasTrackedChanges = SegmentHasChanges(targetCell);
				//We should not update segments without tracked changes, so do not store them 

				if (!currentContentList.SegmentHasTrackedChanges &&
					_settings.ImportUpdateSegmentMode == GeneratorSettings.UpdateSegmentMode.TrackedOnly)
				{
					return;
				}
				//start collecting data
				var targetContent = targetCell.GetFirstChild<Paragraph>();
				if (targetContent != null)
				{
					var tagBuffer = "";//sometimes word is so stupid that tags are not in single runs, buffering is required.

					//we need to iterate whole paragraph and process OpenXMLElement appropriately
					foreach (var item in targetContent.Elements())
					{
						ProcessOpenXmlElement(item, ref currentContentList, ref tagBuffer);
					}
				}
				if (currentContentList.Tokens.Count > 0)
				{
					collector.Add(segmentIdCell, currentContentList);
				}
			}
		}


		/// <summary>
		/// Check if the target cell have some changes.
		/// </summary>
		/// <param name="targetCell"></param>
		/// <returns></returns>
		private bool SegmentHasChanges(TableCell targetCell)
		{
			if (targetCell.Descendants().Any(e => _trackedRevisionsElements.Contains(e.GetType())))
			{
				return true;
			}

			if (targetCell.Descendants().Any(e => e.GetType() == typeof(CommentRangeStart) || e.GetType() == typeof(CommentRangeStart)))
			{
				return true;
			}

			return false;
		}


		/// <summary>
		/// Recursively iterate the sub items in the OpenXmlElement
		/// </summary>
		/// <param name="item"></param>
		/// <param name="currentContentList"></param>
		/// <param name="tagBuffer"></param>
		private void ProcessOpenXmlElement(OpenXmlElement item, ref UpdatedSegmentContent currentContentList, ref string tagBuffer)
		{
			if (item is InsertedRun insertedRun)
			{
				var revisionTokenStart =
					new Token(Token.TokenType.RevisionMarker)
					{
						RevisionType = Token.RevisionMarkerType.InsertStart,
						Date = insertedRun.Date,
						Author = insertedRun.Author
					};

				currentContentList.Tokens.Add(revisionTokenStart);

				foreach (var element in item.Elements())
				{
					ProcessOpenXmlElement(element, ref currentContentList, ref tagBuffer);
				}

				var revisionTokenEnd =
					new Token(Token.TokenType.RevisionMarker)
					{
						RevisionType = Token.RevisionMarkerType.InsertEnd
					};

				currentContentList.Tokens.Add(revisionTokenEnd);
				return;
			}
			if (item is DeletedRun deletedRun)
			{
				var revisionTokenStart =
					new Token(Token.TokenType.RevisionMarker)
					{
						RevisionType = Token.RevisionMarkerType.DeleteStart,
						Date = deletedRun.Date,
						Author = deletedRun.Author
					};

				currentContentList.Tokens.Add(revisionTokenStart);

				foreach (var element in item.Elements())
				{
					ProcessOpenXmlElement(element, ref currentContentList, ref tagBuffer);
				}

				var revisionTokenEnd =
					new Token(Token.TokenType.RevisionMarker)
					{
						RevisionType = Token.RevisionMarkerType.DeleteEnd
					};

				currentContentList.Tokens.Add(revisionTokenEnd);
				return;
			}
			if (item is Run run)
			{
				ProcessTextRun(ref currentContentList, run, ref tagBuffer);
				return;
			}
			if (item is CommentRangeStart)
			{
				currentContentList.Tokens.Add(_comments.First<Token>());
				_comments.RemoveAt(0);
				return;
			}
			if (item is CommentRangeEnd)
			{
				currentContentList.Tokens.Add(new Token("commentend", Token.TokenType.CommentEnd));
				return;
			}
			//SmartTag feature is obsolete since Office 2007 and not supported since OpenXML 2.5
			if (item is OpenXmlUnknownElement && item.LocalName == "smartTag")
			{
				foreach (var possibleRun in item.Elements())
				{
					//only extract text runs
					if (possibleRun.LocalName == "r")
					{
						ProcessTextRun(ref currentContentList, new Run(new Text(item.InnerText)), ref tagBuffer);
					}
				}
			}
		}


		/// <summary>
		/// Check if there is only one paragraph in the target cell, in case somebody pressed Enter, throw an exception.
		/// </summary>
		/// <param name="targetCell"></param>
		/// <param name="segmentId"></param>
		private void CheckParagraphCount(TableCell targetCell, string segmentId)
		{
			if (targetCell.Elements<Paragraph>().Count() > 1)
			{
				throw new TooManyParagraphsException();
			}
		}

		private void ProcessTextRun(ref UpdatedSegmentContent currentContentList, Run textRun, ref string tagbuffer)
		{
			string tagContent = "";

			if (textRun.Elements<Text>().Any())
			{
				foreach (var item in textRun.Elements<Text>())
				{
					tagContent += item.InnerText;
				}
			}
			else if (textRun.Elements<DeletedText>().Any())
			{
				foreach (var item in textRun.Elements<DeletedText>())
				{
					tagContent += item.InnerText;
				}
			}
			if (tagbuffer != "")
			{
				//tag buffer is enabled, so add the content to the current run text
				tagContent = tagbuffer + tagContent;
			}

			//find all tags
			var rx = new Regex(@"</?[\p{Ll}\p{Lu}\p{Zs}\p{Nd}\p{P}]*[^<>]*>",
					   RegexOptions.Compiled);
			var matches = rx.Matches(tagContent);

			//process tags
			if (textRun.RunProperties != null &&
				textRun.RunProperties.Elements<RunStyle>().Count(s => s.Val == "Tag") != 0)
			{
				tagbuffer = ProcessTaggedText(matches, tagContent, ref currentContentList, false);
			}

			//process locked content
			else if (textRun.RunProperties != null &&
					 textRun.RunProperties.Elements<RunStyle>().Count(s => s.Val == "LockedContent") != 0)
			{
				tagbuffer = ProcessTaggedText(matches, tagContent, ref currentContentList, true);
			}
			else
			{
				ProcessOtherRunTypes(ref currentContentList, textRun);
			}
		}

		/// <summary>
		/// Convert tagged text to the tokens, if style is applied to not tagged text this will be converted to string.
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="tagContent"></param>
		/// <param name="currentContentList"></param>
		/// <param name="isLockedContent"></param>
		/// <returns></returns>
		private string ProcessTaggedText(MatchCollection matches, string tagContent, ref UpdatedSegmentContent currentContentList, bool isLockedContent)
		{
			var lastPosition = 0;
			foreach (Match item in matches)
			{
				if (item.Index > lastPosition)
				{
					//overlapping style, most likely problem when style is applied to non tag text
					currentContentList.Tokens.Add(new Token(tagContent.Substring(lastPosition, tagContent.Length - item.Index), Token.TokenType.Text));
				}

				if (isLockedContent)
				{
					currentContentList.Tokens.Add(new Token(item.Value, Token.TokenType.LockedContent));
				}
				else
				{
					currentContentList.Tokens.Add(GetTagType(item.Value));
				}

				lastPosition = item.Index + item.Length;
			}

			return tagContent.Substring(lastPosition);
		}

		/// <summary>
		/// Special handling for plain text, deleted text and special content like breaks etc.
		/// </summary>
		/// <param name="currentContentList"></param>
		/// <param name="textRun"></param>
		private void ProcessOtherRunTypes(ref UpdatedSegmentContent currentContentList, Run textRun)
		{
			foreach (var item in textRun.Elements())
			{
				if (item is Text textItem)
				{
					currentContentList.Tokens.Add(new Token(textRun.GetFirstChild<Text>().InnerText, Token.TokenType.Text));
					continue;
				}

				if (item is DeletedText deletedTextItem)
				{
					currentContentList.Tokens.Add(new Token(deletedTextItem.Text, Token.TokenType.Text));
					continue;
				}

				if (item is TabChar tabItem)
				{
					currentContentList.Tokens.Add(new Token("\t", Token.TokenType.Text));
					continue;
				}

				if (item is Break breakItem)
				{
					currentContentList.Tokens.Add(new Token("\n", Token.TokenType.Text));
					continue;
				}

				if (item is NoBreakHyphen noBreakHyphenItem)
				{
					currentContentList.Tokens.Add(new Token(" ", Token.TokenType.Text));
					continue;
				}

				if (item is SoftHyphen softHyphenItem)
				{
					currentContentList.Tokens.Add(new Token("¬", Token.TokenType.Text));
					continue;
				}
			}
		}

		private Token GetTagType(string matchedText)
		{
			if (matchedText.StartsWith("</"))
			{
				return new Token(matchedText, Token.TokenType.TagClose);
			}
			if (matchedText.EndsWith("/>"))
			{
				return new Token(matchedText, Token.TokenType.TagPlaceholder);
			}
			if (!matchedText.StartsWith("</") && !matchedText.EndsWith("/>"))
			{
				return new Token(matchedText, Token.TokenType.TagOpen);
			}

			return null;
		}

		/// <summary>
		/// Extracts the Segment identifier (trans-unit ID_Segment ID)
		/// </summary>
		/// <param name="tableCell"></param>
		/// <returns></returns>
		private string GetSegmentID(TableCell tableCell)
		{
			var para = tableCell.GetFirstChild<Paragraph>();
			var segmentId = string.Empty;
			var transUnitId = string.Empty;
			foreach (var item in para.Elements<Run>())
			{
				if (item.RunProperties == null || !item.RunProperties.Elements<RunStyle>().Any())
				{
					//backward compatibility, we need to ensure users will be able to work with already prepared files.
					segmentId += item.GetFirstChild<Text>().InnerText;
					return transUnitId + "_" + segmentId;
				}
				if (item.RunProperties.Elements<RunStyle>().Count(s => s.Val == "SegmentID") != 0)
				{
					segmentId += item.GetFirstChild<Text>().InnerText;
				}
				if (item.RunProperties.Elements<RunStyle>().Count(s => s.Val == "TransUnitID") != 0)
				{
					transUnitId += item.GetFirstChild<Text>().InnerText;
				}
			}

			return transUnitId + "_" + segmentId;
		}
	}
}
