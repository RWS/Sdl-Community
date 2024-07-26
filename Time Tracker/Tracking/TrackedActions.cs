using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker.Tracking
{
    public class TrackedActions
    {
        public static void start_tracking(EditorController editorController, Timer timer, bool reset)
        {

            try
            {
                timer.Stop();

                if (reset)
                {
                    Tracked.Reset();


                    Tracked.TrackingState = Tracked.TimerState.Started;
                    Tracked.TrackingTimer = new Stopwatch();
                    Tracked.TrackingTimer.Stop();
                    Tracked.TrackingStart = Structures.Common.DateNull;
                    Tracked.TrackingEnd = Structures.Common.DateNull;
                    Tracked.TrackingPaused = new Stopwatch();


                }


                Tracked.TrackingState = Tracked.TimerState.Started;


                try
                {
                    var doc = editorController.ActiveDocument;
                    try
                    {
                        if ((Tracked.TrackingState != Tracked.TimerState.Started &&
                             Tracked.TrackingState != Tracked.TimerState.Unpaused) ||
                            Tracked.TrackerDocumentId != string.Empty) return;
                        var trackerProject = Helper.GetTrackerProjectFromDocument(doc);
                        var clientProfileInfo = Structures.Common.GetClientFromId(trackerProject.ClientId);

                        #region  |  start new activity tracking  |

                        //start new timer
                        //add to tracker
                        #region  |  get activity type  |
                        var activitiesType = Tracked.Preferences.ActivitiesTypes[0];
                        foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
                        {
                            if (string.Compare(activityType.Name, doc.Mode.ToString(),
                                    StringComparison.OrdinalIgnoreCase) != 0)
                                continue;
                            activitiesType = activityType;
                            break;
                        }
                        #endregion

                        Tracked.TrackerDocumentId = doc.ActiveFile.Id.ToString();
                        Tracked.TrackingState = Tracked.TimerState.Started;
                        Tracked.TrackingTimer = new Stopwatch();
                        Tracked.TrackingTimer.Start();


                        Tracked.TrackingStart = DateTime.Now;
                        Tracked.TrackingEnd = Structures.Common.DateNull;

                        Tracked.TrackingPaused = new Stopwatch();


                        Tracked.TrackerProjectIdStudio = trackerProject.IdStudio;
                        Tracked.TrackerProjectNameStudio = doc.Project.GetProjectInfo().Name;
                        Tracked.TrackerProjectId = trackerProject.Id;
                        Tracked.TrackerProjectName = trackerProject.Name;


                        Tracked.TrackerActivityName = doc.ActiveFile.Name;
                        Tracked.TrackerActivityDescription = string.Empty;
                        Tracked.TrackerActivityType = activitiesType.Name;

                        if (clientProfileInfo == null)
                            return;
                        Tracked.TrackerClientId = clientProfileInfo.Id;
                        Tracked.TrackerClientName = clientProfileInfo.ClientName;

                        #endregion
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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

        public static void pause_tracking(EditorController editorController, Timer timer)
        {
            try
            {
                timer.Stop();



                switch (Tracked.TrackingState)
                {
                    case Tracked.TimerState.Paused:

                        Tracked.TrackingState = Tracked.TimerState.Unpaused;

                        if (Tracked.TrackerDocumentId != string.Empty)
                        {
                            Tracked.TrackingTimer.Start();
                        }
                        break;
                    case Tracked.TimerState.Unpaused:
                        Tracked.TrackingTimer.Stop();
                        Tracked.TrackingState = Tracked.TimerState.Paused;

                        break;
                    default:
                        Tracked.TrackingTimer.Stop();
                        Tracked.TrackingState = Tracked.TimerState.Paused;

                        break;
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
                timer.Stop();


                Tracked.TrackingState = Tracked.TimerState.Stopped;
                Tracked.TrackingTimer.Stop();
                Tracked.TrackingEnd = DateTime.Now;
                Tracked.TrackingPaused.Stop();

                try
                {

                    if (Tracked.TrackerActivityName.Trim() == string.Empty)
                        return;

                    var trackerProject = Structures.Common.GetProjectFromId(Tracked.TrackerProjectId);
                    var clientProfileInfo = Structures.Common.GetClientFromId(trackerProject.ClientId);

                    #region  |  add existing activity   |

                    var trackerProjectActivity = new Structures.TrackerProjectActivity
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = Tracked.TrackerActivityName
                    };


                    #region  |  get activity type  |
                    var activitiesType = Tracked.Preferences.ActivitiesTypes[0];
                    Structures.ClientActivityType clientActivityType = null;
                    foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
                    {
                        if (string.Compare(activityType.Name, Tracked.TrackerActivityType,
                                StringComparison.OrdinalIgnoreCase) != 0) continue;
                        activitiesType = activityType;
                        if (clientProfileInfo != null)
                        {
                            foreach (var type in clientProfileInfo.ClientActivities)
                            {
                                if (activitiesType.Id != type.IdActivity)
                                    continue;

                                if (type.Activated)
                                {
                                    clientActivityType = type;
                                }
                                break;
                            }
                        }
                        break;
                    }
                    #endregion


                    trackerProjectActivity.ActivityTypeId = activitiesType.Id;
                    trackerProjectActivity.ActivityTypeName = activitiesType.Name;
                    if (clientActivityType != null)
                        trackerProjectActivity.ActivityTypeClientId = clientActivityType.Id;

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
                    trackerProjectActivity.InvoicedDate = Structures.Common.DateNull;


                    trackerProjectActivity.Quantity = Convert.ToDecimal(trackerProjectActivity.DateEnd.Subtract(trackerProjectActivity.DateStart).TotalHours);
                    var quantityElapsed = Convert.ToDecimal(Math.Round(Tracked.TrackingTimer.Elapsed.TotalHours, 3));
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


                        var trackerProjects = new List<Structures.TrackerProject> { trackerProject };


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

                        Structures.SettingsSerializer.SaveSettings(Tracked.Preferences);

                        Tracked.TarckerCheckNewActivityAdded = true;
                        Tracked.TarckerCheckNewActivityId = trackerProjectActivity.Id;
                    }
                    else
                    {
                        trackerProject.ProjectActivities.Add(trackerProjectActivity);

                        Structures.SettingsSerializer.SaveSettings(Tracked.Preferences);

                        Tracked.TarckerCheckNewActivityAdded = true;
                        Tracked.TarckerCheckNewActivityId = trackerProjectActivity.Id;

                    }

                    #endregion
                }
                finally
                {
                    Tracked.Reset();
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

        public static void cancel_tracking(EditorController editorController, Timer timer)
        {
            try
            {
                timer.Stop();

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
