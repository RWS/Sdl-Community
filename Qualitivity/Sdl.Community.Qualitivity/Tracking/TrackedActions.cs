using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.QualityMetrics;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Tracking
{
    public class TrackedActions
    {
        public static void start_tracking(EditorController editorController, Timer timer, bool reset)
        {

            Tracked.TrackerLastActivity = DateTime.Now;

            timer.Stop();


            if (reset)
            {
                Tracked.Reset();

              
                Tracked.TrackingState = Tracked.TimerState.Started;
                Tracked.TrackingTimer = new Stopwatch();
                Tracked.TrackingTimer.Stop();
                Tracked.TrackingStart = null;
                Tracked.TrackingEnd = null;
                Tracked.TrackingPaused = new Stopwatch();

             
            }


            Tracked.ActiveDocument = null;
            try
            {
                if (editorController != null && editorController.ActiveDocument != null)
                {
                    if (editorController.ActiveDocument.ActiveFile != null)
                    {
                        Tracked.ActiveDocument = editorController.ActiveDocument;

                        #region  |  initialize the global timer  |

                        Tracked.TrackingState = Tracked.TimerState.Started;
                        Tracked.TrackingTimer = new Stopwatch();
                        Tracked.TrackingTimer.Start();
                        Tracked.TrackingStart = DateTime.Now;
                        Tracked.TrackingEnd = null;
                        Tracked.TrackingPaused = new Stopwatch();

                        #endregion

                        var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (firstOrDefault != null && !Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                            TrackedController.TrackNewDocumentEntry(Tracked.ActiveDocument);

                        var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (projectFile != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

                            if (Tracked.TrackingState == Tracked.TimerState.Started)
                            {
                                trackedDocuments.ActiveDocument.DocumentTimer.Start();
                                trackedDocuments.ActiveDocument.DatetimeOpened = Tracked.TrackingStart;
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

                            //**************************************

                            TrackedController.InitializeActiveSegment(trackedDocuments);
                        }
                    }
                    else
                    {
                        #region  |  set the tracking timer  |

                        Tracked.TrackingState = Tracked.TimerState.Stopped;
                        Tracked.TrackingTimer.Stop();
                        Tracked.TrackingEnd = DateTime.Now;
                        Tracked.TrackingPaused.Stop();
                        Tracked.TrackingTimer = new Stopwatch();

                        #endregion

                        MessageBox.Show(PluginResources.Unable_To_Initialize_Timer, PluginResources.Title_Qualitivity, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
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

            timer.Start();
        }

        public static void pause_tracking(EditorController editorController, Timer timer)
        {
            try
            {
                Tracked.TrackerLastActivity = DateTime.Now;

                timer.Stop();


                if (Tracked.TrackingState == Tracked.TimerState.Paused)
                {

                    Tracked.TrackingState = Tracked.TimerState.Started;
                    Tracked.TrackingPaused = new Stopwatch();


                    if (Tracked.ActiveDocument == null) return;
                    var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        var trackedDocuments = Tracked.DictCacheDocumentItems[firstOrDefault.Id.ToString()];
                        trackedDocuments.ActiveDocument.DocumentTimer.Start();
                        if (trackedDocuments.ActiveSegment.CurrentSegmentSelected != null)
                            trackedDocuments.ActiveSegment.CurrentSegmentTimer.Start();
                    }


                    Tracked.TrackingTimer.Start();
                }
                else
                {
                    if (Tracked.ActiveDocument != null)
                    {
                        var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[firstOrDefault.Id.ToString()];

                            trackedDocuments.ActiveDocument.DocumentTimer.Stop();
                            if (trackedDocuments.ActiveSegment.CurrentSegmentSelected != null)
                                trackedDocuments.ActiveSegment.CurrentSegmentTimer.Stop();
                        }
                    }

                    Tracked.TrackingTimer.Stop();
                    Tracked.TrackingState = Tracked.TimerState.Paused;
                    Tracked.TrackingPaused.Start();
                }
            }
            finally
            {
                Tracked.TrackingIsDirtyC0 = true;
                Tracked.TrackingIsDirtyC1 = true;
                Tracked.TrackingIsDirtyC2 = true;


                timer.Start();
            }
        }

        public static void stop_tracking(EditorController editorController, Timer timer)
        {
            try
            {
                Tracked.TrackerLastActivity = DateTime.Now;

                timer.Stop();

                
                Tracked.TrackingState = Tracked.TimerState.Stopped;
                Tracked.TrackingTimer.Stop();
                Tracked.TrackingEnd = DateTime.Now;
                Tracked.TrackingPaused.Stop();
                Tracked.TrackingTimer = new Stopwatch();

              
                try
                {
                    if (Tracked.ActiveDocument != null)
                    {
                        var endTime = DateTime.Now;
                        foreach (var trackedDocument in Tracked.DictCacheDocumentItems.Values)
                        {
                            foreach (var document in trackedDocument.Documents)
                            {
                                document.DatetimeClosed = endTime;
                                document.DocumentTimer.Stop();
                            }
                        }

                        var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            var trackedDocuments = Tracked.DictCacheDocumentItems[firstOrDefault.Id.ToString()];

                            TrackedController.TrackActiveChanges(trackedDocuments);
                        }

                        foreach (var trackedDocument in Tracked.DictCacheDocumentItems.Values)
                            TrackedController.NewProjectActivity(trackedDocument);

                        Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();
                        QualitivityRevisionController.CleanQualityMetricsDataContainer();
                    }

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


            timer.Start();
        }

        public static void cancel_tracking(EditorController editorController, Timer timer)
        {
            try
            {
                Tracked.TrackerLastActivity = DateTime.Now;

                timer.Stop();

                Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();

                Tracked.TrackingState = Tracked.TimerState.Deleted;


                Tracked.Reset();
            }
            finally
            {


                Tracked.TrackingIsDirtyC0 = true;
                Tracked.TrackingIsDirtyC1 = true;
                Tracked.TrackingIsDirtyC2 = true;

                timer.Start();
            }
        }
    }
}