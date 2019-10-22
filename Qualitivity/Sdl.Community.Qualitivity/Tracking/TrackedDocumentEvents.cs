using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
			Debug.WriteLine("TranslationOriginChanged");
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
			Debug.WriteLine("Confirmation level changed");

			var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

			// With Studio 2019 SR2 at least, this event can get fired *after* ActiveSegmentChanged, e.g. if you confirm a segment,
			// this event appears to apply to the wrong segment, because it's fired too late.
			// Check whether that has happened.

			var props = sender.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			var prop = props.Where(x => string.CompareOrdinal("Item", x.Name) == 0 && !x.GetMethod.GetParameters().Any()).FirstOrDefault();

			if (prop != null)
			{
				var segment = prop.GetValue(sender) as Sdl.FileTypeSupport.Framework.BilingualApi.ISegment;
				if (segment != null)
				{
					// Is the segment whose confirmation level changed different from the one we're tracking?
					if (segment.Properties.Id != trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.Id)
					{
						// Yes - so we probably already received the ActiveSegmentChanged event, and have already
						// logged the confirmation level etc. for the segment we were leaving, but incorrectly, 
						// because the confirmation level hadn't been updated.
						var lastRec = trackedDocuments.ActiveDocument.TrackedRecords.LastOrDefault();
						// Was the last record we logged for the segment whose confirmation level we now know to have changed?
						if (lastRec != null && lastRec.SegmentId == segment.Properties.Id.Id)
						{
							// Yes - amend details
							lastRec.TranslationOrigins.Updated.ConfirmationLevel = segment.Properties.ConfirmationLevel.ToString();
							lastRec.TranslationOrigins.Updated.TranslationStatus = Helper.GetTranslationStatus(segment.Properties.TranslationOrigin);
							lastRec.TranslationOrigins.Updated.OriginType = segment.Properties.TranslationOrigin.OriginType;
						}


						return;
					}

				}
			}


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
					Debug.WriteLine("ActiveSegmentChanged - about to track changes");
					TrackedController.TrackActiveChanges(trackedDocuments);
				}

				TrackedController.InitializeActiveSegment(trackedDocuments);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private const string MarkupTag = "sdlXliffCompareMarkupTag";
		internal static string SimpleParse(List<ContentSection> xSegmentSections, bool includeMarkup)
		{

			var result = new StringBuilder();
			foreach (var xSegmentSection in xSegmentSections)
			{
				if (xSegmentSection.CntType != ContentSection.ContentType.Text)
				{
					if (includeMarkup)
						result.Append("<" + MarkupTag + ">" + xSegmentSection.Content + "</" + MarkupTag + ">");
				}
				else
				{
					result.Append(xSegmentSection.Content);
				}
			}
			return result.ToString();

		}

		static int CommonPrefixLength(string s1, string s2)
		{
			int length = 0;
			for (int i = 0; i < s1.Length && i < s2.Length && s1[i] == s2[i]; i++)
				length++;
			return length;
		}
		static int CommonSuffixLength(string s1, string s2, int prefixLength)
		{
			// need to deal with cases like this:
			// s1: "Cat dog"
			// s2: "Cat  dog"
			// A naive check could find a shared prefix length 4 "Cat " and a shared suffix length 4 " dog"
			// But that's wrong, because it counts the space in s1 as part of the prefix and suffix
			// So we need a max length
			int maxLength = Math.Min(s1.Length - prefixLength, s2.Length - prefixLength);

			int length = 0;
			if (s1.Length > 0 && s2.Length > 0)
			{
				int s1Ix = s1.Length - 1;
				int s2Ix = s2.Length - 1;
				while (s1Ix >= 0 && s2Ix >= 0 && s1[s1Ix] == s2[s2Ix] && length < maxLength)
				{
					length++;
					s1Ix--;
					s2Ix--;
				}
			}
			return length;
		}

		// Extract text that was added to the string by a single user operation
		static string TypedText(string before, string after, int prefix, int suffix)
		{
			if (prefix + suffix == after.Length)
				return string.Empty;
			if (prefix + suffix > after.Length)
			{
				Debug.Assert(false);
				return string.Empty;
			}
			string result = after.Substring(prefix);
			result = result.Substring(0, result.Length - suffix);
			return result;
		}

		// Extract text that was deleted/replaced by a single user operation
		static string RemovedText(string before, string after, int prefix, int suffix)
		{
			if (prefix + suffix == before.Length)
				return string.Empty;
			if (prefix + suffix > before.Length)
			{
				Debug.Assert(false);
				return string.Empty;
			}
			string result = before.Substring(prefix);
			result = result.Substring(0, result.Length - suffix);
			return result;
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


			try
			{
#if DEBUG
				var temp1 = new List<ContentSection>();
				var temp2 = new List<ContentSection>();
				trackedDocument.ActiveSegment.CurrentTargetSections.ForEach(x => temp1.Add(x.Clone() as ContentSection));
				targetSectionsCurrent.ForEach(x => temp2.Add(x.Clone() as ContentSection));
#endif
				// Compare the before/after text, to work out (as much as we can) what was typed/pasted/etc. and
				// what selection (if any) was replaced as a result.
				// We can assume that the change will affect a contiguous span of text, because this event fires in response
				// to whatever change, and the target segment can't have multiple selected spans.
				// Even if we have a segment with the same word repeated in different places, and we perform 'replace all'
				// in Studio, the two replacements fire separately here.
				var newSections = TextComparer.ConcatenateComparableContentSections(trackedDocument.ActiveSegment.CurrentTargetSections);
				var previousSections = TextComparer.ConcatenateComparableContentSections(targetSectionsCurrent);
				var newTextWithMarkup = SimpleParse(newSections, true);
				var oldTextWithMarkup = SimpleParse(previousSections, true);
				var newText = SimpleParse(newSections, false);
				var oldText = SimpleParse(previousSections, false);

				// Find the shared prefix and suffix to identify the affected span
				int prefixLength = CommonPrefixLength(newTextWithMarkup, oldTextWithMarkup);
				int suffixLength = 0;
				if (prefixLength < Math.Min(newTextWithMarkup.Length, oldTextWithMarkup.Length))
				{
					suffixLength = CommonSuffixLength(newTextWithMarkup, oldTextWithMarkup, prefixLength);
				}
				// Extract typed/pasted and removed (selected) text
				string typedText = TypedText(newTextWithMarkup, oldTextWithMarkup, prefixLength, suffixLength);
				string removedText = RemovedText(newTextWithMarkup, oldTextWithMarkup, prefixLength, suffixLength);

				// For logging, report the position in the text without tags, because that's a value that's
				// most likely to be usable when analysing
				int prefixLengthPlain = CommonPrefixLength(newText, oldText);
				Debug.WriteLine("Got typed text of <" + typedText + ">");
				Debug.WriteLine("Got removed text of <" + removedText + ">");
				Debug.WriteLine("newTextWithMarkup: " + newTextWithMarkup);
				Debug.WriteLine("c2: " + oldTextWithMarkup);

				// Try to detect case where (say) 'Parliament' was selected in the text, then the user typed 'P';
				// at this point, text comparison will have left typedText empty, and selection one character short ('P')
				if (!string.IsNullOrEmpty(keyStroke.Key) && typedText.Length == 0 && prefixLength > 0 && !keyStroke.Ctrl && !keyStroke.Alt)
				{
					string altTypedText = TypedText(newTextWithMarkup, oldTextWithMarkup, prefixLength - 1, suffixLength);
					if (altTypedText.Length > 0 && altTypedText.ToLower()[0] == keyStroke.Key.ToLower()[0])
					{
						typedText = altTypedText;
						removedText = RemovedText(newTextWithMarkup, oldTextWithMarkup, prefixLength - 1, suffixLength);
						Debug.WriteLine("Detected overtype of selected text");
						Debug.WriteLine("Got typed text of <" + typedText + ">");
						Debug.WriteLine("Got removed text of <" + removedText + ">");
					}
				}

				// add the key stroke data  |
				AddKeyStrokeData(keyStroke, typedText, removedText, prefixLengthPlain, trackedDocument);
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

		private static void AddKeyStrokeData(KeyStroke keyStroke, string typedText, string removedText, int position, TrackedDocuments trackedDocuments)
		{
			keyStroke.Text = typedText;
			keyStroke.Position = position;
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

			keyStroke.Selection = removedText;

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