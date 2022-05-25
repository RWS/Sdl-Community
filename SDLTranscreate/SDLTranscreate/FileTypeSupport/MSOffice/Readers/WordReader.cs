using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;
using Trados.Transcreate.Model;
using Comment = DocumentFormat.OpenXml.Wordprocessing.Comment;


namespace Trados.Transcreate.FileTypeSupport.MSOffice.Readers
{
	internal class WordReader
	{
		private readonly List<Token> _comments = new List<Token>();
		private readonly ImportOptions _settings;
		private readonly string _sourceLanguage;
		private readonly string _targetLanguage;

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

		public WordReader(ImportOptions settings, string sourceLanguage, string targetLanguage)
		{
			_settings = settings;
			_sourceLanguage = sourceLanguage;
			_targetLanguage = targetLanguage;
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
				foreach (var comment in commentsPart.Comments.Elements<Comment>())
				{
					var commentToken = new Token { Content = comment.InnerText, Type = Token.TokenType.CommentStart };
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
					var targetCell = rowItem.Elements<TableCell>().ElementAt(4);

					TableCell backTranslationCel = null;
					var columnCount = rowItem.ChildElements.Count;
					if (columnCount > 5)
					{
						backTranslationCel = rowItem.Elements<TableCell>().ElementAt(5);
					}

					var currentContentList = ProcessTargetCellContent(segmentIdCell, targetCell, backTranslationCel);
					if (currentContentList.TranslationTokens?.Count > 0 || currentContentList.BackTranslationTokens?.Count > 0)
					{
						collector.Add(segmentIdCell, currentContentList);
					}

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
		/// <param name="segmentIdCell"></param>
		/// <param name="targetCell"></param>
		/// <param name="backTranslationCell"></param>
		private UpdatedSegmentContent ProcessTargetCellContent(string segmentIdCell, TableCell targetCell, TableCell backTranslationCell)
		{
			if (targetCell == null)
			{
				return null;
			}

			var currentContentList = new UpdatedSegmentContent();

			CheckParagraphCount(targetCell);
			currentContentList.TranslationHasTrackedChanges = SegmentHasChanges(targetCell);

			var translationContent = GetParagraphsTrimed(targetCell).FirstOrDefault();
			if (translationContent != null)
			{
				var tagBuffer = "";//sometimes word is so stupid that tags are not in single runs, buffering is required.
				var translationTokens = new List<Token>();
				//we need to iterate whole paragraph and process OpenXMLElement appropriately
				foreach (var item in translationContent.Elements())
				{
					ProcessOpenXmlElement(item, ref translationTokens, ref tagBuffer);
				}

				currentContentList.TranslationTokens = translationTokens;
			}

			var backTranslationContent = GetParagraphsTrimed(backTranslationCell).FirstOrDefault();
			if (backTranslationContent != null)
			{
				CheckParagraphCount(backTranslationCell);
				currentContentList.BackTranslationHasTrackedChanges = SegmentHasChanges(backTranslationCell);

				var tagBuffer = "";//sometimes word is so stupid that tags are not in single runs, buffering is required.
				var backTranslationTokens = new List<Token>();
				//we need to iterate whole paragraph and process OpenXMLElement appropriately
				foreach (var item in backTranslationContent.Elements())
				{
					ProcessOpenXmlElement(item, ref backTranslationTokens, ref tagBuffer);
				}

				currentContentList.BackTranslationTokens = backTranslationTokens;
			}


			return currentContentList;

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
		/// <param name="tokens"></param>
		/// <param name="tagBuffer"></param>
		private void ProcessOpenXmlElement(OpenXmlElement item, ref List<Token> tokens, ref string tagBuffer)
		{
			if (item is InsertedRun insertedRun)
			{
				var revisionTokenStart =
					new Token
					{
						Type = Token.TokenType.RevisionMarker,
						RevisionType = Token.RevisionMarkerType.InsertStart,
						Date = insertedRun.Date,
						Author = insertedRun.Author
					};

				tokens.Add(revisionTokenStart);

				foreach (var element in item.Elements())
				{
					ProcessOpenXmlElement(element, ref tokens, ref tagBuffer);
				}

				var revisionTokenEnd =
					new Token
					{
						Type = Token.TokenType.RevisionMarker,
						RevisionType = Token.RevisionMarkerType.InsertEnd
					};

				tokens.Add(revisionTokenEnd);
				return;
			}
			if (item is DeletedRun deletedRun)
			{
				var revisionTokenStart =
					new Token
					{
						Type = Token.TokenType.RevisionMarker,
						RevisionType = Token.RevisionMarkerType.DeleteStart,
						Date = deletedRun.Date,
						Author = deletedRun.Author
					};

				tokens.Add(revisionTokenStart);

				foreach (var element in item.Elements())
				{
					ProcessOpenXmlElement(element, ref tokens, ref tagBuffer);
				}

				var revisionTokenEnd =
					new Token
					{
						Type = Token.TokenType.RevisionMarker,
						RevisionType = Token.RevisionMarkerType.DeleteEnd
					};

				tokens.Add(revisionTokenEnd);
				return;
			}
			if (item is Run run)
			{
				ProcessTextRun(ref tokens, run, ref tagBuffer);
				return;
			}
			if (item is CommentRangeStart)
			{
				tokens.Add(_comments.First<Token>());
				_comments.RemoveAt(0);
				return;
			}
			if (item is CommentRangeEnd)
			{
				tokens.Add(new Token { Content = "commentend", Type = Token.TokenType.CommentEnd });
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
						ProcessTextRun(ref tokens, new Run(new Text(item.InnerText)), ref tagBuffer);
					}
				}
			}
		}


		/// <summary>
		/// Check if there is only one paragraph in the target cell, in case somebody pressed Enter, throw an exception.
		/// </summary>
		/// <param name="targetCell"></param>
		private void CheckParagraphCount(TableCell targetCell)
		{
			var paragraphs = GetParagraphsTrimed(targetCell);
			if (paragraphs.Count > 1)
			{
				throw new TooManyParagraphsException();
			}
		}

		private static List<Paragraph> GetParagraphsTrimed(TableCell targetCell)
		{
			if (targetCell == null)
			{
				return new List<Paragraph>();
			}
			
			var allParagraphs = targetCell.Elements<Paragraph>().ToList();
			var excludeIndexes = new List<int>();
			for (var i = 0; i < allParagraphs.Count; i++)
			{
				if (string.IsNullOrEmpty(allParagraphs[i].InnerText.Trim()))
				{
					if (!excludeIndexes.Contains(i))
					{
						excludeIndexes.Add(i);
					}
				}
				else
				{
					break;
				}
			}

			for (var i = allParagraphs.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(allParagraphs[i].InnerText.Trim()))
				{
					if (!excludeIndexes.Contains(i))
					{
						excludeIndexes.Add(i);
					}
				}
				else
				{
					break;
				}
			}

			var paragraphs = new List<Paragraph>();
			for (var i = 0; i < allParagraphs.Count; i++)
			{
				if (!excludeIndexes.Contains(i))
				{
					paragraphs.Add(allParagraphs[i]);
				}
			}

			return paragraphs;
		}

		private void ProcessTextRun(ref List<Token> tokens, Run textRun, ref string tagbuffer)
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
				tagbuffer = ProcessTaggedText(matches, tagContent, ref tokens, false);
			}

			//process locked content
			else if (textRun.RunProperties != null &&
					 textRun.RunProperties.Elements<RunStyle>().Count(s => s.Val == "LockedContent") != 0)
			{
				tagbuffer = ProcessTaggedText(matches, tagContent, ref tokens, true);
			}
			else
			{
				ProcessOtherRunTypes(ref tokens, textRun);
			}
		}

		/// <summary>
		/// Convert tagged text to the tokens, if style is applied to not tagged text this will be converted to string.
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="tagContent"></param>
		/// <param name="tokens"></param>
		/// <param name="isLockedContent"></param>
		/// <returns></returns>
		private string ProcessTaggedText(MatchCollection matches, string tagContent, ref List<Token> tokens, bool isLockedContent)
		{
			var lastPosition = 0;
			foreach (Match item in matches)
			{
				if (item.Index > lastPosition)
				{
					//overlapping style, most likely problem when style is applied to non tag text
					tokens.Add(new Token
					{
						Content = tagContent.Substring(lastPosition, tagContent.Length - item.Index),
						Type = Token.TokenType.Text
					});
				}

				if (isLockedContent)
				{
					tokens.Add(new Token { Content = item.Value, Type = Token.TokenType.LockedContent });
				}
				else
				{
					tokens.Add(GetTagType(item.Value));
				}

				lastPosition = item.Index + item.Length;
			}

			return tagContent.Substring(lastPosition);
		}

		/// <summary>
		/// Special handling for plain text, deleted text and special content like breaks etc.
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="textRun"></param>
		private void ProcessOtherRunTypes(ref List<Token> tokens, Run textRun)
		{
			foreach (var item in textRun.Elements())
			{
				if (item is Text textItem)
				{
					tokens.Add(new Token { Content = textRun.GetFirstChild<Text>().InnerText, Type = Token.TokenType.Text });
					continue;
				}

				if (item is DeletedText deletedTextItem)
				{
					tokens.Add(new Token { Content = deletedTextItem.Text, Type = Token.TokenType.Text });
					continue;
				}

				if (item is TabChar tabItem)
				{
					tokens.Add(new Token { Content = "\t", Type = Token.TokenType.Text });
					continue;
				}

				if (item is Break breakItem)
				{
					tokens.Add(new Token { Content = "\n", Type = Token.TokenType.Text });
					continue;
				}

				if (item is NoBreakHyphen noBreakHyphenItem)
				{
					tokens.Add(new Token { Content = " ", Type = Token.TokenType.Text });
					continue;
				}

				if (item is SoftHyphen softHyphenItem)
				{
					tokens.Add(new Token { Content = "¬", Type = Token.TokenType.Text });
					continue;
				}
			}
		}

		private Token GetTagType(string matchedText)
		{
			if (matchedText.StartsWith("</"))
			{
				return new Token { Content = matchedText, Type = Token.TokenType.TagClose };
			}
			if (matchedText.EndsWith("/>"))
			{
				return new Token { Content = matchedText, Type = Token.TokenType.TagPlaceholder };
			}
			if (!matchedText.StartsWith("</") && !matchedText.EndsWith("/>"))
			{
				return new Token { Content = matchedText, Type = Token.TokenType.TagOpen };
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
