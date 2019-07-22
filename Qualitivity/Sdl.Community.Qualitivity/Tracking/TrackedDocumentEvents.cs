using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Comparison;
using Sdl.Community.Hooks;
using Sdl.Community.Qualitivity.Panels.QualityMetrics;
using Sdl.Community.Structures.Documents.Records;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Tracking
{
	public class TrackedDocumentEvents
	{
		public static long GetTargetCursorPosition()
		{
			if (Tracked.StudioWindow.InvokeRequired)
			{
				return (long)Tracked.StudioWindow.Invoke(new Func<long>(GetTargetCursorPosition));
			}

			return Tracked.ActiveDocument.Selection.Target.From != null ? Tracked.ActiveDocument.Selection.Target.From.CursorPosition : 0;
		}

		public static void TranslationOriginChanged(object sender, EventArgs e)
		{
			if (Tracked.ActiveDocument == null)
			{
				return;
			}

			var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
			if (projectFile == null)
			{
				return;
			}

			if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
			{
				return;
			}

			var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
			trackedDocuments.ActiveSegment.CurrentISegmentPairProperties = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.Clone() as ISegmentPairProperties;
		}

		public static void ConfirmationLevelChanged(object sender, EventArgs e)
		{
			if (Tracked.ActiveDocument == null)
			{
				return;
			}

			var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
			if (projectFile == null)
			{
				return;
			}

			if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
			{
				return;
			}

			var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
			trackedDocuments.ActiveSegment.CurrentISegmentPairProperties = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.Clone() as ISegmentPairProperties;
		}

		public static void ActiveSegmentChanged(object sender, EventArgs e)
		{
			try
			{
				Tracked.TrackerLastActivity = DateTime.Now;

				if (Tracked.ActiveDocument == null)
				{
					return;
				}

				var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
				if (projectFile == null)
				{
					return;
				}

				if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
				{
					return;
				}

				if (Tracked.TrackingState != Tracked.TimerState.Started && Tracked.TrackingState != Tracked.TimerState.Paused)
				{
					return;
				}

				var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

				if (trackedDocuments.ActiveSegment.CurrentSegmentSelected != null)
				{
					TrackedController.TrackActiveChanges(trackedDocuments);
				}

				TrackedController.InitializeActiveSegment(trackedDocuments);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public static void ContentChanged(object sender, DocumentContentEventArgs e)
		{
			Tracked.TrackerLastActivity = DateTime.Now;

			if (Tracked.ActiveDocument == null)
			{
				return;
			}

			var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
			if (projectFile == null)
			{
				return;
			}

			var projectFileId = projectFile.Id.ToString();

			// Warning message to user -> if content changes and activity tracking is turned off  |
			WarningMessageTrackingNotRunning(sender, projectFileId);

			if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFileId))
			{
				return;
			}

			//grab the latest keys selected
			var keyStroke = (KeyStroke)Viewer.KsCache.Clone();
			keyStroke.Created = DateTime.Now;

			//get the cache document item               
			var trackedDocument = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
			trackedDocument.ActiveSegment.CurrentSegmentContentHasChanged = true;
			keyStroke.Selection = trackedDocument.ActiveSegment.CurrentTargetSelection;

			//reset the class
			Viewer.KsCache = new KeyStroke();

			//get the latest version of the target content
			TrackedController.ContentProcessor.ProcessSegment(e.Segments.FirstOrDefault(), true, new List<string>());
			var trackingSegmentContentTrg = string.Empty;
			var targetSectionsCurrent = TrackedController.GetRecordContentSections(
				TrackedController.ContentProcessor.SegmentSections
				, ContentSection.LanguageType.Target
				, ref trackingSegmentContentTrg);

			//remove the revision marker for this test
			foreach (var contentSection in targetSectionsCurrent)
			{
				if (contentSection.RevisionMarker == null || contentSection.RevisionMarker.RevType != RevisionMarker.RevisionType.Delete)
				{
					continue;
				}

				contentSection.Content = string.Empty;
				contentSection.RevisionMarker = null;
			}

			var comparisonUnits = ComparisonUnitDifferences(trackedDocument, targetSectionsCurrent, keyStroke);

			try
			{
				// add the key stroke data  |
				AddKeyStrokeData(keyStroke, comparisonUnits, trackedDocument);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				//update the current cause selection list
				trackedDocument.ActiveSegment.CurrentTargetSections = new List<ContentSection>();
				foreach (var section in targetSectionsCurrent)
				{
					trackedDocument.ActiveSegment.CurrentTargetSections.Add((ContentSection)section.Clone());
				}

				//remove the cach holder for current seleciton
				trackedDocument.ActiveSegment.CurrentTargetSelection = string.Empty;
			}
		}

		private static string GetTypedText(IEnumerable<ComparisonUnit> comparisonUnits)
		{
			var result = string.Empty;
			foreach (var comparisonUnit in comparisonUnits)
			{
				if (comparisonUnit.Type == ComparisonUnit.ComparisonType.New)
				{
					foreach (var section in comparisonUnit.Section)
					{
						result += section.Content;
					}
				}
			}

			return result;
		}

		private static string GetSelectedText(IEnumerable<ComparisonUnit> comparisonUnits)
		{
			var text = string.Empty;
			var tempIdentical = string.Empty;
			foreach (var comparisonUnit in comparisonUnits)
			{
				switch (comparisonUnit.Type)
				{
					case ComparisonUnit.ComparisonType.Removed:
						{
							if (!string.IsNullOrEmpty(text))
							{
								text += tempIdentical;
								tempIdentical = string.Empty;
							}

							foreach (var section in comparisonUnit.Section)
							{
								text += section.Content;
							}
							break;
						}
					case ComparisonUnit.ComparisonType.Identical:
						{
							tempIdentical = string.Empty;
							foreach (var section in comparisonUnit.Section)
							{
								tempIdentical += section.Content;
							}
							break;
						}
				}
			}
			return text;
		}

		private static void AddKeyStrokeData(KeyStroke keyStroke, IEnumerable<ComparisonUnit> comparisonUnits, TrackedDocuments trackedDocuments)
		{
			// identify the starting position of content, added, removed or replaced.				
			keyStroke.Text = GetTypedText(comparisonUnits);
			keyStroke.Position = Convert.ToInt32(GetTargetCursorPosition());
			keyStroke.X = Cursor.Position.X;
			keyStroke.Y = Cursor.Position.Y;

			// needs to be revised!
			// 1. exclude translations from providers
			// 2. exclude suggestions from termbase? no api handler here...
			// 3. if content that has changed is greater than 1 char in length, then evaluate
			//  - 3.a. if the char == tab or return, then we assume that this was derived from auto-suggest; however
			//         this does not take into account when the user has selected the auto-suggestion via mouse etc...
			if (string.Compare(keyStroke.Key, @"[Tab]", StringComparison.OrdinalIgnoreCase) == 0 && keyStroke.Text.Length > 1)
			{
				keyStroke.OriginType = @"auto-suggest";
			}

			//if the user hit the back key then attempt to get the selection from the comparison if it is not already present
			if (string.IsNullOrEmpty(keyStroke.Selection))
			{
				keyStroke.Selection = GetSelectedText(comparisonUnits);
			}			

			if (string.IsNullOrEmpty(keyStroke.Text) && string.IsNullOrEmpty(keyStroke.Selection))
			{
				return;
			}

			//add the key stroke object to the list
			trackedDocuments.ActiveSegment.CurrentTranslationKeyStokeObjectId = keyStroke.Id;
			trackedDocuments.ActiveSegment.CurrentTranslationKeyStrokeObjectCheck = true;
			trackedDocuments.ActiveSegment.CurrentKeyStrokes.Add(keyStroke);
		}


		private static IEnumerable<ComparisonUnit> ComparisonUnitDifferences(TrackedDocuments trackedDocument, List<ContentSection> targetSectionsCurrent, KeyStroke keyStroke)
		{
			//compare at a character level to understand what was added or removed (no transposition)
			var textComparer = new TextComparer { Type = TextComparer.ComparisonType.Characters };
			var comparisonUnits = textComparer.GetComparisonTextUnits(trackedDocument.ActiveSegment.CurrentTargetSections,
				targetSectionsCurrent, false);

			// clean up the removed selection from the current placeholder; we are only looking for what is new from this holder                 
			if (trackedDocument.ActiveSegment.CurrentTargetSelection.Length <= 2)
			{
				return comparisonUnits;
			}

			try
			{
				var indexCharDiffStart = 0;
				foreach (var comparisonUnit in comparisonUnits)
				{
					if (comparisonUnit.Type == ComparisonUnit.ComparisonType.Identical)
					{
						indexCharDiffStart += comparisonUnit.Text.Length;
					}
					else
					{
						break;
					}
				}
				if (indexCharDiffStart > trackedDocument.ActiveSegment.CurrentTargetSelection.Length)
				{
					indexCharDiffStart = indexCharDiffStart - trackedDocument.ActiveSegment.CurrentTargetSelection.Length;
				}

				var indexCharDiffCounter = 0;
				foreach (var contentSection in trackedDocument.ActiveSegment.CurrentTargetSections)
				{
					indexCharDiffCounter += contentSection.Content.Length;
					if (indexCharDiffCounter < indexCharDiffStart)
					{
						continue;
					}

					var indexA = indexCharDiffCounter - contentSection.Content.Length;
					var indexB = indexA;
					if (indexCharDiffStart > indexA)
					{
						indexB = indexCharDiffStart - indexA;
					}

					var textSeed = contentSection.Content.Substring(0, indexB);
					var textSelection = contentSection.Content.Substring(indexB);

					if (textSelection.IndexOf(trackedDocument.ActiveSegment.CurrentTargetSelection, StringComparison.Ordinal) <= -1)
					{
						continue;
					}

					//remove the selection
					var index = textSelection.IndexOf(keyStroke.Selection, StringComparison.Ordinal);
					var textBefore = textSelection.Substring(0, index);
					var textAfter = textSelection.Substring(index + trackedDocument.ActiveSegment.CurrentTargetSelection.Length);

					contentSection.Content = textSeed + textBefore + textAfter;

					//redo the comparison
					comparisonUnits = textComparer.GetComparisonTextUnits(trackedDocument.ActiveSegment.CurrentTargetSections, targetSectionsCurrent, false);
					break;
				}
			}
			catch
			{
				//ignore here for now
			}
			return comparisonUnits;
		}

		private static void WarningMessageTrackingNotRunning(object sender, string projectFileId)
		{
			if (!Convert.ToBoolean(Tracked.Settings.GetTrackingProperty(@"warningMessageActivityTrackingNotRunning").Value))
			{
				return;
			}

			var ignoreWarning = false;

			if (Tracked.WarningMessageDocumentsIgnoreActivityNotRunning.ContainsKey(projectFileId))
			{
				ignoreWarning = Tracked.WarningMessageDocumentsIgnoreActivityNotRunning[projectFileId];
			}

			if (ignoreWarning || Tracked.TrackingTimer.IsRunning && Tracked.TrackingState != Tracked.TimerState.Stopped)
			{
				return;
			}

			var dr = MessageBox.Show(PluginResources.The_activity_tracker_is_not_running_ + "\r\n\r\n"
									 + PluginResources.Note_The_current_change_will_be_ignored, Application.ProductName, MessageBoxButtons.YesNo);

			if (dr == DialogResult.Yes)
			{
				Tracked.TrackingState = Tracked.TimerState.Started;

				if (!Tracked.TrackingTimer.IsRunning)
				{
					Tracked.TrackingTimer.Start();
				}

				if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFileId))
				{
					TrackedController.TrackNewDocumentEntry(Tracked.ActiveDocument);
				}

				var trackedDocument = Tracked.DictCacheDocumentItems[projectFileId];

				if (Tracked.TrackingState == Tracked.TimerState.Started)
				{
					trackedDocument.ActiveDocument.DocumentTimer.Start();
				}

				TrackedController.InitializeDocumentTracking(Tracked.ActiveDocument);
				ActiveSegmentChanged(sender, null);

				Tracked.TrackingIsDirtyC0 = true;
				Tracked.TrackingIsDirtyC1 = true;
				Tracked.TrackingIsDirtyC2 = true;
			}
			else
			{
				//remember the decision from the user						
				if (!Tracked.WarningMessageDocumentsIgnoreActivityNotRunning.ContainsKey(projectFileId))
				{
					Tracked.WarningMessageDocumentsIgnoreActivityNotRunning.Add(projectFileId, true);
				}
				else
				{
					Tracked.WarningMessageDocumentsIgnoreActivityNotRunning[projectFileId] = true;
				}
			}
		}

		public static void SelectionChanged(object sender, EventArgs e)
		{
			Tracked.TrackerLastActivity = DateTime.Now;

			if (Tracked.ActiveDocument == null)
			{
				return;
			}

			var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
			if (projectFile == null)
			{
				return;
			}

			if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
			{
				return;
			}

			var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

			trackedDocuments.ActiveSegment.CurrentTargetSelection = Tracked.ActiveDocument.Selection.Target.ToString();

			//Quality Metrics
			QualitivityRevisionController.SetCurrentContentQuickInsertSelection(trackedDocuments.ActiveSegment.CurrentTargetSelection);
		}

		public static void SourceChanged(object sender, EventArgs e)
		{
			Tracked.TrackerLastActivity = DateTime.Now;
		}

		public static void TargetChanged(object sender, EventArgs e)
		{
			Tracked.TrackerLastActivity = DateTime.Now;

			if (Tracked.ActiveDocument == null)
			{
				return;
			}

			try
			{
				var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
				if (projectFile == null)
				{
					return;
				}

				if (!Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
				{
					return;
				}

				var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
				if (!trackedDocuments.ActiveSegment.CurrentTranslationKeyStrokeObjectCheck)
				{
					return;
				}

				if (trackedDocuments.ActiveSegment.CurrentKeyStrokes.Count > 0)
				{
					var ks = trackedDocuments.ActiveSegment.CurrentKeyStrokes.Find(x => x.Id == trackedDocuments.ActiveSegment.CurrentTranslationKeyStokeObjectId);
					if (ks != null && string.Compare(ks.OriginType, @"auto-suggest", StringComparison.OrdinalIgnoreCase) != 0)
					{
						if (Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin != null)
						{
							ks.OriginType = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin.OriginType.Clone().ToString();
							ks.Match = ((int)Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin.MatchPercent).ToString();
							if (string.Compare(ks.OriginType, @"interactive", StringComparison.OrdinalIgnoreCase) == 0)
							{
								ks.OriginType = string.Empty;
								ks.Match = string.Empty;
							}
							else if (ks.OriginType.Trim() != string.Empty)
							{
								ks.Text = Helper.GetCompiledSegmentText(trackedDocuments.ActiveSegment.CurrentTargetSections, true);
							}

							ks.OriginSystem = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin.OriginSystem != null
								? Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin.OriginSystem
								: string.Empty;
						}
					}
				}
				trackedDocuments.ActiveSegment.CurrentTranslationKeyStrokeObjectCheck = false;
				trackedDocuments.ActiveSegment.CurrentTranslationKeyStokeObjectId = string.Empty;
				trackedDocuments.ActiveSegment.CurrentTargetSelection = string.Empty;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}