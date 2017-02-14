using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.QualityMetrics;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Tracking
{
    public class TrackedEditorEvents
    {

        public static event EventHandler EditorControllerClosing;

        private static void OnEditorControllerClosing()
        {
            var handler = EditorControllerClosing;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        public static void InitializeDocumentTrackingEvents(EditorController editorController)
        {

            editorController.ActivationChanged += EditorController_ActivationChanged;
            editorController.Closing += EditorController_Closing;
            editorController.Opened +=EditorController_Opened;
            editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
        }
        private static void EditorController_ActivationChanged(object sender, ActivationChangedEventArgs e)
        {
            Tracked.EditorControllerIsActive = e.Active;
        }
        private static void EditorController_Closing(object sender, CancelDocumentEventArgs e)
        {
            try
            {
                #region  |  remove handlers  |

                if (e.Document != null)
                {
                    e.Document.SegmentsConfirmationLevelChanged -= TrackedDocumentEvents.ConfirmationLevelChanged;
                    e.Document.SegmentsTranslationOriginChanged -= TrackedDocumentEvents.TranslationOriginChanged;
                    e.Document.ActiveSegmentChanged -= TrackedDocumentEvents.ActiveSegmentChanged;
                    e.Document.ContentChanged -= TrackedDocumentEvents.ContentChanged;
                    e.Document.Selection.Changed -= TrackedDocumentEvents.SelectionChanged;
                    e.Document.Selection.Source.Changed -= TrackedDocumentEvents.SourceChanged;
                    e.Document.Selection.Target.Changed -= TrackedDocumentEvents.TargetChanged;
                }

                #endregion

                Tracked.TrackerLastActivity = DateTime.Now;

                //Timer4ProjectArea.Stop();

                if ((Tracked.TrackingState != Tracked.TimerState.Started &&
                     Tracked.TrackingState != Tracked.TimerState.Paused) || Tracked.ActiveDocument == null)
                    return;
                try
                {
                    var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                    if (firstOrDefault != null && Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                    {
                        var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (projectFile != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

                            foreach (var document in trackedDocuments.Documents)
                            {
                                document.DatetimeClosed = DateTime.Now;
                                document.DocumentTimer.Stop();
                            }

                            TrackedController.TrackActiveChanges(trackedDocuments);
                            TrackedController.NewProjectActivity(trackedDocuments);
                        }

                        Tracked.TarckerCheckNewActivityAdded = false;

                        //FilterViewerControl(Tracked.TarckerCheckNewActivityId);

                        var orDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (orDefault != null && Tracked.DictCacheDocumentItems.ContainsKey(orDefault.Id.ToString()))
                        {
                            var @default = Tracked.ActiveDocument.Files.FirstOrDefault();
                            if (@default != null)
                                Tracked.DictCacheDocumentItems.Remove(@default.Id.ToString());
                        }
                    }

                 

                    OnEditorControllerClosing();
                }
                finally
                {
                    #region  |  null tracker cache  |

                    if (Tracked.DictCacheDocumentItems.Count == 0)
                    {
                        Tracked.Reset();
                        Tracked.TrackingState = Tracked.TimerState.Stopped;
                    }
                    Tracked.TrackingIsDirtyC0 = true;
                    Tracked.TrackingIsDirtyC1 = true;
                    Tracked.TrackingIsDirtyC2 = true;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Timer4ProjectArea.Start();
            }
        }
        private static void EditorController_Opened(object sender, DocumentEventArgs e)
        {
            try
            {

                Tracked.TrackerLastActivity = DateTime.Now;

                //Timer4ProjectArea.Stop();
                Tracked.ActiveDocument = null;
                if (e == null || e.Document == null || e.Document.ActiveFile == null) return;

                Tracked.ActiveDocument = e.Document;

                //automatically start tracking if the setting active
                if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("autoStartTrackingOnDocumentOpenEvent").Value))
                {
                    if (Tracked.TrackingState != Tracked.TimerState.Started
                        && Tracked.TrackingState != Tracked.TimerState.Paused)
                    {
                        Tracked.TrackingState = Tracked.TimerState.Started;
                    }
                }


                if ((Tracked.TrackingState != Tracked.TimerState.Started &&
                     Tracked.TrackingState != Tracked.TimerState.Paused) || Tracked.ActiveDocument == null)
                    return;

                //TO CHECK THIS SHOULD NOT BE INITIALIZED UNLESS IT IS NECCESSARY
                if (Tracked.TrackingState == Tracked.TimerState.Started && !Tracked.TrackingTimer.IsRunning)
                {
                    Tracked.TrackingState = Tracked.TimerState.Started;
                    Tracked.TrackingTimer = new Stopwatch();
                    Tracked.TrackingTimer.Start();
                    Tracked.TrackingStart = DateTime.Now;
                    Tracked.TrackingEnd = null;
                    Tracked.TrackingPaused = new Stopwatch();
                }


                var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (firstOrDefault != null && !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                    TrackedController.TrackNewDocumentEntry(Tracked.ActiveDocument);


                var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (projectFile == null) return;

                var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
                if (Tracked.TrackingState == Tracked.TimerState.Started)
                {
                    trackedDocuments.ActiveDocument.DocumentTimer.Start();
                    trackedDocuments.ActiveDocument.DatetimeOpened = DateTime.Now;
                    trackedDocuments.ActiveDocument.DatetimeClosed = null;
                }



                TrackedController.InitializeDocumentTracking(Tracked.ActiveDocument);


                Application.DoEvents();

                //_qualityMetrics.Value.
                //**************************************
                //initialize the quality metrics container

                #region  |  set the default current metric group what is specified for the company  |


                var project = Helper.GetProjectFromId(trackedDocuments.ProjectId);
                if (project.CompanyProfileId > -1)
                {
                    var ci = Helper.GetClientFromId(project.CompanyProfileId);
                    if (ci != null && ci.Id > -1)
                    {
                        if (ci.MetricGroup.Id > -1)
                        {
                            Tracked.Settings.QualityMetricGroup = ci.MetricGroup;
                        }
                    }
                }

                #endregion


                QualitivityRevisionController.InitializeQualityMetricsData(trackedDocuments.QualityMetrics, true);


                try
                {
                    TrackedController.InitializeActiveSegment(trackedDocuments);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Tracked.TrackingIsDirtyC0 = true;
                    Tracked.TrackingIsDirtyC1 = true;
                    Tracked.TrackingIsDirtyC2 = true;
                }



                //ensure that the segment id is set from the QM area
                trackedDocuments.ActiveSegment.CurrentDocumentId = trackedDocuments.ActiveDocument.Id;
                trackedDocuments.ActiveSegment.CurrentSegmentId = Tracked.ActiveDocument.GetActiveSegmentPair().Properties.Id.Id;
                trackedDocuments.ActiveSegment.CurrentParagraphId = Tracked.ActiveDocument.GetActiveSegmentPair().GetParagraphUnitProperties().ParagraphUnitId.Id;
                trackedDocuments.ActiveSegment.CurrentSegmentUniqueId = trackedDocuments.ActiveSegment.CurrentParagraphId + "." + trackedDocuments.ActiveSegment.CurrentSegmentId;

                QualitivityRevisionController.SetCurrentSelectedSegmentId(trackedDocuments.ActiveSegment.CurrentParagraphId, trackedDocuments.ActiveSegment.CurrentSegmentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Timer4ProjectArea.Start();
            }
        }
        private static void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            try
            {
                Tracked.TrackerLastActivity = DateTime.Now;

                //Timer4ProjectArea.Stop();


                #region  |  stop the timer from the previous document  |

                if (Tracked.ActiveDocument != null)
                {
                    var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                    if (firstOrDefault != null && Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                    {
                        var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (projectFile != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

                            trackedDocuments.ActiveDocument.DocumentTimer.Stop();
                            if (trackedDocuments.ActiveSegment.CurrentSegmentSelected != null)
                                trackedDocuments.ActiveSegment.CurrentSegmentTimer.Stop();

                            TrackedController.TrackActiveChanges(trackedDocuments);
                        }
                    }
                }
                #endregion


                Tracked.ActiveDocument = null;

                if (e == null || e.Document == null || e.Document.ActiveFile == null) return;
                {
                    Tracked.ActiveDocument = e.Document;


                    if (Tracked.TrackingState == Tracked.TimerState.Started
                        || Tracked.TrackingState == Tracked.TimerState.Paused)
                    {
                        var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (firstOrDefault != null && !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                            TrackedController.TrackNewDocumentEntry(Tracked.ActiveDocument);

                        var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (projectFile != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];
                            if (Tracked.TrackingState == Tracked.TimerState.Started)
                                trackedDocuments.ActiveDocument.DocumentTimer.Start();

                            TrackedController.InitializeDocumentTracking(Tracked.ActiveDocument);
                            TrackedDocumentEvents.ActiveSegmentChanged(sender, null);



                            Application.DoEvents();
                            //**************************************
                            //initialize the quality metrics container
                            #region  |  set the default current metric group what is specified for the company  |


                            var project = Helper.GetProjectFromId(trackedDocuments.ProjectId);
                            if (project.CompanyProfileId > -1)
                            {
                                var ci = Helper.GetClientFromId(project.CompanyProfileId);
                                if (ci != null && ci.Id > -1)
                                {
                                    if (ci.MetricGroup.Id > -1)
                                    {
                                        Tracked.Settings.QualityMetricGroup = ci.MetricGroup;
                                    }
                                }
                            }

                            #endregion
                            QualitivityRevisionController.InitializeQualityMetricsData(trackedDocuments.QualityMetrics, false);
                        }


                        //**************************************

                        Tracked.TrackingIsDirtyC0 = true;
                        Tracked.TrackingIsDirtyC1 = true;
                        Tracked.TrackingIsDirtyC2 = true;
                    }
                    else
                    {
                        var orDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (orDefault != null && Tracked.DictCacheDocumentItems.ContainsKey(orDefault.Id.ToString()))
                        {
                            TrackedController.InitializeDocumentTracking(Tracked.ActiveDocument);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Timer4ProjectArea.Start();
            }
        }


      
    }
}
