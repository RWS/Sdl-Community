using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Comparison;
using Sdl.Community.DQF.Core;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Report;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.DQF;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.TM.Database;
using Processor = Sdl.Community.DQF.Processor;

namespace Sdl.Community.Qualitivity.Dialogs.DQF
{
    public partial class DqfAddTasks : Form
    {
        internal bool Saved { get; set; }
        public DqfAddTasks()
        {
            InitializeComponent();

            panel_information.Dock = DockStyle.Fill;
            panel_dqf_project.Dock = DockStyle.Fill;
            panel_dqf_documents.Dock = DockStyle.Fill;
            panel_processing_the_upload.Dock = DockStyle.Fill;
        }
        public Activity Activity { get; set; }
        public List<DocumentActivity> DocumentActivities { get; set; }
        public DqfProject DqfProject { get; set; }
        public Project Project { get; set; }

        public bool Finished { get; set; }
        private int IndexMinimum { get; set; }
        private int IndexMaximum { get; set; }
        private int IndexCurrent { get; set; }

        private void button_save_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }
        private void AddressDetails_Load(object sender, EventArgs e)
        {
            Finished = false;
            IndexMinimum = 0;
            IndexMaximum = 3;
            IndexCurrent = 0;

            Saved = false;


            #region  |  DQF Documents  |

            listView_documents.Items.Clear();

            #endregion

            #region  |  DQF Projects  |
            try
            {

                treeView_dqf_projects.BeginUpdate();
                treeView_dqf_projects.Nodes.Clear();
                if (Project != null)
                {
                    foreach (var dqfProject in Project.DqfProjects)
                    {
                        var tn = treeView_dqf_projects.Nodes.Add(dqfProject.Name);
                        tn.ImageIndex = dqfProject.Imported ? 1 : 0;
                        tn.SelectedImageIndex = tn.ImageIndex;
                        tn.Tag = dqfProject;
                    }
                }
                TreeNode tnSelected = null;
                foreach (TreeNode tn in treeView_dqf_projects.Nodes)
                {
                    var dqfProject = tn.Tag as DqfProject;
                    if (dqfProject != null && dqfProject.Id == DqfProject.Id)
                        tnSelected = tn;
                }

                if (treeView_dqf_projects.Nodes.Count > 0)
                {
                    treeView_dqf_projects.SelectedNode = tnSelected ?? treeView_dqf_projects.Nodes[0];
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                treeView_dqf_projects.EndUpdate();
            }
            #endregion



            show_panel(false);


        }

        private void treeView_dqf_projects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView_dqf_projects.SelectedNode != null)
            {
                var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;

                richTextBox_dqf_project_info.Text = string.Empty;
                if (dqfProject != null)
                {
                    richTextBox_dqf_project_info.SelectedText += PluginResources.Project_ID + dqfProject.DqfProjectId + "\r\n";
                    richTextBox_dqf_project_info.SelectedText += PluginResources.Project_Key + dqfProject.DqfProjectKey + "\r\n";

                    richTextBox_dqf_project_info.SelectedText += PluginResources.Project_Name + dqfProject.Name + "\r\n";
                    if (dqfProject.Created != null)
                        richTextBox_dqf_project_info.SelectedText += PluginResources.Created + dqfProject.Created.Value + "\r\n";
                    richTextBox_dqf_project_info.SelectedText += PluginResources.Source_Language + dqfProject.SourceLanguage + "\r\n";


                    richTextBox_dqf_project_info.SelectedText += PluginResources.Content_Type + Configuration.ContentTypes.Find(a => a.Id == dqfProject.ContentType).DisplayName + "\r\n";
                    richTextBox_dqf_project_info.SelectedText += PluginResources.Industry + Configuration.Industries.Find(a => a.Id == dqfProject.Industry).DisplayName + "\r\n";
                    richTextBox_dqf_project_info.SelectedText += PluginResources.Process + Configuration.Processes.Find(a => a.Id == dqfProject.Process).Name + "\r\n";
                    richTextBox_dqf_project_info.SelectedText += PluginResources.Quality_Level + Configuration.QualityLevel.Find(a => a.Id == dqfProject.QualityLevel).DisplayName + "\r\n\r\n";

                    richTextBox_dqf_project_info.SelectedText += PluginResources.Total_Tasks + dqfProject.DqfTasks.Count + "\r\n";
                }


                listView_documents.Items.Clear();


                foreach (var da in DocumentActivities)
                {
                    var item = listView_documents.Items.Add(da.TranslatableDocument.DocumentName);
                    item.Checked = true;
                    item.SubItems.Add("n/a");
                    item.SubItems.Add(da.TranslatableDocument.TargetLanguage);
                    item.SubItems.Add(da.Records.Count.ToString());
                    item.Tag = da;
                }



            }
            else
            {
                listView_documents.Items.Clear();
                richTextBox_dqf_project_info.Text = string.Empty;
            }
        }

        private int GetTotalDocumentRecords(IEnumerable<DocumentActivity> documentActivityList)
        {
            var value = 0;
            var dictList = new List<string>();

            foreach (var da in documentActivityList)
            {
                foreach (var tcr in da.Records)
                {
                    if (!dictList.Contains(da.DocumentId + "_" + tcr.ParagraphId + "_" + tcr.SegmentId))
                    {
                        dictList.Add(da.DocumentId + "_" + tcr.ParagraphId + "_" + tcr.SegmentId);
                        value++;
                    }
                }
            }
            return value;
        }



        private void show_panel(bool back)
        {
            switch (IndexCurrent)
            {
                case 0:
                    {
                        Text = PluginResources.DQF_Project_Task_Wizard_Title;
                        panel_information.BringToFront();
                    } break;
                case 1:
                    {

                        Text = PluginResources.DQF_Project_Task_Step_1_of_3;
                            panel_dqf_project.BringToFront();
                        
                    } break;
                case 2:
                    {
                        Text = PluginResources.DQF_Project_Task_Step_2_of_3;

                        panel_dqf_documents.BringToFront();
                    } break;
                case 3:
                    {
                        Text = PluginResources.DQF_Project_Task_Step_3_of_3;

                        panel_processing_the_upload.BringToFront();
                        Application.DoEvents();

                        var currentString = string.Empty;
                        try
                        {
                            progressBar_import_progress.Value = 0;
                            progressBar_import_progress.Maximum = 100;
                            label_progress_message.Text = string.Format( PluginResources._0_entries_processed, 0);
                            label_progress_percentage.Text = "0%";

                            Application.DoEvents();
                            Cursor = Cursors.WaitCursor;

                            var processor = new Processor();
                            if (treeView_dqf_projects.SelectedNode != null)
                            {
                                var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;
                                if (dqfProject != null && dqfProject.DqfProjectId > -1)
                                {
                                    #region  |  get the working list  |


                                    var documentActivitiesUploadList = new List<DocumentActivity>();
                                    var targetLanguages = new List<string>();
                                  
                                    foreach (ListViewItem item in listView_documents.Items)
                                    {
                                        if (!item.Checked) continue;
                                        var da = item.Tag as DocumentActivity;
                                        documentActivitiesUploadList.Add(da);

                                        if (da != null && !targetLanguages.Contains(da.TranslatableDocument.TargetLanguage))
                                            targetLanguages.Add(da.TranslatableDocument.TargetLanguage);
                                    }

                                    progressBar_import_progress.Maximum = GetTotalDocumentRecords(documentActivitiesUploadList);

                                    #endregion


                                    var documentActivitiesDictionary = new Dictionary<string, List<DocumentActivity>>();

                                    foreach (var documentActivity in documentActivitiesUploadList)
                                        if (!documentActivitiesDictionary.ContainsKey(documentActivity.DocumentId))
                                            documentActivitiesDictionary.Add(documentActivity.DocumentId, new List<DocumentActivity> { documentActivity });
                                        else
                                            documentActivitiesDictionary[documentActivity.DocumentId].Add(documentActivity);


                                    var tcc = new TextComparer {Type = TextComparer.ComparisonType.Words};


                                    var query = new Query();

                                    foreach (var targetLanguage in targetLanguages)
                                    {
                                        foreach (var kvp in documentActivitiesDictionary)
                                        {
                                            if (kvp.Value[0].TranslatableDocument.TargetLanguage != targetLanguage)
                                                continue;
                                            var mergedDocuments = new MergedDocuments(kvp.Value, Activity, tcc, null);

                                            var productivityProjectTask = new ProductivityProjectTask
                                            {
                                                DqfProjectKey = dqfProject.DqfProjectKey,
                                                Projectid = dqfProject.DqfProjectId,
                                                DqfPmanagerKey = Tracked.Settings.DqfSettings.UserKey,
                                                DqfTranslatorKey = Tracked.Settings.DqfSettings.TranslatorKey,
                                                TargetLanguage = mergedDocuments.DocumentTargetLanguage,
                                                FileName = mergedDocuments.DocumentName
                                            };
                                            productivityProjectTask = processor.PostDqfProjectTask(productivityProjectTask);


                                            if (productivityProjectTask.ProjectTaskId <= -1) continue;

                                            #region  |  create ProjectTaskSegment list  |

                                            var ptss = new List<ProjectTaskSegment>();

                                            foreach (var record in mergedDocuments.RecordsDictionary.Values)
                                            {
                                                var pts = new ProjectTaskSegment
                                                {
                                                    Segmentid = record.SegmentId,
                                                    DqfProjectKey = dqfProject.DqfProjectKey,
                                                    DqfTranslatorKey = Tracked.Settings.DqfSettings.TranslatorKey,
                                                    Projectid = dqfProject.DqfProjectId,
                                                    Taskid = productivityProjectTask.ProjectTaskId,
                                                    SourceSegment =
                                                        Helper.GetCompiledSegmentText(
                                                            record.ContentSections.SourceSections, true)
                                                };
                                                if (pts.SourceSegment == string.Empty)
                                                    pts.SourceSegment = " ";
                                                pts.TargetSegment = Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections, true);
                                                if (pts.TargetSegment == string.Empty)
                                                    pts.TargetSegment = " ";
                                                pts.NewTargetSegment = Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections, true);
                                                if (pts.NewTargetSegment == string.Empty)
                                                    pts.NewTargetSegment = " ";
                                                pts.Time = new TimeSpan(record.TicksElapsed).Milliseconds;
                                                pts.TmMatch = GetMatchPercentage(record.TranslationOrigins.Updated.TranslationStatus, record.TranslationOrigins.Updated.OriginType);
                                                pts.Cattool = 23; //Trados Studio
                                                pts.Mtengine = 1;
                                                if (record.TranslationOrigins.Updated.OriginType.ToLower().Trim() == "mt"
                                                    || record.TranslationOrigins.Updated.OriginType.ToLower().Trim() == "amt")
                                                {
                                                    #region  |  get MT engine  |

                                                    if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("APERTIUM", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 2;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("APERTIUM-MOSES", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 3;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("ASIA ONLINE", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 4;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("BING", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 5;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("CAPITA", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 6;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("CARABAO", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 7;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("CCID", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 8;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("CROSSLANG", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 9;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("LINDEN", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 10;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("FIRMA8", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 11;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("FREET", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 12;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("GOOGLE", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 13;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("ICONIC", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 14;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("KANTAN", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 15;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("KODENSHA", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 16;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("LDS TRANSLATOR", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 17;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("LINGUASYS", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 18;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("LUCY", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 19;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("MICROSOFT", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 20;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("MOSES", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 21;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("MYMEMORY", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 22;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("MYMT", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 23;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("OPENTRAD", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 24;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("PANGEAMT", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 25;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("PRAGMA", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 26;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("PROMT", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 27;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("REVERSO", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 28;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("SAFABA", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 29;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("SDL", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 30;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("SOVEE", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 31;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("SYSTRAN", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 32;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("T-TEXT", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 33;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("TAUYOU", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 34;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("TOSHIBA", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 35;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("TRANSSPHERE", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 36;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("WEBLIO", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 37;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("WEBTRANCE", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 38;
                                                    else if (record.TranslationOrigins.Updated.OriginSystem.ToUpper().IndexOf("WEMT", StringComparison.Ordinal) > -1)
                                                        pts.Mtengine = 39;
                                                    else //other
                                                        pts.Mtengine = 40;

                                                    #endregion
                                                }
                                                ptss.Add(pts);
                                            }
                                                    
                                            #endregion

                                            
                                            foreach (var pts in ptss)
                                            {

                                                label_progress_message.Text = string.Format( PluginResources._0_entries_processed, progressBar_import_progress.Value);

                                                if (progressBar_import_progress.Value + 1 <= progressBar_import_progress.Maximum)
                                                {
                                                    progressBar_import_progress.Value = ++progressBar_import_progress.Value;

                                                    var perc = Convert.ToDouble(progressBar_import_progress.Value) / Convert.ToDouble(progressBar_import_progress.Maximum);
                                                    label_progress_percentage.Text = Convert.ToString(Math.Round(perc * 100, 0), CultureInfo.InvariantCulture) + "%";

                                                    Application.DoEvents();
                                                    Cursor = Cursors.WaitCursor;
                                                }
                                                          
                                                    
                                                processor.PostDqfProjectTaskSegment(pts);
                                                   

                                            }
                                                

                                            #region  |  create DQFProjectTask  |

                                            var dqfProjectTask = new DqfProjectTask
                                            {
                                                TableTausdqfprojectsId = dqfProject.Id,
                                                ProjectActivityId = Activity.Id,
                                                DocumentId = kvp.Key,
                                                DocumentName = mergedDocuments.DocumentName,
                                                DqfTranslatorKey = Tracked.Settings.DqfSettings.TranslatorKey,
                                                DqfProjectKey = dqfProject.DqfProjectKey,
                                                DqfProjectId = dqfProject.DqfProjectId,
                                                DqfTaskId = productivityProjectTask.ProjectTaskId,
                                                Uploaded = DateTime.Now,
                                                TargetLanguage = mergedDocuments.DocumentTargetLanguage,
                                                CatTool = 23,
                                                TotalSegments = mergedDocuments.RecordsDictionary.Values.Count()
                                            };



                                            #endregion

                                            dqfProjectTask.Id = query.CreateDqfProjectTask(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, dqfProjectTask);

                                            dqfProject.DqfTasks.Add(dqfProjectTask);
                                        }

                                    }

                                    progressBar_import_progress.Value = progressBar_import_progress.Maximum;
                                    label_progress_message.Text = string.Format( PluginResources._0_entries_processed, progressBar_import_progress.Maximum);
                                    label_progress_percentage.Text = "100%";


                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message + "\r\n\r\n" + currentString, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {                            
                            Cursor = Cursors.Default;
                        }


                    } break;
                
            
            }
            check_enabled();
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
                case 1:
                    {
                        if (treeView_dqf_projects.SelectedNode == null)
                            buttonWizardNextEnabled = false;
                    }
                    break;
                case 2:
                    {
                        if (listView_documents.Items.Count == 0 || listView_documents.CheckedItems.Count == 0)
                            buttonWizardNextEnabled = false;
                    }
                    break;
                case 3:
                    {
                        button_wizard_cancel.Enabled = false;
                        buttonWizardBackEnabled = false;
                    }
                    break;
            }


            button_wizard_back.Enabled = buttonWizardBackEnabled;
            button_wizard_next.Enabled = buttonWizardNextEnabled;
            button_wizard_finish.Enabled = buttonWizardFinishEnabled;


        }

        private double GetMatchPercentage(string match, string originType)
        {
            double value = 0;

            try
            {
                if (string.Compare(originType, @"interactive", StringComparison.OrdinalIgnoreCase) == 0)
                    value = 0;
                else
                {
                    switch (match)
                    {
                        case "PM": value = 100; break;
                        case "CM": value = 100; break;
                        case "AT": value = 0; break;
                        case "100%": value = 100; break;
                        default:
                            {
                                try
                                {
                                    value = Convert.ToDouble(match.Replace("%", ""));
                                }
                                catch
                                {
                                    // ignored
                                }
                            } break;
                    }
                }
            }
            catch
            {
                // ignored
            }
            return value;
        }

        private void button_wizard_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(PluginResources.no_help_found);
        }

        private void button_wizard_back_Click(object sender, EventArgs e)
        {
            if (IndexCurrent <= IndexMinimum) return;
            IndexCurrent--;
            show_panel(true);
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





        private void linkLabel_checkall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (ListViewItem item in listView_documents.Items)
                item.Checked = true;

            check_enabled();
        }

        private void linkLabel_uncheckall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (ListViewItem item in listView_documents.Items)
                item.Checked = false;

            check_enabled();
        }



        private void listView_documents_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            check_enabled();
        }

        private void listView_documents_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            check_enabled();
        }
    }
}
