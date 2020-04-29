using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.Qualitivity.Custom;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Qualitivity.Dialogs.Export
{
   public  class ExportToFile
    {

        public void create_excel_report(string fileName, List<Activity> activityList, List<XmlDocumentActivity> xmlDasList)
        {

            var projectsTmp = new List<Project>();
            var companyProfilesTmp = new List<CompanyProfile>();
            var projectActivitiesTmp = new List<Activity>();


            var spreadsheet = Excel.CreateWorkbook(fileName);
            if (spreadsheet == null)
            {
                return;
            }

         
            Excel.AddBasicStyles(spreadsheet);
 


            Excel.AddSharedString(spreadsheet, "Shared string");
            var worksheet1 = Excel.AddWorksheet(spreadsheet, "Project Activities");
            var worksheet2 = Excel.AddWorksheet(spreadsheet, "Document Activities");
            var worksheet4 = Excel.AddWorksheet(spreadsheet, "Quality Metrics");
            var worksheet3 = Excel.AddWorksheet(spreadsheet, "KeyStroke Data");


            #region  |  Project Activities  |


            Excel.SetCellValue(spreadsheet, worksheet1, 1, 1, "Client Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 2, 1, "Project Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 3, 1, "Status (Project)", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 4, 1, "Activity ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 5, 1, "Activity Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 6, 1, "Description", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 7, 1, "Status (Activity)", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 8, 1, "Billable", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 9, 1, "Date Start", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 10, 1, "Date End", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 11, 1, "Documents", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 12, 1, "Language Rate Total", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 13, 1, "Hourly Rate Total", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 14, 1, "Activity Total", false, false);
            Excel.SetCellValue(spreadsheet, worksheet1, 15, 1, "Currency", false, false);


            for (var i = 0; i < activityList.Count; i++)
            {
                var uIndex = Convert.ToUInt32(i + 2);
                var activity = activityList[i];


                Project activityProject;
                if (projectsTmp.Exists(a => a.Id == activity.ProjectId))
                    activityProject = projectsTmp.Find(a => a.Id == activity.ProjectId);
                else
                {
                    activityProject = Helper.GetProjectFromId(activity.ProjectId);
                    projectsTmp.Add(activityProject);
                }
                
                CompanyProfile activityCompanyProfile;

                if (companyProfilesTmp.Exists(a => a.Id == activity.CompanyProfileId))
                    activityCompanyProfile = companyProfilesTmp.Find(a => a.Id == activity.CompanyProfileId);
                else
                {

                    activityCompanyProfile = Helper.GetClientFromId(activity.CompanyProfileId);
                    if (activityCompanyProfile != null)
                        companyProfilesTmp.Add(activityCompanyProfile);

                }
                


                var pemTotal = activity.LanguageRateChecked ? (decimal)activity.DocumentActivityRates.LanguageRateTotal : 0;
                var hrTotal = activity.HourlyRateChecked ? (decimal)activity.DocumentActivityRates.HourlyRateTotal : 0;
                var total = decimal.Add(pemTotal, hrTotal);


                Excel.SetCellValue(spreadsheet, worksheet1, 1, uIndex, activityCompanyProfile != null ? activityCompanyProfile.Name : string.Empty, false);
                Excel.SetCellValue(spreadsheet, worksheet1, 2, uIndex, activityProject.Name, false);
                Excel.SetCellValue(spreadsheet, worksheet1, 3, uIndex, activityProject.ProjectStatus, false);
                Excel.SetCellValue(spreadsheet, worksheet1, 4, uIndex, activity.Id.ToString(), false);
                Excel.SetCellValue(spreadsheet, worksheet1, 5, uIndex, activity.Name, false);
                Excel.SetCellValue(spreadsheet, worksheet1, 6, uIndex, activity.Description, false);
                Excel.SetCellValue(spreadsheet, worksheet1, 7, uIndex, activity.ActivityStatus.ToString(), false);
                Excel.SetCellValue(spreadsheet, worksheet1, 8, uIndex, activity.Billable, null);
                if (activity.Started != null)
                    Excel.SetCellValue(spreadsheet, worksheet1, 9, uIndex, Helper.GetStringFromDateTime(activity.Started.Value).Replace("T", " "), false, false);
                if (activity.Stopped != null)
                    Excel.SetCellValue(spreadsheet, worksheet1, 10, uIndex, Helper.GetStringFromDateTime(activity.Stopped.Value).Replace("T", " "), false, false);
                Excel.SetCellValue(spreadsheet, worksheet1, 11, uIndex, activity.Activities.Count, null);
                Excel.SetCellValue(spreadsheet, worksheet1, 12, uIndex, pemTotal, null);
                Excel.SetCellValue(spreadsheet, worksheet1, 13, uIndex, hrTotal, null);
                Excel.SetCellValue(spreadsheet, worksheet1, 14, uIndex, total, null);
                Excel.SetCellValue(spreadsheet, worksheet1, 15, uIndex, activity.DocumentActivityRates.HourlyRateCurrency, false);


                //FormatCode = "yyyy-MM-dd hh:mm:ss",
            }



            Excel.SetColumnWidth(worksheet1, 1, 20);
            Excel.SetColumnWidth(worksheet1, 2, 20);
            Excel.SetColumnWidth(worksheet1, 3, 20);
            Excel.SetColumnWidth(worksheet1, 5, 30);
            Excel.SetColumnWidth(worksheet1, 6, 15);
            Excel.SetColumnWidth(worksheet1, 7, 13);
            Excel.SetColumnWidth(worksheet1, 8, 12);
            Excel.SetColumnWidth(worksheet1, 9, 25);
            Excel.SetColumnWidth(worksheet1, 10, 25);
            Excel.SetColumnWidth(worksheet1, 11, 12);
            Excel.SetColumnWidth(worksheet1, 12, 12);
            Excel.SetColumnWidth(worksheet1, 13, 12);
            Excel.SetColumnWidth(worksheet1, 14, 12);
            Excel.SetColumnWidth(worksheet1, 15, 12);

            var autoFilter1 = new AutoFilter { Reference = "A1:O" + activityList.Count + 1 };
            worksheet1.Append(autoFilter1);

            #endregion
            worksheet1.Save();



            #region  |  Document Activities  |

            Excel.SetCellValue(spreadsheet, worksheet2, 1, 1, "Project Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 2, 1, "Activity ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 3, 1, "Activity Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 4, 1, "Document Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 5, 1, "Paragraph ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 6, 1, "Segment ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 7, 1, "Source Language", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 8, 1, "Target Language", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 9, 1, "Original Confirmation Level", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 10, 1, "Original Translation Status", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 11, 1, "Original Origin System", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 12, 1, "Original Origin Type", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 13, 1, "Updated Confirmation Level", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 14, 1, "Updated Translation Status", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 15, 1, "Updated Origin System", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 16, 1, "Updated Origin Type", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 17, 1, "Source (Text)", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 18, 1, "Target (Text)", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 19, 1, "Updated (Text)", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 20, 1, "Start Date", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 21, 1, "Stop Date", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 22, 1, "Active Seconds", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 23, 1, "Active Milliseconds", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 24, 1, "Word Count", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 25, 1, "Edit Distance", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 26, 1, "Edit Distance Relative", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 27, 1, "PEM %", false, false);
            Excel.SetCellValue(spreadsheet, worksheet2, 28, 1, "Comments", false, false);

            for (var i = 0; i < xmlDasList.Count; i++)
            {
                var uIndex = Convert.ToUInt32(i + 2);
                var activity = xmlDasList[i];


                Project activityProject;
                if (projectsTmp.Exists(a => a.Id == activity.ProjectId))
                    activityProject = projectsTmp.Find(a => a.Id == activity.ProjectId);
                else
                {
                    activityProject = Helper.GetProjectFromId(activity.ProjectId);
                    projectsTmp.Add(activityProject);
                }

                Activity projectActivity;
                if (activityProject.Activities.Exists(a => a.Id == activity.ActivityId))
                    projectActivity = activityProject.Activities.Find(a => a.Id == activity.ActivityId);
                else
                {
                    projectActivity = activityProject.Activities.Find(a => a.Id == activity.ActivityId);
                    projectActivitiesTmp.Add(projectActivity);
                }


                Excel.SetCellValue(spreadsheet, worksheet2, 1, uIndex, activityProject.Name, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 2, uIndex, activity.ActivityId.ToString(), false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 3, uIndex, projectActivity.Name, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 4, uIndex, activity.DocumentName, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 5, uIndex, activity.ParagraphId, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 6, uIndex, activity.SegmentId, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 7, uIndex, activity.SourceLang, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 8, uIndex, activity.TargetLang, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 9, uIndex, activity.OriginalConfirmationLevel, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 10, uIndex, activity.OriginalTranslationStatus, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 11, uIndex, activity.OriginalOriginSystem, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 12, uIndex, activity.OriginalOriginType, false, false);

                Excel.SetCellValue(spreadsheet, worksheet2, 13, uIndex, activity.UpdatedConfirmationLevel, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 14, uIndex, activity.UpdatedTranslationStatus, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 15, uIndex, activity.UpdatedOriginSystem, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 16, uIndex, activity.UpdatedOriginType, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 17, uIndex, activity.SourceText, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 18, uIndex, activity.TargetText, false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 19, uIndex, activity.UpdatedText, false, false);
                if (activity.StartDate != null)
                    Excel.SetCellValue(spreadsheet, worksheet2, 20, uIndex, Helper.GetStringFromDateTimeMilli(activity.StartDate.Value).Replace("T", " "), false, false);
                if (activity.StopDate != null)
                    Excel.SetCellValue(spreadsheet, worksheet2, 21, uIndex, Helper.GetStringFromDateTimeMilli(activity.StopDate.Value).Replace("T", " "), false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 22, uIndex, activity.TotalSeconds, null);
                Excel.SetCellValue(spreadsheet, worksheet2, 23, uIndex, activity.TotalMiliseconds, null);
                Excel.SetCellValue(spreadsheet, worksheet2, 24, uIndex, Convert.ToInt32(activity.WordsSource), null);
                Excel.SetCellValue(spreadsheet, worksheet2, 25, uIndex, activity.EditDistance, null);
                Excel.SetCellValue(spreadsheet, worksheet2, 26, uIndex, activity.EditDistanceRelative, null);
                Excel.SetCellValue(spreadsheet, worksheet2, 27, uIndex, activity.PemPercentage + "%", false, false);
                Excel.SetCellValue(spreadsheet, worksheet2, 28, uIndex, activity.CommentsStr, false, false);


            }
            Excel.SetColumnWidth(worksheet2, 13, 30);
            Excel.SetColumnWidth(worksheet2, 14, 30);
            Excel.SetColumnWidth(worksheet2, 15, 30);
            Excel.SetColumnWidth(worksheet2, 16, 23);
            Excel.SetColumnWidth(worksheet2, 17, 30);
            Excel.SetColumnWidth(worksheet2, 18, 30);
            Excel.SetColumnWidth(worksheet2, 19, 30);
            Excel.SetColumnWidth(worksheet2, 20, 23);
            Excel.SetColumnWidth(worksheet2, 21, 23);

            Excel.SetColumnWidth(worksheet2, 28, 23);


            var autoFilter2 = new AutoFilter { Reference = "A1:AB" + xmlDasList.Count + 1 };
            worksheet2.Append(autoFilter2);

            #endregion
            worksheet2.Save();

            #region  |  Quality Metrics  |


            Excel.SetCellValue(spreadsheet, worksheet4, 1, 1, "Activity ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 2, 1, "Activity Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 3, 1, "Document ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 4, 1, "Document Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 5, 1, "Record ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 6, 1, "Paragraph ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 7, 1, "Segment ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 8, 1, "QM GUID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 9, 1, "QM ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 10, 1, "QM Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 11, 1, "QM Status", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 12, 1, "QM Severity", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 13, 1, "QM Weight", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 14, 1, "QM Content", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 15, 1, "QM Comment", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 16, 1, "QM Created", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 17, 1, "QM Modified", false, false);
            Excel.SetCellValue(spreadsheet, worksheet4, 18, 1, "QM User Name", false, false);

            var index = 1;
            foreach (var activity in xmlDasList)
            {
                Project activityProject = null;
                var activity1 = activity;
                if (projectsTmp.Exists(a => a.Id == activity1.ProjectId))
                    activityProject = projectsTmp.Find(a => a.Id == activity.ProjectId);
                else
                {
                    activityProject = Helper.GetProjectFromId(activity.ProjectId);
                    projectsTmp.Add(activityProject);
                }

                Activity projectActivity = null;
                if (activityProject.Activities.Exists(a => a.Id == activity.ActivityId))
                    projectActivity = activityProject.Activities.Find(a => a.Id == activity.ActivityId);
                else
                {
                    projectActivity = activityProject.Activities.Find(a => a.Id == activity.ActivityId);
                    projectActivitiesTmp.Add(projectActivity);
                }


                if (activity.QualityMetrics == null) continue;
                foreach (var t in activity.QualityMetrics)
                {
                    index += 1;
                    var uIndex = Convert.ToUInt32(index);

                    var qm = t;

                    Excel.SetCellValue(spreadsheet, worksheet4, 1, uIndex, activity.ActivityId.ToString(), false, false);
                    Excel.SetCellValue(spreadsheet, worksheet4, 2, uIndex, projectActivity != null ? projectActivity.Name : string.Empty, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet4, 3, uIndex, activity.DocumentId, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet4, 4, uIndex, activity.DocumentName, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet4, 5, uIndex, activity.RecordId, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet4, 6, uIndex, activity.ParagraphId, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet4, 7, uIndex, activity.SegmentId, false, false);

                    if (qm.Name.Trim() != string.Empty)
                    {
                        Excel.SetCellValue(spreadsheet, worksheet4, 8, uIndex, qm.Guid, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 9, uIndex, qm.Id, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 10, uIndex, qm.Name, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 11, uIndex, qm.Status.ToString(), false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 12, uIndex, qm.SeverityName, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 13, uIndex, qm.SeverityValue.ToString(), false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 14, uIndex, qm.Content, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 15, uIndex, qm.Comment, false, false);
                        if (qm.Created != null)
                            Excel.SetCellValue(spreadsheet, worksheet4, 16, uIndex, Helper.GetStringFromDateTimeMilli(qm.Created.Value).Replace("T", " "), false, false);
                        if (qm.Modified != null)
                            Excel.SetCellValue(spreadsheet, worksheet4, 17, uIndex, Helper.GetStringFromDateTimeMilli(qm.Modified.Value).Replace("T", " "), false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 18, uIndex, qm.UserName, false, false);
                    }
                    else
                    {
                        Excel.SetCellValue(spreadsheet, worksheet4, 8, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 9, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 10, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 11, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 12, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 13, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 14, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 15, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 16, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 17, uIndex, string.Empty, false, false);
                        Excel.SetCellValue(spreadsheet, worksheet4, 18, uIndex, string.Empty, false, false);
                    }
                }
            }
            Excel.SetColumnWidth(worksheet2, 9, 23);

            Excel.SetColumnWidth(worksheet2, 11, 23);
            Excel.SetColumnWidth(worksheet2, 13, 23);

            Excel.SetColumnWidth(worksheet2, 15, 23);
            Excel.SetColumnWidth(worksheet2, 16, 23);

            Excel.SetColumnWidth(worksheet2, 17, 23);

            var autoFilter4 = new AutoFilter { Reference = "A1:R" + xmlDasList.Count + 1 };
            worksheet4.Append(autoFilter4);

            #endregion
            worksheet4.Save();

            #region  |  Keystroke data  |

            Excel.SetCellValue(spreadsheet, worksheet3, 1, 1, "Activity ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 2, 1, "Activity Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 3, 1, "Document ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 4, 1, "Document Name", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 5, 1, "Record ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 6, 1, "Paragraph ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 7, 1, "Segment ID", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 8, 1, "Created", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 9, 1, "SHIFT", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 10, 1, "ALT", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 11, 1, "CTRL", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 12, 1, "Text", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 13, 1, "Key", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 14, 1, "Selection", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 15, 1, "Origin System", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 16, 1, "Origin Type", false, false);
            Excel.SetCellValue(spreadsheet, worksheet3, 17, 1, "Match", false, false);
	        Excel.SetCellValue(spreadsheet, worksheet3, 18, 1, "Position", false, false);
	        //Excel.SetCellValue(spreadsheet, worksheet3, 19, 1, "X", false, false);
	        //Excel.SetCellValue(spreadsheet, worksheet3, 20, 1, "Y", false, false);
			index = 1;
            foreach (var activity in xmlDasList)
            {
                Project activityProject = null;
                var activity1 = activity;
                if (projectsTmp.Exists(a => a.Id == activity1.ProjectId))
                    activityProject = projectsTmp.Find(a => a.Id == activity.ProjectId);
                else
                {
                    activityProject = Helper.GetProjectFromId(activity.ProjectId);
                    projectsTmp.Add(activityProject);
                }

                Activity projectActivity = null;
                if (activityProject.Activities.Exists(a => a.Id == activity.ActivityId))
                    projectActivity = activityProject.Activities.Find(a => a.Id == activity.ActivityId);
                else
                {
                    projectActivity = activityProject.Activities.Find(a => a.Id == activity.ActivityId);
                    projectActivitiesTmp.Add(projectActivity);
                }

                if (activity.KeyStrokes == null) continue;
                foreach (var t in activity.KeyStrokes)
                {
                    index += 1;
                    var uIndex = Convert.ToUInt32(index);

                    var ks = t;

                    Excel.SetCellValue(spreadsheet, worksheet3, 1, uIndex, activity.ActivityId.ToString(), false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 2, uIndex, projectActivity != null ? projectActivity.Name : string.Empty, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 3, uIndex, activity.DocumentId, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 4, uIndex, activity.DocumentName, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 5, uIndex, activity.RecordId, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 6, uIndex, activity.ParagraphId, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 7, uIndex, activity.SegmentId, false, false);
                    if (ks.Created != null)
                        Excel.SetCellValue(spreadsheet, worksheet3, 8, uIndex, Helper.GetStringFromDateTimeMilli(ks.Created.Value).Replace("T", " "), false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 9, uIndex, ks.Shift.ToString(), false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 10, uIndex, ks.Alt.ToString(), false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 11, uIndex, ks.Ctrl.ToString(), false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 12, uIndex, ks.Text, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 13, uIndex, ks.Key, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 14, uIndex, ks.Selection, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 15, uIndex, ks.OriginType, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 16, uIndex, ks.OriginSystem, false, false);
                    Excel.SetCellValue(spreadsheet, worksheet3, 17, uIndex, ks.Match, false, false);
	                Excel.SetCellValue(spreadsheet, worksheet3, 18, uIndex, ks.Position.ToString(), false, false);
	                //Excel.SetCellValue(spreadsheet, worksheet3, 19, uIndex, ks.X.ToString(), false, false);
	                //Excel.SetCellValue(spreadsheet, worksheet3, 20, uIndex, ks.Y.ToString(), false, false);
				}
            }
            Excel.SetColumnWidth(worksheet3, 8, 23);
            Excel.SetColumnWidth(worksheet3, 12, 25);
            Excel.SetColumnWidth(worksheet3, 13, 15);
            Excel.SetColumnWidth(worksheet3, 14, 15);
            Excel.SetColumnWidth(worksheet3, 15, 15);
            Excel.SetColumnWidth(worksheet3, 16, 15);

            var autoFilter3 = new AutoFilter { Reference = "A1:Q" + xmlDasList.Count + 1 };
            worksheet3.Append(autoFilter3);

            #endregion
            worksheet3.Save();


            spreadsheet.Close();

        }
        public void create_xml_report(string fileName, List<Activity> tpas, List<XmlDocumentActivity> xmlDasList)
        {
            var projectsTmp = new List<Project>();
            var companyProfilesTmp = new List<CompanyProfile>();

            var clients = new Dictionary<int, List<Activity>>();
            foreach (var t in tpas)
                if (!clients.ContainsKey(t.CompanyProfileId))
                    clients.Add(t.CompanyProfileId, tpas);


            using (var xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8))
            {
             
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("QualitivityProfessional");
                xmlTextWriter.WriteAttributeString("created", Helper.GetStringFromDateTime(DateTime.Now));
                xmlTextWriter.WriteAttributeString("output_version", "1.6");

                var recordIdsAdded = new List<string>();

                foreach (var kvpClients in clients)
                {
                    if (kvpClients.Value.Count <= 0) continue;
                    var companyProfileId = kvpClients.Key;



                    CompanyProfile activityCompanyProfile = null;
                    if (projectsTmp.Exists(a => a.Id == companyProfileId))
                        activityCompanyProfile = companyProfilesTmp.Find(a => a.Id == companyProfileId);
                    else
                    {
                        activityCompanyProfile = Helper.GetClientFromId(companyProfileId);
                        if (activityCompanyProfile != null)
                            companyProfilesTmp.Add(activityCompanyProfile);
                    }


                    xmlTextWriter.WriteStartElement("Client");
                    xmlTextWriter.WriteAttributeString("id", companyProfileId.ToString());
                    xmlTextWriter.WriteAttributeString("name", activityCompanyProfile != null ? activityCompanyProfile.Name : string.Empty);


                    var projects = new Dictionary<int, List<Activity>>();
                    foreach (var t in kvpClients.Value)
                        if (!projects.ContainsKey(t.ProjectId) && companyProfileId == t.CompanyProfileId)
                            projects.Add(t.ProjectId, kvpClients.Value);


                    foreach (var kvpProjects in projects)
                    {
                        if (kvpProjects.Value.Count <= 0) continue;
                        var projectId = kvpProjects.Key;

                        Project activityProject;
                        if (projectsTmp.Exists(a => a.Id == projectId))
                            activityProject = projectsTmp.Find(a => a.Id == projectId);
                        else
                        {
                            activityProject = Helper.GetProjectFromId(projectId);
                            projectsTmp.Add(activityProject);
                        }

                        #region  |  Project  |
                        xmlTextWriter.WriteStartElement("Project");
                        xmlTextWriter.WriteAttributeString("id", kvpProjects.Value[0].ProjectId.ToString());
                        xmlTextWriter.WriteAttributeString("name", activityProject.Name);
                        xmlTextWriter.WriteAttributeString("status", activityProject.ProjectStatus);

                        var activities = new Dictionary<int, List<Activity>>();
                        foreach (var t in kvpProjects.Value)
                            if (!activities.ContainsKey(t.Id) && projectId == t.ProjectId)
                                activities.Add(t.Id, kvpProjects.Value);


                        foreach (var kvpActivities in projects)
                        {
                            if (kvpActivities.Value.Count <= 0) continue;
                            foreach (var activity in kvpActivities.Value)
                            {
                                #region  |  Activity  |


                                var pemTotal = activity.LanguageRateChecked ? (decimal)activity.DocumentActivityRates.LanguageRateTotal : 0;
                                var hrTotal = activity.HourlyRateChecked ? (decimal)activity.DocumentActivityRates.HourlyRateTotal : 0;
                                var total = decimal.Add(pemTotal, hrTotal);

                                xmlTextWriter.WriteStartElement("Activity");
                                xmlTextWriter.WriteAttributeString("id", activity.Id.ToString());
                                xmlTextWriter.WriteAttributeString("name", activity.Name);
                                xmlTextWriter.WriteAttributeString("description", activity.Description);
                                xmlTextWriter.WriteAttributeString("confirmationLevel", activity.ActivityStatus.ToString());
                                xmlTextWriter.WriteAttributeString("billable", activity.Billable.ToString());
                                xmlTextWriter.WriteAttributeString("started", Helper.GetStringFromDateTime(activity.Started.Value));
                                xmlTextWriter.WriteAttributeString("stopped", Helper.GetStringFromDateTime(activity.Stopped.Value));
                                xmlTextWriter.WriteAttributeString("activeSeconds", Convert.ToInt32(Helper.GetTotalTicksFromActivityDocuments(activity.Activities) / 10000000).ToString());
                                xmlTextWriter.WriteAttributeString("documents", activity.Activities.Count.ToString());
                                xmlTextWriter.WriteAttributeString("totalLanguagRate", pemTotal.ToString(CultureInfo.InvariantCulture));
                                xmlTextWriter.WriteAttributeString("totalHourlyRate", hrTotal.ToString(CultureInfo.InvariantCulture));
                                xmlTextWriter.WriteAttributeString("total", total.ToString(CultureInfo.InvariantCulture));
                                xmlTextWriter.WriteAttributeString("currency", activity.DocumentActivityRates.HourlyRateCurrency);



                                foreach (var xmlDocument in xmlDasList)
                                {
                                    if (xmlDocument.ActivityId != activity.Id) continue;
                                    var xList = new List<XmlDocumentActivity>();
                                    foreach (var xmlRecord in xmlDasList)
                                    {
                                        if (recordIdsAdded.Contains(xmlRecord.RecordId)) continue;
                                        if (xmlRecord.DocumentId != xmlDocument.DocumentId ||
                                            xmlRecord.ActivityId != activity.Id) continue;
                                        recordIdsAdded.Add(xmlRecord.RecordId);
                                        xList.Add(xmlRecord);
                                    }

                                    if (xList.Count <= 0) continue;
                                    {
                                        xmlTextWriter.WriteStartElement("Document");
                                        xmlTextWriter.WriteAttributeString("id", xmlDocument.DocumentId);
                                        xmlTextWriter.WriteAttributeString("name", xmlDocument.DocumentName);
                                        xmlTextWriter.WriteAttributeString("started", Helper.GetStringFromDateTimeMilli(xmlDocument.DocumentStartDate.Value));
                                        xmlTextWriter.WriteAttributeString("stopped", Helper.GetStringFromDateTimeMilli(xmlDocument.DocumentStopDate.Value));
                                        xmlTextWriter.WriteAttributeString("activeSeconds", xmlDocument.DocumentTotalSeconds.ToString());
                                        xmlTextWriter.WriteAttributeString("sourceLang", xmlDocument.SourceLang);
                                        xmlTextWriter.WriteAttributeString("targetLang", xmlDocument.TargetLang);


                                        foreach (var xmlRecord in xList)
                                        {


                                            #region  |  Record  |

                                            xmlTextWriter.WriteStartElement("Record");

                                            xmlTextWriter.WriteAttributeString("id", xmlRecord.RecordId);
                                            xmlTextWriter.WriteAttributeString("paragraphId", xmlRecord.ParagraphId);
                                            xmlTextWriter.WriteAttributeString("segmentId", xmlRecord.SegmentId);



                                            xmlTextWriter.WriteAttributeString("started", Helper.GetStringFromDateTimeMilli(xmlRecord.StartDate.Value));
                                            xmlTextWriter.WriteAttributeString("stopped", Helper.GetStringFromDateTimeMilli(xmlRecord.StopDate.Value));
                                            xmlTextWriter.WriteAttributeString("activeseconds", xmlRecord.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                                            xmlTextWriter.WriteAttributeString("activeMiliseconds", xmlRecord.TotalMiliseconds.ToString(CultureInfo.InvariantCulture));
                                            xmlTextWriter.WriteAttributeString("wordsSource", xmlRecord.WordsSource.ToString(CultureInfo.InvariantCulture));

                                            xmlTextWriter.WriteAttributeString("editDistance", xmlRecord.EditDistance.ToString(CultureInfo.InvariantCulture));
                                            xmlTextWriter.WriteAttributeString("editDistanceRelative", xmlRecord.EditDistanceRelative.ToString(CultureInfo.InvariantCulture));
                                            xmlTextWriter.WriteAttributeString("pemPercentage", xmlRecord.PemPercentage.ToString(CultureInfo.InvariantCulture));

                                            xmlTextWriter.WriteStartElement("translationOrigins");
                                            xmlTextWriter.WriteStartElement("original");
                                            xmlTextWriter.WriteAttributeString("confirmationLevel", xmlRecord.OriginalConfirmationLevel);
                                            xmlTextWriter.WriteAttributeString("translationStatus", xmlRecord.OriginalTranslationStatus);
                                            xmlTextWriter.WriteAttributeString("originType", xmlRecord.OriginalOriginType);
                                            xmlTextWriter.WriteAttributeString("originSystem", xmlRecord.OriginalOriginSystem);
                                            xmlTextWriter.WriteEndElement();//original 

                                            xmlTextWriter.WriteStartElement("updated");
                                            xmlTextWriter.WriteAttributeString("confirmationLevel", xmlRecord.UpdatedConfirmationLevel);
                                            xmlTextWriter.WriteAttributeString("translationStatus", xmlRecord.UpdatedTranslationStatus);
                                            xmlTextWriter.WriteAttributeString("originType", xmlRecord.UpdatedOriginType);
                                            xmlTextWriter.WriteAttributeString("originSystem", xmlRecord.UpdatedOriginSystem);
                                            xmlTextWriter.WriteEndElement();//updated 
                                            xmlTextWriter.WriteEndElement();//translationOrigin 

                                            xmlTextWriter.WriteStartElement("contentText");

                                            xmlTextWriter.WriteStartElement("source");
                                            xmlTextWriter.WriteString(xmlRecord.SourceText);
                                            xmlTextWriter.WriteEndElement();//sourceText                                                          

                                            xmlTextWriter.WriteStartElement("targetOriginal");
                                            xmlTextWriter.WriteString(xmlRecord.TargetText);
                                            xmlTextWriter.WriteEndElement();//targetTextOriginal

                                            xmlTextWriter.WriteStartElement("targetUpdated");
                                            xmlTextWriter.WriteString(xmlRecord.UpdatedText);
                                            xmlTextWriter.WriteEndElement();//targetTextUpdated

                                            xmlTextWriter.WriteEndElement();//contentSections

                                            if (xmlRecord.QualityMetrics != null)
                                            {


                                                xmlTextWriter.WriteStartElement("qualityMetrics");


                                                foreach (var qm in xmlRecord.QualityMetrics)
                                                {
                                                    if (qm.Name.Trim() == string.Empty) continue;
                                                    xmlTextWriter.WriteStartElement("qualityMetric");

                                                    xmlTextWriter.WriteAttributeString("id", qm.Id);
                                                    xmlTextWriter.WriteAttributeString("guid", qm.Guid);
                                                    xmlTextWriter.WriteAttributeString("name", qm.Name);
                                                    xmlTextWriter.WriteAttributeString("status", qm.Status.ToString());
                                                    xmlTextWriter.WriteAttributeString("severity", qm.SeverityName);
                                                    xmlTextWriter.WriteAttributeString("severityWeight", qm.SeverityValue.ToString());
                                                    xmlTextWriter.WriteAttributeString("created", Helper.GetStringFromDateTimeMilli(qm.Created.Value));
                                                    xmlTextWriter.WriteAttributeString("modified", Helper.GetStringFromDateTimeMilli(qm.Modified.Value));
                                                    xmlTextWriter.WriteAttributeString("user_name", qm.UserName);


                                                    xmlTextWriter.WriteStartElement("content");
                                                    xmlTextWriter.WriteString(qm.Content);
                                                    xmlTextWriter.WriteEndElement();//content

                                                    xmlTextWriter.WriteStartElement("comment");
                                                    xmlTextWriter.WriteString(qm.Comment);
                                                    xmlTextWriter.WriteEndElement();//comment

                                                    xmlTextWriter.WriteEndElement();//qualityMetric
                                                }


                                                xmlTextWriter.WriteEndElement();//qualityMetrics


                                            }

                                            xmlTextWriter.WriteStartElement("keyStrokes");
                                            if (xmlRecord.KeyStrokes != null && xmlRecord.KeyStrokes.Count > 0)
                                            {
                                                foreach (var ks in xmlRecord.KeyStrokes)
                                                {
                                                    xmlTextWriter.WriteStartElement("ks");
                                                    xmlTextWriter.WriteAttributeString("created", Helper.GetStringFromDateTimeMilli(ks.Created.Value));
                                                    xmlTextWriter.WriteAttributeString("alt", ks.Alt.ToString());
                                                    xmlTextWriter.WriteAttributeString("ctrl", ks.Ctrl.ToString());
                                                    xmlTextWriter.WriteAttributeString("shift", ks.Shift.ToString());
                                                    xmlTextWriter.WriteAttributeString("key", ks.Key);
                                                    xmlTextWriter.WriteAttributeString("text", ks.Text);
                                                    xmlTextWriter.WriteAttributeString("selection", ks.Selection);
                                                    xmlTextWriter.WriteAttributeString("system", ks.OriginSystem);
                                                    xmlTextWriter.WriteAttributeString("origin", ks.OriginType);
                                                    xmlTextWriter.WriteAttributeString("match", ks.Match);
	                                                xmlTextWriter.WriteAttributeString("position", ks.Position.ToString());
	                                                //xmlTextWriter.WriteAttributeString("x", ks.X.ToString());
	                                                //xmlTextWriter.WriteAttributeString("y", ks.Y.ToString());
													xmlTextWriter.WriteEndElement();//ks
                                                }
                                            }
                                            xmlTextWriter.WriteEndElement();//key_strokes


                                            xmlTextWriter.WriteStartElement("comments");
                                            if (xmlRecord.Comments != null && xmlRecord.Comments.Count > 0)
                                            {
                                                foreach (var comment in xmlRecord.Comments)
                                                {
                                                    xmlTextWriter.WriteStartElement("Comment");
                                                    xmlTextWriter.WriteAttributeString("author", comment.Author);
                                                    xmlTextWriter.WriteAttributeString("created", Helper.GetStringFromDateTime(comment.Created.Value));
                                                    xmlTextWriter.WriteAttributeString("severity", comment.Severity);
                                                    xmlTextWriter.WriteAttributeString("version", comment.Version);
                                                    xmlTextWriter.WriteString(comment.Content);
                                                    xmlTextWriter.WriteEndElement();//Comments
                                                }
                                            }
                                            xmlTextWriter.WriteEndElement();//Comments


                                            xmlTextWriter.WriteEndElement();//Record

                                            #endregion

                                        }
                                        xmlTextWriter.WriteEndElement();//Document
                                    }
                                }

                                xmlTextWriter.WriteEndElement();//Activity
                            }
                        }

                        xmlTextWriter.WriteEndElement();//Project

                        #endregion
                    }
                    xmlTextWriter.WriteEndElement();//Client
                    #endregion
                }
                xmlTextWriter.WriteEndElement();

            }
        }
    }
}
