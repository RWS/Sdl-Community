using System;
using System.Collections.Generic;
using System.Drawing;
using PostEdit.Compare.Model;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Linq;
//using PostEdit.Compare.Properties;
using Sdl.Community.PostEdit.Compare.Properties;
using Settings = Sdl.Community.PostEdit.Compare.Core.Settings;


namespace PostEdit.Compare
{
    public partial class FormMain : Form
    {


        public FormMain(IModel mdl)
        {
            _mModel = mdl;            
            InitializeComponent();

            ReadApplicationSettings();


            InitializePanelCompare();
            InitializeDockManagerCompare(Cache.Application.Settings.ComparePanelSettingsFullPath);

            _panelCompare.panel_listViewMessage.Visible = false;
            CreateEntriesLogReport();
            LoadPropertiesComparisonProjects(null);

        }
       private void CheckAutomation()
        {
            var settings = new Automation.AutomationComunicationSettings();
            try
            {
                if (!File.Exists(settings.ApplicationSettingsFullPath)) 
                    return;
                //run automation
                settings = Automation.SettingsSerializer.ReadSettings();


                AddComparisonEntriesToComboboxes(settings.folderPathLeft, settings.folderPathRight);

                _panelCompare.comboBox_main_compare_left.Text = settings.folderPathLeft;
                _panelCompare.comboBox_main_compare_right.Text = settings.folderPathRight;

                CompareDirectories(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                File.Delete(settings.ApplicationSettingsFullPath);
            }

        }



        private void ReadApplicationSettings()
        {
            try
            {
                Cache.Application.Settings = SettingsSerializer.ReadSettings();
            }
            catch (Exception ex)
            {
                try
                {
                    var settingsCore = new SettingsCore();

                    if (File.Exists(settingsCore.ApplicationSettingsFullPath))
                    {
                        File.Delete(settingsCore.ApplicationSettingsFullPath);
                    }
                    Cache.Application.Settings = SettingsSerializer.ReadSettings();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(this, ex.Message + "\r\n" + exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            #region | static panel path settings  |

            Cache.Application.Settings.ApplicationSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostEdit.Compare");


            if (!Directory.Exists(Cache.Application.Settings.ApplicationSettingsPath))
                Directory.CreateDirectory(Cache.Application.Settings.ApplicationSettingsPath);

            Cache.Application.Settings.ApplicationSettingsFullPath = Path.Combine(Cache.Application.Settings.ApplicationSettingsPath, "PostEdit.Compare.settings.xml");


            Cache.Application.Settings.ComparePanelSettingsFullPath = Path.Combine(Cache.Application.Settings.ApplicationSettingsPath, "PostEdit.Compare.Panel.settings.xml");
            Cache.Application.Settings.ComparisonComparePanelSettingsFullPath = Path.Combine(Cache.Application.Settings.ApplicationSettingsPath, "PostEdit.Comparison.Projects.Panel.settings.xml");
            Cache.Application.Settings.EventsLogPanelSettingsFullPath = Path.Combine(Cache.Application.Settings.ApplicationSettingsPath, "PostEdit.Events.Log.Panel.Settings.xml");

            Cache.Application.Settings.ReportViewerSettings.ShowOriginalRevisionMarkerTargetSegment = false;

            Cache.Application.Settings.ReportViewerSettings.comparisonType = Settings.ComparisonType.Words;

            Cache.Application.Settings.AllowEndUserDocking = true;

            #endregion


        }
        private void SaveApplicationSettings()
        {
            if (Cache.Application.Settings == null) return;
            SettingsSerializer.SaveSettings(Cache.Application.Settings);


            dockPanel_manager.SaveAsXml(Cache.Application.Settings.ComparePanelSettingsFullPath);
        }


        private static void WriteGuiReportImages()
        {

            var embededFile = "PostEdit.Compare.Reports.Images.Compare.png";
            var outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "Compare.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.FilesCopy.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "FilesCopy.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.FilesMove.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "FilesMove.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.FilesDelete.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "FilesDelete.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.Filters.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "Filters.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.FolderCompare.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "FolderCompare.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.ProjectCompare.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "ProjectCompare.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.Projects.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "Projects.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.ReportsAuto.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "ReportsAuto.png");
            WriteFileFromAssembly(outputFile, embededFile);

            embededFile = "PostEdit.Compare.Reports.Images.ReportsSave.png";
            outputFile = Path.Combine(Cache.Application.Settings.ReportViewerSettings.ApplicationSettingsPathImages, "ReportsSave.png");
            WriteFileFromAssembly(outputFile, embededFile);


        }

        private static void WriteFileFromAssembly(string outputFile, string embededFile)
        {
            if (File.Exists(outputFile)) return;
            var asb = Assembly.GetExecutingAssembly();
            using (var inputStream = asb.GetManifestResourceStream(embededFile))
            {
                Stream outputStream = File.Open(outputFile, FileMode.Create);

                if (inputStream == null) return;
                var bsInput = new BufferedStream(inputStream);
                var bsOutput = new BufferedStream(outputStream);

                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = bsInput.Read(buffer, 0, 1024)) > 0)
                {
                    bsOutput.Write(buffer, 0, bytesRead);
                }

                bsInput.Flush();
                bsOutput.Flush();
                bsInput.Close();
                bsOutput.Close();
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {            
            Text = Application.ProductName + @" (" + Application.ProductVersion + @")";

            var aProp = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            aProp.SetValue(_panelCompare.listView_main, true, null);


            FormMain_ResizeEnd(null, null);



            toolStripButton_ignoreDifferencesLeftSide.CheckState = (Cache.Application.Settings.ShowDifferencesFilesLeft ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_ignoreDifferencesRightSide.CheckState = (Cache.Application.Settings.ShowDifferencesFilesRight ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_ignoreEqualFiles.CheckState = (Cache.Application.Settings.ShowEqualFiles ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_ignoreOrphansRightSide.CheckState = (Cache.Application.Settings.ShowOrphanFilesRight ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_ignoreOrphansLeftSide.CheckState = (Cache.Application.Settings.ShowOrphanFilesLeft ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_showEmptyFolders.CheckState = (Cache.Application.Settings.ShowEmptyFolders ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_Activate_Filters.CheckState = (Cache.Application.Settings.ActivateFilters ? CheckState.Checked : CheckState.Unchecked);

            LoadFilterSettings();

            LoadFilterGuiSettings();




            CheckToolTips();


            Visible = true;
            Application.DoEvents();




            Cache.Application.Settings.EventsLogTrackFilters = false;
            Cache.Application.Settings.EventsLogTrackProjects = false;



            WriteGuiReportImages();
            CreateEntriesLogReport();

            CheckAutomation();
        }



        private void LoadFilterGuiSettings()
        {
            toolStripButton_ignoreEqualFiles.Image = !toolStripButton_ignoreEqualFiles.Checked ? imageList1.Images["Filter_Flag_Black_Off"] : imageList1.Images["Filter_Flag_Black_On"];
            toolStripButton_ignoreOrphansLeftSide.Image = !toolStripButton_ignoreOrphansLeftSide.Checked ? imageList1.Images["Filter_Flag_Blue_Left_Off"] : imageList1.Images["Filter_Flag_Blue_Left_On"];
            toolStripButton_ignoreOrphansRightSide.Image = !toolStripButton_ignoreOrphansRightSide.Checked ? imageList1.Images["Filter_Flag_Blue_Right_Off"] : imageList1.Images["Filter_Flag_Blue_Right_On"];
            toolStripButton_ignoreDifferencesLeftSide.Image = !toolStripButton_ignoreDifferencesLeftSide.Checked ? imageList1.Images["Filter_Flag_Red_Left_Off"] : imageList1.Images["Filter_Flag_Red_Left_On"];
            toolStripButton_ignoreDifferencesRightSide.Image = !toolStripButton_ignoreDifferencesRightSide.Checked ? imageList1.Images["Filter_Flag_Red_Right_Off"] : imageList1.Images["Filter_Flag_Red_Right_On"];
            toolStripButton_showEmptyFolders.Image = !toolStripButton_showEmptyFolders.Checked ? imageList1.Images["Filter_Flag_Folder_Off"] : imageList1.Images["Filter_Flag_Folder_On"];
            toolStripButton_Activate_Filters.Image = !toolStripButton_Activate_Filters.Checked ? imageList1.Images["Filter_Feather_Disabled"] : imageList1.Images["Filter_Feather_Enabled"];
        }
        private void LoadFilterSettings()
        {


            Settings.FilterSetting fsSelected = null;

            if (toolStripComboBox_fileFilters.SelectedItem != null)
            {
                if (toolStripComboBox_fileFilters.Items.Count > 0)
                {
                    fsSelected = (Settings.FilterSetting)((ComboboxItem)toolStripComboBox_fileFilters.SelectedItem).Value;
                }
            }

            toolStripComboBox_fileFilters.Items.Clear();

            if (Cache.Application.Settings.FilterSettings.Count <= 0) return;
            var index = -1;
            var selectedIndex = 0;
            foreach (var fs in Cache.Application.Settings.FilterSettings)
            {
                if (fs.Name.Trim() == string.Empty) continue;
                var cbi = new ComboboxItem
                {
                    Text = string.Empty,
                    Value = fs
                };

                var filterText = string.Empty;
                if (fs.FilterNamesInclude.Count > 0)
                {
                    filterText = fs.FilterNamesInclude.Aggregate(filterText, (current, str) => current + ((current != string.Empty ? ";" : string.Empty) + str));
                }
                if (fs.FilterNamesExclude.Count > 0)
                {
                    filterText = fs.FilterNamesExclude.Aggregate(filterText, (current, str) => current + ((current != string.Empty ? ";" : string.Empty) + "-" + str));
                }


                if (fs.FilterDateUsed)
                {
                    if (fs.FilterDate.Type == Settings.FilterDate.FilterType.GreaterThan)
                    {
                        filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                      + ">" + fs.FilterDate.Date.Date.ToShortDateString() + " " + fs.FilterDate.Date.ToShortTimeString();
                    }
                    else
                    {
                        filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                      + "<" + fs.FilterDate.Date.Date.ToShortDateString() + " " + fs.FilterDate.Date.ToShortTimeString();
                    }
                }

                var attributes = string.Empty;
                if (fs.FilterAttributeArchiveUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                }
                if (fs.FilterAttributeSystemUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs.FilterAttributeSystemType == "Included" ? "S" : "-S");
                }
                if (fs.FilterAttributeHiddenUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                }
                if (fs.FilterAttributeReadOnlyUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                }

                cbi.Text += "" + filterText + (filterText.Trim() != string.Empty ? "; " : string.Empty) + attributes;

                if (cbi.Text.Trim() == string.Empty) continue;
                index++;
                toolStripComboBox_fileFilters.Items.Add(cbi);

                if (fsSelected != null
                    && string.Compare(fs.Name, fsSelected.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    selectedIndex = index;
                }
            }
            toolStripComboBox_fileFilters.SelectedIndex = selectedIndex;
        }

        private void LoadSettings(int setNode0, int setNode1, int setNode2)
        {
            var f = new Forms.Settings();

            if (setNode0 > -1)
                f.treeView_main.SelectedNode = f.treeView_main.Nodes[setNode0];
            if (setNode1 > -1)
                f.treeView_main.SelectedNode = f.treeView_main.SelectedNode.Nodes[setNode1];
            if (setNode2 > -1)
                f.treeView_main.SelectedNode = f.treeView_main.SelectedNode.Nodes[setNode2];

            f.checkBox_automaticallyExpandComparisonFolders.Checked = Cache.Application.Settings.AutomaciallyExpandComparisonFolders;

            #region  |  Events Log  |

            f.numericUpDown_maximum_comparisonLogEntries.Value = Cache.Application.Settings.ComparisonLogMaxEntries;
            f.checkBox_eventsLogTrackCompare.Checked = Cache.Application.Settings.EventsLogTrackCompare;
            f.checkBox_eventsLogTrackReports.Checked = Cache.Application.Settings.EventsLogTrackReports;
            f.checkBox_eventsLogTrackProjects.Checked = Cache.Application.Settings.EventsLogTrackProjects;
            f.checkBox_eventsLogTrackFiles.Checked = Cache.Application.Settings.EventsLogTrackFiles;
            f.checkBox_eventsLogTrackFilters.Checked = Cache.Application.Settings.EventsLogTrackFilters;

            #endregion

            #region  |  rate groups  |

            f.RateGroups = new List<Settings.PriceGroup>();
            foreach (var p in Cache.Application.Settings.PriceGroups)
                f.RateGroups.Add((Settings.PriceGroup)p.Clone());

            #endregion

            #region  |  comparison project  |

            f.ComparisonProjects = new List<Settings.ComparisonProject>();
            foreach (var comparisonProject in Cache.Application.Settings.ComparisonProjects)
                f.ComparisonProjects.Add((Settings.ComparisonProject)comparisonProject.Clone());

            #endregion

            #region  |  folder viewer  |

            f.checkBox_folderViewer_columns_name.Checked = Cache.Application.Settings.ViewListViewColumnName;
            f.checkBox_folderViewer_columns_type.Checked = Cache.Application.Settings.ViewListViewColumnType;
            f.checkBox_folderViewer_columns_size.Checked = Cache.Application.Settings.ViewListViewColumnSize;
            f.checkBox_folderViewer_columns_modified.Checked = Cache.Application.Settings.ViewListViewColumnModified;

            f.checkBox_folder_viewer_show_equal_files.Checked = Cache.Application.Settings.ShowEqualFiles;
            f.checkBox_folder_viewer_show_orphan_files_left.Checked = Cache.Application.Settings.ShowOrphanFilesLeft;
            f.checkBox_folder_viewer_show_orphan_files_right.Checked = Cache.Application.Settings.ShowOrphanFilesRight;
            f.checkBox_folder_viewer_show_mismatches_left.Checked = Cache.Application.Settings.ShowDifferencesFilesLeft;
            f.checkBox_folder_viewer_show_mismatches_right.Checked = Cache.Application.Settings.ShowDifferencesFilesRight;
            f.checkBox_folder_viewer_show_empty_folders.Checked = Cache.Application.Settings.ShowEmptyFolders;

            f.numericUpDown_maximum_folderComparisonEntries.Value = Cache.Application.Settings.FolderViewerFoldersMaxEntries;

            #endregion

            #region  |  report viewer  |


            f.checkBox_showOriginalSourceSegment.Checked = Cache.Application.Settings.ReportViewerSettings.ShowOriginalSourceSegment;
            f.checkBox_showOriginalTargetSegment.Checked = Cache.Application.Settings.ReportViewerSettings.ShowOriginalTargetSegment;
            f.checkBox_showOriginalRevisionMarkerTargetSegment.Checked = Cache.Application.Settings.ReportViewerSettings.ShowOriginalRevisionMarkerTargetSegment;
            f.checkBox_showSegmentComments.Checked = Cache.Application.Settings.ReportViewerSettings.ShowSegmentComments;
            f.checkBox_showTargetSegmentComparison.Checked = Cache.Application.Settings.ReportViewerSettings.ShowTargetComparison;
            f.checkBox_showUpdatedTargetSegment.Checked = Cache.Application.Settings.ReportViewerSettings.ShowUpdatedTargetSegment;
            f.checkBox_showUpdatedRevisionMarkerTargetSegment.Checked = Cache.Application.Settings.ReportViewerSettings.ShowUpdatedRevisionMarkerTargetSegment;
            f.checkBox_showLockedSegments.Checked = Cache.Application.Settings.ReportViewerSettings.ShowSegmentLocked;

            f.checkBox_showSegmentStatus.Checked = Cache.Application.Settings.ReportViewerSettings.ShowSegmentStatus;
            f.checkBox_showSegmentMatch.Checked = Cache.Application.Settings.ReportViewerSettings.ShowSegmentMatch;
            f.checkBox_showSegmentTERPAnalysis.Checked = Cache.Application.Settings.ReportViewerSettings.ShowSegmentTerp;
            f.textBox_javaExecutablePath.Text = Cache.Application.Settings.ReportViewerSettings.JavaExecutablePath;
            f.checkBox_showSegmentPEM.Checked = Cache.Application.Settings.ReportViewerSettings.ShowSegmentPem;




            f.checkBox_viewFilesWithNoTranslationDifferences.Checked = Cache.Application.Settings.ReportViewerSettings.ReportFilterFilesWithNoRecordsFiltered;
            f.checkBox_showGoogleChartsInReport.Checked = Cache.Application.Settings.ReportViewerSettings.ShowGoogleChartsInReport;
            f.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Checked = Cache.Application.Settings.ReportViewerSettings.CalculateSummaryAnalysisBasedOnFilteredRows;

            
            f.checkBox_viewLockedSegments.Checked = Cache.Application.Settings.ReportViewerSettings.ReportFilterLockedSegments;
            f.checkBox_viewSegmentsWithComments.Checked = Cache.Application.Settings.ReportViewerSettings.ReportFilterSegmentsContainingComments;
            f.checkBox_viewSegmentsWithNoChanges.Checked = Cache.Application.Settings.ReportViewerSettings.ReportFilterSegmentsWithNoChanges;
            f.checkBox_viewSegmentsWithStatusChanges.Checked = Cache.Application.Settings.ReportViewerSettings.ReportFilterSegmentStatusChanged;
            f.checkBox_viewSegmentsWithTranslationChanges.Checked = Cache.Application.Settings.ReportViewerSettings.ReportFilterChangedTargetContent;

            f.tagVisualizationComboBox.SelectedItem = Cache.Application.Settings.ReportViewerSettings.TagVisualStyle.ToString();

            f.comboBox_segments_match_value_original.SelectedItem = Cache.Application.Settings.ReportViewerSettings.ReportFilterTranslationMatchValuesOriginal;
            f.comboBox_segments_match_value_updated.SelectedItem = Cache.Application.Settings.ReportViewerSettings.ReportFilterTranslationMatchValuesUpdated;



            #endregion

            #region  |  reports  |

            f.checkBox_reportsAutoSave.Checked = Cache.Application.Settings.ReportsAutoSave;
            f.checkBox_reportsCreateMonthlySubFolders.Checked = Cache.Application.Settings.ReportsCreateMonthlySubFolders;
            f.textBox_reportsAutoSaveFullPath.Text = Cache.Application.Settings.ReportsAutoSaveFullPath;




            #endregion

            #region  |  filter setttings  |

            f.FilterSettings = new List<Settings.FilterSetting>();
            foreach (var fs in Cache.Application.Settings.FilterSettings)
            {
                f.FilterSettings.Add((Settings.FilterSetting)fs.Clone());
            }


            #endregion

            #region  |  comparer settings  |

            
            f.ComparisonType = Settings.ComparisonType.Words;
            f.IncludeTagContentInComparison = Cache.Application.Settings.ReportViewerSettings.ComparisonIncludeTags;


            f.StyleNewText = Cache.Application.Settings.ReportViewerSettings.StyleNewText;
            f.StyleRemovedText = Cache.Application.Settings.ReportViewerSettings.StyleRemovedText;

            f.StyleNewTag = Cache.Application.Settings.ReportViewerSettings.StyleNewTag;
            f.StyleRemovedTag = Cache.Application.Settings.ReportViewerSettings.StyleRemovedTag;

            #endregion

            f.ShowDialog();
            if (!f.Saved) return;
            {
              
                Cache.Application.Settings.AutomaciallyExpandComparisonFolders = f.checkBox_automaticallyExpandComparisonFolders.Checked;

               
                #region  |  Events Log  |

                var maxEntriesCountChanged = (Cache.Application.Settings.ComparisonLogMaxEntries != Convert.ToInt32(f.numericUpDown_maximum_comparisonLogEntries.Value));

                Cache.Application.Settings.ComparisonLogMaxEntries = Convert.ToInt32(f.numericUpDown_maximum_comparisonLogEntries.Value);

                Cache.Application.Settings.EventsLogTrackCompare = f.checkBox_eventsLogTrackCompare.Checked;
                Cache.Application.Settings.EventsLogTrackReports = f.checkBox_eventsLogTrackReports.Checked;
                Cache.Application.Settings.EventsLogTrackProjects = f.checkBox_eventsLogTrackProjects.Checked;
                Cache.Application.Settings.EventsLogTrackFiles = f.checkBox_eventsLogTrackFiles.Checked;
                Cache.Application.Settings.EventsLogTrackFilters = f.checkBox_eventsLogTrackFilters.Checked;


                if (f.ClearComparisonLogHistory)
                {
                    Cache.Application.Settings.ComparisonLogEntries.Clear();

                    CreateEntriesLogReport();
                }
                else if (maxEntriesCountChanged)
                {
                    if (Cache.Application.Settings.ComparisonLogEntries.Count > Cache.Application.Settings.ComparisonLogMaxEntries)
                    {
                        Cache.Application.Settings.ComparisonLogEntries.RemoveRange(Cache.Application.Settings.ComparisonLogMaxEntries,
                            Cache.Application.Settings.ComparisonLogEntries.Count - Cache.Application.Settings.ComparisonLogMaxEntries);
                    }

                    CreateEntriesLogReport();
                }





                #endregion

                #region  |  rate groups  |

                Cache.Application.Settings.PriceGroups = f.RateGroups;

                #endregion

                #region  |  comparison project  |


                var comparisonProjectExisting =
                    (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                if (comparisonProjectExisting != null)
                {
                    var comparisonProjectExistingTest = (Settings.ComparisonProject)comparisonProjectExisting.Clone();

                    var reinitializeProjects = false;
                    if (Cache.Application.Settings.ComparisonProjects.Count != f.ComparisonProjects.Count)
                    {
                        reinitializeProjects = true;
                    }
                    else
                    {
                        foreach (var cp1 in Cache.Application.Settings.ComparisonProjects)
                        {
                            var foundProject = false;
                            var foldersDirty = false;

                            foreach (var cp2 in f.ComparisonProjects)
                            {
                                if (cp1.Id != cp2.Id) continue;
                                foundProject = true;

                                if (string.Compare(cp1.PathLeft, cp2.PathLeft, StringComparison.OrdinalIgnoreCase) != 0
                                    || string.Compare(cp1.PathRight, cp2.PathRight, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    foldersDirty = true;
                                }

                                //check the alignment
                                if (cp1.FileAlignment.Count != cp2.FileAlignment.Count)
                                {
                                    foldersDirty = true;
                                }
                                else
                                {
                                    if (cp1.FileAlignment.Select(fa1 => cp2.FileAlignment.Any(fa2 => fa1.PathLeft == fa2.PathLeft && fa1.PathRight == fa2.PathRight)).Any
                                        (foundAlignment => !foundAlignment))
                                    {
                                        foldersDirty = true;
                                    }
                                }
                                break;
                            }

                            if (foundProject && !foldersDirty) continue;
                            reinitializeProjects = true;
                            break;
                        }
                    }

                    if (reinitializeProjects)
                    {
                        Cache.Application.Settings.ComparisonProjects = f.ComparisonProjects;

                        LoadPropertiesComparisonProjects(comparisonProjectExisting);

                        if (string.Compare(comparisonProjectExistingTest.PathLeft, _panelCompare.comboBox_main_compare_left.Text, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
                            _panelCompare.comboBox_main_compare_left.BackColor = Directory.Exists(_panelCompare.comboBox_main_compare_left.Text) ? Color.Yellow : Color.Pink;
                        }
                        if (string.Compare(comparisonProjectExistingTest.PathRight, _panelCompare.comboBox_main_compare_right.Text, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            _panelCompare.toolStripButton_loadRightSide.Enabled = true;
                            _panelCompare.comboBox_main_compare_right.BackColor = Directory.Exists(_panelCompare.comboBox_main_compare_right.Text) ? Color.Yellow : Color.Pink;
                        }
                    }
                }


                #endregion

                #region  |  folder viewer  |

                Cache.Application.Settings.ViewListViewColumnName = f.checkBox_folderViewer_columns_name.Checked;
                Cache.Application.Settings.ViewListViewColumnType = f.checkBox_folderViewer_columns_type.Checked;
                Cache.Application.Settings.ViewListViewColumnSize = f.checkBox_folderViewer_columns_size.Checked;
                Cache.Application.Settings.ViewListViewColumnModified = f.checkBox_folderViewer_columns_modified.Checked;

                Cache.Application.Settings.ShowEqualFiles = f.checkBox_folder_viewer_show_equal_files.Checked;
                Cache.Application.Settings.ShowOrphanFilesLeft = f.checkBox_folder_viewer_show_orphan_files_left.Checked;
                Cache.Application.Settings.ShowOrphanFilesRight = f.checkBox_folder_viewer_show_orphan_files_right.Checked;
                Cache.Application.Settings.ShowDifferencesFilesLeft = f.checkBox_folder_viewer_show_mismatches_left.Checked;
                Cache.Application.Settings.ShowDifferencesFilesRight = f.checkBox_folder_viewer_show_mismatches_right.Checked;
                Cache.Application.Settings.ShowEmptyFolders = f.checkBox_folder_viewer_show_empty_folders.Checked;

                toolStripButton_ignoreEqualFiles.Checked = Cache.Application.Settings.ShowEqualFiles;
                toolStripButton_ignoreOrphansLeftSide.Checked = Cache.Application.Settings.ShowOrphanFilesLeft;
                toolStripButton_ignoreOrphansRightSide.Checked = Cache.Application.Settings.ShowOrphanFilesRight;
                toolStripButton_ignoreDifferencesLeftSide.Checked = Cache.Application.Settings.ShowDifferencesFilesLeft;
                toolStripButton_ignoreDifferencesRightSide.Checked = Cache.Application.Settings.ShowDifferencesFilesRight;
                toolStripButton_showEmptyFolders.Checked = Cache.Application.Settings.ShowEmptyFolders;

                Cache.Application.Settings.FolderViewerFoldersMaxEntries = Convert.ToInt32(f.numericUpDown_maximum_folderComparisonEntries.Value);

                if (f.ClearFolderComparisonHistory)
                {
                    string leftPath = _panelCompare.comboBox_main_compare_left.Text;
                    string rightPath = _panelCompare.comboBox_main_compare_right.Text;

                    IsInitializingPanel = true;

                    _panelCompare.comboBox_main_compare_left.Items.Clear();
                    _panelCompare.comboBox_main_compare_right.Items.Clear();

                    Cache.Application.Settings.FolderViewerFoldersLeft.Clear();
                    Cache.Application.Settings.FolderViewerFoldersRight.Clear();

                    AddComparisonEntriesToComboboxes(leftPath, rightPath);

                    IsInitializingPanel = false;
                }

                toolStripButton_ignoreEqualFiles.Image = !toolStripButton_ignoreEqualFiles.Checked ? imageList1.Images["Filter_Flag_Black_Off"] : imageList1.Images["Filter_Flag_Black_On"];
                toolStripButton_ignoreOrphansLeftSide.Image = !toolStripButton_ignoreOrphansLeftSide.Checked ? imageList1.Images["Filter_Flag_Blue_Left_Off"] : imageList1.Images["Filter_Flag_Blue_Left_On"];
                toolStripButton_ignoreOrphansRightSide.Image = !toolStripButton_ignoreOrphansRightSide.Checked ? imageList1.Images["Filter_Flag_Blue_Right_Off"] : imageList1.Images["Filter_Flag_Blue_Right_On"];
                toolStripButton_ignoreDifferencesLeftSide.Image = !toolStripButton_ignoreDifferencesLeftSide.Checked ? imageList1.Images["Filter_Flag_Red_Left_Off"] : imageList1.Images["Filter_Flag_Red_Left_On"];
                toolStripButton_ignoreDifferencesRightSide.Image = !toolStripButton_ignoreDifferencesRightSide.Checked ? imageList1.Images["Filter_Flag_Red_Right_Off"] : imageList1.Images["Filter_Flag_Red_Right_On"];
                toolStripButton_showEmptyFolders.Image = !toolStripButton_showEmptyFolders.Checked ? imageList1.Images["Filter_Flag_Folder_Off"] : imageList1.Images["Filter_Flag_Folder_On"];

                #endregion

                #region  |  report viewer  |

                Cache.Application.Settings.ReportViewerSettings.ShowOriginalSourceSegment = f.checkBox_showOriginalSourceSegment.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowOriginalTargetSegment = f.checkBox_showOriginalTargetSegment.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowOriginalRevisionMarkerTargetSegment = f.checkBox_showOriginalRevisionMarkerTargetSegment.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowSegmentComments = f.checkBox_showSegmentComments.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowTargetComparison = f.checkBox_showTargetSegmentComparison.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowUpdatedTargetSegment = f.checkBox_showUpdatedTargetSegment.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowUpdatedRevisionMarkerTargetSegment = f.checkBox_showUpdatedRevisionMarkerTargetSegment.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowSegmentLocked = f.checkBox_showLockedSegments.Checked;

                Cache.Application.Settings.ReportViewerSettings.ShowSegmentStatus = f.checkBox_showSegmentStatus.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowSegmentMatch = f.checkBox_showSegmentMatch.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowSegmentTerp = f.checkBox_showSegmentTERPAnalysis.Checked;
                Cache.Application.Settings.ReportViewerSettings.JavaExecutablePath = f.textBox_javaExecutablePath.Text;
                Cache.Application.Settings.ReportViewerSettings.ShowSegmentPem = f.checkBox_showSegmentPEM.Checked;

                Cache.Application.Settings.ReportViewerSettings.ReportFilterFilesWithNoRecordsFiltered = f.checkBox_viewFilesWithNoTranslationDifferences.Checked;
                Cache.Application.Settings.ReportViewerSettings.ShowGoogleChartsInReport = f.checkBox_showGoogleChartsInReport.Checked;
                Cache.Application.Settings.ReportViewerSettings.CalculateSummaryAnalysisBasedOnFilteredRows = f.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Checked;

              
                Cache.Application.Settings.ReportViewerSettings.ReportFilterLockedSegments = f.checkBox_viewLockedSegments.Checked;
                Cache.Application.Settings.ReportViewerSettings.ReportFilterSegmentsContainingComments = f.checkBox_viewSegmentsWithComments.Checked;
                Cache.Application.Settings.ReportViewerSettings.ReportFilterSegmentsWithNoChanges = f.checkBox_viewSegmentsWithNoChanges.Checked;
                Cache.Application.Settings.ReportViewerSettings.ReportFilterSegmentStatusChanged = f.checkBox_viewSegmentsWithStatusChanges.Checked;
                Cache.Application.Settings.ReportViewerSettings.ReportFilterChangedTargetContent = f.checkBox_viewSegmentsWithTranslationChanges.Checked;

                Cache.Application.Settings.ReportViewerSettings.TagVisualStyle = (Settings.TagVisual)Enum.Parse(typeof(Settings.TagVisual), f.tagVisualizationComboBox.SelectedItem.ToString(), true);

                Cache.Application.Settings.ReportViewerSettings.ReportFilterTranslationMatchValuesOriginal = f.comboBox_segments_match_value_original.SelectedItem.ToString();
                Cache.Application.Settings.ReportViewerSettings.ReportFilterTranslationMatchValuesUpdated = f.comboBox_segments_match_value_updated.SelectedItem.ToString();


                #endregion

                #region  |  reports  |

                Cache.Application.Settings.ReportsAutoSave = f.checkBox_reportsAutoSave.Checked;
                Cache.Application.Settings.ReportsCreateMonthlySubFolders = f.checkBox_reportsCreateMonthlySubFolders.Checked;
                Cache.Application.Settings.ReportsAutoSaveFullPath = f.textBox_reportsAutoSaveFullPath.Text;

                #endregion

                #region  |  filter setttings  |

                Cache.Application.Settings.FilterSettings = f.FilterSettings;

                var fsSelected = new Settings.FilterSetting();
                if (toolStripComboBox_fileFilters.Items.Count > 0)
                {
                    fsSelected = (Settings.FilterSetting)((ComboboxItem)toolStripComboBox_fileFilters.SelectedItem).Value;
                }

                toolStripComboBox_fileFilters.Items.Clear();
                var index = -1;
                var selectedIndex = 0;
                foreach (var fs in Cache.Application.Settings.FilterSettings)
                {
                    if (fs.Name.Trim() == string.Empty) continue;
                    var cbi = new ComboboxItem();
                    cbi.Text = string.Empty;
                    cbi.Value = fs;

                    var filterText = string.Empty;
                    if (fs.FilterNamesInclude.Count > 0)
                    {
                        filterText = fs.FilterNamesInclude.Aggregate(filterText, (current, str) => current + ((current != string.Empty ? ";" : string.Empty) + str));
                    }
                    if (fs.FilterNamesExclude.Count > 0)
                    {
                        filterText = fs.FilterNamesExclude.Aggregate(filterText, (current, str) => current + ((current != string.Empty ? ";" : string.Empty) + "-" + str));
                    }
                    if (fs.FilterDateUsed)
                    {
                        if (fs.FilterDate.Type == Settings.FilterDate.FilterType.GreaterThan)
                        {
                            filterText += (filterText.Trim() != string.Empty ? ";" : string.Empty)
                                          + ">" + fs.FilterDate.Date.Date.ToShortDateString() + " " + fs.FilterDate.Date.ToShortTimeString();
                        }
                        else
                        {
                            filterText += (filterText.Trim() != string.Empty ? ";" : string.Empty)
                                          + "<" + fs.FilterDate.Date.Date.ToShortDateString() + " " + fs.FilterDate.Date.ToShortTimeString();
                        }
                    }

                    var attributes = string.Empty;
                    if (fs.FilterAttributeArchiveUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                      + (fs.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                    }
                    if (fs.FilterAttributeSystemUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                      + (fs.FilterAttributeSystemType == "Included" ? "S" : "-S");
                    }
                    if (fs.FilterAttributeHiddenUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                      + (fs.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                    }
                    if (fs.FilterAttributeReadOnlyUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                      + (fs.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                    }


                    cbi.Text += "" + filterText + (filterText.Trim() != string.Empty ? "; " : string.Empty) + attributes;

                    if (cbi.Text.Trim() == string.Empty) continue;
                    index++;
                    toolStripComboBox_fileFilters.Items.Add(cbi);

                    if (string.Compare(fs.Id, fsSelected.Id, StringComparison.OrdinalIgnoreCase) == 0)
                        selectedIndex = index;
                }

                if (toolStripComboBox_fileFilters.Items.Count > 0)
                    toolStripComboBox_fileFilters.SelectedIndex = selectedIndex;


                #endregion

                #region  |  comparer settings  |

                
                Cache.Application.Settings.ReportViewerSettings.comparisonType = Settings.ComparisonType.Words;
                Cache.Application.Settings.ReportViewerSettings.ComparisonIncludeTags = f.IncludeTagContentInComparison;

                Cache.Application.Settings.ReportViewerSettings.StyleNewText = f.StyleNewText;
                Cache.Application.Settings.ReportViewerSettings.StyleRemovedText = f.StyleRemovedText;

                Cache.Application.Settings.ReportViewerSettings.StyleNewTag = f.StyleNewTag;
                Cache.Application.Settings.ReportViewerSettings.StyleRemovedTag = f.StyleRemovedTag;



                #endregion

                FormMain_ResizeEnd(null, null);

                SaveApplicationSettings();


                try
                {
                    _panelCompare.panel_listViewMessage.Visible = true;


                    UpdateVisualCompareDirectories();
                    UpdateVisualCompareDirectories();


                    CheckToolTips();

                    _panelCompare.listView_main.Invalidate(false);

                    CreateEntriesLogReport();
                }
                finally
                {
                    _panelCompare.panel_listViewMessage.Visible = false;
                }
            }
        }



        #region  |  Intrinsic form events  |



        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveApplicationSettings();
        }



        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new About();
            f.ShowDialog();
        }

        private void toolStripButton_settings_Click(object sender, EventArgs e)
        {

            LoadSettings(0, -1, -1);

        }




        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void compareFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareMe();
        }

        private void createAComparisonReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateReport();
        }




        private void expandAllFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_expandAll_Click(sender, e);
        }

        private void expandAllSubfolderForSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            expandAllSubfoldersToolStripMenuItem_Click(sender, e);
        }


        private void collapseAllFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_collapseAll_Click(sender, e);
        }
        private void collapseAllSubfoldersForSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            collapseAllSubfoldersToolStripMenuItem_Click(sender, e);
        }



        private void showEqualFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_ignoreEqualFiles_Click(sender, e);
        }

        private void showOrphanFilesleftSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_ignoreOrphanLeftSide_Click(sender, e);
        }

        private void showOrphanFilesrightSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_ignoreOrpansRightSide_Click(sender, e);
        }

        private void showFilesWithDifferencesleftSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_ignoreDifferencesLeftSide_Click(sender, e);
        }

        private void showFilesWithDifferencesrightSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_ignoreDifferencesRightSide_Click(sender, e);
        }

        private void showEmptyFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_showEmptyFolders_Click(sender, e);
        }

       

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_settings_Click(sender, e);
        }

        private void activateFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_Activate_Filters_Click(sender, e);
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://posteditcompare.wiki-site.com/index.php/Main_Page");
        }

        private void toolStripComboBox_fileFilters_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }




        private void viewCompareWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPanelCompare(true);
        }

        private void viewComparisonProjectsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPanelComparisonProjectsPanel(true);
        }

        private void viewEventsLogWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPanelEventsLogPanel(true);
        }

        private void resetAllWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetDefaultWindowStatesCompare();
        }




        private void filterToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            showEqualFilesToolStripMenuItem.Checked = toolStripButton_ignoreEqualFiles.Checked;
            showOrphanFilesleftSideToolStripMenuItem.Checked = toolStripButton_ignoreOrphansLeftSide.Checked;
            showOrphanFilesrightSideToolStripMenuItem.Checked = toolStripButton_ignoreOrphansRightSide.Checked;
            showFilesWithDifferencesleftSideToolStripMenuItem.Checked = toolStripButton_ignoreDifferencesLeftSide.Checked;
            showFilesWithDifferencesrightSideToolStripMenuItem.Checked = toolStripButton_ignoreDifferencesRightSide.Checked;
            showEmptyFoldersToolStripMenuItem.Checked = toolStripButton_showEmptyFolders.Checked;
            activateFiltersToolStripMenuItem.Checked = toolStripButton_Activate_Filters.Checked;
        }

        private void actionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            MostlyLeftSelection = false;

            var iFolders = 0;
            if (_panelCompare.listView_main.SelectedIndices.Count > 0)
            {
                expandAllFoldersToolStripMenuItem.Enabled = true;
                collapseAllFoldersToolStripMenuItem.Enabled = true;

                for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                {
                    var mbr = (DataNode)_panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]].Tag;

                    if (mbr.SelectionType == DataNode.Selection.Left)
                        MostlyLeftSelection = true;

                    if (mbr.Type != DataNode.ItemType.File)                      
                        iFolders++;
                }
            }
            else
            {
                expandAllFoldersToolStripMenuItem.Enabled = false;
                collapseAllFoldersToolStripMenuItem.Enabled = false;
            }

            if (iFolders > 0)
            {
                expandAllSubfolderForSelectedToolStripMenuItem.Enabled = true;
                collapseAllSubfoldersForSelectedToolStripMenuItem.Enabled = true;
            }
            else
            {
                expandAllSubfolderForSelectedToolStripMenuItem.Enabled = false;
                collapseAllSubfoldersForSelectedToolStripMenuItem.Enabled = false;
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            MostlyLeftSelection = false;

            var iFiles = 0;
            if (_panelCompare.listView_main.Items.Count > 0)
            {
                selectAllToolStripMenuItem.Enabled = true;
                clearSelectionToolStripMenuItem.Enabled = true;
            }
            else
            {
                selectAllToolStripMenuItem.Enabled = false;
                clearSelectionToolStripMenuItem.Enabled = false;
            }

            if (_panelCompare.listView_main.SelectedIndices.Count > 0)
            {
                for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                {
                    var mbr = (DataNode)_panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]].Tag;

                    if (mbr.SelectionType == DataNode.Selection.Left)
                        MostlyLeftSelection = true;

                    if (mbr.Type == DataNode.ItemType.File)
                        iFiles++;               
                }
            }

            if (MostlyLeftSelection)
            {
                copyFilesToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Copy_to_the_right;
                moveFilesToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Move_to_the_right;
            }
            else
            {
                copyFilesToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Copy_to_the_left;
                moveFilesToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Move_to_the_left;
            }

            if (iFiles > 0)
            {
                copyFilesToToolStripMenuItem.Enabled = true;
                moveFilesToToolStripMenuItem.Enabled = true;
                deleteFilesToolStripMenuItem.Enabled = true;
            }
            else
            {
                copyFilesToToolStripMenuItem.Enabled = false;
                moveFilesToToolStripMenuItem.Enabled = false;
                deleteFilesToolStripMenuItem.Enabled = false;
            }
        }

        private void projectsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            nextToolStripMenuItem.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled;
            previousToolStripMenuItem.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled;
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_comparison_project_new_Click(sender, e);
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton_comparison_project_right_side_move_Click(sender, e);
        }

        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripDropDownButton_comparison_project_left_side_move_Click(sender, e);
        }


        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_panelCompare.listView_main.Items.Count > 0)
            {
                for (var i = 0; i < _panelCompare.listView_main.Items.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[i];
                    var dn = (DataNode)lvi.Tag;
                    lvi.Selected = true;
                    dn.SelectionType = DataNode.Selection.Middle;
                }
            }
            InitVirtualListViewNodes();
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (_panelCompare.listView_main.Items.Count > 0)
            {
                for (var i = 0; i < _panelCompare.listView_main.Items.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[i];
                    var dn = (DataNode)lvi.Tag;
                    lvi.Selected = false;
                    dn.SelectionType = DataNode.Selection.None;
                }
            }
            InitVirtualListViewNodes();
        }

        private void copyFilesToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyToToolStripMenuItem_Click(sender, e);
        }

        private void moveFilesToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moveToToolStripMenuItem_Click(sender, e);
        }

        private void deleteFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteToolStripMenuItem_Click(sender, e);
        }




        private void folderBrowsingToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void browseFolderleftSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowseFolder(true);
        }

        private void browseFolderrightSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowseFolder(false);
        }

        private void setBaseFolderleftSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetBaseFolder(0);
        }

        private void setBaseFolderrightSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetBaseFolder(5);
        }

        private void upOneLevelleftSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderUpOnLevel(true);
        }

        private void upOneLevelrightSideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderUpOnLevel(false);
        }

        private void bothFoldersUpOneLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BothFoldersUpOnLevel();
        }

        #endregion

    }



}
