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
        public static void TranslationOriginChanged(object sender, EventArgs e)
        {
            var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (firstOrDefault == null || Tracked.ActiveDocument == null || !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                return;
            var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (projectFile == null) return;
            var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
            trackedDocuments.ActiveSegment.CurrentISegmentPairProperties = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.Clone() as ISegmentPairProperties;
        }

        public static void ConfirmationLevelChanged(object sender, EventArgs e)
        {
            var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (firstOrDefault == null || Tracked.ActiveDocument == null || !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                return;
            var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (projectFile == null) return;
            var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
            trackedDocuments.ActiveSegment.CurrentISegmentPairProperties = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.Clone() as ISegmentPairProperties;
        }

        public static void ActiveSegmentChanged(object sender, EventArgs e)
        {
            try
            {
                Tracked.TrackerLastActivity = DateTime.Now;
                var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (firstOrDefault == null || Tracked.ActiveDocument == null || !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                    return;
                if (Tracked.TrackingState != Tracked.TimerState.Started &&
                    Tracked.TrackingState != Tracked.TimerState.Paused) return;
                var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (projectFile == null) return;
                var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

                if (trackedDocuments.ActiveSegment.CurrentSegmentSelected != null)
                    TrackedController.TrackActiveChanges(trackedDocuments);

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

            #region  |  warning message to user -> if content changes and activity tracking is turned off  |

            var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (firstOrDefault != null && Convert.ToBoolean(Tracked.Settings.GetTrackingProperty(@"warningMessageActivityTrackingNotRunning").Value) && Tracked.ActiveDocument != null && Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
            {
                var ignoreWarning = false;
                var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (projectFile != null && Tracked.WarningMessageDocumentsIgnoreActivityNotRunning.ContainsKey(projectFile.Id.ToString()))
                {
                    var orDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                    if (orDefault != null)
                        ignoreWarning = Tracked.WarningMessageDocumentsIgnoreActivityNotRunning[orDefault.Id.ToString()];
                }
                if (!ignoreWarning)
                {
                    var dr = MessageBox.Show(PluginResources.The_activity_tracker_is_not_running_ + "\r\n\r\n"
                                             + PluginResources.Note_The_current_change_will_be_ignored, Application.ProductName, MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Tracked.TrackingState = Tracked.TimerState.Started;


                        if (!Tracked.TrackingTimer.IsRunning)
                            Tracked.TrackingTimer.Start();

                        var orDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (orDefault != null && !Tracked.DictCacheDocumentItems.ContainsKey(orDefault.Id.ToString()))
                            TrackedController.TrackNewDocumentEntry(Tracked.ActiveDocument);

                        var @default = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (@default != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[@default.Id.ToString()];

                            if (Tracked.TrackingState == Tracked.TimerState.Started)
                                trackedDocuments.ActiveDocument.DocumentTimer.Start();
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
                        var orDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (orDefault != null && !Tracked.WarningMessageDocumentsIgnoreActivityNotRunning.ContainsKey(orDefault.Id.ToString()))
                        {
                            var @default = Tracked.ActiveDocument.Files.FirstOrDefault();
                            if (@default != null)
                                Tracked.WarningMessageDocumentsIgnoreActivityNotRunning.Add(@default.Id.ToString(), true);
                        }
                        else
                        {
                            var file = Tracked.ActiveDocument.Files.FirstOrDefault();
                            if (file != null)
                                Tracked.WarningMessageDocumentsIgnoreActivityNotRunning[file.Id.ToString()] = true;
                        }
                    }
                }
            }
            #endregion

            var o = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (o == null || Tracked.ActiveDocument == null || !Tracked.DictCacheDocumentItems.ContainsKey(o.Id.ToString()))
                return;
            {
                //grab the latest keys selected
                var keyStroke = (KeyStroke)Viewer.KsCache.Clone();
                keyStroke.Created = DateTime.Now;


                //get the cache document item
                var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (projectFile == null) return;
                var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
                trackedDocuments.ActiveSegment.CurrentSegmentContentHasChanged = true;
                keyStroke.Selection = trackedDocuments.ActiveSegment.CurrentTargetSelection;


                //reset the class
                Viewer.KsCache = new KeyStroke();

                //get the latest version of the target content
                TrackedController.ContentProcessor.ProcessSegment(e.Segments.FirstOrDefault(), true, new List<string>());
                var trackingSegmentContentTrg = string.Empty;
                var targetSectionsCurrent = TrackedController.GetRecordContentSections(TrackedController.ContentProcessor.SegmentSections
                    , ContentSection.LanguageType.Target
                    , ref trackingSegmentContentTrg);


                //remove the revision marker for this test
                foreach (var t in targetSectionsCurrent)
                {
                    if (t.RevisionMarker == null ||
                        t.RevisionMarker.RevType != RevisionMarker.RevisionType.Delete)
                        continue;
                    t.Content = string.Empty;
                    t.RevisionMarker = null;
                }

                #region  |  compare the content  |


                //compare at a character level to understand what was added or removed (no transposition)
                var textComparer = new TextComparer { Type = TextComparer.ComparisonType.Characters };
                var comparisonUnits = textComparer.GetComparisonTextUnits(trackedDocuments.ActiveSegment.CurrentTargetSections, targetSectionsCurrent, false);


                #region  |  fix the selection removal from the cache  |

                // clean up the removed selection from the current placeholder; we are only looking for what is new from this holder                 
                if (trackedDocuments.ActiveSegment.CurrentTargetSelection.Length > 2)
                {
                    try
                    {
                        var indexCharDiffStart = 0;
                        foreach (var t in comparisonUnits)
                        {
                            if (t.Type == ComparisonUnit.ComparisonType.Identical)
                                indexCharDiffStart += t.Text.Length;
                            else
                                break;
                        }
                        if (indexCharDiffStart > trackedDocuments.ActiveSegment.CurrentTargetSelection.Length)
                            indexCharDiffStart = indexCharDiffStart - trackedDocuments.ActiveSegment.CurrentTargetSelection.Length;


                        var indexCharDiffCounter = 0;
                        foreach (var t in trackedDocuments.ActiveSegment.CurrentTargetSections)
                        {
                            indexCharDiffCounter += t.Content.Length;
                            if (indexCharDiffCounter < indexCharDiffStart)
                                continue;
                            var indexStartingPointA = indexCharDiffCounter - t.Content.Length;
                            var indexStartingPointB = indexStartingPointA;
                            if (indexCharDiffStart > indexStartingPointA)
                                indexStartingPointB = indexCharDiffStart - indexStartingPointA;

                            var indexStartingPointBBefore = t.Content.Substring(0, indexStartingPointB);
                            var indexStartingPointBAfter = t.Content.Substring(indexStartingPointB);

                            if (indexStartingPointBAfter.IndexOf(trackedDocuments.ActiveSegment.CurrentTargetSelection, StringComparison.Ordinal) <= -1)
                                continue;
                            //remove the selection
                            var indexBefore = indexStartingPointBAfter.IndexOf(keyStroke.Selection, StringComparison.Ordinal);
                            var strBefore = indexStartingPointBAfter.Substring(0, indexBefore);
                            var strAfter = indexStartingPointBAfter.Substring(indexBefore + trackedDocuments.ActiveSegment.CurrentTargetSelection.Length);

                            t.Content = indexStartingPointBBefore + strBefore + strAfter;

                            //redo the comparison
                            comparisonUnits = textComparer.GetComparisonTextUnits(trackedDocuments.ActiveSegment.CurrentTargetSections, targetSectionsCurrent, false);
                            break;
                        }
                    }
                    catch
                    {
                        //ignore here for now
                    }
                }


                #endregion


                #endregion

                try
                {
                    #region  |  add the key stroke data  |

                    var textDelete = string.Empty;
                    keyStroke.Text = string.Empty;

                    foreach (var comparisonUnit in comparisonUnits)
                        switch (comparisonUnit.Type)
                        {
                            case ComparisonUnit.ComparisonType.New:
                                foreach (var trgu in comparisonUnit.Section)
                                    keyStroke.Text += trgu.Content;
                                break;
                            case ComparisonUnit.ComparisonType.Removed:
                                textDelete = comparisonUnit.Section.Aggregate(textDelete, (current, trgu) => current + trgu.Content);
                                break;
                        }

                    // logical deduction
                    // needs to be revised!
                    // 1. exclude translations from providers
                    // 2. exclude suggestions from termbase? no api handler here... TODO
                    // 3. if content changed is greater than 1 char in length then check the key char
                    //  - 3.a. if the char == tab or return, then we can assume that this was derived from auto-suggest; however
                    //         this does not take into account when the user has selected the auto-suggestion via mouse selection
                    if (string.Compare(keyStroke.Key, @"[Tab]", StringComparison.OrdinalIgnoreCase) == 0 && keyStroke.Text.Length > 1)
                        keyStroke.OriginType = @"auto-suggest";

                    //if the user hit the back key then attempt to get the selection from the comparison if it is not already present
                    if (string.Compare(keyStroke.Key, @"[Back]", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(keyStroke.Key, @"[Delete]", StringComparison.OrdinalIgnoreCase) == 0
                        && keyStroke.Text == string.Empty
                        && keyStroke.Selection == string.Empty
                        && textDelete != string.Empty)
                    {
                        keyStroke.Selection = textDelete;
                    }


                    if (keyStroke.Text == string.Empty && keyStroke.Selection == string.Empty) return;
                    //add the key stroke object to the list
                    trackedDocuments.ActiveSegment.CurrentTranslationKeyStokeObjectId = keyStroke.Id;
                    trackedDocuments.ActiveSegment.CurrentTranslationKeyStrokeObjectCheck = true;
                    trackedDocuments.ActiveSegment.CurrentKeyStrokes.Add(keyStroke);

                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    //update the current cause selection list
                    trackedDocuments.ActiveSegment.CurrentTargetSections = new List<ContentSection>();
                    foreach (var section in targetSectionsCurrent)
                        trackedDocuments.ActiveSegment.CurrentTargetSections.Add((ContentSection)section.Clone());

                    //remove the cach holder for current seleciton
                    trackedDocuments.ActiveSegment.CurrentTargetSelection = string.Empty;
                }
            }
        }

        public static void SelectionChanged(object sender, EventArgs e)
        {
            Tracked.TrackerLastActivity = DateTime.Now;
            var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (firstOrDefault != null && (Tracked.ActiveDocument == null ||
                                           !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))) return;
            var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
            if (projectFile == null) return;
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
            try
            {
                var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (firstOrDefault == null || Tracked.ActiveDocument == null || !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                    return;
                var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (projectFile == null) return;
                var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
                if (!trackedDocuments.ActiveSegment.CurrentTranslationKeyStrokeObjectCheck) return;
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
                                ks.Text = Helper.GetCompiledSegmentText(trackedDocuments.ActiveSegment.CurrentTargetSections, true);

                            ks.OriginSystem = Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin.OriginSystem != null ? Tracked.ActiveDocument.ActiveSegmentPair.Target.Properties.TranslationOrigin.OriginSystem : string.Empty;
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