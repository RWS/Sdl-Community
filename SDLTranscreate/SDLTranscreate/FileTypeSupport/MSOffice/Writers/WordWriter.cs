using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Comment = DocumentFormat.OpenXml.Wordprocessing.Comment;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Writers
{
	internal class WordWriter
	{
		private readonly string _sourceLang;

		private readonly string _targetLang;

		private readonly List<int> _listOfUsedComments;

		private GeneratorSettings _conversionSettings;

		private WordprocessingDocument _wordDocument;

		private MainDocumentPart _wordMainDocumentPart;

		private Body _wordBody;

		private List<Table> _wordTables;

		private WordprocessingCommentsPart _wordComments;

		private DocumentSettingsPart _wordSettings;

		private string _noMatch;

		private string _exactMatch;

		private string _fuzzyMatch;

		private string _contextMatch;

		private CultureInfo _sourceCulture;

		private CultureInfo _targetCulture;

		private int _commentId;

		private int _trackChangeId;


		public WordWriter(string sourceLang, string targetLang)
		{
			_listOfUsedComments = new List<int>(); //used for comment pairing.

			if (string.IsNullOrEmpty(sourceLang))
			{
				sourceLang = "en-US";
			}
			if (string.IsNullOrEmpty(targetLang))
			{
				targetLang = "en-US";
			}

			_sourceLang = sourceLang;
			_targetLang = targetLang;

			_sourceCulture = new CultureInfo(_sourceLang);
			_targetCulture = new CultureInfo(_targetLang);
		}

		public Style GenerateTagStyle()
		{
			var style = new Style
			{
				Type = StyleValues.Character,
				StyleId = "Tag",
				CustomStyle = true
			};

			var styleName = new StyleName
			{
				Val = "Tag"
			};

			var basedOn = new BasedOn
			{
				Val = "DefaultParagraphFont"
			};

			var uIPriority = new UIPriority
			{
				Val = 1
			};

			var primaryStyle = new PrimaryStyle();

			var styleRunProperties = new StyleRunProperties();
			var italic = new Italic();
			DocumentFormat.OpenXml.Wordprocessing.Color color = new DocumentFormat.OpenXml.Wordprocessing.Color
			{
				Val = "FF0066"
			};

			styleRunProperties.Append(italic);
			styleRunProperties.Append(color);

			style.Append(styleName);
			style.Append(basedOn);
			style.Append(uIPriority);
			style.Append(primaryStyle);
			style.Append(styleRunProperties);

			return style;
		}

		public Style GenerateSegmentIdStyle()
		{
			var style = new Style
			{
				Type = StyleValues.Character,
				StyleId = "SegmentID",
				CustomStyle = true
			};

			var styleName = new StyleName
			{
				Val = "SegmentID"
			};

			var basedOn = new BasedOn
			{
				Val = "DefaultParagraphFont"
			};

			var uIPriority = new UIPriority
			{
				Val = 1
			};

			var primaryStyle = new PrimaryStyle();

			var styleRunProperties = new StyleRunProperties();
			DocumentFormat.OpenXml.Wordprocessing.Color color = new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = "auto" };
			styleRunProperties.Append(color);

			style.Append(styleName);
			style.Append(basedOn);
			style.Append(uIPriority);
			style.Append(primaryStyle);
			style.Append(styleRunProperties);

			return style;
		}

		public Style GenerateLockedContentStyle()
		{
			var style = new Style
			{
				Type = StyleValues.Character,
				StyleId = "LockedContent",
				CustomStyle = true
			};

			var styleName = new StyleName
			{
				Val = "LockedContent"
			};

			var basedOn = new BasedOn
			{
				Val = "DefaultParagraphFont"
			};

			var uIPriority = new UIPriority
			{
				Val = 1
			};

			var primaryStyle = new PrimaryStyle();

			var styleRunProperties = new StyleRunProperties();
			var italic = new Italic();

			DocumentFormat.OpenXml.Wordprocessing.Color color =
				new DocumentFormat.OpenXml.Wordprocessing.Color
				{
					Val = "808080",
					ThemeColor = ThemeColorValues.Background1,
					ThemeShade = "80"
				};

			styleRunProperties.Append(italic);
			styleRunProperties.Append(color);

			style.Append(styleName);
			style.Append(basedOn);
			style.Append(uIPriority);
			style.Append(primaryStyle);
			style.Append(styleRunProperties);

			return style;
		}

		public Style GenerateTransUnitStyle()
		{
			var style = new Style
			{
				Type = StyleValues.Character,
				StyleId = "TransUnitID",
				CustomStyle = true
			};

			var styleName = new StyleName
			{
				Val = "TransUnitID"
			};

			var basedOn = new BasedOn
			{
				Val = "DefaultParagraphFont"
			};

			var uIPriority = new UIPriority
			{
				Val = 1
			};

			var primaryStyle = new PrimaryStyle();

			var styleRunProperties = new StyleRunProperties();
			var vanish = new Vanish();

			DocumentFormat.OpenXml.Wordprocessing.Color color = new DocumentFormat.OpenXml.Wordprocessing.Color
			{
				Val = "auto"
			};

			var fontSize = new FontSize
			{
				Val = "2"
			};

			styleRunProperties.Append(vanish);
			styleRunProperties.Append(color);
			styleRunProperties.Append(fontSize);

			style.Append(styleName);
			style.Append(basedOn);
			style.Append(uIPriority);
			style.Append(primaryStyle);
			style.Append(styleRunProperties);

			return style;
		}

		internal void AddNewTable(string name)
		{
			CreateNewTable(name);
		}

		internal void Initialize(string fileId, string outputFile, GeneratorSettings settings)
		{
			_conversionSettings = settings;

			_wordDocument = WordprocessingDocument.Create(outputFile, WordprocessingDocumentType.Document);
			_wordMainDocumentPart = _wordDocument.AddMainDocumentPart();

			var document = new Document
			{
				MCAttributes = new MarkupCompatibilityAttributes
				{
					Ignorable = "w14 wp14"
				}
			};

			document.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			document.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			document.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			document.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			document.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			document.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			document.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			document.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			document.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			document.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			document.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			document.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			document.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			document.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			document.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");
			_wordMainDocumentPart.Document = document;

			var styleDefinitionsPart = _wordMainDocumentPart.AddNewPart<StyleDefinitionsPart>();
			GenerateStyleDefinitionContent(styleDefinitionsPart);

			_wordComments = _wordMainDocumentPart.AddNewPart<WordprocessingCommentsPart>("rId6");
			GenerateCommentsPartContent(_wordComments);

			_wordBody = new Body();
			_wordTables = new List<Table>();

			SetPackageProperties(fileId);
		}

		internal void Complete()
		{
			foreach (var wordTable in _wordTables)
			{
				_wordBody.Append(wordTable);
			}

			SetPageLayout();

			_wordMainDocumentPart.Document.Append(_wordBody);
			_wordMainDocumentPart.Document.Save();
			_wordDocument.Close();
		}

		internal void WriteEntry(string segmentId, string paragraphUnitId,
			string relevance,
			string textFunction,
			List<Token> sourceTokens,
			List<Token> targetToken,
			ISegmentPairProperties segmentProperties,
			List<Token> backTranslationTokens)
		{
			//var segmentMatchColor = GetSegmentMatchColor(segmentProperties.TranslationOrigin);

			CreateSideBySideRow(
				GetCell(GenerateSegmentIdCellContent(segmentId, paragraphUnitId, _sourceLang, SourceCulture), null),
				GetCell(GenerateContextTypeCellContent(relevance, _sourceLang, SourceCulture), null),
				GetCell(GenerateContextTypeCellContent(textFunction, _sourceLang, SourceCulture), null),
				GetCell(GenerateCellContent(sourceTokens, _sourceLang, SourceCulture), null),
				GetCell(GenerateCellContent(targetToken, _targetLang, TargetCulture), null),
				GetCell(GenerateCellContent(backTranslationTokens, _sourceLang, SourceCulture), null));
		}

		private string ExactMatch
		{
			get
			{
				if (_exactMatch == null)
				{
					_exactMatch = ColorTranslator.ToHtml(_conversionSettings.ExactMatchColor);
				}
				return _exactMatch;
			}
			set
			{
				_exactMatch = value;
			}
		}

		private string FuzzyMatch
		{
			get
			{
				if (_fuzzyMatch == null)
				{
					_fuzzyMatch = ColorTranslator.ToHtml(_conversionSettings.FuzzyMatchColor).Replace("#", "");
				}
				return _fuzzyMatch;
			}
			set
			{
				_fuzzyMatch = value;
			}
		}

		private string ContextMatch
		{
			get
			{
				if (_contextMatch == null)
				{
					_contextMatch = ColorTranslator.ToHtml(_conversionSettings.ContextMatchColor).Replace("#", "");
				}
				return _contextMatch;
			}
			set
			{
				_contextMatch = value;
			}
		}

		private string NoMatch
		{
			get
			{
				if (_noMatch == null)
				{
					_noMatch = ColorTranslator.ToHtml(_conversionSettings.NoMatchColor).Replace("#", "");
				}
				return _noMatch;
			}
			set
			{
				_noMatch = value;
			}
		}

		private CultureInfo SourceCulture => _sourceCulture ?? (_sourceCulture = new CultureInfo(_sourceLang));

		private CultureInfo TargetCulture => _targetCulture ?? (_targetCulture = new CultureInfo(_targetLang));

		private TableCell GetCell(Paragraph cellContent, string cellFill)
		{
			var tableCell = new TableCell();
			var tableCellProperties = new TableCellProperties();

			if (cellFill != null)
			{
				var shading = new Shading
				{
					Val = ShadingPatternValues.Clear,
					Color = "auto",
					Fill = cellFill
				};
				tableCellProperties.Append(shading);
			}

			tableCell.Append(tableCellProperties);
			tableCell.Append(cellContent);

			return tableCell;
		}

		/// <summary>
		/// Get segment confirmation status and the match percentage
		/// </summary>
		/// <param name="segmentProperties"></param>
		/// <param name="lang"></param>
		/// <param name="langCulture"></param>
		/// <returns></returns>
		private Paragraph GenerateSegmentStatusDetails(ISegmentPairProperties segmentProperties, string lang, CultureInfo langCulture)
		{
			var paragraph = new Paragraph();

			var paragraphProperties = new ParagraphProperties();
			var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
			var languages1 = CreateLanguage(lang, langCulture);
			paragraphMarkRunProperties.Append(languages1);
			SetParagraphDirection(ref paragraphProperties, lang, langCulture);
			paragraphProperties.Append(paragraphMarkRunProperties);

			var run = new Run();

			var runProperties = new RunProperties();
			var languages2 = CreateLanguage(lang, langCulture);
			runProperties.Append(languages2);
			run.Append(runProperties);

			var text = new Text
			{
				Space = SpaceProcessingModeValues.Preserve,
				Text = new SegmentStatus(segmentProperties.ConfirmationLevel) + " " +
					   GetSegmentMatchValue(segmentProperties)
			};
			run.Append(text);

			paragraph.Append(paragraphProperties);
			paragraph.Append(run);

			return paragraph;
		}

		/// <summary>
		/// Returns percentage value of the segment or CM in case of context match
		/// </summary>
		/// <param name="segmentProperties"></param>
		/// <returns></returns>
		private string GetSegmentMatchValue(ISegmentPairProperties segmentProperties)
		{
			if (segmentProperties.TranslationOrigin == null)
			{
				return "";
			}
			if (segmentProperties.TranslationOrigin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget)
			{
				return "(CM)";
			}
			return "(" + segmentProperties.TranslationOrigin.MatchPercent + "%)";
		}

		private Paragraph GenerateContextTypeCellContent(string contextType, string lang, CultureInfo langCulture)
		{
			var paragraph = new Paragraph();

			var paragraphProperties = new ParagraphProperties();
			var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
			var languages1 = CreateLanguage(lang, langCulture);
			paragraphMarkRunProperties.Append(languages1);
			SetParagraphDirection(ref paragraphProperties, lang, langCulture);
			paragraphProperties.Append(paragraphMarkRunProperties);

			var run = new Run();

			var runProperties = new RunProperties();
			var languages2 = CreateLanguage(lang, langCulture);
			runProperties.Append(languages2);
			run.Append(runProperties);

			var text = new Text
			{
				Space = SpaceProcessingModeValues.Preserve,
				Text = contextType
			};

			run.Append(text);

			paragraph.Append(paragraphProperties);
			paragraph.Append(run);

			return paragraph;
		}

		private Paragraph GenerateSegmentIdCellContent(string segmentId, string paragraphUnitId, string lang, CultureInfo langCulture)
		{
			var paragraph = new Paragraph();

			var paragraphProperties = new ParagraphProperties();
			var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
			var languages1 = CreateLanguage(lang, langCulture);
			paragraphMarkRunProperties.Append(languages1);
			SetParagraphDirection(ref paragraphProperties, lang, langCulture);
			paragraphProperties.Append(paragraphMarkRunProperties);

			//Segment ID
			var run1 = new Run();

			var runProperties1 = new RunProperties();
			var runStyle1 = new RunStyle
			{
				Val = "SegmentID"
			};

			var languages2 = CreateLanguage(lang, langCulture);
			runProperties1.Append(runStyle1);
			runProperties1.Append(languages2);
			run1.Append(runProperties1);

			var text1 = new Text
			{
				Space = SpaceProcessingModeValues.Preserve,
				Text = segmentId
			};
			run1.Append(text1);

			//paragraph unit ID
			var run2 = new Run();
			var runProperties2 = new RunProperties();
			var runStyle2 = new RunStyle
			{
				Val = "TransUnitID"
			};
			runProperties2.Append(runStyle2);
			runProperties2.Append(languages2.Clone() as Languages);
			run2.Append(runProperties2);

			var text2 = new Text
			{
				Space = SpaceProcessingModeValues.Preserve,
				Text = paragraphUnitId
			};
			run2.Append(text2);

			paragraph.Append(paragraphProperties);
			paragraph.Append(run1);
			paragraph.Append(run2);

			return paragraph;
		}

		/// <summary>
		/// Text only parts are stored as plain text
		/// </summary>
		/// <param name="stringContent"></param>
		/// <param name="lang"></param>
		/// <param name="langCulture"></param>
		/// <returns></returns>
		private Paragraph GenerateCellContent(string stringContent, string lang, CultureInfo langCulture)
		{
			var paragraph = new Paragraph();

			var paragraphProperties = new ParagraphProperties();
			var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
			var languages1 = CreateLanguage(lang, langCulture);
			paragraphMarkRunProperties.Append(languages1);
			SetParagraphDirection(ref paragraphProperties, lang, langCulture);
			paragraphProperties.Append(paragraphMarkRunProperties);

			var run = new Run();

			var runProperties = new RunProperties();
			var languages2 = CreateLanguage(lang, langCulture);
			runProperties.Append(languages2);
			run.Append(runProperties);

			var text = new Text
			{
				Space = SpaceProcessingModeValues.Preserve,
				Text = stringContent
			};

			run.Append(text);

			paragraph.Append(paragraphProperties);
			paragraph.Append(run);

			return paragraph;
		}

		/// <summary>
		/// And here the magic happens. The list of tokens is transformed to the Word paragraph.
		/// </summary>
		/// <param name="listTokens"></param>
		/// <param name="lang"></param>
		/// <param name="langCulture"></param>
		/// <returns></returns>
		private Paragraph GenerateCellContent(List<Token> listTokens, string lang, CultureInfo langCulture)
		{
			var paragraph = new Paragraph();
			var paragraphProperties = new ParagraphProperties();
			var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
			var languages1 = CreateLanguage(lang, langCulture);
			paragraphMarkRunProperties.Append(languages1);
			SetParagraphDirection(ref paragraphProperties, lang, langCulture);
			paragraphProperties.Append(paragraphMarkRunProperties);

			paragraph.Append(paragraphProperties);

			//Iterate all items in list of tokens and create separate run for each of them.
			DeletedRun deletedRun = null;
			InsertedRun insertedRun = null;
			var inDeletedRun = false;
			var inInsertedRun = false;

			foreach (var item in listTokens)
			{
				var run = new Run();

				var deletedText = new DeletedText
				{
					Space = SpaceProcessingModeValues.Preserve
				};

				var text = new Text
				{
					Space = SpaceProcessingModeValues.Preserve
				};

				var runProperties = new RunProperties();
				var runTagStyle = new RunStyle
				{
					Val = "Tag"
				};

				var runLockedStyle = new RunStyle
				{
					Val = "LockedContent"
				};

				var languages2 = CreateLanguage(lang, langCulture);
				CommentRangeStart commentRangeStart = null;
				CommentRangeEnd commentRangeEnd = null;

				switch (item.Type)
				{
					case Token.TokenType.TagOpen:
						runProperties.Append(runTagStyle);
						run.Append(runProperties);
						HandleText(ref deletedRun, ref insertedRun, ref text, ref deletedText, "<" + item.Content + ">", run);
						break;
					case Token.TokenType.TagClose:
						runProperties.Append(runTagStyle);
						run.Append(runProperties);
						HandleText(ref deletedRun, ref insertedRun, ref text, ref deletedText, "</" + item.Content + ">", run);
						break;
					case Token.TokenType.TagPlaceholder:
						runProperties.Append(runTagStyle);
						run.Append(runProperties);
						HandleText(ref deletedRun, ref insertedRun, ref text, ref deletedText, "<" + item.Content + "/>", run);
						break;
					case Token.TokenType.LockedContent:
						runProperties.Append(runLockedStyle);
						run.Append(runProperties);
						HandleText(ref deletedRun, ref insertedRun, ref text, ref deletedText, "<" + item.Content + "/>", run);
						break;
					case Token.TokenType.Text:
						run.Append(runProperties);
						HandleText(ref deletedRun, ref insertedRun, ref text, ref deletedText, item.Content, run);
						break;
					case Token.TokenType.CommentStart:
						GenerateComment(item);
						var commentReference1 = new CommentReference
						{
							Id = (_commentId - 1).ToString()
						};
						commentRangeStart = new CommentRangeStart
						{
							Id = (_commentId - 1).ToString()
						};
						_listOfUsedComments.Add(_commentId - 1);
						break;
					case Token.TokenType.CommentEnd:
						commentRangeEnd = new CommentRangeEnd
						{
							Id = _listOfUsedComments.Last().ToString()
						};
						break;
					case Token.TokenType.SpecialType:
						run.Append(item.SpecialContent);
						break;
					case Token.TokenType.RevisionMarker:
						PrepareTrackChangesRuns(ref deletedRun, ref insertedRun, item, ref inDeletedRun, ref inInsertedRun);
						//switch to next token item
						break;
					default:
						run.Append(runProperties);
						HandleText(ref deletedRun, ref insertedRun, ref text, ref deletedText, item.Content, run);
						break;
				}

				//Set run language - needs to be here to generate valid document
				runProperties.Append(languages2);

				if (commentRangeStart != null)
				{
					if (deletedRun != null)
					{
						deletedRun.Append(commentRangeStart);
					}
					else if (insertedRun != null)
					{
						insertedRun.Append(commentRangeStart);
					}
					else
					{
						paragraph.Append(commentRangeStart);
					}
					continue;
				}

				if (commentRangeEnd != null)
				{
					//prepare comment reference
					Run commentReferenceRun = CreateCommentReference();
					if (deletedRun != null)
					{
						deletedRun.Append(commentRangeEnd);
						deletedRun.Append(commentReferenceRun);
					}
					else if (insertedRun != null)
					{
						insertedRun.Append(commentRangeEnd);
						insertedRun.Append(commentReferenceRun);
					}
					else
					{
						paragraph.Append(commentRangeEnd);
						paragraph.Append(commentReferenceRun);
					}
					continue;
				}

				//handle track changes insertion into current paragraph
				if (deletedRun != null && inDeletedRun == false)
				{
					deletedRun.Append(run);
					paragraph.Append(deletedRun);
					deletedRun = null;
				}
				else if (deletedRun != null)
				{
					deletedRun.Append(run);
				}
				else if (insertedRun != null && inInsertedRun == false)
				{
					insertedRun.Append(run);
					paragraph.Append(insertedRun);
					insertedRun = null;
				}
				else if (insertedRun != null)
				{
					insertedRun.Append(run);
				}
				else
				{
					paragraph.Append(run);
				}
			}
			return paragraph;
		}

		/// <summary>
		/// Creates new CommentReferenceRun with actual comment ID.
		/// </summary>
		/// <returns></returns>
		private Run CreateCommentReference()
		{
			var commentReferenceRun = new Run();
			var commentReferenceRunProperties = new RunProperties();
			var commentReferenceRunStyle = new RunStyle
			{
				Val = "CommentReference"
			};

			commentReferenceRunProperties.Append(commentReferenceRunStyle);
			var commentReference = new CommentReference
			{
				Id = _listOfUsedComments.Last().ToString()
			};

			_listOfUsedComments.RemoveAt(_listOfUsedComments.Count - 1);//remove from used comments list
			commentReferenceRun.Append(commentReferenceRunProperties);
			commentReferenceRun.Append(commentReference);

			return commentReferenceRun;
		}

		/// <summary>
		/// Based on track changes run type it sets the required properties
		/// </summary>
		/// <param name="deletedRun"></param>
		/// <param name="insertedRun"></param>
		/// <param name="item"></param>
		/// <param name="inDeletedRun"></param>
		/// <param name="inInsertedRun"></param>
		private void PrepareTrackChangesRuns(ref DeletedRun deletedRun, ref InsertedRun insertedRun, Token item, ref bool inDeletedRun, ref bool inInsertedRun)
		{
			//Prepare settings part (used for track changes)
			if (_wordSettings == null)
			{
				_wordSettings = _wordMainDocumentPart.AddNewPart<DocumentSettingsPart>("rId3");
				Settings settings = new Settings();
				TrackRevisions trackRevisions = new TrackRevisions();
				settings.Append(trackRevisions);
				_wordSettings.Settings = settings;
			}

			switch (item.RevisionType)
			{
				case Token.RevisionMarkerType.DeleteStart:
					deletedRun = new DeletedRun()
					{
						Author = item.Author,
						Date = item.Date,
						Id = _trackChangeId.ToString()
					};
					inDeletedRun = true;
					_trackChangeId++;
					break;
				case Token.RevisionMarkerType.DeleteEnd:
					inDeletedRun = false;
					break;
				case Token.RevisionMarkerType.InsertStart:
					insertedRun = new InsertedRun()
					{
						Author = item.Author,
						Date = item.Date,
						Id = _trackChangeId.ToString()
					};
					inInsertedRun = true;
					_trackChangeId++;
					break;
				case Token.RevisionMarkerType.InsertEnd:
					inInsertedRun = false;
					break;
				default:
					break;
			}
		}


		/// <summary>
		/// Based on currently opened track change type will assign string value to proper property
		/// </summary>
		/// <param name="deletedRun"></param>
		/// <param name="insertedRun"></param>
		/// <param name="text"></param>
		/// <param name="deletedText"></param>
		/// <param name="textValue"></param>
		/// <param name="currentRun"></param>
		private void HandleText(ref DeletedRun deletedRun, ref InsertedRun insertedRun, ref Text text, ref DeletedText deletedText, string textValue, Run currentRun)
		{
			if (deletedRun != null)
			{
				deletedText.Text = textValue;
				currentRun.Append(deletedText);
				return;
			}
			else
			{
				text.Text = textValue;
				currentRun.Append(text);
			}
			return;
		}

		private Languages CreateLanguage(string lang, CultureInfo langculture)
		{
			if (langculture.TextInfo.IsRightToLeft)
			{
				return new Languages() { Val = lang, Bidi = lang };
			}
			else
			{
				return new Languages() { Val = lang };
			}
		}

		private void SetParagraphDirection(ref ParagraphProperties paragraphProperties, string lang, CultureInfo langculture)
		{
			if (langculture.TextInfo.IsRightToLeft)
			{
				BiDi biDi = new BiDi();
				paragraphProperties.Append(biDi);
			}
		}

		private void GenerateComment(Token commentToken)
		{
			_commentId++;
			Comment comment = new Comment() { Initials = GetInitials(commentToken.Author), Author = commentToken.Author, Date = commentToken.Date, Id = (_commentId - 1).ToString() };
			Paragraph paragraph = new Paragraph();

			ParagraphProperties paragraphProperties = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId = new ParagraphStyleId() { Val = "CommentText" };

			paragraphProperties.Append(paragraphStyleId);

			Run run1 = new Run();

			RunProperties runProperties = new RunProperties();
			RunStyle runStyle = new RunStyle() { Val = "CommentReference" };

			runProperties.Append(runStyle);
			AnnotationReferenceMark annotationReferenceMark1 = new AnnotationReferenceMark();

			run1.Append(runProperties);
			run1.Append(annotationReferenceMark1);

			Run run2 = new Run();
			Text text1 = new Text();
			text1.Text = commentToken.Content;

			run2.Append(text1);

			paragraph.Append(paragraphProperties);
			paragraph.Append(run1);
			paragraph.Append(run2);

			comment.Append(paragraph);
			_wordComments.Comments.Append(comment);
		}

		private string GetInitials(string authorFullName)
		{
			string[] splitName = authorFullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string initials = "";
			foreach (var item in splitName)
			{
				initials += item.Substring(0, 1).ToUpper();
			}
			return initials;
		}

		private void SetPackageProperties(string projectId)
		{
			_wordDocument.PackageProperties.Keywords = "FileId:" + projectId;
		}

		private void GenerateCommentsPartContent(WordprocessingCommentsPart part)
		{
			Comments comments = new Comments { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
			comments.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			comments.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			comments.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			comments.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			comments.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			comments.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			comments.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			comments.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			comments.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			comments.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			comments.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			comments.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			comments.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			comments.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			comments.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			part.Comments = comments;
		}

		private void SetPageLayout()
		{
			SectionProperties sectionProperties = new SectionProperties();
			PageSize pageSize = new PageSize() { Width = (UInt32Value)15840U, Height = (UInt32Value)12240U, Orient = PageOrientationValues.Landscape };
			PageMargin pageMargin1 = new PageMargin() { Top = 720, Right = (UInt32Value)720U, Bottom = 720, Left = (UInt32Value)720U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
			sectionProperties.Append(pageSize);
			sectionProperties.Append(pageMargin1);
			_wordBody.Append(sectionProperties);
		}

		private void CreateNewTable(string name)
		{
			var wordTable = new Table();

			var tableProperties = new TableProperties();
			var tableStyle = new TableStyle
			{
				Val = "TableGrid"
			};

			var tableWidth = new TableWidth
			{
				Width = "0",
				Type = TableWidthUnitValues.Auto
			};

			var tableLook = new TableLook
			{
				Val = "04A0",
				FirstRow = true,
				LastRow = false,
				FirstColumn = true,
				LastColumn = false,
				NoHorizontalBand = false,
				NoVerticalBand = true
			};

			tableProperties.Append(tableStyle);
			tableProperties.Append(tableWidth);
			tableProperties.Append(tableLook);

			var tableGrid = new TableGrid();
			var gridColumn1 = new GridColumn();
			var gridColumn2 = new GridColumn();
			var gridColumn3 = new GridColumn();
			var gridColumn4 = new GridColumn();
			var gridColumn5 = new GridColumn();

			tableGrid.Append(gridColumn1);
			tableGrid.Append(gridColumn2);
			tableGrid.Append(gridColumn3);
			tableGrid.Append(gridColumn4);
			tableGrid.Append(gridColumn5);

			wordTable.Append(tableProperties);
			wordTable.Append(tableGrid);

			_wordTables.Add(wordTable);

			CreateSideBySideRow(
				GetCell(GenerateCellContent("Segment ID", _sourceLang, SourceCulture), "8DB3E2"),
				GetCell(GenerateCellContent("Relevance", _sourceLang, SourceCulture), "8DB3E2"),
				GetCell(GenerateCellContent("Text Function", _sourceLang, SourceCulture), "8DB3E2"),
				GetCell(GenerateCellContent("Source Segment", _sourceLang, SourceCulture), "8DB3E2"),
				GetCell(GenerateCellContent("Target Segment", _targetLang, SourceCulture), "8DB3E2"),
				GetCell(GenerateCellContent("Back Translation", _sourceLang, SourceCulture), "8DB3E2"));
		}

		private void CreateSideBySideRow(TableCell segmentIdCell, TableCell relevanceCell, TableCell textFunction,
			TableCell sourceCell, TableCell targetCell, TableCell backTranslationCell)
		{
			var tableRow1 = new TableRow();

			tableRow1.Append(segmentIdCell);
			tableRow1.Append(relevanceCell);
			tableRow1.Append(textFunction);
			tableRow1.Append(sourceCell);
			tableRow1.Append(targetCell);
			if (_conversionSettings.ExportBackTranslations)
			{
				tableRow1.Append(backTranslationCell);
			}

			_wordTables[_wordTables.Count - 1].Append(tableRow1);
		}

		private string GetSegmentMatchColor(ITranslationOrigin origin)
		{
			string result = NoMatch;
			if (origin == null)
			{
				return NoMatch;
			}

			if (origin.IsStructureContextMatch && origin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget)
			{
				result = ContextMatch;
			}
			else
			{
				if (origin.MatchPercent == 100)
				{
					result = ExactMatch;
				}
				if (origin.MatchPercent > 0 && origin.MatchPercent < 100)
				{
					result = FuzzyMatch;
				}
				if (origin.MatchPercent == 0)
				{
					result = NoMatch;
				}
			}

			return result;
		}

		private void GenerateStyleDefinitionContent(StyleDefinitionsPart part)
		{
			var styles1 = new Styles() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14" } };
			styles1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			styles1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			styles1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			styles1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");

			var docDefaults1 = new DocDefaults();

			var runPropertiesDefault1 = new RunPropertiesDefault();

			var runPropertiesBaseStyle1 = new RunPropertiesBaseStyle();
			var runFonts1 = new RunFonts() { AsciiTheme = ThemeFontValues.MinorHighAnsi, HighAnsiTheme = ThemeFontValues.MinorHighAnsi, EastAsiaTheme = ThemeFontValues.MinorHighAnsi, ComplexScriptTheme = ThemeFontValues.MinorBidi };
			var fontSize1 = new FontSize() { Val = "22" };
			var fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "22" };
			var languages1 = new Languages() { Val = "en-US", EastAsia = "en-US", Bidi = "ar-SA" };

			runPropertiesBaseStyle1.Append(runFonts1);
			runPropertiesBaseStyle1.Append(fontSize1);
			runPropertiesBaseStyle1.Append(fontSizeComplexScript1);
			runPropertiesBaseStyle1.Append(languages1);

			runPropertiesDefault1.Append(runPropertiesBaseStyle1);

			var paragraphPropertiesDefault1 = new ParagraphPropertiesDefault();

			var paragraphPropertiesBaseStyle1 = new ParagraphPropertiesBaseStyle();
			var spacingBetweenLines1 = new SpacingBetweenLines() { After = "200", Line = "276", LineRule = LineSpacingRuleValues.Auto };

			paragraphPropertiesBaseStyle1.Append(spacingBetweenLines1);

			paragraphPropertiesDefault1.Append(paragraphPropertiesBaseStyle1);

			docDefaults1.Append(runPropertiesDefault1);
			docDefaults1.Append(paragraphPropertiesDefault1);

			LatentStyles latentStyles1 = new LatentStyles() { DefaultLockedState = false, DefaultUiPriority = 99, DefaultSemiHidden = true, DefaultUnhideWhenUsed = true, DefaultPrimaryStyle = false, Count = 267 };
			LatentStyleExceptionInfo latentStyleExceptionInfo1 = new LatentStyleExceptionInfo() { Name = "Normal", UiPriority = 0, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo2 = new LatentStyleExceptionInfo() { Name = "heading 1", UiPriority = 9, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo3 = new LatentStyleExceptionInfo() { Name = "heading 2", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo4 = new LatentStyleExceptionInfo() { Name = "heading 3", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo5 = new LatentStyleExceptionInfo() { Name = "heading 4", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo6 = new LatentStyleExceptionInfo() { Name = "heading 5", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo7 = new LatentStyleExceptionInfo() { Name = "heading 6", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo8 = new LatentStyleExceptionInfo() { Name = "heading 7", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo9 = new LatentStyleExceptionInfo() { Name = "heading 8", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo10 = new LatentStyleExceptionInfo() { Name = "heading 9", UiPriority = 9, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo11 = new LatentStyleExceptionInfo() { Name = "toc 1", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo12 = new LatentStyleExceptionInfo() { Name = "toc 2", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo13 = new LatentStyleExceptionInfo() { Name = "toc 3", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo14 = new LatentStyleExceptionInfo() { Name = "toc 4", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo15 = new LatentStyleExceptionInfo() { Name = "toc 5", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo16 = new LatentStyleExceptionInfo() { Name = "toc 6", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo17 = new LatentStyleExceptionInfo() { Name = "toc 7", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo18 = new LatentStyleExceptionInfo() { Name = "toc 8", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo19 = new LatentStyleExceptionInfo() { Name = "toc 9", UiPriority = 39 };
			LatentStyleExceptionInfo latentStyleExceptionInfo20 = new LatentStyleExceptionInfo() { Name = "caption", UiPriority = 35, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo21 = new LatentStyleExceptionInfo() { Name = "Title", UiPriority = 10, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo22 = new LatentStyleExceptionInfo() { Name = "Default Paragraph Font", UiPriority = 1 };
			LatentStyleExceptionInfo latentStyleExceptionInfo23 = new LatentStyleExceptionInfo() { Name = "Subtitle", UiPriority = 11, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo24 = new LatentStyleExceptionInfo() { Name = "Strong", UiPriority = 22, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo25 = new LatentStyleExceptionInfo() { Name = "Emphasis", UiPriority = 20, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo26 = new LatentStyleExceptionInfo() { Name = "Table Grid", UiPriority = 59, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo27 = new LatentStyleExceptionInfo() { Name = "Placeholder Text", UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo28 = new LatentStyleExceptionInfo() { Name = "No Spacing", UiPriority = 1, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo29 = new LatentStyleExceptionInfo() { Name = "Light Shading", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo30 = new LatentStyleExceptionInfo() { Name = "Light List", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo31 = new LatentStyleExceptionInfo() { Name = "Light Grid", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo32 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo33 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo34 = new LatentStyleExceptionInfo() { Name = "Medium List 1", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo35 = new LatentStyleExceptionInfo() { Name = "Medium List 2", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo36 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo37 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo38 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo39 = new LatentStyleExceptionInfo() { Name = "Dark List", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo40 = new LatentStyleExceptionInfo() { Name = "Colorful Shading", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo41 = new LatentStyleExceptionInfo() { Name = "Colorful List", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo42 = new LatentStyleExceptionInfo() { Name = "Colorful Grid", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo43 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 1", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo44 = new LatentStyleExceptionInfo() { Name = "Light List Accent 1", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo45 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 1", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo46 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 1", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo47 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 1", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo48 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 1", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo49 = new LatentStyleExceptionInfo() { Name = "Revision", UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo50 = new LatentStyleExceptionInfo() { Name = "List Paragraph", UiPriority = 34, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo51 = new LatentStyleExceptionInfo() { Name = "Quote", UiPriority = 29, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo52 = new LatentStyleExceptionInfo() { Name = "Intense Quote", UiPriority = 30, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo53 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 1", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo54 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 1", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo55 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 1", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo56 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 1", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo57 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 1", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo58 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 1", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo59 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 1", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo60 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 1", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo61 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 2", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo62 = new LatentStyleExceptionInfo() { Name = "Light List Accent 2", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo63 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 2", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo64 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 2", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo65 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 2", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo66 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 2", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo67 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 2", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo68 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 2", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo69 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 2", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo70 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 2", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo71 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 2", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo72 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 2", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo73 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 2", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo74 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 2", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo75 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 3", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo76 = new LatentStyleExceptionInfo() { Name = "Light List Accent 3", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo77 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 3", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo78 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 3", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo79 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 3", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo80 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 3", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo81 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 3", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo82 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 3", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo83 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 3", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo84 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 3", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo85 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 3", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo86 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 3", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo87 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 3", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo88 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 3", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo89 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 4", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo90 = new LatentStyleExceptionInfo() { Name = "Light List Accent 4", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo91 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 4", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo92 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 4", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo93 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 4", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo94 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 4", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo95 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 4", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo96 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 4", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo97 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 4", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo98 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 4", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo99 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 4", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo100 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 4", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo101 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 4", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo102 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 4", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo103 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 5", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo104 = new LatentStyleExceptionInfo() { Name = "Light List Accent 5", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo105 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 5", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo106 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 5", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo107 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 5", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo108 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 5", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo109 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 5", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo110 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 5", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo111 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 5", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo112 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 5", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo113 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 5", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo114 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 5", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo115 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 5", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo116 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 5", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo117 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 6", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo118 = new LatentStyleExceptionInfo() { Name = "Light List Accent 6", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo119 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 6", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo120 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 6", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo121 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 6", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo122 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 6", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo123 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 6", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo124 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 6", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo125 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 6", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo126 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 6", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo127 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 6", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo128 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 6", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo129 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 6", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo130 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 6", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
			LatentStyleExceptionInfo latentStyleExceptionInfo131 = new LatentStyleExceptionInfo() { Name = "Subtle Emphasis", UiPriority = 19, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo132 = new LatentStyleExceptionInfo() { Name = "Intense Emphasis", UiPriority = 21, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo133 = new LatentStyleExceptionInfo() { Name = "Subtle Reference", UiPriority = 31, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo134 = new LatentStyleExceptionInfo() { Name = "Intense Reference", UiPriority = 32, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo135 = new LatentStyleExceptionInfo() { Name = "Book Title", UiPriority = 33, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
			LatentStyleExceptionInfo latentStyleExceptionInfo136 = new LatentStyleExceptionInfo() { Name = "Bibliography", UiPriority = 37 };
			LatentStyleExceptionInfo latentStyleExceptionInfo137 = new LatentStyleExceptionInfo() { Name = "TOC Heading", UiPriority = 39, PrimaryStyle = true };

			latentStyles1.Append(latentStyleExceptionInfo1);
			latentStyles1.Append(latentStyleExceptionInfo2);
			latentStyles1.Append(latentStyleExceptionInfo3);
			latentStyles1.Append(latentStyleExceptionInfo4);
			latentStyles1.Append(latentStyleExceptionInfo5);
			latentStyles1.Append(latentStyleExceptionInfo6);
			latentStyles1.Append(latentStyleExceptionInfo7);
			latentStyles1.Append(latentStyleExceptionInfo8);
			latentStyles1.Append(latentStyleExceptionInfo9);
			latentStyles1.Append(latentStyleExceptionInfo10);
			latentStyles1.Append(latentStyleExceptionInfo11);
			latentStyles1.Append(latentStyleExceptionInfo12);
			latentStyles1.Append(latentStyleExceptionInfo13);
			latentStyles1.Append(latentStyleExceptionInfo14);
			latentStyles1.Append(latentStyleExceptionInfo15);
			latentStyles1.Append(latentStyleExceptionInfo16);
			latentStyles1.Append(latentStyleExceptionInfo17);
			latentStyles1.Append(latentStyleExceptionInfo18);
			latentStyles1.Append(latentStyleExceptionInfo19);
			latentStyles1.Append(latentStyleExceptionInfo20);
			latentStyles1.Append(latentStyleExceptionInfo21);
			latentStyles1.Append(latentStyleExceptionInfo22);
			latentStyles1.Append(latentStyleExceptionInfo23);
			latentStyles1.Append(latentStyleExceptionInfo24);
			latentStyles1.Append(latentStyleExceptionInfo25);
			latentStyles1.Append(latentStyleExceptionInfo26);
			latentStyles1.Append(latentStyleExceptionInfo27);
			latentStyles1.Append(latentStyleExceptionInfo28);
			latentStyles1.Append(latentStyleExceptionInfo29);
			latentStyles1.Append(latentStyleExceptionInfo30);
			latentStyles1.Append(latentStyleExceptionInfo31);
			latentStyles1.Append(latentStyleExceptionInfo32);
			latentStyles1.Append(latentStyleExceptionInfo33);
			latentStyles1.Append(latentStyleExceptionInfo34);
			latentStyles1.Append(latentStyleExceptionInfo35);
			latentStyles1.Append(latentStyleExceptionInfo36);
			latentStyles1.Append(latentStyleExceptionInfo37);
			latentStyles1.Append(latentStyleExceptionInfo38);
			latentStyles1.Append(latentStyleExceptionInfo39);
			latentStyles1.Append(latentStyleExceptionInfo40);
			latentStyles1.Append(latentStyleExceptionInfo41);
			latentStyles1.Append(latentStyleExceptionInfo42);
			latentStyles1.Append(latentStyleExceptionInfo43);
			latentStyles1.Append(latentStyleExceptionInfo44);
			latentStyles1.Append(latentStyleExceptionInfo45);
			latentStyles1.Append(latentStyleExceptionInfo46);
			latentStyles1.Append(latentStyleExceptionInfo47);
			latentStyles1.Append(latentStyleExceptionInfo48);
			latentStyles1.Append(latentStyleExceptionInfo49);
			latentStyles1.Append(latentStyleExceptionInfo50);
			latentStyles1.Append(latentStyleExceptionInfo51);
			latentStyles1.Append(latentStyleExceptionInfo52);
			latentStyles1.Append(latentStyleExceptionInfo53);
			latentStyles1.Append(latentStyleExceptionInfo54);
			latentStyles1.Append(latentStyleExceptionInfo55);
			latentStyles1.Append(latentStyleExceptionInfo56);
			latentStyles1.Append(latentStyleExceptionInfo57);
			latentStyles1.Append(latentStyleExceptionInfo58);
			latentStyles1.Append(latentStyleExceptionInfo59);
			latentStyles1.Append(latentStyleExceptionInfo60);
			latentStyles1.Append(latentStyleExceptionInfo61);
			latentStyles1.Append(latentStyleExceptionInfo62);
			latentStyles1.Append(latentStyleExceptionInfo63);
			latentStyles1.Append(latentStyleExceptionInfo64);
			latentStyles1.Append(latentStyleExceptionInfo65);
			latentStyles1.Append(latentStyleExceptionInfo66);
			latentStyles1.Append(latentStyleExceptionInfo67);
			latentStyles1.Append(latentStyleExceptionInfo68);
			latentStyles1.Append(latentStyleExceptionInfo69);
			latentStyles1.Append(latentStyleExceptionInfo70);
			latentStyles1.Append(latentStyleExceptionInfo71);
			latentStyles1.Append(latentStyleExceptionInfo72);
			latentStyles1.Append(latentStyleExceptionInfo73);
			latentStyles1.Append(latentStyleExceptionInfo74);
			latentStyles1.Append(latentStyleExceptionInfo75);
			latentStyles1.Append(latentStyleExceptionInfo76);
			latentStyles1.Append(latentStyleExceptionInfo77);
			latentStyles1.Append(latentStyleExceptionInfo78);
			latentStyles1.Append(latentStyleExceptionInfo79);
			latentStyles1.Append(latentStyleExceptionInfo80);
			latentStyles1.Append(latentStyleExceptionInfo81);
			latentStyles1.Append(latentStyleExceptionInfo82);
			latentStyles1.Append(latentStyleExceptionInfo83);
			latentStyles1.Append(latentStyleExceptionInfo84);
			latentStyles1.Append(latentStyleExceptionInfo85);
			latentStyles1.Append(latentStyleExceptionInfo86);
			latentStyles1.Append(latentStyleExceptionInfo87);
			latentStyles1.Append(latentStyleExceptionInfo88);
			latentStyles1.Append(latentStyleExceptionInfo89);
			latentStyles1.Append(latentStyleExceptionInfo90);
			latentStyles1.Append(latentStyleExceptionInfo91);
			latentStyles1.Append(latentStyleExceptionInfo92);
			latentStyles1.Append(latentStyleExceptionInfo93);
			latentStyles1.Append(latentStyleExceptionInfo94);
			latentStyles1.Append(latentStyleExceptionInfo95);
			latentStyles1.Append(latentStyleExceptionInfo96);
			latentStyles1.Append(latentStyleExceptionInfo97);
			latentStyles1.Append(latentStyleExceptionInfo98);
			latentStyles1.Append(latentStyleExceptionInfo99);
			latentStyles1.Append(latentStyleExceptionInfo100);
			latentStyles1.Append(latentStyleExceptionInfo101);
			latentStyles1.Append(latentStyleExceptionInfo102);
			latentStyles1.Append(latentStyleExceptionInfo103);
			latentStyles1.Append(latentStyleExceptionInfo104);
			latentStyles1.Append(latentStyleExceptionInfo105);
			latentStyles1.Append(latentStyleExceptionInfo106);
			latentStyles1.Append(latentStyleExceptionInfo107);
			latentStyles1.Append(latentStyleExceptionInfo108);
			latentStyles1.Append(latentStyleExceptionInfo109);
			latentStyles1.Append(latentStyleExceptionInfo110);
			latentStyles1.Append(latentStyleExceptionInfo111);
			latentStyles1.Append(latentStyleExceptionInfo112);
			latentStyles1.Append(latentStyleExceptionInfo113);
			latentStyles1.Append(latentStyleExceptionInfo114);
			latentStyles1.Append(latentStyleExceptionInfo115);
			latentStyles1.Append(latentStyleExceptionInfo116);
			latentStyles1.Append(latentStyleExceptionInfo117);
			latentStyles1.Append(latentStyleExceptionInfo118);
			latentStyles1.Append(latentStyleExceptionInfo119);
			latentStyles1.Append(latentStyleExceptionInfo120);
			latentStyles1.Append(latentStyleExceptionInfo121);
			latentStyles1.Append(latentStyleExceptionInfo122);
			latentStyles1.Append(latentStyleExceptionInfo123);
			latentStyles1.Append(latentStyleExceptionInfo124);
			latentStyles1.Append(latentStyleExceptionInfo125);
			latentStyles1.Append(latentStyleExceptionInfo126);
			latentStyles1.Append(latentStyleExceptionInfo127);
			latentStyles1.Append(latentStyleExceptionInfo128);
			latentStyles1.Append(latentStyleExceptionInfo129);
			latentStyles1.Append(latentStyleExceptionInfo130);
			latentStyles1.Append(latentStyleExceptionInfo131);
			latentStyles1.Append(latentStyleExceptionInfo132);
			latentStyles1.Append(latentStyleExceptionInfo133);
			latentStyles1.Append(latentStyleExceptionInfo134);
			latentStyles1.Append(latentStyleExceptionInfo135);
			latentStyles1.Append(latentStyleExceptionInfo136);
			latentStyles1.Append(latentStyleExceptionInfo137);

			Style style1 = new Style() { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true };
			StyleName styleName1 = new StyleName() { Val = "Normal" };
			PrimaryStyle primaryStyle1 = new PrimaryStyle();

			style1.Append(styleName1);
			style1.Append(primaryStyle1);

			Style style2 = new Style() { Type = StyleValues.Character, StyleId = "DefaultParagraphFont", Default = true };
			StyleName styleName2 = new StyleName() { Val = "Default Paragraph Font" };
			UIPriority uIPriority1 = new UIPriority() { Val = 1 };
			SemiHidden semiHidden1 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed1 = new UnhideWhenUsed();

			style2.Append(styleName2);
			style2.Append(uIPriority1);
			style2.Append(semiHidden1);
			style2.Append(unhideWhenUsed1);

			Style style3 = new Style() { Type = StyleValues.Table, StyleId = "TableNormal", Default = true };
			StyleName styleName3 = new StyleName() { Val = "Normal Table" };
			UIPriority uIPriority2 = new UIPriority() { Val = 99 };
			SemiHidden semiHidden2 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed2 = new UnhideWhenUsed();

			StyleTableProperties styleTableProperties1 = new StyleTableProperties();
			TableIndentation tableIndentation1 = new TableIndentation() { Width = 0, Type = TableWidthUnitValues.Dxa };

			TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
			TopMargin topMargin1 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
			TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin() { Width = 108, Type = TableWidthValues.Dxa };
			BottomMargin bottomMargin1 = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
			TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin() { Width = 108, Type = TableWidthValues.Dxa };

			tableCellMarginDefault1.Append(topMargin1);
			tableCellMarginDefault1.Append(tableCellLeftMargin1);
			tableCellMarginDefault1.Append(bottomMargin1);
			tableCellMarginDefault1.Append(tableCellRightMargin1);

			styleTableProperties1.Append(tableIndentation1);
			styleTableProperties1.Append(tableCellMarginDefault1);

			style3.Append(styleName3);
			style3.Append(uIPriority2);
			style3.Append(semiHidden2);
			style3.Append(unhideWhenUsed2);
			style3.Append(styleTableProperties1);

			Style style4 = new Style() { Type = StyleValues.Numbering, StyleId = "NoList", Default = true };
			StyleName styleName4 = new StyleName() { Val = "No List" };
			UIPriority uIPriority3 = new UIPriority() { Val = 99 };
			SemiHidden semiHidden3 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed3 = new UnhideWhenUsed();

			style4.Append(styleName4);
			style4.Append(uIPriority3);
			style4.Append(semiHidden3);
			style4.Append(unhideWhenUsed3);

			Style style5 = new Style() { Type = StyleValues.Table, StyleId = "TableGrid" };
			StyleName styleName5 = new StyleName() { Val = "Table Grid" };
			BasedOn basedOn1 = new BasedOn() { Val = "TableNormal" };
			UIPriority uIPriority4 = new UIPriority() { Val = 59 };
			Rsid rsid1 = new Rsid() { Val = "00B439FD" };

			StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
			SpacingBetweenLines spacingBetweenLines2 = new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

			styleParagraphProperties1.Append(spacingBetweenLines2);

			StyleTableProperties styleTableProperties2 = new StyleTableProperties();
			TableIndentation tableIndentation2 = new TableIndentation() { Width = 0, Type = TableWidthUnitValues.Dxa };

			TableBorders tableBorders1 = new TableBorders();
			TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
			LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
			BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
			RightBorder rightBorder1 = new RightBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
			InsideHorizontalBorder insideHorizontalBorder1 = new InsideHorizontalBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
			InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };

			tableBorders1.Append(topBorder1);
			tableBorders1.Append(leftBorder1);
			tableBorders1.Append(bottomBorder1);
			tableBorders1.Append(rightBorder1);
			tableBorders1.Append(insideHorizontalBorder1);
			tableBorders1.Append(insideVerticalBorder1);

			TableCellMarginDefault tableCellMarginDefault2 = new TableCellMarginDefault();
			TopMargin topMargin2 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
			TableCellLeftMargin tableCellLeftMargin2 = new TableCellLeftMargin() { Width = 108, Type = TableWidthValues.Dxa };
			BottomMargin bottomMargin2 = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
			TableCellRightMargin tableCellRightMargin2 = new TableCellRightMargin() { Width = 108, Type = TableWidthValues.Dxa };

			tableCellMarginDefault2.Append(topMargin2);
			tableCellMarginDefault2.Append(tableCellLeftMargin2);
			tableCellMarginDefault2.Append(bottomMargin2);
			tableCellMarginDefault2.Append(tableCellRightMargin2);

			styleTableProperties2.Append(tableIndentation2);
			styleTableProperties2.Append(tableBorders1);
			styleTableProperties2.Append(tableCellMarginDefault2);

			style5.Append(styleName5);
			style5.Append(basedOn1);
			style5.Append(uIPriority4);
			style5.Append(rsid1);
			style5.Append(styleParagraphProperties1);
			style5.Append(styleTableProperties2);


			styles1.Append(docDefaults1);
			styles1.Append(latentStyles1);
			styles1.Append(style1);
			styles1.Append(style2);
			styles1.Append(style3);
			styles1.Append(style4);
			styles1.Append(style5);

			//custom styles
			styles1.Append(GenerateTagStyle());
			styles1.Append(GenerateLockedContentStyle());
			styles1.Append(GenerateTransUnitStyle());
			styles1.Append(GenerateSegmentIdStyle());

			part.Styles = styles1;
		}
	}
}
