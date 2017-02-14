using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Qualitivity.Dialogs.Export;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Report;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Qualitivity.Dialogs
{
    public partial class ActivityReportWizard : Form
    {
        internal bool Saved { get; set; }

        public ActivityReportWizard()
        {
            InitializeComponent();



            #region  |  setup the activity listview control  |


            olvColumn_source.AspectGetter = delegate(object x)
            {
                var source = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    source = ((Activity)x).Activities[0].TranslatableDocument.SourceLanguage;
                }
                return source;
            };
            olvColumn_source.ImageGetter = delegate(object x)
            {
                var source = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    source = ((Activity)x).Activities[0].TranslatableDocument.SourceLanguage + ".gif";
                }
                return !imageList1.Images.ContainsKey(source) ? "empty.png" : source;
            };

            olvColumn_target.AspectGetter = delegate(object x)
            {
                var target = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    target = ((Activity)x).Activities[0].TranslatableDocument.TargetLanguage;
                }
                return target;
            };
            olvColumn_target.ImageGetter = delegate(object x)
            {
                var target = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    target = ((Activity)x).Activities[0].TranslatableDocument.TargetLanguage + ".gif";
                }
                return !imageList1.Images.ContainsKey(target) ? "empty.png" : target;
            };


            olvColumn_activity_status.ImageGetter =
                x => ((Activity)x).ActivityStatus == Activity.Status.New ? "question_blue" : "tick";


            olvColumn_billable.ImageGetter = x => ((Activity)x).Billable ? "vyes" : "vno";

            olvColumn_documents.AspectGetter = x => ((Activity)x).Activities.Count.ToString();
            olvColumn_activity_total.AspectGetter = delegate(object x)
            {
                var totalHr = Math.Round(((Activity)x).DocumentActivityRates.HourlyRateTotal, 2);
                totalHr = ((Activity)x).HourlyRateChecked ? totalHr : 0;

                var totalPem = Math.Round(((Activity)x).DocumentActivityRates.LanguageRateTotal, 2);
                totalPem = ((Activity)x).LanguageRateChecked ? totalPem : 0;


                var totalCustom = Math.Round(((Activity)x).DocumentActivityRates.CustomRateTotal, 2);
                totalCustom = ((Activity)x).CustomRateChecked ? totalCustom : 0;

                var total = Math.Round(totalHr + totalPem + totalCustom, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                return total + " " + currency;

            };
            olvColumn_hr_total.AspectGetter = delegate(object x)
            {
                var total = Math.Round(((Activity)x).DocumentActivityRates.HourlyRateTotal, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                total = ((Activity)x).HourlyRateChecked ? total : 0;
                return total + " " + currency;

            };
            olvColumn_pem_total.AspectGetter = delegate(object x)
            {
                var total = Math.Round(((Activity)x).DocumentActivityRates.LanguageRateTotal, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                total = ((Activity)x).LanguageRateChecked ? total : 0;
                return total + " " + currency;

            };
            olvColumn_custom_total.AspectGetter = delegate(object x)
            {
                var total = Math.Round(((Activity)x).DocumentActivityRates.CustomRateTotal, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                total = ((Activity)x).CustomRateChecked ? total : 0;
                return total + " " + currency;

            };


            olvColumn_activity_name.ImageGetter = delegate
            {
                return "calendar";
            };

            #endregion


            panel_welcome.Dock = DockStyle.Fill;
            panel_project_activties.Dock = DockStyle.Fill;
            panel_report_options.Dock = DockStyle.Fill;
            panel_processing.Dock = DockStyle.Fill;
        }

        public List<Activity> SelectedActivities { get; set; }

        public Project SelectedProject { get; set; }

        public bool Finished { get; set; }
        private int IndexMinimum { get; set; }
        private int IndexMaximum { get; set; }
        private int IndexCurrent { get; set; }

        private string DateTimeStamp { get; set; }


        private void AddressDetails_Load(object sender, EventArgs e)
        {



            Finished = false;
            IndexMinimum = 0;
            IndexMaximum = 3;
            IndexCurrent = 0;

            Saved = false;

            show_panel(false);

            initialize_Onload();



        }


        private void initialize_Onload()
        {

            DateTimeStamp = ".DATE=" + DateTime.Now.Year
                + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0')
                + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0')
                + "." + DateTime.Now.Minute.ToString().PadLeft(2, '0')
                + "." + DateTime.Now.Second.ToString().PadLeft(2, '0');

            var qualitivityReportsPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsPath.Trim(), "Reports");
            if (!Directory.Exists(qualitivityReportsPath))
                Directory.CreateDirectory(qualitivityReportsPath);

            var qualitivityReportsYear = Path.Combine(qualitivityReportsPath, DateTime.Now.Year.ToString());
            if (!Directory.Exists(qualitivityReportsYear))
                Directory.CreateDirectory(qualitivityReportsYear);

            var qualitivityReportsMonth = Path.Combine(qualitivityReportsYear, DateTime.Now.Month.ToString().PadLeft(2, '0'));
            if (!Directory.Exists(qualitivityReportsMonth))
                Directory.CreateDirectory(qualitivityReportsMonth);

            textBox_output_folder.Text = qualitivityReportsMonth;

            textBox_report_compresson_name.Text = @"Activity Report - PID=" + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + DateTimeStamp;
            try
            {
                objectListView1.BeginUpdate();

                objectListView1.ShowGroups = false;

                if (SelectedProject != null)
                {
                    objectListView1.SetObjects(SelectedProject.Activities);

                    foreach (var activity in SelectedProject.Activities)
                        activity.IsChecked = false;
                }


                if (objectListView1.Items.Count > 0)
                {
                    foreach (OLVListItem lvi in objectListView1.Items)
                    {
                        var activity = lvi.RowObject as Activity;
                        if (SelectedActivities.Exists(a => activity != null && a.Id == activity.Id))
                            lvi.Checked = true;
                    }
                    objectListView1.SelectedIndex = 0;
                }

                checkBox_project_activity_export_to_file_CheckedChanged(null, null);
                checkBox_project_activity_report_CheckedChanged(null, null);
                checkBox_report_compression_CheckedChanged(null, null);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                objectListView1.EndUpdate();

                check_enabled();
            }
        }


        private void show_panel(bool back)
        {
            switch (IndexCurrent)
            {
                case 0:
                    {
                        Text = PluginResources.Qualitivity___Activity_Report_Wizard;
                        panel_welcome.BringToFront();
                    } break;
                case 1:
                    {

                        Text = PluginResources.Qualitivity___Activity_Report_Task___Step_1_of_3;
                        panel_project_activties.BringToFront();

                    } break;
                case 2:
                    {
                        Text = PluginResources.Qualitivity___Activity_Report_Task___Step_2_of_3;

                        panel_report_options.BringToFront();
                    } break;
                case 3:
                    {
                        Text = PluginResources.Qualitivity___Activity_Report_Task___Step_3_of_3;

                        panel_processing.BringToFront();
                        Application.DoEvents();




                        panel_button_control.Enabled = false;



                        var maximumReports = 0;
                        #region  |  get maximum report count  |
                        if (checkBox_project_activity_report.Checked)
                        {
                            if (checkBox_project_activity_report_single.Checked)
                                maximumReports++;
                            else
                                maximumReports += objectListView1.CheckedItems.Count;
                        }
                        if (checkBox_project_activity_document_report.Checked)
                            maximumReports += objectListView1.CheckedItems.Count;

                        if (checkBox_project_activity_quality_metric_report.Checked)
                            maximumReports += objectListView1.CheckedItems.Count;

                        if (checkBox_project_activity_export_to_file_excel.Checked
                                   && (checkBox_project_activity_export_to_file_excel.Checked || checkBox_project_activity_export_to_file_xml.Checked))
                        {
                            if (checkBox_project_activity_export_to_file_excel.Checked)
                            {
                                if (checkBox_project_activity_export_to_file_single.Checked)
                                    maximumReports++;
                                else
                                    maximumReports += objectListView1.CheckedItems.Count;
                            }
                            if (checkBox_project_activity_export_to_file_xml.Checked)
                            {
                                if (checkBox_project_activity_export_to_file_single.Checked)
                                    maximumReports++;
                                else
                                    maximumReports += objectListView1.CheckedItems.Count;
                            }
                        }
                        #endregion

                        try
                        {
                            progressBar_import_progress.Value = 0;
                            progressBar_import_progress.Maximum = maximumReports;
                            label_progress_message.Text = string.Format(PluginResources.__0__entries_processed, 0);
                            label_progress_percentage.Text = @"0%";

                            Application.DoEvents();
                            Cursor = Cursors.WaitCursor;

                            if (maximumReports > 0)
                            {
                                var checkedActivities = (from OLVListItem lvi in objectListView1.CheckedItems select lvi.RowObject as Activity).ToList();


                                var documentActivitiesDict = checkedActivities.ToDictionary(activity => activity.Id, Helper.GetDocumentActivityObjects);


                                var companyProfile = new CompanyProfile();
                                var projectName = string.Empty;
                                if (SelectedProject != null)
                                {
                                    companyProfile = Helper.GetClientFromId(SelectedProject.CompanyProfileId);
                                    projectName = SelectedProject.Name;
                                }

                                var reports = new Processor();
                                var currentCounter = 0;

                                var reportFiles = new List<string>();

                                #region  |  checkBox_project_activity_report  |
                                if (checkBox_project_activity_report.Checked)
                                {
                                    if (checkBox_project_activity_report_single.Checked)
                                    {
                                        UpdateProgressCounter(currentCounter++, maximumReports);


                                        var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Project.Activity_o_.xml.html");
                                        var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Project.Activity_o_.xml");
                                        reports.CreateActivityReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, SelectedProject
                                            , checkedActivities, companyProfile, Tracked.Settings.UserProfile, documentActivitiesDict);

                                        var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Activity PID="
                                            + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + DateTimeStamp + ".html");

                                        File.Move(htmlFileFullPath, oFile);

                                        reportFiles.Add(oFile);
                                    }
                                    else
                                    {
                                        foreach (var activity in checkedActivities)
                                        {
                                            UpdateProgressCounter(currentCounter++, maximumReports);

                                            var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Project.Activity_o_.xml.html");
                                            var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Project.Activity_o_.xml");
                                            reports.CreateActivityReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, SelectedProject
                                                , new List<Activity> { activity }, companyProfile, Tracked.Settings.UserProfile, documentActivitiesDict);

                                            var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Activity PID="
                                            + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + ".AID=" + activity.Id.ToString().PadLeft(6, '0') + DateTimeStamp + ".html");

                                            File.Move(htmlFileFullPath, oFile);

                                            reportFiles.Add(oFile);
                                        }
                                    }
                                }
                                #endregion

                                #region  |  checkBox_project_activity_document_report  |

                                if (checkBox_project_activity_document_report.Checked)
                                {
                                    foreach (var activity in checkedActivities)
                                    {
                                        UpdateProgressCounter(currentCounter++, maximumReports);

                                        var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, @"Qualitivity.Activity.Document.Overview_o_.xml.html");
                                        var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, @"Qualitivity.Activity.Document.Overview_o_.xml");
                                        reports.CreateTrackChangesReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, documentActivitiesDict[activity.Id], activity, companyProfile);

                                        var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Activity Document Overview PID="
                                            + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + ".AID=" + activity.Id.ToString().PadLeft(6, '0') + DateTimeStamp + ".html");

                                        File.Move(htmlFileFullPath, oFile);

                                        reportFiles.Add(oFile);
                                    }
                                }

                                #endregion

                                #region  |  checkBox_project_activity_quality_metric_report  |

                                if (checkBox_project_activity_quality_metric_report.Checked)
                                {
                                    foreach (var activity in checkedActivities)
                                    {
                                        UpdateProgressCounter(currentCounter++, maximumReports);

                                        var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, @"Qualitivity.Activity.Quality.Metrics_o_.xml.html");
                                        var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, @"Qualitivity.Activity.Quality.Metrics_o_.xml");
                                        reports.CreateQualityMetricsReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, projectName, documentActivitiesDict[activity.Id], activity, companyProfile);

                                        var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Activity Quality Metrics PID="
                                            + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + ".AID=" + activity.Id.ToString().PadLeft(6, '0') + DateTimeStamp + ".html");

                                        File.Move(htmlFileFullPath, oFile);
                                        reportFiles.Add(oFile);

                                    }
                                }

                                #endregion

                                #region  |  checkBox_project_activity_export_to_file_excel  |

                                if (checkBox_project_activity_export_to_file_excel.Checked
                                    && (checkBox_project_activity_export_to_file_excel.Checked || checkBox_project_activity_export_to_file_xml.Checked))
                                {
                                    var xmlDasListDict = new Dictionary<int, List<XmlDocumentActivity>>();

                                    foreach (var activity in checkedActivities)
                                    {
                                        #region  |  create output data  |

                                        var xmlDasList = new List<XmlDocumentActivity>();
                                        foreach (var da in documentActivitiesDict[activity.Id])
                                        {
                                            foreach (var record in da.Records)
                                            {
                                                #region  |  create xml_document_activity object   |

                                                var xmlDas = new XmlDocumentActivity
                                                {
                                                    ProjectId = da.ProjectId,
                                                    ProjectName = SelectedProject.Name,
                                                    ActivityId = da.ProjectActivityId,
                                                    ActivityName = activity.Name,
                                                    RecordId = record.Id.ToString(),
                                                    DocumentId = da.DocumentId,
                                                    DocumentName = da.TranslatableDocument.DocumentName,
                                                    DocumentStartDate = da.Started,
                                                    DocumentStopDate = da.Stopped,
                                                    DocumentTotalSeconds = da.TicksActivity/10000000,
                                                    ParagraphId = record.ParagraphId,
                                                    SegmentId = record.SegmentId,
                                                    OriginalConfirmationLevel =
                                                        record.TranslationOrigins.Original.ConfirmationLevel,
                                                    OriginalTranslationStatus =
                                                        record.TranslationOrigins.Original.TranslationStatus,
                                                    OriginalOriginSystem =
                                                        record.TranslationOrigins.Original.OriginSystem,
                                                    OriginalOriginType = record.TranslationOrigins.Original.OriginType,
                                                    UpdatedConfirmationLevel =
                                                        record.TranslationOrigins.Updated.ConfirmationLevel,
                                                    UpdatedTranslationStatus =
                                                        record.TranslationOrigins.Updated.TranslationStatus,
                                                    UpdatedOriginSystem = record.TranslationOrigins.Updated.OriginSystem,
                                                    UpdatedOriginType = record.TranslationOrigins.Updated.OriginType,
                                                    SourceLang = da.TranslatableDocument.SourceLanguage,
                                                    TargetLang = da.TranslatableDocument.TargetLanguage,
                                                    SourceText =
                                                        Helper.GetCompiledSegmentText(
                                                            record.ContentSections.SourceSections,
                                                            activity.ComparisonOptions.IncludeTagsInComparison),
                                                    TargetText =
                                                        Helper.GetCompiledSegmentText(
                                                            record.ContentSections.TargetOriginalSections,
                                                            activity.ComparisonOptions.IncludeTagsInComparison),
                                                    UpdatedText =
                                                        Helper.GetCompiledSegmentText(
                                                            record.ContentSections.TargetUpdatedSections,
                                                            activity.ComparisonOptions.IncludeTagsInComparison)
                                                };




                                                if (xmlDas.OriginalTranslationStatus == string.Empty && xmlDas.TargetText.Trim() == string.Empty)
                                                    xmlDas.OriginalTranslationStatus = @"Not Translated";



                                                xmlDas.StartDate = record.Started;
                                                xmlDas.StopDate = record.Stopped;
                                                xmlDas.TotalSeconds = record.TicksElapsed / 10000000;
                                                xmlDas.TotalMiliseconds = record.TicksElapsed / 10000;



                                                var dld = new EditDistance(record, activity);

                                                xmlDas.WordsSource = record.WordCount;
                                                xmlDas.EditDistance = dld.Edits;
                                                xmlDas.EditDistanceRelative = dld.EditDistanceRelative;
                                                xmlDas.PemPercentage = string.Compare(record.TranslationOrigins.Updated.OriginType, @"interactive", StringComparison.OrdinalIgnoreCase) == 0 ? dld.PemPercentage : 100;


                                                xmlDas.KeyStrokes = record.TargetKeyStrokes;

                                                if (record.QualityMetrics != null)
                                                    xmlDas.QualityMetrics = record.QualityMetrics;



                                                #region  |  comments  |
                                                xmlDas.CommentsStr = string.Empty;
                                                xmlDas.Comments = record.Comments;
                                                if (record.Comments != null && record.Comments.Count > 0)
                                                {
                                                    foreach (var comment in record.Comments)
                                                    {
                                                        xmlDas.CommentsStr += (xmlDas.CommentsStr.Trim() != string.Empty ? "\r\n\r\n" : string.Empty)
                                                            + PluginResources.Created__ + Helper.GetStringFromDateTimeMilli(comment.Created.Value)
                                                            + PluginResources.___Severity__ + comment.Severity
                                                            + PluginResources.___Author__ + comment.Author
                                                            + PluginResources.___Comment__ + comment.Content;
                                                    }
                                                }
                                                #endregion

                                                xmlDasList.Add(xmlDas);
                                                #endregion
                                            }
                                        }

                                        xmlDasListDict.Add(activity.Id, xmlDasList);

                                        #endregion
                                    }

                                    var exportToFile = new ExportToFile();

                                    if (checkBox_project_activity_export_to_file_single.Checked)
                                    {

                                        var xmlList = new List<XmlDocumentActivity>();
                                        foreach (var xmlListItem in xmlDasListDict.Values)
                                            xmlList.AddRange(xmlListItem);

                                        if (checkBox_project_activity_export_to_file_excel.Checked)
                                        {
                                            UpdateProgressCounter(currentCounter++, maximumReports);

                                            var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Export PID="
                                                + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + DateTimeStamp + ".xlsx");

                                            exportToFile.create_excel_report(oFile, checkedActivities, xmlList);
                                            reportFiles.Add(oFile);
                                        }
                                        if (checkBox_project_activity_export_to_file_xml.Checked)
                                        {
                                            UpdateProgressCounter(currentCounter++, maximumReports);

                                            var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Export PID="
                                                + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + DateTimeStamp + ".xml");

                                            exportToFile.create_xml_report(oFile, checkedActivities, xmlList);
                                            reportFiles.Add(oFile);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var activity in checkedActivities)
                                        {
                                            if (checkBox_project_activity_export_to_file_excel.Checked)
                                            {
                                                UpdateProgressCounter(currentCounter++, maximumReports);

                                                var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Export PID="
                                                + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + ".AID=" + activity.Id.ToString().PadLeft(6, '0') + DateTimeStamp + ".xlsx");

                                                var activity1 = checkedActivities.Find(a => a.Id == activity.Id);
                                                exportToFile.create_excel_report(oFile, new List<Activity> { activity1 }, xmlDasListDict[activity.Id]);
                                                reportFiles.Add(oFile);
                                            }
                                            if (!checkBox_project_activity_export_to_file_xml.Checked) continue;
                                            {
                                                UpdateProgressCounter(currentCounter++, maximumReports);

                                                var oFile = Path.Combine(textBox_output_folder.Text.Trim(), @"Qualitivity Export PID="
                                                                                                            + (SelectedProject != null ? SelectedProject.Id.ToString().PadLeft(6, '0') : "000000") + ".AID=" + activity.Id.ToString().PadLeft(6, '0') + DateTimeStamp + ".xml");

                                                var activity1 = checkedActivities.Find(a => a.Id == activity.Id);

                                                exportToFile.create_xml_report(oFile, new List<Activity> { activity1 }, xmlDasListDict[activity.Id]);
                                                reportFiles.Add(oFile);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region  |  create zip archive  |

                                if (checkBox_report_compression.Checked && reportFiles.Count > 0)
                                {
                                    var oFileZip = textBox_report_compresson_name.Text.Trim();
                                    if (!oFileZip.ToLower().EndsWith(".zip"))
                                        oFileZip = oFileZip + ".zip";

                                    var oFilePathZip = Path.Combine(textBox_output_folder.Text.Trim(), oFileZip);

                                    using (var newFile = ZipFile.Open(oFilePathZip, ZipArchiveMode.Create))
                                    {
                                        foreach (var file in reportFiles)
                                            newFile.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Fastest);
                                    }


                                    try
                                    {
                                        foreach (var file in reportFiles)
                                            File.Delete(file);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                                #endregion

                                #region  |  open output folder in win explorer  |
                                if (checkBox_open_output_folder_when_complete.Checked)
                                {
                                    if (Directory.Exists(textBox_output_folder.Text.Trim()))
                                    {
                                        Process.Start(textBox_output_folder.Text.Trim());
                                    }
                                    else
                                    {
                                        MessageBox.Show(this, PluginResources.Invalid_directory_, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            panel_button_control.Enabled = true;
                            Cursor = Cursors.Default;
                        }
                    } break;


            }
            check_enabled();
        }


        private void UpdateProgressCounter(int current, int maximum)
        {
            label_progress_message.Text = string.Format(PluginResources.__0__entries_processed, progressBar_import_progress.Value);

            progressBar_import_progress.Maximum = maximum;

            if (current + 1 > progressBar_import_progress.Maximum) return;
            progressBar_import_progress.Value = ++current;

            var perc = Convert.ToDouble(progressBar_import_progress.Value) / Convert.ToDouble(progressBar_import_progress.Maximum);
            label_progress_percentage.Text = Convert.ToString(Math.Round(perc * 100, 0), CultureInfo.InvariantCulture) + "%";

            Application.DoEvents();
            Cursor = Cursors.WaitCursor;
        }




        private void check_enabled()
        {
            bool buttonWizardNextEnabled;
            bool buttonWizardFinishEnabled;

            var buttonWizardBackEnabled = IndexCurrent != IndexMinimum;


            if (IndexCurrent == IndexMaximum)
            {
                buttonWizardNextEnabled = false;
                buttonWizardFinishEnabled = true;
            }
            else
            {
                buttonWizardNextEnabled = true;
                buttonWizardFinishEnabled = false;
            }

            switch (IndexCurrent)
            {
                case 0:
                    {

                    } break;
                case 1:
                    {
                        if (objectListView1.Items.Count == 0 || objectListView1.CheckedItems.Count == 0)
                            buttonWizardNextEnabled = false;

                    } break;
                case 2:
                    {


                        if (textBox_output_folder.Text.Trim() != string.Empty && Directory.Exists(textBox_output_folder.Text))
                        {

                            if (checkBox_report_compression.Checked && textBox_report_compresson_name.Text.Trim() == string.Empty)
                                buttonWizardNextEnabled = false;
                            else
                                buttonWizardNextEnabled = true;
                        }
                        else
                        {
                            buttonWizardNextEnabled = false;
                        }
                    } break;
                case 3:
                    {


                        buttonWizardNextEnabled = false;

                        button_wizard_cancel.Enabled = false;
                        buttonWizardBackEnabled = false;
                    } break;


            }

            label_activity_count.Text = string.Format(PluginResources.Total_Activities__0___Selected__1_, objectListView1.Items.Count, objectListView1.CheckedObjects.Count);

            button_wizard_back.Enabled = buttonWizardBackEnabled;
            button_wizard_next.Enabled = buttonWizardNextEnabled;
            button_wizard_finish.Enabled = buttonWizardFinishEnabled;


        }


        private void button_wizard_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(PluginResources.no_help_found_);
        }

        private void button_wizard_back_Click(object sender, EventArgs e)
        {
            if (IndexCurrent > IndexMinimum)
            {
                IndexCurrent--;
                show_panel(true);
            }
        }

        private void button_wizard_next_Click(object sender, EventArgs e)
        {
            if (IndexCurrent < IndexMaximum)
            {
                IndexCurrent++;
                show_panel(false);
            }
        }

        private void button_wizard_finish_Click(object sender, EventArgs e)
        {



            Finished = true;
            Close();
        }

        private void button_wizard_cancel_Click(object sender, EventArgs e)
        {
            Finished = false;
            Close();
        }



        private void objectListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            check_enabled();
        }

        private void objectListView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            check_enabled();
        }

        private void linkLabel_check_all_listview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                objectListView1.BeginUpdate();
                objectListView1.CheckObjects(objectListView1.Objects);

            }
            finally
            {
                objectListView1.EndUpdate();
                label_activity_count.Text = string.Format(PluginResources.Total_Activities__0___Selected__1_, objectListView1.Items.Count, objectListView1.CheckedObjects.Count);
                check_enabled();
            }
        }

        private void linkLabel_uncheck_all_listview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                objectListView1.BeginUpdate();
                objectListView1.UncheckObjects(objectListView1.Objects);

            }
            finally
            {
                objectListView1.EndUpdate();
                check_enabled();
            }
        }

        private void textBox_output_folder_TextChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void button_select_folder_Click(object sender, EventArgs e)
        {
            try
            {
                var sPath = textBox_output_folder.Text;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }

                var fsd = new FolderSelectDialog
                {
                    Title = PluginResources.Select_Reports_Folder,
                    InitialDirectory = sPath
                };
                if (!fsd.ShowDialog(IntPtr.Zero)) return;
                if (fsd.FileName.Trim() == string.Empty) return;
                sPath = fsd.FileName;


                textBox_output_folder.Text = sPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                check_enabled();
            }
        }

        private void checkBox_report_compression_CheckedChanged(object sender, EventArgs e)
        {
            textBox_report_compresson_name.Enabled = checkBox_report_compression.Checked;
            check_enabled();
        }

        private void textBox_report_compresson_name_TextChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void checkBox_project_activity_report_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_project_activity_report.Checked)
            {
                checkBox_project_activity_report_single.Enabled = true;
            }
            else
            {
                checkBox_project_activity_report_single.Enabled = false;
                checkBox_project_activity_report_single.Checked = false;
            }
            check_enabled();
        }

        private void checkBox_project_activity_report_single_CheckedChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void checkBox_project_activity_document_report_CheckedChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void checkBox_project_activity_quality_metric_report_CheckedChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void checkBox_project_activity_export_to_file_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_project_activity_export_to_file.Checked)
            {
                checkBox_project_activity_export_to_file_single.Enabled = true;
                checkBox_project_activity_export_to_file_excel.Enabled = true;
                checkBox_project_activity_export_to_file_xml.Enabled = true;
            }
            else
            {
                checkBox_project_activity_export_to_file_single.Enabled = false;
                checkBox_project_activity_export_to_file_excel.Enabled = false;
                checkBox_project_activity_export_to_file_xml.Enabled = false;

                checkBox_project_activity_export_to_file_single.Checked = false;
                checkBox_project_activity_export_to_file_excel.Checked = false;
                checkBox_project_activity_export_to_file_xml.Checked = false;
            }

            check_enabled();
        }

        private void checkBox_project_activity_export_to_file_excel_CheckedChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void checkBox_project_activity_export_to_file_xml_CheckedChanged(object sender, EventArgs e)
        {
            check_enabled();
        }

        private void checkBox_open_output_folder_when_complete_CheckedChanged(object sender, EventArgs e)
        {
            check_enabled();
        }


    }
}
