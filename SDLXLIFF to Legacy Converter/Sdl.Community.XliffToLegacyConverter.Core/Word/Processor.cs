using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Word;
using Sdl.Community.XliffReadWrite.SDLXLIFF;
using Comment = Sdl.Community.XliffReadWrite.SDLXLIFF.Comment;

namespace Sdl.Community.XliffToLegacyConverter.Core.Word
{
	internal class Processor
	{
		internal delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

		internal event ChangedEventHandler Progress;

		internal void OnProgress(int maximum, int current, string message)
		{
			if (Progress == null) return;
			try
			{
				var percent = Convert.ToInt32(current <= maximum && maximum > 0 ? (Convert.ToDecimal(current) / Convert.ToDecimal(maximum)) * Convert.ToDecimal(100) : maximum);

				Progress(maximum, current, percent, message);
			}
			catch
			{
				// ignored
			}
		}

		#region  |  Private Variables  |

		private object _oMissing = System.Reflection.Missing.Value;
		private object _oEndOfDoc = "\\endofdoc";

		private _Application _oWord;
		private _Document _oDoc;

		private object _tw4WinNormal;
		private object _tw4WinExternal = "tw4winExternal";
		private object _tw4WinInternal = "tw4winInternal";
		private object _tw4WinMark = "tw4winMark";

		public Style StyleTw4WinNormal { get; private set; }
		public Style StyleTw4WinExternal { get; private set; }
		public Style StyleTw4WinInternal { get; private set; }
		public Style StyleTw4WinMark { get; private set; }

		public int FoundIndexes { get; private set; }

		#endregion

		#region  |  Internal Methods     |

		private string _newGuid = string.Empty;

		internal int SegmentsImported;
		internal int SegmentsNotImported;

		internal Dictionary<string, ParagraphUnit> GetSegments(object oFileName)
		{
			SegmentsImported = 0;
			SegmentsNotImported = 0;

			var paragraphUnits = new Dictionary<string, ParagraphUnit>();

			try
			{
				#region  |  set progress info  |

				var indexMaximum = 100;



				var indexProcessingCurrent = 0;
				OnProgress(1000, indexProcessingCurrent, "Opening file...");


				#endregion

				_newGuid = Guid.NewGuid().ToString();

				#region  |  Open Document     |

				_oWord = new Application { Visible = false };


				_oWord.Application.Options.ConfirmConversions = false;

				_oDoc = _oWord.Documents.Open(ref oFileName, ref _oMissing,
					ref _oMissing, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing);


				_oWord.ActiveWindow.View.ShowAll = true;
				_oWord.ActiveWindow.View.ShowHiddenText = true;

				_oWord.Application.Options.BackgroundSave = false;

				_oWord.Application.Options.CheckGrammarAsYouType = false;
				_oWord.Application.Options.CheckGrammarWithSpelling = false;
				_oWord.Application.Options.CheckHangulEndings = false;
				_oWord.Application.Options.CheckSpellingAsYouType = false;
				_oWord.Application.Options.SuggestSpellingCorrections = false;

				_oWord.Application.Options.CreateBackup = false;
				_oWord.Application.Options.SaveInterval = 0;


				if (_oDoc.Revisions.Count > 0)
				{
					throw new Exception("Found revision tags in document; all revisions need to be accepted and/or rejected prior to processing");
				}


				_oDoc.ShowGrammaticalErrors = false;
				_oDoc.ShowRevisions = false;
				_oDoc.ShowSpellingErrors = false;
				_oDoc.GrammarChecked = false;
				_oDoc.SpellingChecked = false;
				_oDoc.TrackRevisions = false;
				_oDoc.AutoHyphenation = false;



				#endregion

				#region  |  Remove HyperLinks |

				var aHyperlinkRanges = new ArrayList();
				foreach (Hyperlink hpls in _oDoc.Hyperlinks)
				{
					try
					{
						aHyperlinkRanges.Add(hpls.Range);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}

				foreach (Range rng1 in aHyperlinkRanges)
					foreach (Hyperlink hpl in rng1.Hyperlinks)
						hpl.Delete();

				#endregion

				#region  |  Prepare Document  |

				object unit = WdUnits.wdStory;
				object extend = WdMovementType.wdMove;
				_oWord.Selection.HomeKey(ref unit, ref extend);

				object oStart = 0;
				object oEnd = (_oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End) + 1;
				var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
				var fnd = rngDoc.Find;




				#endregion

				#region  |  Prepare Styles    |


				SetExternalStyle();
				SetInternalStyle();
				SetMarkStyle();

				var includeLegacyStructure = true;

				var regFile = new Regex(@"\<file source\=""(?<x1>[^""]*)""\s+target\=""(?<x2>[^""]*)""\s+segmentCount\=""(?<x3>[^""]*)""\s+path\=""(?<x4>[^""]*)""(\s+includeLegacyStructure\=""(?<x5>[^""]*)""|)[^\>]*\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var mRegFile = regFile.Match(rngDoc.Text);
				if (mRegFile.Success)
				{
					var source = mRegFile.Groups["x1"].Value;
					var target = mRegFile.Groups["x2"].Value;
					var segmentCount = mRegFile.Groups["x3"].Value;
					var path = mRegFile.Groups["x4"].Value;
					var strIncludeLegacyStructure = mRegFile.Groups["x5"].Value;

					if (strIncludeLegacyStructure.Trim() != string.Empty)
						includeLegacyStructure = Convert.ToBoolean(strIncludeLegacyStructure);
				}

				if (includeLegacyStructure)
				{
					ExecuteReplaceAlltw4winExternal_START();
					ExecuteReplaceAlltw4winExternal_END();

					ExecuteReplaceAlltw4winInternal_Find();

					ExecuteReplaceAlltw4winMark_FindPercentage();
				}
				else
				{
					ExecuteReplaceAlltw4winMark_START();
					ExecuteReplaceAlltw4winMark_END();

					ExecuteReplaceAlltw4winInternal_Find();

					ExecuteReplaceAlltw4winMark_FindPercentage();
				}

				#endregion

				#region  |  Prepare Document  |

				unit = WdUnits.wdStory;
				extend = WdMovementType.wdMove;
				_oWord.Selection.HomeKey(ref unit, ref extend);

				oStart = 0;
				oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End + 1;
				rngDoc = _oDoc.Range(ref oStart, ref oEnd);
				fnd = rngDoc.Find;

				#endregion

				if (includeLegacyStructure)
				{
					var regSegs = new Regex(@"\<seg_[^\>]*\>.*?\<\/seg_[^\>]*\>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
					var mcRegSegs = regSegs.Matches(rngDoc.Text);
					indexMaximum = mcRegSegs.Count;
				}
				else
				{
					var regSegs = new Regex(@"\{0_STARTSEG_\>.*?\<_ENDSEG_0\}", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
					var mcRegSegs = regSegs.Matches(rngDoc.Text);
					indexMaximum = mcRegSegs.Count;
				}

				#region  |  Get Segments      |

				var sbAlerts = new StringBuilder();

				FoundIndexes = 0;
				var id = string.Empty;
				try
				{

					var regSeg = new Regex(@"\<seg_" + _newGuid + @" id\=""(?<x1>[^""]*)""\s+pid\=""(?<x2>[^""]*)""\s+status\=""(?<x3>[^""]*)""\s+locked\=""(?<x4>[^""]*)""\s+match\=""(?<x5>[^""]*)""[^\>]*\>" + "(\r|\n|\r\n)" + @"(?<x6>.*?)" + "(\r|\n|\r\n)" + @"\<\/seg_" + _newGuid + @"\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					var regInnerFile = new Regex(@"\<innerFile name\=""(?<x1>[^""]*)""[^\>]*\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					var regPercentage = new Regex(@"\<\}(?<x1>[^\{]*)\{\>$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					var regSource = new Regex(@"\{0\>(?<xSource>.*?)\<\}(?<xPercentage>[^\{]*)\{\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					var regInternal = new Regex(@"\<intern_" + _newGuid + @"\>(?<tag>.*?)\</intern_" + _newGuid + @"\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

					fnd.ClearFormatting();
					if (includeLegacyStructure)
					{
						fnd.Text = @"(\<seg_" + _newGuid + @" )(*)(\<\}[0-9]@\{\>)(*)(\<\/seg_" + _newGuid + @"\>)";
					}
					else
					{
						fnd.Text = @"(\{0_STARTSEG_\>)(*)(\<\}[0-9]@\{\>)(*)(\<_ENDSEG_0\})";
					}

					fnd.Forward = true;
					fnd.MatchWildcards = true;

					var rGetSegs01 = new Regex(
						@"(\{0\>)"
						+ @"(?<source>.*?|)"
						+ @"\<pcnt_" + _newGuid + @"\>"
						+ @"\<\}(?<percentage>[^\{]*)\{\>"
						+ @"\<\/pcnt_" + _newGuid + @"\>"
						+ @"(?<target>.*?|)"
						+ @"(\<0\})"
						, RegexOptions.Singleline);

					var rGetSegs02 = new Regex(
						@"(\{0_STARTSEG_\>)"
						+ @"(?<source>.*?|)"
						+ @"\<pcnt_" + _newGuid + @"\>"
						+ @"\<\}(?<percentage>[^\{]*)\{\>"
						+ @"\<\/pcnt_" + _newGuid + @"\>"
						+ @"(?<target>.*?|)"
						+ @"(\<_ENDSEG_0\})"
						, RegexOptions.Singleline);

					ExecuteFind(fnd);
					while (fnd.Found)
					{
						if (includeLegacyStructure)
						{
							#region  |  with includeLegacyStructure  |

							var mRegSeg = regSeg.Match(rngDoc.Text);

							if (mRegSeg.Success)
							{
								id = mRegSeg.Groups["x1"].Value;
								var paragraphId = mRegSeg.Groups["x2"].Value;
								var segmentStatus = Core.Processor.GetSegmentStatusFromVisual(mRegSeg.Groups["x3"].Value);
								var isLocked = Convert.ToBoolean(mRegSeg.Groups["x4"].Value);
								var matchTypeId = mRegSeg.Groups["x5"].Value;

								#region  |  check filter  |

								var importTranslation = true;

								if (isLocked && XliffReadWrite.Processor.ProcessorSettings.DoNotImportLocked)
									importTranslation = false;
								else if (!isLocked && XliffReadWrite.Processor.ProcessorSettings.DoNotImportUnLocked)
									importTranslation = false;
								else if (string.Compare(matchTypeId, "Perfect Match", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportPerfectMatch)
									importTranslation = false;
								else if (string.Compare(matchTypeId, "Context Match", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportContextMatch)
									importTranslation = false;
								else if (string.Compare(matchTypeId, "Exact Match", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportExactMatch)
									importTranslation = false;
								else if (string.Compare(matchTypeId, "Fuzzy Match", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportFuzzyMatch)
									importTranslation = false;
								else if (string.Compare(matchTypeId, "No Match", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportNoMatch)
									importTranslation = false;

								if (string.Compare(segmentStatus, "Not Translated", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportNotTranslated)
									importTranslation = false;
								else if (string.Compare(segmentStatus, "Draft", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportDraft)
									importTranslation = false;
								else if (string.Compare(segmentStatus, "Translated", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslated)
									importTranslation = false;
								else if (string.Compare(segmentStatus, "Translation Approved", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationApproved)
									importTranslation = false;
								else if (string.Compare(segmentStatus, "Translation Rejected", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationRejected)
									importTranslation = false;
								else if (string.Compare(segmentStatus, "Sign-off Rejected", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOffRejected)
									importTranslation = false;
								else if (string.Compare(segmentStatus, "Sign-off", StringComparison.OrdinalIgnoreCase) == 0 && XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOff)
									importTranslation = false;

								#endregion

								if (importTranslation)
								{
									#region  |  importTranslation  |

									var content = mRegSeg.Groups["x6"].Value;
									var mcRGetSegs = rGetSegs01.Matches(content);
									if (mcRGetSegs.Count > 0)
									{										
										var sourceSections = new List<SegmentSection>();
										var targetSections = new List<SegmentSection>();
										var segmentPairs = new List<SegmentPair>();
										var comments = new List<Comment>();
										var percentage = string.Empty;
										var paragraphUnit = new ParagraphUnit(paragraphId, segmentPairs, "");
										var segmentPair = new SegmentPair
										{
											Id = id,
											IsLocked = isLocked,
											Comments = comments,
											SegmentStatus = segmentStatus,
											TranslationOrigin = new TranslationOrigin()
										};

										foreach (Match mRGetSegs in mcRGetSegs)
										{
											var sourceText = mRGetSegs.Groups["source"].Value;
											percentage = mRGetSegs.Groups["percentage"].Value;
											var targetText = mRGetSegs.Groups["target"].Value;
											
											segmentPair.TranslationOrigin.MatchPercentage =  GetPercentage(percentage);

											AddContentSections(regInternal, sourceText, sourceSections);
											AddContentSections(regInternal, targetText, targetSections);
										}

										segmentPair.TranslationOrigin.MatchPercentage = GetPercentage(percentage);

										UpdateTagSections(sourceSections);
										UpdateTagSections(targetSections);

										segmentPair.Source = Core.Processor.GetSectionsToText(sourceSections);
										segmentPair.SourceSections = sourceSections;

										segmentPair.Target = Core.Processor.GetSectionsToText(targetSections);
										segmentPair.TargetSections = targetSections;

										if (segmentPair.Target.Trim() == string.Empty && XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations)
										{
											SegmentsNotImported++;
										}
										else
										{
											SegmentsImported++;

											paragraphUnit.SegmentPairs.Add(segmentPair);

											if (paragraphId != string.Empty)
											{
												if (!paragraphUnits.ContainsKey(paragraphId))
													paragraphUnits.Add(paragraphId, paragraphUnit);
												else
												{
													paragraphUnits[paragraphId].SegmentPairs.AddRange(paragraphUnit.SegmentPairs);
												}
											}
										}
									}
									else
									{
										throw new Exception("Unable to parse content; missing markup for segment id: " + id + "");
									}
									#endregion
								}
								else
								{
									SegmentsNotImported++;
								}
							}
							else
							{
								throw new Exception("Error reading segment: " + rngDoc.Text + "");
							}
							#endregion
						}
						else
						{
							#region  |  without includeLegacyStructure  |

							var mRGetSegs = rGetSegs02.Match(rngDoc.Text);

							if (mRGetSegs.Success)
							{
								#region  |  Check for errors  |

								if (mRGetSegs.Groups["source"].Value.IndexOf(@"{0_STARTSEG_>", StringComparison.Ordinal) > -1
									|| mRGetSegs.Groups["target"].Value.IndexOf(@"{0_STARTSEG_>", StringComparison.Ordinal) > -1
									|| mRGetSegs.Groups["source"].Value.IndexOf(@"<_ENDSEG_0}", StringComparison.Ordinal) > -1
									|| mRGetSegs.Groups["target"].Value.IndexOf(@"<_ENDSEG_0}", StringComparison.Ordinal) > -1
									)
								{
									throw new Exception("Error parsing trados tag markup styles for segment:\r\n" + rngDoc.Text + "\r\n\r\n");
								}
								if (mRGetSegs.Groups["source"].Value.IndexOf(@"{0>", StringComparison.Ordinal) > -1
									|| mRGetSegs.Groups["target"].Value.IndexOf(@"{0>", StringComparison.Ordinal) > -1
									|| mRGetSegs.Groups["source"].Value.IndexOf(@"<0}", StringComparison.Ordinal) > -1
									|| mRGetSegs.Groups["target"].Value.IndexOf(@"<0}", StringComparison.Ordinal) > -1)
								{
									sbAlerts.Append("Alert: possile error parsing trados tag markup styles for segment:\r\n" + rngDoc.Text + "\r\n\r\n");
								}

								#endregion

								var sourceSections = new List<SegmentSection>();
								var targetSections = new List<SegmentSection>();
								var segmentPairs = new List<SegmentPair>();
								var comments = new List<Comment>();
								var paragraphUnit = new ParagraphUnit(string.Empty, segmentPairs, string.Empty);

								var segmentPair = new SegmentPair
								{
									Id = string.Empty,
									IsLocked = false,
									Comments = comments,
									SegmentStatus = string.Empty
								};

								var translationOrigin = new TranslationOrigin();
								segmentPair.TranslationOrigin = translationOrigin;

								var sourceText = mRGetSegs.Groups["source"].Value;
								var percentage = mRGetSegs.Groups["percentage"].Value;
								var targetText = mRGetSegs.Groups["target"].Value;

								segmentPair.TranslationOrigin.MatchPercentage = GetPercentage(percentage);

								AddContentSections(regInternal, sourceText, sourceSections);
								AddContentSections(regInternal, targetText, targetSections);

								UpdateTagSections(sourceSections);
								UpdateTagSections(targetSections);

								segmentPair.Source = Core.Processor.GetSectionsToText(sourceSections);
								segmentPair.SourceSections = sourceSections;

								segmentPair.Target = Core.Processor.GetSectionsToText(targetSections);
								segmentPair.TargetSections = targetSections;

								if (segmentPair.Target.Trim() == string.Empty && XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations)
								{
									SegmentsNotImported++;
								}
								else
								{
									SegmentsImported++;

									paragraphUnit.SegmentPairs.Add(segmentPair);


									if (!paragraphUnits.ContainsKey(string.Empty))
										paragraphUnits.Add(string.Empty, paragraphUnit);
									else
									{
										paragraphUnits[string.Empty].SegmentPairs.AddRange(paragraphUnit.SegmentPairs);
									}
								}

								id = indexProcessingCurrent.ToString();
							}
							else
							{
								throw new Exception(string.Format("Error reading segment: {0}", rngDoc.Text));
							}

							#endregion
						}

						indexProcessingCurrent++;

						OnProgress(indexMaximum, indexProcessingCurrent, string.Format("Processing segment id: {0}", id));

						ExecuteFind(fnd);
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("Error while reading the word document near segment id: {0}", id) + "\r\n\r\n" + ex.Message);
				}

				#endregion

				if (!includeLegacyStructure)
				{
					//check for alerts
					if (sbAlerts.ToString().Trim() != string.Empty)
					{
						throw new Exception(sbAlerts.ToString());
					}
				}
			}
			catch (Exception)
			{
				WpKillWordApp();
				throw;

			}
			finally
			{
				#region  |  Close Document    |

				_oWord.ActiveWindow.View.ShowHiddenText = false;

				object oSave = false;
				if (_oDoc != null)
				{
					_oDoc.Close(ref oSave, ref _oMissing, ref _oMissing);
					_oDoc = null;
				}


				if (_oWord != null)
				{
					_oWord.Quit(ref oSave, ref _oMissing, ref _oMissing);
					_oWord = null;
				}

				#endregion
			}

			return paragraphUnits;
		}

		private static void UpdateTagSections(IList<SegmentSection> sections)
		{
			var tagUnits = new List<TagUnit>();
			var index = 10000;
			for (var i = 0; i < sections.Count; i++)
			{
				var section = sections[i];
				switch (section.Type)
				{
					case SegmentSection.ContentType.Tag:
						{
							index++;
							var tagId = "xpt" + index;
							var tagName = Core.Processor.GetStartTagName(section.Content, ref tagId);

							tagUnits.Add(new TagUnit(tagId, tagName, section.Content, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
							section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, tagUnits[tagUnits.Count - 1].Id);
						}
						break;
					case SegmentSection.ContentType.TagClosing:
						var closingTagName = Core.Processor.GetEndTagName(section.Content);
						if (tagUnits.Count > 0)
						{
							if (string.Compare(tagUnits[tagUnits.Count - 1].Name, closingTagName, StringComparison.OrdinalIgnoreCase) == 0)
							{
								section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, tagUnits[tagUnits.Count - 1].Id);
								tagUnits.RemoveAt(tagUnits.Count - 1);
							}
							else
							{
								section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, string.Empty);
							}
						}
						else
						{
							section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, string.Empty);
						}

						break;
					default:
						{
							index++;
							var tagId = "xpt" + index;
							Core.Processor.GetStartTagName(section.Content, ref tagId);
							section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, tagId);
						}
						break;
				}

				sections[i] = section;
			}
		}

		private static void AddContentSections(Regex regexInternalTags, string content, ICollection<SegmentSection> contentSections)
		{

			var matches = regexInternalTags.Matches(content);
			var lastIndex = 0;

			foreach (Match match in matches)
			{
				var matchIndex = match.Index;

				if (matchIndex > lastIndex)
				{
					var text = content.Substring(lastIndex, (matchIndex - lastIndex)).Replace("\v", "\n");
					contentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, "", text));
				}

				lastIndex = match.Index + match.Length;

				var tags = Core.Processor.SeperateTags(match.Groups["tag"].Value);

				foreach (var tag in tags)
				{
					switch (tag.Type)
					{
						case TagUnit.TagUnitType.IsPlaceholder:
							{
								contentSections.Add(new SegmentSection(SegmentSection.ContentType.Placeholder, tag.Id, tag.Content));
							}
							break;
						case TagUnit.TagUnitType.IsTag:
							{
								switch (tag.State)
								{
									case TagUnit.TagUnitState.IsOpening:
										{
											contentSections.Add(new SegmentSection(SegmentSection.ContentType.Tag, tag.Id, tag.Content.Replace("\v", "\r\n")));
											break;
										}
									case TagUnit.TagUnitState.IsClosing:
										{
											contentSections.Add(new SegmentSection(SegmentSection.ContentType.TagClosing, tag.Id, tag.Content.Replace("\v", "\r\n")));
											break;
										}
									case TagUnit.TagUnitState.IsEmpty:
										{
											contentSections.Add(new SegmentSection(SegmentSection.ContentType.Placeholder, tag.Id, tag.Content.Replace("\v", "\r\n")));
											break;
										}
								}
							}
							break;
						case TagUnit.TagUnitType.IsLockedContent:
							{
								contentSections.Add(new SegmentSection(SegmentSection.ContentType.LockedContent, tag.Id, tag.Content.Replace("\v", "\r\n")));
							}
							break;
					}
				}
			}

			if (lastIndex < content.Length)
			{
				var text = content.Substring(lastIndex).Replace("\v", "\n");
				contentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, "", text));
			}
		}

		private static int GetPercentage(string percentage)
		{
			if (percentage != string.Empty)
			{
				try
				{
					var success = int.TryParse(percentage, out var percent);
					if (success)
					{
						return percent;
					}
				}
				catch
				{
					// catch all; ignore
				}
			}

			return 0;
		}

		internal void SaveAsDocument(object oFileName, bool saveAsDocX)
		{
			try
			{
				#region  |  Open Document     |

				_oWord = new Application { Visible = false };


				_oWord.Application.Options.ConfirmConversions = false;

				_oDoc = _oWord.Documents.Open(ref oFileName, ref _oMissing,
					ref _oMissing, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing
					, ref _oMissing, ref _oMissing);


				_oWord.ActiveWindow.View.ShowAll = true;
				_oWord.ActiveWindow.View.ShowHiddenText = false;

				_oWord.Application.Options.BackgroundSave = false;

				_oWord.Application.Options.CheckGrammarAsYouType = false;
				_oWord.Application.Options.CheckGrammarWithSpelling = false;
				_oWord.Application.Options.CheckHangulEndings = false;
				_oWord.Application.Options.CheckSpellingAsYouType = false;
				_oWord.Application.Options.SuggestSpellingCorrections = false;

				_oWord.Application.Options.CreateBackup = false;
				_oWord.Application.Options.SaveInterval = 0;


				if (_oDoc.Revisions.Count > 0)
				{
					throw new Exception("Found revision tags in document; all revisions need to be accepted and/or rejected prior to processing");
				}


				_oDoc.ShowGrammaticalErrors = false;
				_oDoc.ShowRevisions = false;
				_oDoc.ShowSpellingErrors = false;
				_oDoc.GrammarChecked = false;
				_oDoc.SpellingChecked = false;
				_oDoc.TrackRevisions = false;
				_oDoc.AutoHyphenation = false;



				#endregion

				if (saveAsDocX)
				{
					oFileName = oFileName.ToString().Substring(0, oFileName.ToString().Length - 4) + ".docx";
					object fileFormat = WdSaveFormat.wdFormatXMLDocument;

					_oDoc.SaveAs(oFileName, ref fileFormat, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing);
				}
				else
				{
					oFileName = oFileName.ToString().Substring(0, oFileName.ToString().Length - 4) + ".doc";
					object fileFormat = WdSaveFormat.wdFormatDocument97;

					_oDoc.SaveAs(oFileName, ref fileFormat, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing);
				}
			}
			catch (Exception ex)
			{
				WpKillWordApp();
				throw;
			}
			finally
			{
				#region  |  Close Document    |

				object oSave = true;
				if (_oDoc != null)
				{
					_oDoc.Close(ref oSave, ref _oMissing, ref _oMissing);
					_oDoc = null;
				}


				if (_oWord != null)
				{
					_oWord.Quit(ref oSave, ref _oMissing, ref _oMissing);
					_oWord = null;
				}



				#endregion
			}
		}

		internal void WpKillWordApp()
		{
			try
			{
				var processes = Process.GetProcessesByName("WINWORD");
				foreach (var p in processes)
				{
					if (p.MainWindowTitle.Trim() == ""
						|| string.CompareOrdinal(p.MainWindowTitle.Trim()
						, "Microsoft Word") == 0)
						p.Kill();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		#endregion

		#region  |  Private Methods    |


		private bool ExecuteReplace(Find find, object replaceWithOption)
		{
			return ExecuteReplace(find, WdReplace.wdReplaceOne, replaceWithOption);
		}
		private static bool ExecuteReplace(Find find, object replaceOption, object replaceWithOption)
		{
			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = replaceWithOption;
			var replace = replaceOption;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private static bool ExecuteFind(Find find)
		{
			return ExecuteFind(find, Type.Missing, Type.Missing);
		}

		private static bool ExecuteFind(Find find, object wrapFind, object forwardFind)
		{
			// Simple wrapper around Find.Execute:
			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = forwardFind;
			var wrap = wrapFind;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			var replace = Type.Missing;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAlltw4winInternal_Find()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinInternal);
			find.Text = "";
			find.Replacement.Text = "<intern_" + _newGuid + @">^&</intern_" + _newGuid + ">";
			find.Replacement.set_Style(ref _tw4WinInternal);
			find.Format = true;
			find.Forward = true;

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAlltw4winMark_FindPercentage()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinMark);
			find.Text = @"\<\}*\{\>";
			find.MatchWildcards = true;
			find.Format = true;
			find.Forward = true;
			find.Replacement.Text = @"<pcnt_" + _newGuid + @">^&</pcnt_" + _newGuid + @">";
			find.Replacement.set_Style(ref _tw4WinMark);

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			object matchWildcards = true;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAlltw4winExternal_START()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinExternal);
			find.Text = "<seg ";
			find.Replacement.Text = "<seg_" + _newGuid + " ";
			find.Replacement.set_Style(ref _tw4WinExternal);
			find.Format = true;
			find.Forward = true;

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAlltw4winExternal_END()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinExternal);
			find.Text = "</seg>";
			find.Replacement.Text = "</seg_" + _newGuid + ">";
			find.Replacement.set_Style(ref _tw4WinExternal);
			find.Format = true;
			find.Forward = true;

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAlltw4winMark_START()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinMark);
			find.Text = "{0>";
			find.Replacement.Text = "{0_STARTSEG_>";
			find.Replacement.set_Style(ref _tw4WinMark);
			find.Format = true;
			find.Forward = true;

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAlltw4winMark_END()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinMark);
			find.Text = "<0}";
			find.Replacement.Text = "<_ENDSEG_0}";
			find.Replacement.set_Style(ref _tw4WinMark);
			find.Format = true;
			find.Forward = true;

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAll_START_tw4winMark()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinMark);
			find.Text = "{0_STARTSEG_>";
			find.Replacement.Text = "{0>";
			find.Replacement.set_Style(ref _tw4WinMark);
			find.Format = true;


			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private bool ExecuteReplaceAll_END_tw4winMark()
		{
			object unit = WdUnits.wdStory;
			object extend = WdMovementType.wdMove;
			_oWord.Selection.HomeKey(ref unit, ref extend);

			object oStart = 0;
			object oEnd = _oDoc.Bookmarks.get_Item(ref _oEndOfDoc).End;
			var rngDoc = _oDoc.Range(ref oStart, ref oEnd);
			var find = rngDoc.Find;

			find.ClearFormatting();

			find.set_Style(ref _tw4WinMark);
			find.Text = "<_ENDSEG_0}";
			find.Replacement.Text = "<0}";
			find.Replacement.set_Style(ref _tw4WinMark);
			find.Format = true;

			var findText = Type.Missing;
			var matchCase = Type.Missing;
			var matchWholeWord = Type.Missing;
			var matchWildcards = Type.Missing;
			var matchSoundsLike = Type.Missing;
			var matchAllWordForms = Type.Missing;
			var forward = Type.Missing;
			var wrap = Type.Missing;
			var format = Type.Missing;
			var replaceWith = Type.Missing;
			object replace = WdReplace.wdReplaceAll;
			var matchKashida = Type.Missing;
			var matchDiacritics = Type.Missing;
			var matchAlefHamza = Type.Missing;
			var matchControl = Type.Missing;

			return find.Execute(ref findText, ref matchCase,
				ref matchWholeWord, ref matchWildcards, ref matchSoundsLike,
				ref matchAllWordForms, ref forward, ref wrap, ref format,
				ref replaceWith, ref replace, ref matchKashida,
				ref matchDiacritics, ref matchAlefHamza, ref matchControl);
		}

		private int IndexNormalStart(int start)
		{
			_oWord.Selection.Start = start;

			var objFind = _oWord.Selection.Find;
			objFind.ClearFormatting();

			objFind.Text = @"(\<\}[0-9]@\{\>)(*)(\<_ENDSEG_0\})";
			objFind.Replacement.Text = "";
			objFind.Forward = true;
			objFind.Wrap = WdFindWrap.wdFindStop;
			objFind.Format = true;
			objFind.MatchCase = false;
			objFind.MatchWholeWord = false;
			objFind.MatchWildcards = true;
			objFind.MatchSoundsLike = false;
			objFind.MatchAllWordForms = false;

			if (objFind.Execute(ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing) == true)
				return _oWord.Selection.Start;
			return -1;
		}

		private bool GetNormalStyle()
		{
			foreach (Style sty in _oDoc.Styles)
			{
				if (!sty.InUse || sty.Hidden)
					continue;

				if (string.CompareOrdinal(sty.NameLocal, "default paragraph font") != 0
					&& string.CompareOrdinal(sty.NameLocal, "normal") != 0
					&& !sty.NameLocal.ToLower().StartsWith("default paragraph font"))
					continue;

				StyleTw4WinNormal = sty;
				_tw4WinNormal = sty;
				break;
			}
			return _tw4WinNormal != null;
		}

		private bool SetExternalStyle()
		{
			var foundTw4WinExternal = false;
			foreach (Style sty in _oDoc.Styles)
			{
				if (string.CompareOrdinal(sty.NameLocal.Trim(), "tw4winExternal") != 0)
					continue;
				StyleTw4WinExternal = sty;
				foundTw4WinExternal = true;
				break;
			}
			if (foundTw4WinExternal)
			{
				return true;
			}

			#region  |  Add tw4winExternal |

			object wdStylechar = WdStyleType.wdStyleTypeCharacter;

			var dummyStyle = _oDoc.Styles.Add((string)_tw4WinExternal, ref wdStylechar);

			dummyStyle.Font.Color = WdColor.wdColorGray50;
			dummyStyle.Font.ColorIndex = WdColorIndex.wdGray50;
			dummyStyle.Font.ColorIndexBi = WdColorIndex.wdGray50;

			dummyStyle.Font.Bold = 0;
			dummyStyle.Font.BoldBi = 0;

			dummyStyle.Font.DiacriticColor = WdColor.wdColorAutomatic;
			dummyStyle.Font.DisableCharacterSpaceGrid = false;
			dummyStyle.Font.DoubleStrikeThrough = 0;
			dummyStyle.Font.Emboss = 0;

			dummyStyle.Font.EmphasisMark = WdEmphasisMark.wdEmphasisMarkNone;
			dummyStyle.Font.Engrave = 0;
			dummyStyle.Font.Hidden = 0;
			dummyStyle.Font.Italic = 0;
			dummyStyle.Font.ItalicBi = 0;
			dummyStyle.Font.Kerning = 0;

			dummyStyle.Font.Name = "Courier New";
			dummyStyle.Font.NameAscii = "Courier New";
			dummyStyle.Font.NameBi = "Courier New";
			dummyStyle.Font.NameOther = "Courier New";

			dummyStyle.Font.Outline = 0;
			dummyStyle.Font.Position = 0;
			dummyStyle.Font.Scaling = 100;

			dummyStyle.Font.Shadow = 0;
			dummyStyle.Font.Size = 12;
			dummyStyle.Font.SizeBi = 12;

			dummyStyle.Font.SmallCaps = 0;
			dummyStyle.Font.Spacing = 0;
			dummyStyle.Font.StrikeThrough = 0;
			dummyStyle.Font.Subscript = 0;
			dummyStyle.Font.Superscript = 0;

			dummyStyle.Font.Underline = WdUnderline.wdUnderlineNone;
			dummyStyle.Font.UnderlineColor = WdColor.wdColorAutomatic;

			dummyStyle.Hidden = false;

			dummyStyle.NoProofing = -1;

			StyleTw4WinExternal = dummyStyle;

			#endregion

			return false;
		}
		private bool SetInternalStyle()
		{
			var foundTw4WinInternal = false;
			foreach (Style sty in _oDoc.Styles)
			{
				if (string.CompareOrdinal(sty.NameLocal.Trim(), "tw4winInternal") != 0)
					continue;
				StyleTw4WinInternal = sty;
				foundTw4WinInternal = true;
				break;
			}
			if (foundTw4WinInternal)
			{
				return true;
			}

			#region  |  Add tw4winInternal |

			object wdStylechar = WdStyleType.wdStyleTypeCharacter;

			var dummyStyle = _oDoc.Styles.Add((string)_tw4WinInternal, ref wdStylechar);

			dummyStyle.Font.Color = WdColor.wdColorRed;
			dummyStyle.Font.ColorIndex = WdColorIndex.wdRed;
			dummyStyle.Font.ColorIndexBi = WdColorIndex.wdRed;

			dummyStyle.Font.Bold = 0;
			dummyStyle.Font.BoldBi = 0;

			dummyStyle.Font.DiacriticColor = WdColor.wdColorAutomatic;
			dummyStyle.Font.DisableCharacterSpaceGrid = false;
			dummyStyle.Font.DoubleStrikeThrough = 0;
			dummyStyle.Font.Emboss = 0;

			dummyStyle.Font.EmphasisMark = WdEmphasisMark.wdEmphasisMarkNone;
			dummyStyle.Font.Engrave = 0;
			dummyStyle.Font.Hidden = 0;
			dummyStyle.Font.Italic = 0;
			dummyStyle.Font.ItalicBi = 0;
			dummyStyle.Font.Kerning = 0;

			dummyStyle.Font.Name = "Courier New";
			dummyStyle.Font.NameAscii = "Courier New";
			dummyStyle.Font.NameBi = "Courier New";
			dummyStyle.Font.NameOther = "Courier New";

			dummyStyle.Font.Outline = 0;

			dummyStyle.Font.Position = 0;
			dummyStyle.Font.Scaling = 100;

			dummyStyle.Font.Shadow = 0;
			dummyStyle.Font.Size = 12;
			dummyStyle.Font.SizeBi = 12;

			dummyStyle.Font.SmallCaps = 0;
			dummyStyle.Font.Spacing = 0;
			dummyStyle.Font.StrikeThrough = 0;
			dummyStyle.Font.Subscript = 0;
			dummyStyle.Font.Superscript = 0;

			dummyStyle.Font.Underline = WdUnderline.wdUnderlineNone;
			dummyStyle.Font.UnderlineColor = WdColor.wdColorAutomatic;

			dummyStyle.Hidden = false;

			dummyStyle.NoProofing = -1;

			StyleTw4WinInternal = dummyStyle;

			#endregion

			return false;
		}
		private bool SetMarkStyle()
		{

			var foundTw4WinMark = false;
			foreach (Style sty in _oDoc.Styles)
			{
				if (string.CompareOrdinal(sty.NameLocal.Trim(), "tw4winMark") != 0)
					continue;

				StyleTw4WinMark = sty;
				foundTw4WinMark = true;
				break;
			}
			if (foundTw4WinMark)
			{
				return true;
			}

			#region  |  Add tw4winMark    |

			object wdStylechar = WdStyleType.wdStyleTypeCharacter;

			var dummyStyle = _oDoc.Styles.Add((string)_tw4WinMark, ref wdStylechar);

			dummyStyle.Font.Color = WdColor.wdColorViolet;
			dummyStyle.Font.ColorIndex = WdColorIndex.wdViolet;
			dummyStyle.Font.ColorIndexBi = WdColorIndex.wdViolet;

			dummyStyle.Font.Bold = 0;
			dummyStyle.Font.BoldBi = 0;

			dummyStyle.Font.DiacriticColor = WdColor.wdColorAutomatic;
			dummyStyle.Font.DisableCharacterSpaceGrid = false;
			dummyStyle.Font.DoubleStrikeThrough = 0;
			dummyStyle.Font.Emboss = 0;

			dummyStyle.Font.EmphasisMark = WdEmphasisMark.wdEmphasisMarkNone;
			dummyStyle.Font.Engrave = 0;
			dummyStyle.Font.Hidden = -1;
			dummyStyle.Font.Italic = 0;
			dummyStyle.Font.ItalicBi = 0;
			dummyStyle.Font.Kerning = 0;

			dummyStyle.Font.Name = "Courier New";
			dummyStyle.Font.NameAscii = "Courier New";
			dummyStyle.Font.NameBi = "Courier New";
			dummyStyle.Font.NameOther = "Courier New";

			dummyStyle.Font.Outline = 0;

			dummyStyle.Font.Position = 0;
			dummyStyle.Font.Scaling = 100;

			dummyStyle.Font.Shadow = 0;
			dummyStyle.Font.Size = 12;
			dummyStyle.Font.SizeBi = 12;

			dummyStyle.Font.SmallCaps = 0;
			dummyStyle.Font.Spacing = 0;
			dummyStyle.Font.StrikeThrough = 0;
			dummyStyle.Font.Subscript = -1;
			dummyStyle.Font.Superscript = 0;

			dummyStyle.Font.Underline = WdUnderline.wdUnderlineNone;
			dummyStyle.Font.UnderlineColor = WdColor.wdColorAutomatic;

			dummyStyle.Hidden = false;

			dummyStyle.NoProofing = 0;

			StyleTw4WinMark = dummyStyle;

			#endregion

			return false;
		}

		private void RemoveAllTradosStyles()
		{
			ReplaceAllHiddenSegStart();
			ReplaceAllHiddenEnding();
			ReplaceAllHiddenStart();

			RemoveTradosStyles();
		}

		private void ReplaceAllHiddenSegStart()
		{
			object obj1 = WdUnits.wdStory;
			object obj2 = WdMovementType.wdMove;
			object obj3 = WdReplace.wdReplaceAll;

			var objFind = _oWord.Selection.Find;

			_oWord.Selection.HomeKey(ref obj1, ref obj2);

			objFind.ClearFormatting();
			objFind.Text = @"\{0_STARTSEG_\>*\<\}[0-9]@\{\>";
			objFind.Replacement.Text = "";
			objFind.Forward = true;
			objFind.Wrap = WdFindWrap.wdFindContinue;
			objFind.Format = false;
			objFind.MatchCase = false;
			objFind.MatchWholeWord = false;
			objFind.MatchWildcards = true;
			objFind.MatchSoundsLike = false;
			objFind.MatchAllWordForms = false;

			objFind.Execute(ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref obj3, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing);
		}
		private void ReplaceAllHiddenEnding()
		{
			object obj1 = WdUnits.wdStory;
			object obj2 = WdMovementType.wdMove;
			object obj3 = WdReplace.wdReplaceAll;

			var objFind = _oWord.Selection.Find;

			_oWord.Selection.HomeKey(ref obj1, ref obj2);

			objFind.ClearFormatting();
			objFind.Text = @"\<_ENDSEG_0\}";
			objFind.Replacement.Text = "";
			objFind.Forward = true;
			objFind.Wrap = WdFindWrap.wdFindContinue;
			objFind.Format = false;
			objFind.MatchCase = false;
			objFind.MatchWholeWord = false;
			objFind.MatchWildcards = true;
			objFind.MatchSoundsLike = false;
			objFind.MatchAllWordForms = false;

			objFind.Execute(ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref obj3, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing);

		}
		private void ReplaceAllHiddenStart()
		{
			object obj1 = WdUnits.wdStory;
			object obj2 = WdMovementType.wdMove;
			object obj3 = WdReplace.wdReplaceAll;

			var objFind = _oWord.Selection.Find;

			_oWord.Selection.HomeKey(ref obj1, ref obj2);

			objFind.ClearFormatting();
			objFind.Text = @"\{0_STARTSEG_\>";
			objFind.Replacement.Text = "";
			objFind.Forward = true;
			objFind.Wrap = WdFindWrap.wdFindContinue;
			objFind.Format = false;
			objFind.MatchCase = false;
			objFind.MatchWholeWord = false;
			objFind.MatchWildcards = true;
			objFind.MatchSoundsLike = false;
			objFind.MatchAllWordForms = false;

			objFind.Execute(ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing, ref obj3, ref _oMissing, ref _oMissing, ref _oMissing, ref _oMissing);
		}

		private void RemoveTradosStyles()
		{
			StyleTw4WinExternal?.Delete();
			StyleTw4WinInternal?.Delete();
			StyleTw4WinMark?.Delete();
		}

		#endregion
	}
}
