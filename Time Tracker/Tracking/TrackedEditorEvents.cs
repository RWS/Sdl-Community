using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker.Tracking
{
    public class TrackedEditorEvents
    {
        public static void InitializeDocumentTrackingEvents(EditorController editorController)
        {
         
            editorController.Closing += EditorController_Closing;
            editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
            editorController.Opened += EditorController_Opened;
        }

        private static void EditorController_Closing(object sender, CancelDocumentEventArgs e)
        {
            try
            {
                //stop timer for duration of processing
                //Timer4ProjectArea.Stop();

                if ((Tracked.TrackingState != Tracked.TimerState.Started &&
                     Tracked.TrackingState != Tracked.TimerState.Unpaused &&
                     Tracked.TrackingState != Tracked.TimerState.Paused) ||
                    e.Document.ActiveFile.Id.ToString() != Tracked.TrackerDocumentId) return;
                try
                {
                    Tracked.TrackingTimer.Stop();

                    var trackerProject = Helper.GetTrackerProjectFromDocument(e.Document);
                    var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                    #region  |  add existing activity   |

                    var trackerProjectActivity = new TrackerProjectActivity
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = e.Document.ActiveFile.Name
                    };


                    #region  |  get activity type  |

                    var activitiesType = Tracked.Preferences.ActivitiesTypes[0];
                    ClientActivityType type = null;
                    foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
                    {
                        if (string.Compare(activityType.Name, e.Document.Mode.ToString(),
                                StringComparison.OrdinalIgnoreCase) != 0) continue;
                        activitiesType = activityType;
                        if (clientProfileInfo != null)
                        {
                            foreach (var clientActivityType in clientProfileInfo.ClientActivities)
                            {
                                if (activitiesType.Id != clientActivityType.IdActivity) continue;
                                if (clientActivityType.Activated)
                                {
                                    type = clientActivityType;
                                }
                                break;
                            }
                        }
                        break;
                    }
                    #endregion
                    trackerProjectActivity.ActivityTypeId = activitiesType.Id;
                    trackerProjectActivity.ActivityTypeName = activitiesType.Name;
                    if (type != null)
                        trackerProjectActivity.ActivityTypeClientId = type.Id;

                    trackerProjectActivity.Billable = activitiesType.Billable;
                    if (clientProfileInfo != null)
                    {
                        trackerProjectActivity.ClientId = clientProfileInfo.Id;
                        trackerProjectActivity.ClientName = clientProfileInfo.ClientName;
                    }
                    trackerProjectActivity.Currency = activitiesType.Currency;
                    trackerProjectActivity.DateStart = Tracked.TrackingStart;
                    Tracked.TrackingEnd = DateTime.Now;
                    trackerProjectActivity.DateEnd = Tracked.TrackingEnd;

                    trackerProjectActivity.Description = activitiesType.Description;
                    trackerProjectActivity.HourlyRate = activitiesType.HourlyRate;
                    trackerProjectActivity.HourlyRateAdjustment = 0;
                    trackerProjectActivity.Invoiced = false;
                    trackerProjectActivity.InvoicedDate = Common.DateNull;


                    trackerProjectActivity.Quantity = Convert.ToDecimal(trackerProjectActivity.DateEnd.Subtract(trackerProjectActivity.DateStart).TotalHours);
                    var quantityElapsed = Convert.ToDecimal(Math.Round(Tracked.TrackingTimer.Elapsed.TotalHours, 3));//todo
                    if (quantityElapsed < trackerProjectActivity.Quantity)
                        trackerProjectActivity.Quantity = quantityElapsed;

                    trackerProjectActivity.Status = @"New";
                    trackerProjectActivity.Total = Math.Round(trackerProjectActivity.Quantity * trackerProjectActivity.HourlyRate, 2);
                    trackerProjectActivity.TrackerProjectId = trackerProject.Id;
                    trackerProjectActivity.TrackerProjectName = trackerProject.Name;
                    trackerProjectActivity.TrackerProjectStatus = trackerProject.ProjectStatus;

                    if (Tracked.Preferences.TrackerConfirmActivities)
                    {
                        var trackProjectActivity = new Dialogs.TrackProjectActivity();


                        var trackerProjects = new List<TrackerProject> { trackerProject };


                        trackerProjectActivity.TrackerProjectId = trackerProject.Id;
                        trackerProjectActivity.TrackerProjectName = trackerProject.Name;
                        trackerProjectActivity.TrackerProjectStatus = trackerProject.ProjectStatus;


                        trackProjectActivity.Project = trackerProject;
                        trackProjectActivity.Projects = trackerProjects;
                        trackProjectActivity.Activity = trackerProjectActivity;


                        trackProjectActivity.IsEdit = true;
                        trackProjectActivity.ShowDialog();
                        if (!trackProjectActivity.Saved)
                            return;

                        trackerProjectActivity.ActivityTypeClientId = trackProjectActivity.Activity.ActivityTypeClientId;
                        trackerProjectActivity.ActivityTypeId = trackProjectActivity.Activity.ActivityTypeId;
                        trackerProjectActivity.ActivityTypeName = trackProjectActivity.Activity.ActivityTypeName;

                        trackerProjectActivity.Billable = trackProjectActivity.Activity.Billable;
                        trackerProjectActivity.ClientId = trackProjectActivity.Activity.ClientId;
                        trackerProjectActivity.ClientName = trackProjectActivity.Activity.ClientName;

                        trackerProjectActivity.Currency = trackProjectActivity.Activity.Currency;
                        trackerProjectActivity.DateEnd = trackProjectActivity.Activity.DateEnd;
                        trackerProjectActivity.DateStart = trackProjectActivity.Activity.DateStart;

                        trackerProjectActivity.Description = trackProjectActivity.Activity.Description;

                        trackerProjectActivity.HourlyRate = trackProjectActivity.Activity.HourlyRate;
                        trackerProjectActivity.HourlyRateAdjustment = trackProjectActivity.Activity.HourlyRateAdjustment;

                        trackerProjectActivity.Invoiced = trackProjectActivity.Activity.Invoiced;
                        trackerProjectActivity.InvoicedDate = trackProjectActivity.Activity.InvoicedDate;

                        trackerProjectActivity.Name = trackProjectActivity.Activity.Name;
                        trackerProjectActivity.Quantity = trackProjectActivity.Activity.Quantity;

                        trackerProjectActivity.TrackerProjectId = trackProjectActivity.Activity.TrackerProjectId;
                        trackerProjectActivity.TrackerProjectName = trackProjectActivity.Activity.TrackerProjectName;
                        trackerProjectActivity.TrackerProjectStatus = trackProjectActivity.Activity.TrackerProjectStatus;

                        trackerProjectActivity.Total = trackProjectActivity.Activity.Total;

                        trackerProject.ProjectActivities.Add(trackerProjectActivity);

                        SettingsSerializer.SaveSettings(Tracked.Preferences);

                        Tracked.TarckerCheckNewActivityAdded = true;
                        Tracked.TarckerCheckNewActivityId = trackerProjectActivity.Id;
                    }
                    else
                    {
                        trackerProject.ProjectActivities.Add(trackerProjectActivity);

                        SettingsSerializer.SaveSettings(Tracked.Preferences);

                        Tracked.TarckerCheckNewActivityAdded = true;
                        Tracked.TarckerCheckNewActivityId = trackerProjectActivity.Id;
                    }

                    #endregion


                }
                finally
                {
                    #region  |  null tracker cache  |


                    Tracked.Reset();

                    Tracked.TrackingState = Tracked.TimerState.Started;

                    //ProjectActivityStartTrackerEnabled = false;
                    //ProjectActivityStopTrackerEnabled = true;

                    Tracked.TrackingIsDirtyC0 = true;
                    Tracked.TrackingIsDirtyC1 = true;
                    Tracked.TrackingIsDirtyC2 = true;



                    #endregion
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                //restart timer
                //Timer4ProjectArea.Start();
            }
        }

        private static void EditorController_Opened(object sender, DocumentEventArgs e)
        {
            try
            {
                //stop timer for duration of processing
                //Timer4ProjectArea.Stop();

                if ((Tracked.TrackingState != Tracked.TimerState.Started &&
                     Tracked.TrackingState != Tracked.TimerState.Unpaused &&
                     Tracked.TrackingState != Tracked.TimerState.Paused) || Tracked.TrackerDocumentId != string.Empty)
                    return;
                var trackerProject = Helper.GetTrackerProjectFromDocument(e.Document);
                var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);


                #region  |  start new activity tracking  |


                #region  |  get activity type  |

                var activitiesType = Tracked.Preferences.ActivitiesTypes[0];

                foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
                {
                    if (string.Compare(activityType.Name, e.Document.Mode.ToString(),
                            StringComparison.OrdinalIgnoreCase) != 0) continue;
                    activitiesType = activityType;

                    break;
                }
                #endregion


                var firstOrDefault = e.Document.Files.FirstOrDefault();
                if (firstOrDefault != null)
                    Tracked.TrackerDocumentId = firstOrDefault.Id.ToString();
                Tracked.TrackingState = Tracked.TimerState.Started;
                Tracked.TrackingTimer = new Stopwatch();
                Tracked.TrackingTimer.Start();


                Tracked.TrackingStart = DateTime.Now;
                Tracked.TrackingEnd = Common.DateNull;

                Tracked.TrackingPaused = new Stopwatch();


                Tracked.TrackerProjectIdStudio = trackerProject.IdStudio;
                Tracked.TrackerProjectNameStudio = e.Document.Project.GetProjectInfo().Name;
                Tracked.TrackerProjectId = trackerProject.Id;
                Tracked.TrackerProjectName = trackerProject.Name;


                Tracked.TrackerActivityName = e.Document.ActiveFile.Name;
                Tracked.TrackerActivityDescription = string.Empty;
                Tracked.TrackerActivityType = activitiesType.Name;

                if (clientProfileInfo != null)
                {
                    Tracked.TrackerClientId = clientProfileInfo.Id;
                    Tracked.TrackerClientName = clientProfileInfo.ClientName;
                }


                //ProjectActivityStartTrackerEnabled = false;
                //ProjectActivityStopTrackerEnabled = true;

                Tracked.TrackingIsDirtyC0 = true;
                Tracked.TrackingIsDirtyC1 = true;
                Tracked.TrackingIsDirtyC2 = true;


                #endregion

            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {

                //restart timer
                //Timer4ProjectArea.Start();
            }
        }

        private static void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            try
            {
                //stop timer for duration of processing
                //Timer4ProjectArea.Stop();

                if (e == null || e.Document == null) return;
                if ((Tracked.TrackingState != Tracked.TimerState.Started &&
                     Tracked.TrackingState != Tracked.TimerState.Unpaused &&
                     Tracked.TrackingState != Tracked.TimerState.Paused) || Tracked.TrackerDocumentId != string.Empty)
                    return;

                var trackerProject = Helper.GetTrackerProjectFromDocument(e.Document);
                if (trackerProject == null)
                    return;

                var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                #region  |  start new activity tracking  |


                #region  |  get activity type  |
                var activitiesType = Tracked.Preferences.ActivitiesTypes[0];
                foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
                {
                    if (string.Compare(activityType.Name, e.Document.Mode.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
                        continue;

                    activitiesType = activityType;

                    break;
                }
                #endregion

                Tracked.TrackerDocumentId = e.Document.ActiveFile.Id.ToString();
                Tracked.TrackingState = Tracked.TimerState.Started;
                Tracked.TrackingTimer = new Stopwatch();
                Tracked.TrackingTimer.Start();


                Tracked.TrackingStart = DateTime.Now;
                Tracked.TrackingEnd = Common.DateNull;

                Tracked.TrackingPaused = new Stopwatch();


                Tracked.TrackerProjectIdStudio = trackerProject.IdStudio;
                Tracked.TrackerProjectNameStudio = e.Document.Project.GetProjectInfo().Name;
                Tracked.TrackerProjectId = trackerProject.Id;
                Tracked.TrackerProjectName = trackerProject.Name;


                Tracked.TrackerActivityName = e.Document.ActiveFile.Name;
                Tracked.TrackerActivityDescription = string.Empty;
                Tracked.TrackerActivityType = activitiesType.Name;

                if (clientProfileInfo != null)
                {
                    Tracked.TrackerClientId = clientProfileInfo.Id;
                    Tracked.TrackerClientName = clientProfileInfo.ClientName;
                }




                Tracked.TrackingIsDirtyC0 = true;
                Tracked.TrackingIsDirtyC1 = true;
                Tracked.TrackingIsDirtyC2 = true;




                #endregion
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                //Timer4ProjectArea.Start();
            }
        }
    }
}
