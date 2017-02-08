using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using PostEdit.Compare.Forms;
using PostEdit.Compare.Model;
using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Community.PostEdit.Compare.Core.Comparison;
using WeifenLuo.WinFormsUI.Docking;
using Application = PostEdit.Compare.Cache.Application;
using Comparer = Sdl.Community.PostEdit.Compare.Core.Comparison.Comparer;
using Settings = Sdl.Community.PostEdit.Compare.Core.Settings;
using Timer = System.Timers.Timer;
using Sdl.Community.PostEdit.Compare.Properties;

namespace PostEdit.Compare
{
    public partial class FormMain
    {


        private PanelCompare _panelCompare;
        private PanelComparisonProjects _panelComparisonProjects;
        private PanelEventsLog _panelEventsLog;

        private DeserializeDockContent _deserializeDockContentCompare;

        private bool IsInitializingPanel { get; set; }

        private void InitializePanelCompare()
        {


            MouseLeftIsDown = false;

            float[] myFactors = { .2f, .4f, .6f, .6f, .4f, .2f };
            float[] myPositions = { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
            _mBlend.Factors = myFactors;
            _mBlend.Positions = myPositions;

            dockPanel_manager.Dock = DockStyle.Fill;


            _panelCompare = new PanelCompare();
            _panelComparisonProjects = new PanelComparisonProjects();
            _panelEventsLog = new PanelEventsLog();

            CheckEnabledButtonToolbarMain();

            ReportDialog = new ReportViewer(null);
            ReportDialog.PanelReportViewer.webBrowserReport.DocumentCompleted += WebBrowserReportDocumentCompleted;

        }


        private void InitializeDockManagerCompare(string comparePanelSettingsFullPath)
        {
            _deserializeDockContentCompare = GetContentFromPersistStringComparePanel;

            dockPanel_manager.ActiveDocumentChanged += dockPanel_manager_ActiveDocumentChanged;

            if (File.Exists(comparePanelSettingsFullPath))
            {
                try
                {
                    dockPanel_manager.LoadFromXml(comparePanelSettingsFullPath, _deserializeDockContentCompare);

                    if (_panelCompare != null && _panelCompare.DockPanel != null)
                    {
                        ApplyEventHandlersCompare();
                        LoadPropertiesCompare();


                        if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                        {
                            ApplyEventHandlersComparisonsProjects();
                            LoadPropertiesComparisonProjects(null);
                        }

                        if (_panelEventsLog != null && _panelEventsLog.DockPanel != null)
                        {
                            CreateEntriesLogReport();
                        }
                    }
                    else
                    {
                        ResetDefaultWindowStatesCompare();
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    File.Delete(comparePanelSettingsFullPath);

                    ResetDefaultWindowStatesCompare();

                    Console.WriteLine(ex.Message);

                }
            }
            else
            {
                ResetDefaultWindowStatesCompare();
            }
        }

        private void dockPanel_manager_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (_panelCompare.Pane == null)
            {
                CheckEnabledButtonToolbarMain();
            }

        }

        private void ShowPanelCompare(bool resetState)
        {
            try
            {
                if (resetState)
                {
                    if (_panelCompare != null && _panelCompare.DockPanel != null)
                        _panelCompare.Close();

                    _panelCompare = new PanelCompare();
                    _panelCompare.Show(dockPanel_manager, DockState.Document);


                    float[] myFactors = { .2f, .4f, .6f, .6f, .4f, .2f };
                    float[] myPositions = { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
                    _mBlend.Factors = myFactors;
                    _mBlend.Positions = myPositions;

                    var aProp = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                    aProp.SetValue(_panelCompare.listView_main, true, null);

                    ApplyEventHandlersCompare();
                    LoadPropertiesCompare();
                    FormMain_ResizeEnd(null, null);
                    CheckEnabledButtonToolbarMain();

                    if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                    {
                        LoadPropertiesComparisonProjects(null);
                    }
                }
                else if (_panelCompare == null || _panelCompare.DockPanel == null)
                {
                    _panelCompare = new PanelCompare();
                    _panelCompare.Show(dockPanel_manager, DockState.Document);


                    float[] myFactors = { .2f, .4f, .6f, .6f, .4f, .2f };
                    float[] myPositions = { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
                    _mBlend.Factors = myFactors;
                    _mBlend.Positions = myPositions;


                    var aProp = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                    aProp.SetValue(_panelCompare.listView_main, true, null);

                    ApplyEventHandlersCompare();
                    LoadPropertiesCompare();
                    FormMain_ResizeEnd(null, null);
                    CheckEnabledButtonToolbarMain();

                    if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                    {
                        LoadPropertiesComparisonProjects(null);
                    }
                }
                else
                {
                    _panelCompare.Show();
                    _panelCompare.Focus();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (_panelCompare != null && _panelCompare.DockPanel != null)
                    _panelCompare.Close();

                _panelCompare = new PanelCompare();
                _panelCompare.Show(dockPanel_manager, DockState.Document);

                float[] myFactors = { .2f, .4f, .6f, .6f, .4f, .2f };
                float[] myPositions = { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
                _mBlend.Factors = myFactors;
                _mBlend.Positions = myPositions;

                var aProp = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                aProp.SetValue(_panelCompare.listView_main, true, null);

                ApplyEventHandlersCompare();
                LoadPropertiesCompare();
                FormMain_ResizeEnd(null, null);
                CheckEnabledButtonToolbarMain();

                if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                {
                    LoadPropertiesComparisonProjects(null);
                }
            }
            finally
            {
                if (_panelCompare != null) _panelCompare.panel_listViewMessage.Visible = false;
            }

        }
        private void ShowPanelComparisonProjectsPanel(bool resetState)
        {
            try
            {
                if (resetState)
                {
                    if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                        _panelComparisonProjects.Close();

                    _panelComparisonProjects = new PanelComparisonProjects();
                    _panelComparisonProjects.Show(dockPanel_manager, DockState.DockRight);

                    ApplyEventHandlersComparisonsProjects();
                    LoadPropertiesComparisonProjects(null);
                }
                else if (_panelComparisonProjects == null || _panelComparisonProjects.DockPanel == null)
                {
                    _panelComparisonProjects = new PanelComparisonProjects();
                    _panelComparisonProjects.Show(dockPanel_manager, DockState.DockRight);

                    ApplyEventHandlersComparisonsProjects();
                    LoadPropertiesComparisonProjects(null);
                }
                else
                {
                    _panelComparisonProjects.Show();
                    _panelComparisonProjects.Focus();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                    _panelComparisonProjects.Close();

                _panelComparisonProjects = new PanelComparisonProjects();
                _panelComparisonProjects.Show(dockPanel_manager, DockState.DockRight);

                ApplyEventHandlersComparisonsProjects();
                LoadPropertiesComparisonProjects(null);
            }

        }
        private void ShowPanelEventsLogPanel(bool resetState)
        {
            try
            {

                if (resetState)
                {
                    if (_panelEventsLog != null && _panelEventsLog.DockPanel != null)
                        _panelEventsLog.Close();

                    _panelEventsLog = new PanelEventsLog();
                    _panelEventsLog.Show(dockPanel_manager, DockState.DockBottom);

                    CreateEntriesLogReport();
                }
                else if (_panelEventsLog == null || _panelEventsLog.DockPanel == null)
                {
                    _panelEventsLog = new PanelEventsLog();
                    _panelEventsLog.Show(dockPanel_manager, DockState.DockBottom);
                    _panelEventsLog.Focus();

                    CreateEntriesLogReport();
                }
                else
                {
                    _panelEventsLog.Show();
                    _panelEventsLog.Focus();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (_panelEventsLog != null && _panelEventsLog.DockPanel != null)
                    _panelEventsLog.Close();

                _panelEventsLog = new PanelEventsLog();
                _panelEventsLog.Show(dockPanel_manager, DockState.DockBottom);

                CreateEntriesLogReport();
            }

        }

        private void ApplyEventHandlersCompare()
        {
            _panelCompare.Resize -= panel_CompareProject_Resize;

            _panelCompare.compareFilesToolStripMenuItem.Click -= compareFilesToolStripMenuItem_Click;
            _panelCompare.compareToToolStripMenuItem.Click -= compareToToolStripMenuItem_Click;

            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItemClicked -= toolStripDropDownButton_comparison_project_left_side_move_DropDownItemClicked;
            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItemClicked -= toolStripDropDownButton_comparison_project_right_side_move_DropDownItemClicked;


            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Click -= toolStripDropDownButton_comparison_project_left_side_move_Click;
            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Click -= toolStripDropDownButton_comparison_project_right_side_move_Click;


            _panelCompare.listView_main.DragOver -= listView_main_DragOver;
            _panelCompare.listView_main.DragDrop -= listView_main_DragDrop;

            _panelCompare.listView_main.DoubleClick -= listView_main_DoubleClick;

            _panelCompare.listView_main.MouseClick -= listView_main_MouseClick;
            _panelCompare.listView_main.MouseDoubleClick -= listView_main_MouseDoubleClick;
            _panelCompare.listView_main.RetrieveVirtualItem -= listView_main_RetrieveVirtualItem;


            _panelCompare.listView_main.DrawColumnHeader -= listView_main_DrawColumnHeader;
            _panelCompare.listView_main.DrawItem -= listView_main_DrawItem;
            _panelCompare.listView_main.DrawSubItem -= listView_main_DrawSubItem;


            _panelCompare.listView_main.KeyUp -= listView_main_KeyUp;

            _panelCompare.listView_main.MouseDown -= listView_main_MouseDown;
            _panelCompare.listView_main.MouseUp -= listView_main_MouseUp;

            _panelCompare.listView_main.ColumnWidthChanging -= listView_main_ColumnWidthChanging;

            _panelCompare.contextMenuStrip1.Opening -= contextMenuStrip1_Opening;

            _panelCompare.expandFolderToolStripMenuItem.Click -= expandFolderToolStripMenuItem_Click;
            _panelCompare.expandAllSubfoldersToolStripMenuItem.Click -= expandAllSubfoldersToolStripMenuItem_Click;
            _panelCompare.collapseAllSubfoldersToolStripMenuItem.Click -= collapseAllSubfoldersToolStripMenuItem_Click;

            _panelCompare.setAsBasefolderToolStripMenuItem.Click -= setAsBasefolderToolStripMenuItem_Click;
            _panelCompare.compareFoldersToolStripMenuItem.Click -= compareFoldersToolStripMenuItem_Click;
            _panelCompare.createComparisonReportToolStripMenuItem.Click -= createComparisonReportToolStripMenuItem_Click;


            _panelCompare.copyToToolStripMenuItem.Click -= copyToToolStripMenuItem_Click;
            _panelCompare.copyToFolderToolStripMenuItem.Click -= copyToFolderToolStripMenuItem_Click;
            _panelCompare.moveToToolStripMenuItem.Click -= moveToToolStripMenuItem_Click;

            _panelCompare.deleteToolStripMenuItem.Click -= deleteToolStripMenuItem_Click;
            _panelCompare.renameToolStripMenuItem.Click -= renameToolStripMenuItem_Click;
            _panelCompare.attributesToolStripMenuItem.Click -= attributesToolStripMenuItem_Click;
            _panelCompare.propertiesToolStripMenuItem.Click -= propertiesToolStripMenuItem_Click;




            _panelCompare.comboBox_main_compare_left.SelectedIndexChanged -= comboBox_main_compare_left_SelectedIndexChanged;
            _panelCompare.comboBox_main_compare_left.KeyUp -= comboBox_main_compare_left_KeyUp;

            _panelCompare.toolStripButton_loadLeftSide.Click -= toolStripButton_loadLeftSide_Click;
            _panelCompare.toolStripButton_browseFolderLeftSide.Click -= toolStripButton_browseFolderLeftSide_Click;

            _panelCompare.toolStripButton_upOneLevelLeftSide.Click -= toolStripButton_upOneLevelLeftSide_Click;
            _panelCompare.toolStripButton_bothFoldersUpOneLevel.Click -= toolStripButton_bothFoldersUpOneLevel_Click;
            _panelCompare.toolStripButton_setBaseFolder_leftSide.Click -= toolStripButton_setBaseFolder_leftSide_Click;
            _panelCompare.comboBox_main_compare_right.SelectedIndexChanged -= comboBox_main_compare_right_SelectedIndexChanged;
            _panelCompare.comboBox_main_compare_right.KeyUp -= comboBox_main_compare_right_KeyUp;

            _panelCompare.toolStripButton_loadRightSide.Click -= toolStripButton_loadRightSide_Click;
            _panelCompare.toolStripButton_browseFolderRightSide.Click -= toolStripButton_browseFolderRightSide_Click;
            _panelCompare.toolStripButton_setBaseFolder_rightSide.Click -= toolStripButton_setBaseFolder_rightSide_Click;
            _panelCompare.toolStripButton_upOneLevelRightSide.Click -= toolStripButton_upOneLevelRightSide_Click;

            _panelCompare.comboBox_main_compare_left.TextChanged -= comboBox_main_compare_left_TextChanged;
            _panelCompare.comboBox_main_compare_right.TextChanged -= comboBox_main_compare_right_TextChanged;



            _panelCompare.Resize += panel_CompareProject_Resize;


            _panelCompare.compareFilesToolStripMenuItem.Click += compareFilesToolStripMenuItem_Click;
            _panelCompare.compareToToolStripMenuItem.Click += compareToToolStripMenuItem_Click;

            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItemClicked += toolStripDropDownButton_comparison_project_left_side_move_DropDownItemClicked;
            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItemClicked += toolStripDropDownButton_comparison_project_right_side_move_DropDownItemClicked;


            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Click += toolStripDropDownButton_comparison_project_left_side_move_Click;
            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Click += toolStripDropDownButton_comparison_project_right_side_move_Click;

            _panelCompare.listView_main.DragOver += listView_main_DragOver;
            _panelCompare.listView_main.DragDrop += listView_main_DragDrop;

            _panelCompare.listView_main.DoubleClick += listView_main_DoubleClick;

            _panelCompare.listView_main.MouseClick += listView_main_MouseClick;
            _panelCompare.listView_main.MouseDoubleClick += listView_main_MouseDoubleClick;
            _panelCompare.listView_main.RetrieveVirtualItem += listView_main_RetrieveVirtualItem;


            _panelCompare.listView_main.DrawColumnHeader += listView_main_DrawColumnHeader;
            _panelCompare.listView_main.DrawItem += listView_main_DrawItem;
            _panelCompare.listView_main.DrawSubItem += listView_main_DrawSubItem;


            _panelCompare.listView_main.KeyUp += listView_main_KeyUp;
            _panelCompare.listView_main.MouseDown += listView_main_MouseDown;
            _panelCompare.listView_main.MouseUp += listView_main_MouseUp;

            _panelCompare.listView_main.ColumnWidthChanging += listView_main_ColumnWidthChanging;



            _panelCompare.contextMenuStrip1.Opening += contextMenuStrip1_Opening;

            _panelCompare.expandFolderToolStripMenuItem.Click += expandFolderToolStripMenuItem_Click;
            _panelCompare.expandAllSubfoldersToolStripMenuItem.Click += expandAllSubfoldersToolStripMenuItem_Click;
            _panelCompare.collapseAllSubfoldersToolStripMenuItem.Click += collapseAllSubfoldersToolStripMenuItem_Click;

            _panelCompare.setAsBasefolderToolStripMenuItem.Click += setAsBasefolderToolStripMenuItem_Click;
            _panelCompare.compareFoldersToolStripMenuItem.Click += compareFoldersToolStripMenuItem_Click;
            _panelCompare.createComparisonReportToolStripMenuItem.Click += createComparisonReportToolStripMenuItem_Click;


            _panelCompare.copyToToolStripMenuItem.Click += copyToToolStripMenuItem_Click;
            _panelCompare.copyToFolderToolStripMenuItem.Click += copyToFolderToolStripMenuItem_Click;

            _panelCompare.moveToToolStripMenuItem.Click += moveToToolStripMenuItem_Click;

            _panelCompare.deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            _panelCompare.renameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            _panelCompare.attributesToolStripMenuItem.Click += attributesToolStripMenuItem_Click;
            _panelCompare.propertiesToolStripMenuItem.Click += propertiesToolStripMenuItem_Click;



            _panelCompare.comboBox_main_compare_left.SelectedIndexChanged += comboBox_main_compare_left_SelectedIndexChanged;
            _panelCompare.comboBox_main_compare_left.KeyUp += comboBox_main_compare_left_KeyUp;

            _panelCompare.toolStripButton_loadLeftSide.Click += toolStripButton_loadLeftSide_Click;
            _panelCompare.toolStripButton_browseFolderLeftSide.Click += toolStripButton_browseFolderLeftSide_Click;
            _panelCompare.toolStripButton_setBaseFolder_leftSide.Click += toolStripButton_setBaseFolder_leftSide_Click;
            _panelCompare.toolStripButton_upOneLevelLeftSide.Click += toolStripButton_upOneLevelLeftSide_Click;
            _panelCompare.toolStripButton_bothFoldersUpOneLevel.Click += toolStripButton_bothFoldersUpOneLevel_Click;

            _panelCompare.comboBox_main_compare_right.SelectedIndexChanged += comboBox_main_compare_right_SelectedIndexChanged;
            _panelCompare.comboBox_main_compare_right.KeyUp += comboBox_main_compare_right_KeyUp;

            _panelCompare.toolStripButton_loadRightSide.Click += toolStripButton_loadRightSide_Click;
            _panelCompare.toolStripButton_browseFolderRightSide.Click += toolStripButton_browseFolderRightSide_Click;
            _panelCompare.toolStripButton_setBaseFolder_rightSide.Click += toolStripButton_setBaseFolder_rightSide_Click;
            _panelCompare.toolStripButton_upOneLevelRightSide.Click += toolStripButton_upOneLevelRightSide_Click;


            _panelCompare.comboBox_main_compare_left.TextChanged += comboBox_main_compare_left_TextChanged;
            _panelCompare.comboBox_main_compare_right.TextChanged += comboBox_main_compare_right_TextChanged;

        }
        private void ApplyEventHandlersComparisonsProjects()
        {
            _panelComparisonProjects.listView_comparison_projects.ItemSelectionChanged -= listView_comparison_projects_ItemSelectionChanged;
            _panelComparisonProjects.listView_comparison_projects.DoubleClick -= listView_comparison_projects_DoubleClick;
            _panelComparisonProjects.listView_comparison_projects.KeyUp -= listView_comparison_projects_KeyUp;


            _panelComparisonProjects.loadComparisonProjectToolStripMenuItem.Click -= loadComparisonProjectToolStripMenuItem_Click;
            _panelComparisonProjects.newCompareListItemToolStripMenuItem.Click -= newCompareListItemToolStripMenuItem_Click;
            _panelComparisonProjects.editCompareListItemToolStripMenuItem.Click -= editCompareListItemToolStripMenuItem_Click;
            _panelComparisonProjects.removeCompareListItemToolStripMenuItem.Click -= removeCompareListItemToolStripMenuItem_Click;
            _panelComparisonProjects.viewFileAlignmentToolStripMenuItem.Click -= viewFileAlignmentToolStripMenuItem_Click;


            _panelComparisonProjects.toolStripButton_comparison_project_load.Click -= toolStripButton_comparison_project_load_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_save.Click -= toolStripButton_comparison_project_save_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_settings.Click -= toolStripButton_comparison_project_settings_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_new.Click -= toolStripButton_comparison_project_new_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_edit.Click -= toolStripButton_comparison_project_edit_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_remove.Click -= toolStripButton_comparison_project_remove_Click;





            _panelComparisonProjects.listView_comparison_projects.ItemSelectionChanged += listView_comparison_projects_ItemSelectionChanged;
            _panelComparisonProjects.listView_comparison_projects.DoubleClick += listView_comparison_projects_DoubleClick;
            _panelComparisonProjects.listView_comparison_projects.KeyUp += listView_comparison_projects_KeyUp;

            _panelComparisonProjects.loadComparisonProjectToolStripMenuItem.Click += loadComparisonProjectToolStripMenuItem_Click;
            _panelComparisonProjects.newCompareListItemToolStripMenuItem.Click += newCompareListItemToolStripMenuItem_Click;
            _panelComparisonProjects.editCompareListItemToolStripMenuItem.Click += editCompareListItemToolStripMenuItem_Click;
            _panelComparisonProjects.removeCompareListItemToolStripMenuItem.Click += removeCompareListItemToolStripMenuItem_Click;
            _panelComparisonProjects.viewFileAlignmentToolStripMenuItem.Click += viewFileAlignmentToolStripMenuItem_Click;


            _panelComparisonProjects.toolStripButton_comparison_project_load.Click += toolStripButton_comparison_project_load_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_save.Click += toolStripButton_comparison_project_save_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_settings.Click += toolStripButton_comparison_project_settings_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_new.Click += toolStripButton_comparison_project_new_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_edit.Click += toolStripButton_comparison_project_edit_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_remove.Click += toolStripButton_comparison_project_remove_Click;
            _panelComparisonProjects.toolStripButton_comparison_project_file_alignment.Click += toolStripButton_comparison_project_file_alignment_Click;

        }


        private void LoadPropertiesComparisonProjects(Settings.ComparisonProject comparisonProjectSelected)
        {

            try
            {
                IsInitializingPanel = true;

                if (_panelComparisonProjects != null)
                {
                    #region  |  comparison projects  |

                    _panelComparisonProjects.listView_comparison_projects.Items.Clear();

                    foreach (var comparisonProject in Application.Settings.ComparisonProjects)
                    {
                        var itmx = _panelComparisonProjects.listView_comparison_projects.Items.Add(comparisonProject.Name);
                        itmx.SubItems.Add(comparisonProject.PathLeft + " <> " + comparisonProject.PathRight);

                        itmx.Tag = comparisonProject;


                        var isPathNotFoundError = false;
                        if (!Directory.Exists(comparisonProject.PathLeft))
                        {
                            itmx.SubItems[1].BackColor = Color.Pink;
                            isPathNotFoundError = true;
                            itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_left_directory_path_does_not_exist;
                        }
                        else if (!Directory.Exists(comparisonProject.PathLeft))
                        {
                            itmx.SubItems[1].BackColor = Color.Pink;
                            isPathNotFoundError = true;
                            itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_right_directory_path_does_not_exist;
                        }

                        itmx.Tag = comparisonProject;


                        itmx.ImageIndex = isPathNotFoundError ? 4 : 3;

                    }
                    if (_panelComparisonProjects.listView_comparison_projects.Items.Count > 0)
                        _panelComparisonProjects.listView_comparison_projects.Items[0].Selected = true;


                    #endregion




                    if (Application.Settings.ComparisonProjects.Count > 0)
                    {
                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Clear();
                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Clear();

                        var index = 0;
                        foreach (var comparisonProject in Application.Settings.ComparisonProjects)
                        {
                            var name = comparisonProject.Name + ": " + comparisonProject.PathLeft + " <> " + comparisonProject.PathRight;
                            if (index == 0)
                            {
                                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProject;
                            }
                            else
                            {
                                var tsi = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                                tsi.Tag = comparisonProject;
                            }
                            index++;
                        }

                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;
                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                        {
                            var comparisonProjectNew =
                                (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                            AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);
                        }

                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;
                    _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                    SelectComparisonProject(comparisonProjectSelected, false);



                    listView_comparison_projects_ItemSelectionChanged(null, null);

                }

            }
            finally
            {
                IsInitializingPanel = false;
            }
        }
        private void LoadPropertiesCompare()
        {
            try
            {
                CompareToActive = false;
                CompareFilePairActive = false;


                _panelCompare.toolStripButton_loadLeftSide.Enabled = false;
                _panelCompare.toolStripButton_loadRightSide.Enabled = false;

                IsInitializingPanel = true;

                if (Application.Settings.FolderViewerFoldersLeft.Count > 0)
                {
                    foreach (var folder in Application.Settings.FolderViewerFoldersLeft)
                    {
                        _panelCompare.comboBox_main_compare_left.Items.Add(folder);
                    }
                    _panelCompare.comboBox_main_compare_left.SelectedIndex = 0;
                }
                if (Application.Settings.FolderViewerFoldersRight.Count <= 0) return;
                {
                    foreach (var folder in Application.Settings.FolderViewerFoldersRight)
                    {
                        _panelCompare.comboBox_main_compare_right.Items.Add(folder);
                    }
                    _panelCompare.comboBox_main_compare_right.SelectedIndex = 0;
                }
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }


        public void ResetDefaultWindowStatesCompare()
        {
            if (_panelCompare != null && _panelCompare.DockPanel != null)
                _panelCompare.Close();
            ShowPanelCompare(true);


            if (_panelComparisonProjects != null && _panelComparisonProjects.DockPanel != null)
                _panelComparisonProjects.Close();
            ShowPanelComparisonProjectsPanel(true);


            if (_panelEventsLog != null && _panelEventsLog.DockPanel != null)
                _panelEventsLog.Close();
            ShowPanelEventsLogPanel(true);






        }




        #region  |   Panel functions  |

        //private ListViewSortManager m_sortMgr;
        private readonly IModel _mModel;
        private readonly List<DataNode> _mMapper = new List<DataNode>(64);
        private readonly List<DataNode> _mMapperReport = new List<DataNode>(64);
        private const TextFormatFlags SmTff = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;
        private Rectangle _mRect;
        private readonly Color _mColorFrom = SystemColors.HotTrack;
        private readonly Color _mColorTo = SystemColors.ControlLight;
        private readonly Blend _mBlend = new Blend();



        private Dictionary<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>> FileComparisonParagraphUnits { get; set; }

        private static ReportViewer ReportDialog { get; set; }


        private void CheckToolTips()
        {
            toolStripButton_showEmptyFolders.ToolTipText = !Application.Settings.ShowEmptyFolders ? "Show empty folders" : "Hide empty folders";
            toolStripButton_Activate_Filters.ToolTipText = !Application.Settings.ActivateFilters ? "Activate filters" : "Suppress filters";
            toolStripButton_ignoreEqualFiles.ToolTipText = !Application.Settings.ShowEqualFiles ? "Show files that are equal" : "Hide files that are equal";
            toolStripButton_ignoreOrphansLeftSide.ToolTipText = !Application.Settings.ShowOrphanFilesLeft ? "Show orphans files on the left side" : "Hide orphans files on the left side";
            toolStripButton_ignoreOrphansRightSide.ToolTipText = !Application.Settings.ShowOrphanFilesRight ? "Show orphans files on the right side" : "Hide orphans files on the right side";
            toolStripButton_ignoreDifferencesLeftSide.ToolTipText = !Application.Settings.ShowDifferencesFilesLeft ? "Show mismatches - left side newer" : "Hide mismatches - left side newer";
            toolStripButton_ignoreDifferencesRightSide.ToolTipText = !Application.Settings.ShowDifferencesFilesRight ? "Show mismatches - right side newer" : "Hide mismatches - right side newer";
        }


        #region  |  filters  |



        private readonly List<Regex> _filtersExtensionsRegex = new List<Regex>();
        private bool _filtersExtensionsInclude = true;


        private bool _filtersUseDateTime;
        private DateTime _filtersDateTime = Common.DateNull;
        private bool _filtersDateTimeBefore = true;

        private string _filtersFilterAttributeAchiveType = string.Empty;
        private bool _filtersFilterAttributeArchiveUsed;
        private string _filtersFilterAttributeHiddenType = string.Empty;
        private bool _filtersFilterAttributeHiddenUsed;
        private string _filtersFilterAttributeReadOnlyType = string.Empty;
        private bool _filtersFilterAttributeReadOnlyUsed;
        private string _filtersFilterAttributeSystemType = string.Empty;
        private bool _filtersFilterAttributeSystemUsed;


        private bool AddToFilteredList(DataNode dn)
        {
            var success = true;
            var bEqual = (dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal;
            var bMismatch = (dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch;
            var bNewerRightside = (dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside;
            var bNewerLeftside = (dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside;
            var bOrphansLeftside = (dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside;
            var bOrphansRightside = (dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside;


            if (dn.Type != DataNode.ItemType.File)
                return true;
            var b1 = false;
            var b2 = false;
            var b3 = true;

            #region  |  filters_useDateTime  |

            if (_filtersUseDateTime)
            {
                if (_filtersDateTimeBefore)
                {
                    if (dn.ModifiedLeft <= _filtersDateTime
                        || dn.ModifiedRight <= _filtersDateTime)
                    {
                        b1 = true;
                    }
                }
                else
                {
                    if (dn.ModifiedLeft >= _filtersDateTime
                        || dn.ModifiedRight >= _filtersDateTime)
                    {
                        b1 = true;
                    }
                }
            }
            else
            {
                b1 = true;
            }
            #endregion


            if (b1)
            {
                #region  |  check file extensions  |

                if (_filtersExtensionsRegex.Count > 0)
                {
                    foreach (var t in _filtersExtensionsRegex)
                    {
                        if (_filtersExtensionsInclude)
                        {
                            #region  |  include extensions  |
                            if (dn.NameLeft.Trim() != string.Empty)
                            {
                                b2 = t.Match(dn.NameLeft).Success;
                            }
                            if (!b2 && string.Compare(dn.NameLeft, dn.NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                if (dn.NameRight.Trim() != string.Empty)
                                {
                                    b2 = t.Match(dn.NameRight).Success;
                                }
                            }
                            if (b2)
                            {
                                break;
                            }
                            #endregion
                        }
                        else
                        {
                            #region  |  exclude extensions  |

                            if (dn.NameLeft.Trim() != string.Empty)
                            {
                                b2 = !t.Match(dn.NameLeft).Success;


                            }
                            if (!b2 && string.Compare(dn.NameLeft, dn.NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                if (dn.NameRight.Trim() != string.Empty)
                                {
                                    b2 = !t.Match(dn.NameRight).Success;

                                }
                            }
                            if (b2)
                            {
                                break;
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    b2 = true;
                }
                #endregion
            }


            #region  |  check file properties  |

            if (b1 && b2 && (_filtersFilterAttributeArchiveUsed || _filtersFilterAttributeHiddenUsed
                    || _filtersFilterAttributeReadOnlyUsed || _filtersFilterAttributeSystemUsed))
            {
                var propertiesArrayLeft = dn.PropertiesLeft.Split(';').ToList();
                var propertiesArrayRight = dn.PropertiesRight.Split(';').ToList();

                if (b3 && _filtersFilterAttributeArchiveUsed)
                {
                    if (_filtersFilterAttributeAchiveType == "Included")
                    {
                        if (!propertiesArrayLeft.Contains("A") && !propertiesArrayLeft.Contains("A"))
                        {
                            b3 = false;
                        }
                    }
                    else if (propertiesArrayLeft.Contains("A") && propertiesArrayLeft.Contains("A"))
                    {
                        b3 = false;
                    }
                }
                if (b3 && _filtersFilterAttributeHiddenUsed)
                {
                    if (_filtersFilterAttributeHiddenType == "Included")
                    {
                        if (!propertiesArrayLeft.Contains("H") && !propertiesArrayLeft.Contains("H"))
                        {
                            b3 = false;
                        }
                    }
                    else if (propertiesArrayLeft.Contains("H") && propertiesArrayLeft.Contains("H"))
                    {
                        b3 = false;
                    }
                }
                if (b3 && _filtersFilterAttributeReadOnlyUsed)
                {
                    if (_filtersFilterAttributeReadOnlyType == "Included")
                    {
                        if (!propertiesArrayLeft.Contains("R") && !propertiesArrayLeft.Contains("R"))
                        {
                            b3 = false;
                        }
                    }
                    else if (propertiesArrayLeft.Contains("R") && propertiesArrayLeft.Contains("R"))
                    {
                        b3 = false;
                    }
                }
                if (b3 && _filtersFilterAttributeSystemUsed)
                {
                    if (_filtersFilterAttributeSystemType == "Included")
                    {
                        if (!propertiesArrayLeft.Contains("S") && !propertiesArrayLeft.Contains("S"))
                        {
                            b3 = false;
                        }
                    }
                    else if (propertiesArrayLeft.Contains("S") && propertiesArrayLeft.Contains("S"))
                    {
                        b3 = false;
                    }
                }
            }
            #endregion

            if (b1 && b2 && b3)
            {
                if (!Application.Settings.ShowEqualFiles && bEqual)
                {
                    success = false;
                }
                if (!Application.Settings.ShowOrphanFilesLeft && bOrphansLeftside)
                {
                    success = false;
                }
                if (!Application.Settings.ShowOrphanFilesRight && bOrphansRightside)
                {
                    success = false;
                }
                if (!Application.Settings.ShowDifferencesFilesLeft && bNewerLeftside)
                {
                    success = false;
                }
                if (!Application.Settings.ShowDifferencesFilesRight && bNewerRightside)
                {
                    success = false;
                }
                if ((!Application.Settings.ShowDifferencesFilesLeft || !Application.Settings.ShowDifferencesFilesRight)
                    && bMismatch && (!bNewerLeftside & !bNewerRightside))
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        private void SetFilterList(bool enabled)
        {
            //filters_extensions.Clear();
            _filtersExtensionsRegex.Clear();


            _filtersUseDateTime = false;
            _filtersDateTime = Common.DateNull;
            _filtersDateTimeBefore = true;

            _filtersFilterAttributeAchiveType = string.Empty;
            _filtersFilterAttributeArchiveUsed = false;
            _filtersFilterAttributeHiddenType = string.Empty;
            _filtersFilterAttributeHiddenUsed = false;
            _filtersFilterAttributeReadOnlyType = string.Empty;
            _filtersFilterAttributeReadOnlyUsed = false;
            _filtersFilterAttributeSystemType = string.Empty;
            _filtersFilterAttributeSystemUsed = false;

            if (enabled)
            {
                if (toolStripComboBox_fileFilters.Text.Trim() != string.Empty)
                {
                    if (toolStripComboBox_fileFilters.Items.Count > 0)
                    {
                        var fsSelected = (Settings.FilterSetting)((ComboboxItem)toolStripComboBox_fileFilters.SelectedItem).Value;

                        #region  |  fs_selected.filterNamesInclude  |

                        _filtersExtensionsInclude = fsSelected.FilterNamesInclude.Count > 0 ? true : false;

                        var filtersExtensions = _filtersExtensionsInclude ? fsSelected.FilterNamesInclude : fsSelected.FilterNamesExclude;


                        foreach (var fsName in filtersExtensions)
                        {
                            if (fsName.Trim() == string.Empty) continue;
                            const bool foundFilterInList = false;

                            var filterStr = fsName;

                            if (filterStr.StartsWith("*"))
                                filterStr = "^" + filterStr;
                            _filtersExtensionsRegex.Add(new Regex(filterStr, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                        }
                        #endregion



                        #region  |  fs_selected.filterDate  |

                        if (fsSelected.FilterDateUsed)
                        {
                            _filtersUseDateTime = true;
                            _filtersDateTime = fsSelected.FilterDate.Date;
                            _filtersDateTimeBefore = fsSelected.FilterDate.Type == Settings.FilterDate.FilterType.LessThan ? true : false;
                        }
                        else
                        {
                            _filtersUseDateTime = false;
                            _filtersDateTime = Common.DateNull;
                            _filtersDateTimeBefore = true;
                        }
                        #endregion



                        #region  |  fs_selected.properties  |

                        _filtersFilterAttributeAchiveType = fsSelected.FilterAttributeAchiveType;
                        _filtersFilterAttributeArchiveUsed = fsSelected.FilterAttributeArchiveUsed;

                        _filtersFilterAttributeHiddenType = fsSelected.FilterAttributeHiddenType;
                        _filtersFilterAttributeHiddenUsed = fsSelected.FilterAttributeHiddenUsed;

                        _filtersFilterAttributeReadOnlyType = fsSelected.FilterAttributeReadOnlyType;
                        _filtersFilterAttributeReadOnlyUsed = fsSelected.FilterAttributeReadOnlyUsed;

                        _filtersFilterAttributeSystemType = fsSelected.FilterAttributeSystemType;
                        _filtersFilterAttributeSystemUsed = fsSelected.FilterAttributeSystemUsed;

                        #endregion
                    }
                }
            }
        }




        private void toolStripButton_Activate_Filters_Click(object sender, EventArgs e)
        {

            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();


                toolStripButton_Activate_Filters.Image = toolStripButton_Activate_Filters.Checked ? imageList1.Images["Filter_Feather_Disabled"] : imageList1.Images["Filter_Feather_Enabled"];

                Application.Settings.ActivateFilters = !Application.Settings.ActivateFilters;
                toolStripButton_Activate_Filters.CheckState = Application.Settings.ActivateFilters ? CheckState.Checked : CheckState.Unchecked;


                //Enabled = true;
                SetFilterList(toolStripButton_Activate_Filters.Checked);

                UpdateVisualCompareDirectories();

                UpdateVisualCompareDirectories();

                CheckToolTips();


                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }


        }
        private void toolStripButton_filter_settings_edit_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox_fileFilters.SelectedItem != null)
            {
                if (toolStripComboBox_fileFilters.Items.Count <= 0) return;
                var cbi = (ComboboxItem)toolStripComboBox_fileFilters.SelectedItem;
                var fsSelected = (Settings.FilterSetting)cbi.Value;




                var filterTextBefore = string.Empty;
                var filterTextUpdated = string.Empty;
                if (fsSelected.FilterNamesInclude.Count > 0)
                {
                    filterTextBefore = fsSelected.FilterNamesInclude.Aggregate(filterTextBefore, (current, str) => current + (current != string.Empty ? ";" : string.Empty) + str);
                }
                if (fsSelected.FilterNamesExclude.Count > 0)
                {
                    filterTextBefore = fsSelected.FilterNamesExclude.Aggregate(filterTextBefore, (current, str) => current + (current != string.Empty ? ";" : string.Empty) + "-" + str);
                }


                if (fsSelected.FilterDateUsed)
                {
                    if (fsSelected.FilterDate.Type == Settings.FilterDate.FilterType.GreaterThan)
                    {
                        filterTextBefore += (filterTextBefore.Trim() != string.Empty ? ";" : string.Empty)
                                            + ">" + fsSelected.FilterDate.Date.Date.ToShortDateString() + " " + fsSelected.FilterDate.Date.ToShortTimeString();
                    }
                    else
                    {
                        filterTextBefore += (filterTextBefore.Trim() != string.Empty ? ";" : string.Empty)
                                            + "<" + fsSelected.FilterDate.Date.Date.ToShortDateString() + " " + fsSelected.FilterDate.Date.ToShortTimeString();
                    }
                }
                var attributes = string.Empty;
                if (fsSelected.FilterAttributeArchiveUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                }
                if (fsSelected.FilterAttributeSystemUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeSystemType == "Included" ? "S" : "-S");
                }
                if (fsSelected.FilterAttributeHiddenUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                }
                if (fsSelected.FilterAttributeReadOnlyUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                }


                filterTextBefore += "" + filterTextBefore + (filterTextBefore.Trim() != string.Empty ? "; " : string.Empty) + attributes;


                var f = new FilterAppend
                {
                    FilterSetting = (Settings.FilterSetting)fsSelected.Clone(),
                    IsEdit = true
                };

                f.ShowDialog();
                if (!f.Saved) return;

                fsSelected.Name = f.FilterSetting.Name;
                fsSelected.FilterNamesInclude = f.FilterSetting.FilterNamesInclude;
                fsSelected.FilterNamesExclude = f.FilterSetting.FilterNamesExclude;
                fsSelected.UseRegularExpressionMatching = f.FilterSetting.UseRegularExpressionMatching;
                fsSelected.FilterDateUsed = f.FilterSetting.FilterDateUsed;
                fsSelected.FilterDate.Date = f.FilterSetting.FilterDate.Date;
                fsSelected.FilterDate.Type = f.FilterSetting.FilterDate.Type;
                fsSelected.IsDefault = f.FilterSetting.IsDefault;
                fsSelected.FilterAttributeAchiveType = f.FilterSetting.FilterAttributeAchiveType;
                fsSelected.FilterAttributeArchiveUsed = f.FilterSetting.FilterAttributeArchiveUsed;
                fsSelected.FilterAttributeHiddenType = f.FilterSetting.FilterAttributeHiddenType;
                fsSelected.FilterAttributeHiddenUsed = f.FilterSetting.FilterAttributeHiddenUsed;
                fsSelected.FilterAttributeReadOnlyType = f.FilterSetting.FilterAttributeReadOnlyType;
                fsSelected.FilterAttributeReadOnlyUsed = f.FilterSetting.FilterAttributeReadOnlyUsed;
                fsSelected.FilterAttributeSystemType = f.FilterSetting.FilterAttributeSystemType;
                fsSelected.FilterAttributeSystemUsed = f.FilterSetting.FilterAttributeSystemUsed;

                cbi.Value = fsSelected;


                if (Application.Settings != null)
                {
                    SettingsSerializer.SaveSettings(Application.Settings);
                }


                #region  |  filter setttings  |


                toolStripComboBox_fileFilters.Items.Clear();
                var index = -1;
                var selectedIndex = 0;
                if (Application.Settings != null)
                    foreach (var filterSetting in Application.Settings.FilterSettings)
                    {
                        if (filterSetting.Name.Trim() == string.Empty) continue;
                        cbi = new ComboboxItem
                        {
                            Text = string.Empty,
                            Value = filterSetting
                        };

                        var filterText = string.Empty;
                        if (filterSetting.FilterNamesInclude.Count > 0)
                        {
                            filterText = filterSetting.FilterNamesInclude.Aggregate(filterText, (current, str) => current + (current != string.Empty ? ";" : string.Empty) + str);
                        }
                        if (filterSetting.FilterNamesExclude.Count > 0)
                        {
                            filterText = filterSetting.FilterNamesExclude.Aggregate(filterText, (current, str) => current + (current != string.Empty ? ";" : string.Empty) + "-" + str);
                        }
                        if (filterSetting.FilterDateUsed)
                        {
                            if (filterSetting.FilterDate.Type == Settings.FilterDate.FilterType.GreaterThan)
                            {
                                filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                              + ">" + filterSetting.FilterDate.Date.Date.ToShortDateString() + " " + filterSetting.FilterDate.Date.ToShortTimeString();
                            }
                            else
                            {
                                filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                              + "<" + filterSetting.FilterDate.Date.Date.ToShortDateString() + " " + filterSetting.FilterDate.Date.ToShortTimeString();
                            }
                        }

                        attributes = string.Empty;
                        if (filterSetting.FilterAttributeArchiveUsed)
                        {
                            attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                          + (filterSetting.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                        }
                        if (filterSetting.FilterAttributeSystemUsed)
                        {
                            attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                          + (filterSetting.FilterAttributeSystemType == "Included" ? "S" : "-S");
                        }
                        if (filterSetting.FilterAttributeHiddenUsed)
                        {
                            attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                          + (filterSetting.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                        }
                        if (filterSetting.FilterAttributeReadOnlyUsed)
                        {
                            attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                          + (filterSetting.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                        }


                        cbi.Text += filterText + (filterText.Trim() != string.Empty ? "; " : string.Empty) + attributes;

                        if (cbi.Text.Trim() == string.Empty) continue;
                        index++;
                        toolStripComboBox_fileFilters.Items.Add(cbi);

                        if (string.Compare(filterSetting.Id, fsSelected.Id, StringComparison.OrdinalIgnoreCase) != 0) continue;
                        selectedIndex = index;
                        filterTextUpdated = cbi.Text;
                    }

                if (toolStripComboBox_fileFilters.Items.Count > 0)
                    toolStripComboBox_fileFilters.SelectedIndex = selectedIndex;


                #endregion




                #region  |  ComparisonLogEntry  |


                var comparisonLogEntry = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.FilterEdit)
                {
                    Date = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    ItemName = fsSelected.Name,
                    Value01 = filterTextBefore,
                    Value02 = filterTextUpdated,
                    Items = null
                };





                update_comparison_log(comparisonLogEntry);

                #endregion


                CompareMe();

            }
            else
            {
                var fsSelected = new Settings.FilterSetting();

                var comboboxItem = new ComboboxItem { Value = fsSelected };



                var f = new FilterAppend
                {
                    FilterSetting = (Settings.FilterSetting)fsSelected.Clone(),
                    IsEdit = false
                };

                f.ShowDialog();
                if (!f.Saved)
                    return;
                fsSelected.Name = f.FilterSetting.Name;
                fsSelected.FilterNamesInclude = f.FilterSetting.FilterNamesInclude;
                fsSelected.FilterNamesExclude = f.FilterSetting.FilterNamesExclude;
                fsSelected.UseRegularExpressionMatching = f.FilterSetting.UseRegularExpressionMatching;
                fsSelected.FilterDateUsed = f.FilterSetting.FilterDateUsed;
                fsSelected.FilterDate.Date = f.FilterSetting.FilterDate.Date;
                fsSelected.FilterDate.Type = f.FilterSetting.FilterDate.Type;
                fsSelected.IsDefault = f.FilterSetting.IsDefault;
                fsSelected.FilterAttributeAchiveType = f.FilterSetting.FilterAttributeAchiveType;
                fsSelected.FilterAttributeArchiveUsed = f.FilterSetting.FilterAttributeArchiveUsed;
                fsSelected.FilterAttributeHiddenType = f.FilterSetting.FilterAttributeHiddenType;
                fsSelected.FilterAttributeHiddenUsed = f.FilterSetting.FilterAttributeHiddenUsed;
                fsSelected.FilterAttributeReadOnlyType = f.FilterSetting.FilterAttributeReadOnlyType;
                fsSelected.FilterAttributeReadOnlyUsed = f.FilterSetting.FilterAttributeReadOnlyUsed;
                fsSelected.FilterAttributeSystemType = f.FilterSetting.FilterAttributeSystemType;
                fsSelected.FilterAttributeSystemUsed = f.FilterSetting.FilterAttributeSystemUsed;

                comboboxItem.Text = string.Empty;
                comboboxItem.Value = fsSelected;

                var filterText = string.Empty;
                if (fsSelected.FilterNamesInclude.Count > 0)
                {
                    filterText = fsSelected.FilterNamesInclude.Aggregate(filterText, (current, str) => current + (current != string.Empty ? ";" : string.Empty) + str);
                }
                else if (fsSelected.FilterNamesExclude.Count > 0)
                {
                    filterText = fsSelected.FilterNamesExclude.Aggregate(filterText, (current, str) => current + (current != string.Empty ? ";" : string.Empty) + str);
                    filterText = "-" + filterText;
                }



                if (fsSelected.FilterDateUsed)
                {
                    if (fsSelected.FilterDate.Type == Settings.FilterDate.FilterType.GreaterThan)
                    {
                        filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                      + ">" + fsSelected.FilterDate.Date.Date.ToShortDateString() + " " + fsSelected.FilterDate.Date.ToShortTimeString();
                    }
                    else
                    {
                        filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                      + "<" + fsSelected.FilterDate.Date.Date.ToShortDateString() + " " + fsSelected.FilterDate.Date.ToShortTimeString();
                    }
                }
                var attributes = string.Empty;
                if (fsSelected.FilterAttributeArchiveUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                }
                if (fsSelected.FilterAttributeSystemUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeSystemType == "Included" ? "S" : "-S");
                }
                if (fsSelected.FilterAttributeHiddenUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                }
                if (fsSelected.FilterAttributeReadOnlyUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fsSelected.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                }



                comboboxItem.Text += "" + filterText + (filterText.Trim() != string.Empty ? "; " : string.Empty) + attributes;

                toolStripComboBox_fileFilters.Items.Add(comboboxItem);

                Application.Settings.FilterSettings.Add(fsSelected);

                toolStripComboBox_fileFilters.SelectedItem = comboboxItem;

                toolStripComboBox_fileFilters.Text = comboboxItem.Text;


                if (Application.Settings != null)
                {
                    SettingsSerializer.SaveSettings(Application.Settings);
                }

                #region  |  ComparisonLogEntry  |


                var comparisonLogEntry = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.FiltersAdd)
                {
                    Date = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    ItemName = fsSelected.Name,
                    Value01 = string.Empty,
                    Value02 = comboboxItem.Text,
                    Items = null
                };





                update_comparison_log(comparisonLogEntry);

                #endregion

                CompareMe();
            }

        }

        private void toolStripComboBox_fileFilters_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void toolStripComboBox_fileFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterList(toolStripButton_Activate_Filters.Checked);
            UpdateVisualCompareDirectories();
        }



        #endregion
        #region  |  comparison projects  |

        private void EditComparisonProject()
        {
            if (_panelComparisonProjects.listView_comparison_projects.SelectedItems.Count <= 0) return;
            var itmx = _panelComparisonProjects.listView_comparison_projects.SelectedItems[0];


            var f = new ComparisonProject();

            var comparisonProject = (Settings.ComparisonProject)itmx.Tag;
            f.IsEdit = true;
            f.comparisonProject = (Settings.ComparisonProject)comparisonProject.Clone();


            f.ShowDialog();
            {
                if (!f.Saved) return;
                var recompare = string.Compare(comparisonProject.Name, f.comparisonProject.Name, StringComparison.OrdinalIgnoreCase) != 0
                                || string.Compare(comparisonProject.PathLeft, f.comparisonProject.PathLeft, StringComparison.OrdinalIgnoreCase) != 0
                                || string.Compare(comparisonProject.PathRight, f.comparisonProject.PathRight, StringComparison.OrdinalIgnoreCase) != 0;






                comparisonProject.Name = f.comparisonProject.Name;
                comparisonProject.PathLeft = f.comparisonProject.PathLeft;
                comparisonProject.PathRight = f.comparisonProject.PathRight;


                itmx.SubItems[0].Text = comparisonProject.Name;
                itmx.SubItems[1].Text = comparisonProject.PathLeft + @" <> " + f.comparisonProject.PathRight;





                comparisonProject.FileAlignment = f.comparisonProject.FileAlignment;

                foreach (var cp in Application.Settings.ComparisonProjects)
                {
                    if (string.Compare(cp.Id, f.comparisonProject.Id, StringComparison.OrdinalIgnoreCase) != 0 ||
                        string.Compare(cp.PathRight, f.comparisonProject.PathRight,
                            StringComparison.OrdinalIgnoreCase) != 0) continue;
                    cp.FileAlignment = comparisonProject.FileAlignment;
                    break;
                }










                itmx.Tag = f.comparisonProject;


                var isPathNotFoundError = false;
                if (!Directory.Exists(f.comparisonProject.PathLeft))
                {
                    itmx.SubItems[2].BackColor = Color.Pink;
                    isPathNotFoundError = true;
                    itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_left_directory_path_does_not_exist;
                }
                else if (!Directory.Exists(f.comparisonProject.PathLeft))
                {
                    itmx.SubItems[3].BackColor = Color.Pink;
                    isPathNotFoundError = true;
                    itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_right_directory_path_does_not_exist;
                }
                //1 = Exclamation
                //2 = Exclamation-Circle-blue
                //3 = Exclamation-Circle-green
                //4 = Exclamation-Circle-red
                //5 = Exclamation-Circle-yellow
                //6 = No


                itmx.ImageIndex = isPathNotFoundError ? 4 : 3;
                itmx.Selected = true;



                try
                {
                    IsInitializingPanel = true;

                    if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                    {
                        var project = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;


                        if (project.Id == comparisonProject.Id)
                        {
                            project = comparisonProject;
                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = project;
                            var name = project.Name + ": " + project.PathLeft + " <> " + project.PathRight;
                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Text = name;
                        }
                        else
                        {
                            var moveLeftToRight = (from ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems
                                                   select (Settings.ComparisonProject)tsi.Tag).Any(tsiFvfp => string.Compare(tsiFvfp.Id, comparisonProject.Id, StringComparison.OrdinalIgnoreCase) == 0);


                            if (moveLeftToRight)
                            {
                                #region  |  moveLeftToLeft |


                                foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                                {
                                    var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                                    if (string.Compare(tsiFvfp.Id, comparisonProject.Id, StringComparison.OrdinalIgnoreCase) != 0)
                                        continue;
                                    tsiFvfp = comparisonProject;
                                    tsi.Tag = tsiFvfp;
                                    var name = tsiFvfp.Name + ": " + tsiFvfp.PathLeft + " <> " + tsiFvfp.PathRight;
                                    tsi.Text = name;
                                    break;
                                }


                                #endregion

                            }
                            else
                            {
                                #region  |  moveLeftToRight |


                                foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems)
                                {
                                    var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                                    if (string.Compare(tsiFvfp.Id, comparisonProject.Id, StringComparison.OrdinalIgnoreCase) != 0) continue;
                                    tsiFvfp = comparisonProject;
                                    tsi.Tag = tsiFvfp;
                                    var name = tsiFvfp.Name + ": " + tsiFvfp.PathLeft + " <> " + tsiFvfp.PathRight;
                                    tsi.Text = name;
                                    break;
                                }



                                #endregion
                            }




                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;
                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SaveApplicationSettings();
                    IsInitializingPanel = false;






                    #region  |  ComparisonLogEntry  |


                    var comparisonLogEntry = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.ComparisonProjectEdit)
                        {
                            Date = DateTime.Now,
                            Id = Guid.NewGuid().ToString(),
                            ItemName = f.comparisonProject.Name,
                            Value01 = string.Empty,
                            Value02 = string.Empty,
                            Items = null
                        };





                    update_comparison_log(comparisonLogEntry);
                    #endregion



                }

                if (!recompare)
                    return;
                AddComparisonEntriesToComboboxes(comparisonProject.PathLeft, comparisonProject.PathRight);

                CompareMe();
            }
        }
        private void SelectComparisonProject(Settings.ComparisonProject comparisonProjectSelected, bool compare)
        {
            try
            {
                IsInitializingPanel = true;

                if (_panelComparisonProjects.listView_comparison_projects.Items.Count > 0)
                {

                    if (comparisonProjectSelected == null)
                    {
                        comparisonProjectSelected = (Settings.ComparisonProject)_panelComparisonProjects.listView_comparison_projects.Items[0].Tag;
                    }
                    else
                    {
                        var foundInList = (from ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items
                                           select (Settings.ComparisonProject)itmx.Tag).Any(comparisonProjectNew => comparisonProjectNew.Id == comparisonProjectSelected.Id);
                        if (!foundInList)
                        {
                            comparisonProjectSelected = (Settings.ComparisonProject)_panelComparisonProjects.listView_comparison_projects.Items[0].Tag;
                        }
                    }

                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {

                        var comparisonProjectNew = (Settings.ComparisonProject)itmx.Tag;

                        if (comparisonProjectNew.Id == comparisonProjectSelected.Id)
                        {
                            var comparisonProjectExisting = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;


                            if (string.Compare(comparisonProjectExisting.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) != 0)
                            {

                                var moveLeftToRight = false;


                                var indexCounter = 0;
                                foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                                {
                                    var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                                    if (string.Compare(tsiFvfp.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        moveLeftToRight = true;
                                        break;
                                    }
                                    indexCounter++;
                                }

                                if (moveLeftToRight)
                                {
                                    #region  |  moveLeftToRight |

                                    indexCounter = 0;
                                    var indexDelete = -1;
                                    foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                                    {
                                        var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                                        if (string.Compare(tsiFvfp.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) == 0)
                                        {
                                            indexDelete = indexCounter;
                                            break;
                                        }
                                        indexCounter++;
                                    }
                                    if (indexDelete > -1)
                                    {
                                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.RemoveAt(indexDelete);


                                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                                        {
                                            var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                                            var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;

                                            var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                                            tsNew.Tag = comparisonProjectPrevious;

                                            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Insert(0, tsNew);


                                        }
                                    }

                                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);

                                    #endregion

                                }
                                else
                                {
                                    #region  |  moveRightToLeft  |

                                    indexCounter = 0;
                                    var indexDelete = -1;
                                    foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems)
                                    {
                                        var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                                        if (string.Compare(tsiFvfp.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) == 0)
                                        {
                                            indexDelete = indexCounter;
                                            break;
                                        }
                                        indexCounter++;
                                    }
                                    if (indexDelete > -1)
                                    {
                                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.RemoveAt(indexDelete);


                                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                                        {
                                            var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                                            var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;


                                            var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Add(name);
                                            tsNew.Tag = comparisonProjectPrevious;

                                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Insert(0, tsNew);
                                        }
                                    }

                                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);


                                    #endregion
                                }


                                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;
                                _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;
                            }
                            itmx.Selected = true;
                        }
                        else
                        {
                            itmx.Selected = false;
                        }

                    }

                }
                if (!compare) return;

                #region  |  ComparisonLogEntry  |


                var cle = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.ComparisonProjectCompare)
                {
                    Date = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    ItemName = comparisonProjectSelected.Name,
                    Value01 = comparisonProjectSelected.PathLeft,
                    Value02 = comparisonProjectSelected.PathRight,
                    Items = null
                };


                update_comparison_log(cle);

                #endregion


                CompareDirectories(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }
        private void LoadComparisonProject()
        {
            try
            {
                IsInitializingPanel = true;

                if (_panelComparisonProjects.listView_comparison_projects.SelectedItems.Count <= 0) return;
                var item = _panelComparisonProjects.listView_comparison_projects.SelectedItems[0];
                var comparisonProjectNew = (Settings.ComparisonProject)item.Tag;

                var comparisonProjectExisting = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;


                if (string.Compare(comparisonProjectExisting.Id, comparisonProjectNew.Id,
                        StringComparison.OrdinalIgnoreCase) == 0 && _panelCompare.listView_main.Items.Count != 0 && string.CompareOrdinal(comparisonProjectExisting.PathLeft,
                        _panelCompare.comboBox_main_compare_left.Text) == 0 && string.CompareOrdinal(comparisonProjectExisting.PathRight,
                        _panelCompare.comboBox_main_compare_right.Text) == 0) return;
                var moveLeftToRight = false;


                var indexCounter = 0;
                foreach (ToolStripItem toolStripItem in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                {
                    var tsiFvfp = (Settings.ComparisonProject)toolStripItem.Tag;

                    if (string.Compare(tsiFvfp.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        moveLeftToRight = true;
                        break;
                    }
                    indexCounter++;
                }

                if (moveLeftToRight)
                {
                    #region  |  moveLeftToRight |

                    indexCounter = 0;
                    var indexDelete = -1;
                    foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                    {
                        var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                        if (string.Compare(tsiFvfp.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            indexDelete = indexCounter;
                            break;
                        }
                        indexCounter++;
                    }
                    if (indexDelete > -1)
                    {
                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.RemoveAt(indexDelete);


                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                        {
                            var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                            var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;

                            var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                            tsNew.Tag = comparisonProjectPrevious;

                            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Insert(0, tsNew);


                        }
                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);

                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {
                        var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                        itmx.Selected = comparisonProject == comparisonProjectNew;
                    }
                    #endregion

                }
                else
                {
                    #region  |  moveRightToLeft  |

                    indexCounter = 0;
                    var indexDelete = -1;
                    foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems)
                    {
                        var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                        if (string.Compare(tsiFvfp.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            indexDelete = indexCounter;
                            break;
                        }
                        indexCounter++;
                    }
                    if (indexDelete > -1)
                    {
                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.RemoveAt(indexDelete);


                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                        {
                            var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                            var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;


                            var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Add(name);
                            tsNew.Tag = comparisonProjectPrevious;

                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Insert(0, tsNew);
                        }
                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);


                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {
                        var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                        itmx.Selected = comparisonProject == comparisonProjectNew;
                    }

                    #endregion
                }




                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;





                #region  |  ComparisonLogEntry  |


                var cle =
                    new Settings.ComparisonLogEntry(
                        Settings.ComparisonLogEntry.EntryType.ComparisonProjectCompare)
                    {
                        Date = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        ItemName = comparisonProjectNew.Name,
                        Value01 = comparisonProjectNew.PathLeft,
                        Value02 = comparisonProjectNew.PathRight,
                        Items = null
                    };





                update_comparison_log(cle);

                #endregion


                CompareDirectories(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }
        private void SaveComparisonProject()
        {
            var f = new ComparisonProject
            {
                IsEdit = false,
                comparisonProject = new Settings.ComparisonProject
                {
                    Name = Resources.FormMain_SaveComparisonProject_New_comparison_project,
                    Created = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    PathLeft = _panelCompare.comboBox_main_compare_left.Text,
                    PathRight = _panelCompare.comboBox_main_compare_right.Text
                }
            };



            var alreadyAddedFolderPair = Application.Settings.ComparisonProjects.Any(comparisonProject =>
                string.Compare(comparisonProject.PathLeft, f.comparisonProject.PathLeft, StringComparison.OrdinalIgnoreCase) == 0
                && string.Compare(comparisonProject.PathRight, f.comparisonProject.PathRight, StringComparison.OrdinalIgnoreCase) == 0);

            var continueAdd = true;
            if (alreadyAddedFolderPair)
            {
                var dr = MessageBox.Show(this, Resources.FormMain_SaveComparisonProject, System.Windows.Forms.Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes)
                    continueAdd = false;
            }

            if (!continueAdd) return;
            {
                f.ShowDialog();
                {
                    if (!f.Saved) return;
                    var foundNameUsed = Application.Settings.ComparisonProjects.Any(comparisonProject =>
                            string.Compare(comparisonProject.Name, f.comparisonProject.Name, StringComparison.OrdinalIgnoreCase) == 0);

                    #region  |  foundNameUsed  |
                    if (foundNameUsed)
                    {
                        var i = 0;
                        while (true)
                        {
                            i++;
                            var newName = f.comparisonProject.Name + " " + i.ToString().PadLeft(2, '0');

                            foundNameUsed = Application.Settings.ComparisonProjects.Any(comparisonProject => string.Compare(comparisonProject.Name, newName, StringComparison.OrdinalIgnoreCase) == 0);
                            if (foundNameUsed) continue;
                            f.comparisonProject.Name = newName;
                            break;
                        }
                    }
                    #endregion



                    Application.Settings.ComparisonProjects.Add(f.comparisonProject);


                    #region  |  ComparisonLogEntry  |


                    var comparisonLogEntry = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.ComparisonProjectNew)
                    {
                        Date = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        ItemName = f.comparisonProject.Name,
                        Value01 = string.Empty,
                        Value02 = string.Empty,
                        Items = null
                    };





                    update_comparison_log(comparisonLogEntry);

                    #endregion




                    try
                    {
                        IsInitializingPanel = true;

                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                        {
                            var name = f.comparisonProject.Name + ": " + f.comparisonProject.PathLeft + " <> " + f.comparisonProject.PathRight;
                            var tsi = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                            tsi.Tag = f.comparisonProject;


                            if (_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0)
                            {
                                var comparisonProjectNew = f.comparisonProject;

                                if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                                {
                                    var comparisonProjectPrevious =
                                        (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                                    name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;


                                    var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Add(name);
                                    tsNew.Tag = comparisonProjectPrevious;

                                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Insert(0, tsNew);

                                }

                                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                                AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);
                            }
                        }
                        else
                        {
                            var name = f.comparisonProject.Name + ": " + f.comparisonProject.PathLeft + " <> " + f.comparisonProject.PathRight;
                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = f.comparisonProject;

                        }


                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;


                        if (_panelComparisonProjects != null)
                        {
                            var itmx = _panelComparisonProjects.listView_comparison_projects.Items.Add(f.comparisonProject.Name);
                            itmx.SubItems.Add(f.comparisonProject.PathLeft + " <> " + f.comparisonProject.PathRight);

                            itmx.Tag = f.comparisonProject;


                            var isPathNotFoundError = false;
                            if (!Directory.Exists(f.comparisonProject.PathLeft))
                            {
                                itmx.SubItems[2].BackColor = Color.Pink;
                                isPathNotFoundError = true;
                                itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_left_directory_path_does_not_exist;
                            }
                            else if (!Directory.Exists(f.comparisonProject.PathLeft))
                            {
                                itmx.SubItems[3].BackColor = Color.Pink;
                                isPathNotFoundError = true;
                                itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_right_directory_path_does_not_exist;
                            }


                            itmx.Tag = f.comparisonProject;


                            itmx.ImageIndex = isPathNotFoundError ? 4 : 3;


                            foreach (ListViewItem item in _panelComparisonProjects.listView_comparison_projects.Items)
                            {
                                if (itmx != item)
                                {
                                    item.Selected = false;
                                }
                            }
                            itmx.Selected = true;
                        }

                        CompareDirectories(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SaveApplicationSettings();
                        IsInitializingPanel = false;
                    }
                }
            }
        }
        private void RemoveComparisonProject()
        {
            if (_panelComparisonProjects.listView_comparison_projects.SelectedItems.Count <= 0) return;
            try
            {
                var dr = MessageBox.Show(this, Resources.FormMain_RemoveComparisonProject, System.Windows.Forms.Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;
                foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.SelectedItems)
                {
                    var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                    foreach (var item in Application.Settings.ComparisonProjects)
                    {
                        if (string.Compare(comparisonProject.Id, item.Id, StringComparison.OrdinalIgnoreCase) != 0 ||
                            string.Compare(comparisonProject.Name, item.Name, StringComparison.OrdinalIgnoreCase) != 0 ||
                            string.Compare(comparisonProject.Created, item.Created, StringComparison.OrdinalIgnoreCase) != 0 ||
                            string.Compare(comparisonProject.PathLeft, item.PathLeft, StringComparison.OrdinalIgnoreCase) != 0 ||
                            string.Compare(comparisonProject.PathRight, item.PathRight, StringComparison.OrdinalIgnoreCase) != 0)
                            continue;

                        #region  |  ComparisonLogEntry  |


                        var comparisonLogEntry =
                            new Settings.ComparisonLogEntry(
                                Settings.ComparisonLogEntry.EntryType.ComparisonProjectRemove)
                            {
                                Date = DateTime.Now,
                                Id = Guid.NewGuid().ToString(),
                                ItemName = item.Name,
                                Value01 = string.Empty,
                                Value02 = string.Empty,
                                Items = null
                            };





                        update_comparison_log(comparisonLogEntry);

                        #endregion

                        Application.Settings.ComparisonProjects.Remove(item);

                        break;
                    }

                    itmx.Remove();





                    if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                    {
                        var comparisonProjectCurrent = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                        if (string.Compare(comparisonProjectCurrent.Id, comparisonProject.Id, StringComparison.OrdinalIgnoreCase) == 0
                            && string.Compare(comparisonProjectCurrent.Name, comparisonProject.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0)
                            {
                                toolStripDropDownButton_comparison_project_left_side_move_Click(null, null);
                            }
                            else if (_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0)
                            {
                                toolStripDropDownButton_comparison_project_right_side_move_Click(null, null);
                            }
                            else
                            {
                                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = null;
                            }
                        }
                    }

                    if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0)
                    {
                        var indexCounter = 0;
                        var indexDelete = -1;
                        foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                        {
                            var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                            if (string.Compare(tsiFvfp.Id, comparisonProject.Id, StringComparison.OrdinalIgnoreCase) == 0
                                && string.Compare(tsiFvfp.Name, comparisonProject.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                indexDelete = indexCounter;
                                break;
                            }
                            indexCounter++;
                        }
                        if (indexDelete > -1)
                        {
                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.RemoveAt(indexDelete);
                        }
                    }
                    if (_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0)
                    {
                        var indexCounter = 0;
                        var indexDelete = -1;
                        foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems)
                        {
                            var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                            if (string.Compare(tsiFvfp.Id, comparisonProject.Id, StringComparison.OrdinalIgnoreCase) == 0
                                && string.Compare(tsiFvfp.Name, comparisonProject.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                indexDelete = indexCounter;
                                break;
                            }
                            indexCounter++;
                        }
                        if (indexDelete > -1)
                        {
                            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.RemoveAt(indexDelete);
                        }
                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                    _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                }
            }
            finally
            {
                SaveApplicationSettings();
            }
        }
        private void NewComparisonProject(Settings.ComparisonProject comparisonProject)
        {
            var f = new ComparisonProject
            {
                IsEdit = false,
                comparisonProject = new Settings.ComparisonProject()
            };

            f.comparisonProject = comparisonProject;

            f.ShowDialog();
            {
                if (!f.Saved) return;
                var foundNameUsed = Application.Settings.ComparisonProjects.Any(project => string.Compare(project.Name, f.comparisonProject.Name, StringComparison.OrdinalIgnoreCase) == 0);

                #region  |  foundNameUsed  |
                if (foundNameUsed)
                {
                    var i = 0;
                    while (true)
                    {
                        i++;
                        var newName = f.comparisonProject.Name + " " + i.ToString().PadLeft(2, '0');

                        foundNameUsed = Application.Settings.ComparisonProjects.Any(project => string.Compare(project.Name, newName, StringComparison.OrdinalIgnoreCase) == 0);
                        if (foundNameUsed) continue;
                        f.comparisonProject.Name = newName;
                        break;
                    }
                }
                #endregion


                var itmx = _panelComparisonProjects.listView_comparison_projects.Items.Add(f.comparisonProject.Name);
                itmx.SubItems.Add(f.comparisonProject.PathLeft + @" <> " + f.comparisonProject.PathRight);

                itmx.Tag = f.comparisonProject;


                var isPathNotFoundError = false;
                if (!Directory.Exists(f.comparisonProject.PathLeft))
                {
                    itmx.SubItems[2].BackColor = Color.Pink;
                    isPathNotFoundError = true;
                    itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_left_directory_path_does_not_exist;
                }
                else if (!Directory.Exists(f.comparisonProject.PathLeft))
                {
                    itmx.SubItems[3].BackColor = Color.Pink;
                    isPathNotFoundError = true;
                    itmx.ToolTipText = Resources.FormMain_SaveComparisonProject_The_right_directory_path_does_not_exist;
                }

                itmx.Tag = f.comparisonProject;


                itmx.ImageIndex = isPathNotFoundError ? 4 : 3;

                Application.Settings.ComparisonProjects.Add(f.comparisonProject);




                #region  |  ComparisonLogEntry  |


                var comparisonLogEntry = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.ComparisonProjectNew)
                {
                    Date = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    ItemName = f.comparisonProject.Name,
                    Value01 = string.Empty,
                    Value02 = string.Empty,
                    Items = null
                };





                update_comparison_log(comparisonLogEntry);

                #endregion



                foreach (ListViewItem item in _panelComparisonProjects.listView_comparison_projects.Items)
                {
                    if (itmx != item)
                    {
                        item.Selected = false;
                    }
                }
                itmx.Selected = true;


                try
                {
                    IsInitializingPanel = true;
                    if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                    {
                        var name = f.comparisonProject.Name + ": " + f.comparisonProject.PathLeft + " <> " + f.comparisonProject.PathRight;
                        var tsi = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                        tsi.Tag = f.comparisonProject;


                        if (_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0)
                        {
                            var comparisonProjectNew = f.comparisonProject;

                            if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                            {
                                var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                                name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;


                                var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Add(name);
                                tsNew.Tag = comparisonProjectPrevious;

                                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Insert(0, tsNew);

                            }

                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                            AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);
                        }
                    }
                    else
                    {
                        var name = f.comparisonProject.Name + ": " + f.comparisonProject.PathLeft + " <> " + f.comparisonProject.PathRight;
                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = f.comparisonProject;

                    }


                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                    _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;
                }
                finally
                {
                    SaveApplicationSettings();
                    IsInitializingPanel = false;
                }
            }
        }

        private void ViewComparisonProjectFileAlignment()
        {
            if (_panelComparisonProjects.listView_comparison_projects.SelectedItems.Count <= 0) return;
            var itmx = _panelComparisonProjects.listView_comparison_projects.SelectedItems[0];
            var comparisonProject = (Settings.ComparisonProject)itmx.Tag;



            var f = new ComparisonProjectFileAlignment
            {
                ComparisonProject = (Settings.ComparisonProject)comparisonProject.Clone()
            };



            f.ShowDialog();

            if (!f.Saved) return;
            comparisonProject.FileAlignment = f.ComparisonProject.FileAlignment;


            foreach (var cp in Application.Settings.ComparisonProjects)
            {
                if (string.Compare(cp.Id, f.ComparisonProject.Id, StringComparison.OrdinalIgnoreCase) != 0 ||
                    string.Compare(cp.PathRight, f.ComparisonProject.PathRight, StringComparison.OrdinalIgnoreCase) != 0) continue;
                cp.FileAlignment = comparisonProject.FileAlignment;
                break;
            }
            SaveApplicationSettings();
        }


        private void listView_comparison_projects_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveComparisonProject();
        }

        private void viewFileAlignmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_comparison_project_file_alignment_Click(sender, e);
        }

        private void removeCompareListItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_comparison_project_remove_Click(sender, e);
        }

        private void editCompareListItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_comparison_project_edit_Click(sender, e);
        }

        private void newCompareListItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_comparison_project_new_Click(sender, e);
        }

        private void loadComparisonProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadComparisonProject();
        }



        private void toolStripButton_comparison_project_load_Click(object sender, EventArgs e)
        {
            LoadComparisonProject();
        }

        private void toolStripButton_comparison_project_remove_Click(object sender, EventArgs e)
        {
            RemoveComparisonProject();
        }


        private void toolStripButton_comparison_project_edit_Click(object sender, EventArgs e)
        {
            EditComparisonProject();
        }


        private void toolStripButton_comparison_project_new_Click(object sender, EventArgs e)
        {
            var comparisonProjectCurrent = new Settings.ComparisonProject
            {
                Name = Resources.FormMain_SaveComparisonProject_New_comparison_project,
                PathLeft = _panelCompare.comboBox_main_compare_left.Text,
                PathRight = _panelCompare.comboBox_main_compare_right.Text
            };
            NewComparisonProject(comparisonProjectCurrent);
        }

        private void toolStripButton_comparison_project_settings_Click(object sender, EventArgs e)
        {
            LoadSettings(3, -1, -1);
        }

        private void toolStripButton_comparison_project_save_Click(object sender, EventArgs e)
        {
            SaveComparisonProject();
        }


        private void toolStripButton_comparison_project_file_alignment_Click(object sender, EventArgs e)
        {
            ViewComparisonProjectFileAlignment();
        }

        private void listView_comparison_projects_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (_panelComparisonProjects.listView_comparison_projects.SelectedItems.Count > 0)
            {
                var comparisonProject = (Settings.ComparisonProject)_panelComparisonProjects.listView_comparison_projects.SelectedItems[0].Tag;
                _panelComparisonProjects.toolStripStatusLabel_created.Text = comparisonProject.Created;

                _panelComparisonProjects.toolStripButton_comparison_project_load.Enabled = true;
                _panelComparisonProjects.toolStripButton_comparison_project_new.Enabled = true;
                _panelComparisonProjects.toolStripButton_comparison_project_save.Enabled = _panelCompare.listView_main.Items.Count > 0 ? true : false;
                _panelComparisonProjects.toolStripButton_comparison_project_edit.Enabled = true;
                _panelComparisonProjects.toolStripButton_comparison_project_remove.Enabled = true;
                _panelComparisonProjects.toolStripButton_comparison_project_file_alignment.Enabled = true;



                _panelComparisonProjects.loadComparisonProjectToolStripMenuItem.Enabled = true;
                _panelComparisonProjects.newCompareListItemToolStripMenuItem.Enabled = true;
                _panelComparisonProjects.editCompareListItemToolStripMenuItem.Enabled = true;
                _panelComparisonProjects.removeCompareListItemToolStripMenuItem.Enabled = true;
                _panelComparisonProjects.viewFileAlignmentToolStripMenuItem.Enabled = true;

            }
            else
            {
                _panelComparisonProjects.toolStripStatusLabel_created.Text = "...";

                _panelComparisonProjects.toolStripButton_comparison_project_load.Enabled = false;
                _panelComparisonProjects.toolStripButton_comparison_project_new.Enabled = true;
                _panelComparisonProjects.toolStripButton_comparison_project_save.Enabled = _panelCompare.listView_main.Items.Count > 0 ? true : false;
                _panelComparisonProjects.toolStripButton_comparison_project_edit.Enabled = false;
                _panelComparisonProjects.toolStripButton_comparison_project_remove.Enabled = false;
                _panelComparisonProjects.toolStripButton_comparison_project_file_alignment.Enabled = false;



                _panelComparisonProjects.loadComparisonProjectToolStripMenuItem.Enabled = false;
                _panelComparisonProjects.newCompareListItemToolStripMenuItem.Enabled = true;
                _panelComparisonProjects.editCompareListItemToolStripMenuItem.Enabled = false;
                _panelComparisonProjects.removeCompareListItemToolStripMenuItem.Enabled = false;
                _panelComparisonProjects.viewFileAlignmentToolStripMenuItem.Enabled = false;
            }
        }

        private void listView_comparison_projects_DoubleClick(object sender, EventArgs e)
        {
            LoadComparisonProject();
        }




        private void toolStripDropDownButton_comparison_project_left_side_move_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                IsInitializingPanel = true;

                if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0)
                {

                    var comparisonProjectNew = (Settings.ComparisonProject)e.ClickedItem.Tag;

                    var indexCounter = 0;
                    var indexDelete = -1;
                    foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems)
                    {
                        var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                        if (string.Compare(tsiFvfp.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) == 0
                             && string.Compare(tsiFvfp.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            indexDelete = indexCounter;
                            break;
                        }
                        indexCounter++;
                    }
                    if (indexDelete > -1)
                    {
                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.RemoveAt(indexDelete);


                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                        {
                            var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                            var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;

                            var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                            tsNew.Tag = comparisonProjectPrevious;

                            _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Insert(0, tsNew);


                        }
                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);

                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {

                        var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                        if (comparisonProject.Id == comparisonProjectNew.Id && string.Compare(comparisonProject.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            itmx.Selected = true;
                        }
                        else
                        {
                            itmx.Selected = false;
                        }
                    }
                }



                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                CompareMe();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }
        private void toolStripDropDownButton_comparison_project_right_side_move_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                IsInitializingPanel = true;

                if (_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0)
                {

                    var comparisonProjectNew = (Settings.ComparisonProject)e.ClickedItem.Tag;

                    var indexCounter = 0;
                    var indexDelete = -1;
                    foreach (ToolStripItem tsi in _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems)
                    {
                        var tsiFvfp = (Settings.ComparisonProject)tsi.Tag;

                        if (string.Compare(tsiFvfp.Id, comparisonProjectNew.Id, StringComparison.OrdinalIgnoreCase) == 0
                            && string.Compare(tsiFvfp.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            indexDelete = indexCounter;
                            break;
                        }
                        indexCounter++;
                    }
                    if (indexDelete > -1)
                    {
                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.RemoveAt(indexDelete);


                        if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                        {
                            var comparisonProjectPrevious =
                                (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                            var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;


                            var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Add(name);
                            tsNew.Tag = comparisonProjectPrevious;

                            _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Insert(0, tsNew);
                        }
                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);


                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {

                        var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                        if (comparisonProject.Id == comparisonProjectNew.Id
                             && string.Compare(comparisonProject.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            itmx.Selected = true;
                        }
                        else
                        {
                            itmx.Selected = false;
                        }
                    }
                }



                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;

                _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                CompareMe();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }

        private void toolStripDropDownButton_comparison_project_left_side_move_Click(object sender, EventArgs e)
        {

            try
            {
                IsInitializingPanel = true;

                if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0)
                {

                    var comparisonProjectNew = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems[0].Tag;

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.RemoveAt(0);


                    if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                    {
                        var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                        var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;

                        var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Add(name);
                        tsNew.Tag = comparisonProjectPrevious;

                        _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Insert(0, tsNew);

                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);

                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {

                        var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                        if (comparisonProject.Id == comparisonProjectNew.Id
                            && string.Compare(comparisonProject.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            itmx.Selected = true;
                        }
                        else
                        {
                            itmx.Selected = false;
                        }
                    }
                }

                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;
                _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                CompareMe();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }



        }
        private void toolStripDropDownButton_comparison_project_right_side_move_Click(object sender, EventArgs e)
        {
            try
            {
                IsInitializingPanel = true;

                if (_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0)
                {

                    var comparisonProjectNew = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems[0].Tag;

                    _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.RemoveAt(0);


                    if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
                    {
                        var comparisonProjectPrevious = (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;

                        var name = comparisonProjectPrevious.Name + ": " + comparisonProjectPrevious.PathLeft + " <> " + comparisonProjectPrevious.PathRight;


                        var tsNew = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Add(name);
                        tsNew.Tag = comparisonProjectPrevious;

                        _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Insert(0, tsNew);

                    }

                    _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag = comparisonProjectNew;

                    AddComparisonEntriesToComboboxes(comparisonProjectNew.PathLeft, comparisonProjectNew.PathRight);


                    foreach (ListViewItem itmx in _panelComparisonProjects.listView_comparison_projects.Items)
                    {

                        var comparisonProject = (Settings.ComparisonProject)itmx.Tag;

                        if (comparisonProject.Id == comparisonProjectNew.Id
                             && string.Compare(comparisonProject.Name, comparisonProjectNew.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            itmx.Selected = true;
                        }
                        else
                        {
                            itmx.Selected = false;
                        }
                    }
                }

                _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_left_side_move.DropDownItems.Count > 0;
                _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.Enabled = _panelCompare.toolStripDropDownButton_comparison_project_right_side_move.DropDownItems.Count > 0;

                CompareMe();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }


        #endregion
        #region  |  comparsion folders  |


        private static void listView_main_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void listView_main_DragDrop(object sender, DragEventArgs e)
        {
            try
            {

                Cursor = Cursors.WaitCursor;


                IsInitializingPanel = true;

                var lv = _panelCompare.listView_main;
                var lvi = lv.GetItemAt(e.X, e.Y);
                var halfway = lv.Width / 2 + Left;

                var objectList = (string[])e.Data.GetData(DataFormats.FileDrop, false);


                if (objectList.Count() == 2
                    && Directory.Exists(objectList[0]) && Directory.Exists(objectList[1]))
                {
                    #region  |  folders  |

                    if (string.Compare(objectList[0], _panelCompare.comboBox_main_compare_left.Text, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
                        _panelCompare.comboBox_main_compare_left.BackColor = Color.Yellow;
                    }

                    if (string.Compare(objectList[1], _panelCompare.comboBox_main_compare_right.Text, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        _panelCompare.toolStripButton_loadRightSide.Enabled = true;
                        _panelCompare.comboBox_main_compare_right.BackColor = Color.Yellow;
                    }

                    _panelCompare.comboBox_main_compare_left.Text = objectList[0];
                    _panelCompare.comboBox_main_compare_right.Text = objectList[1];




                    IsInitializingPanel = false;
                    #endregion
                }
                else if (Directory.Exists(objectList[0]))
                {
                    #region  |  folder  |
                    if (e.X <= halfway)
                    {
                        if (string.Compare(objectList[0], _panelCompare.comboBox_main_compare_left.Text, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
                            _panelCompare.comboBox_main_compare_left.BackColor = Color.Yellow;
                        }

                        _panelCompare.comboBox_main_compare_left.Text = objectList[0];
                    }
                    else
                    {
                        if (string.Compare(objectList[0], _panelCompare.comboBox_main_compare_right.Text, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            _panelCompare.toolStripButton_loadRightSide.Enabled = true;
                            _panelCompare.comboBox_main_compare_right.BackColor = Color.Yellow;
                        }

                        _panelCompare.comboBox_main_compare_right.Text = objectList[0];
                    }

                    IsInitializingPanel = false;
                    #endregion
                }
                else
                {
                    #region  |  files  |

                    if (objectList.Count() != 1 || !File.Exists(objectList[0])) return;

                    #region  |  any file  |


                    if (e.X <= halfway)
                    {
                        if (string.Compare(objectList[0], _panelCompare.comboBox_main_compare_left.Text, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
                            _panelCompare.comboBox_main_compare_left.BackColor = Color.Yellow;
                        }

                        _panelCompare.comboBox_main_compare_left.Text = objectList[0];
                    }
                    else
                    {
                        if (string.Compare(objectList[0], _panelCompare.comboBox_main_compare_right.Text, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            _panelCompare.toolStripButton_loadRightSide.Enabled = true;
                            _panelCompare.comboBox_main_compare_right.BackColor = Color.Yellow;
                        }

                        _panelCompare.comboBox_main_compare_right.Text = objectList[0];
                    }

                    IsInitializingPanel = false;

                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                IsInitializingPanel = false;
                SaveApplicationSettings();
                Cursor = Cursors.Default;
            }
        }



        private void BrowseFolder(bool leftSide)
        {
            try
            {
                IsInitializingPanel = true;
                var sPath = leftSide ? _panelCompare.comboBox_main_compare_left.Text : _panelCompare.comboBox_main_compare_right.Text;

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
                    Title = string.Format(Resources.FormMain_BrowseFolder_Select_base_folder_0_side, leftSide ? Resources.FormMain_BrowseFolder_left : Resources.FormMain_BrowseFolder_right),
                    InitialDirectory = sPath
                };
                if (!fsd.ShowDialog(IntPtr.Zero)) return;
                if (fsd.FileName.Trim() == string.Empty) return;
                sPath = fsd.FileName;

                if (leftSide)
                {
                    _panelCompare.comboBox_main_compare_left.Text = sPath;
                    _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
                    _panelCompare.comboBox_main_compare_left.BackColor = Color.Yellow;
                }
                else
                {
                    _panelCompare.comboBox_main_compare_right.Text = sPath;
                    _panelCompare.toolStripButton_loadRightSide.Enabled = true;
                    _panelCompare.comboBox_main_compare_right.BackColor = Color.Yellow;
                }
            }
            finally
            {
                IsInitializingPanel = false;
            }


        }


        public void FolderUpOnLevel(bool leftSide)
        {
            var path = leftSide ? _panelCompare.comboBox_main_compare_left.Text : _panelCompare.comboBox_main_compare_right.Text;

            var isDirectory = false;
            if (path.Trim() == string.Empty) return;
            if (Directory.Exists(path))
            {
                isDirectory = true;
            }

            if (!isDirectory)
            {
                if (File.Exists(path))
                {
                    isDirectory = true;
                    path = Path.GetDirectoryName(path);
                }
            }

            if (!isDirectory) return;
            try
            {
                IsInitializingPanel = true;
                var pathName = Path.GetDirectoryName(path);
                if (Directory.Exists(pathName))
                {
                    if (leftSide)
                        _panelCompare.comboBox_main_compare_left.Text = pathName;
                    else
                        _panelCompare.comboBox_main_compare_right.Text = pathName;
                }
                IsInitializingPanel = false;



                CompareMe();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }
        public void BothFoldersUpOnLevel()
        {
            if (!Directory.Exists(_panelCompare.comboBox_main_compare_left.Text) ||
                !Directory.Exists(_panelCompare.comboBox_main_compare_right.Text)) return;
            try
            {
                IsInitializingPanel = true;
                var pathNameLeft = Path.GetDirectoryName(_panelCompare.comboBox_main_compare_left.Text);
                var pathNameRight = Path.GetDirectoryName(_panelCompare.comboBox_main_compare_right.Text);
                if (Directory.Exists(pathNameLeft) && Directory.Exists(pathNameRight))
                {

                    _panelCompare.comboBox_main_compare_left.Text = pathNameLeft;

                    _panelCompare.comboBox_main_compare_right.Text = pathNameRight;
                }
                IsInitializingPanel = false;



                CompareMe();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }
        }

        private void listView_main_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = _panelCompare.listView_main.Columns[e.ColumnIndex].Width;
        }

        private void comboBox_main_compare_left_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitializingPanel) return;
            _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
            _panelCompare.comboBox_main_compare_left.BackColor = Color.Yellow;


            CompareMe();
        }
        private void comboBox_main_compare_right_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitializingPanel) return;
            _panelCompare.toolStripButton_loadRightSide.Enabled = true;
            _panelCompare.comboBox_main_compare_right.BackColor = Color.Yellow;


            CompareMe();
        }

        private void comboBox_main_compare_left_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return) return;
            _panelCompare.toolStripButton_loadLeftSide.Enabled = true;
            _panelCompare.comboBox_main_compare_left.BackColor = Color.Yellow;



            CompareMe();
        }
        private void comboBox_main_compare_right_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return) return;
            _panelCompare.toolStripButton_loadRightSide.Enabled = true;
            _panelCompare.comboBox_main_compare_right.BackColor = Color.Yellow;


            CompareMe();
        }

        private void comboBox_main_compare_left_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializingPanel) return;
            _panelCompare.toolStripButton_loadLeftSide.Enabled = true;

            _panelCompare.comboBox_main_compare_left.BackColor = Directory.Exists(_panelCompare.comboBox_main_compare_left.Text) ? Color.Yellow : Color.Pink;

        }
        private void comboBox_main_compare_right_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializingPanel) return;
            _panelCompare.toolStripButton_loadRightSide.Enabled = true;

            _panelCompare.comboBox_main_compare_right.BackColor = Directory.Exists(_panelCompare.comboBox_main_compare_right.Text) ? Color.Yellow : Color.Pink;
        }


        private void AddComparisonEntriesToComboboxes(string pathLeft, string pathRight)
        {

            var isInitializingPanelTmp = IsInitializingPanel;
            try
            {
                IsInitializingPanel = true;

                var indexLeft = 0;
                foreach (string item in _panelCompare.comboBox_main_compare_left.Items)
                {
                    if (string.Compare(item, pathLeft, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _panelCompare.comboBox_main_compare_left.Items.RemoveAt(indexLeft);
                        break;
                    }
                    indexLeft++;
                }

                _panelCompare.comboBox_main_compare_left.Items.Insert(0, pathLeft);
                _panelCompare.comboBox_main_compare_left.SelectedIndex = 0;


                if (_panelCompare.comboBox_main_compare_left.Items.Count > Application.Settings.FolderViewerFoldersMaxEntries)
                {
                    while (_panelCompare.comboBox_main_compare_left.Items.Count > Application.Settings.FolderViewerFoldersMaxEntries)
                    {
                        _panelCompare.comboBox_main_compare_left.Items.RemoveAt(_panelCompare.comboBox_main_compare_left.Items.Count - 1);
                    }
                }

                Application.Settings.FolderViewerFoldersLeft.Clear();
                foreach (string folder in _panelCompare.comboBox_main_compare_left.Items)
                {
                    Application.Settings.FolderViewerFoldersLeft.Add(folder);
                }




                var indexRight = 0;
                foreach (string item in _panelCompare.comboBox_main_compare_right.Items)
                {
                    if (string.Compare(item, pathRight, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _panelCompare.comboBox_main_compare_right.Items.RemoveAt(indexRight);
                        break;
                    }
                    indexRight++;
                }



                _panelCompare.comboBox_main_compare_right.Items.Insert(0, pathRight);
                _panelCompare.comboBox_main_compare_right.SelectedIndex = 0;

                if (_panelCompare.comboBox_main_compare_right.Items.Count > Application.Settings.FolderViewerFoldersMaxEntries)
                {
                    while (_panelCompare.comboBox_main_compare_right.Items.Count > Application.Settings.FolderViewerFoldersMaxEntries)
                    {
                        _panelCompare.comboBox_main_compare_right.Items.RemoveAt(_panelCompare.comboBox_main_compare_right.Items.Count - 1);
                    }
                }


                Application.Settings.FolderViewerFoldersRight.Clear();
                foreach (string folder in _panelCompare.comboBox_main_compare_right.Items)
                {
                    Application.Settings.FolderViewerFoldersRight.Add(folder);
                }



            }
            finally
            {
                IsInitializingPanel = isInitializingPanelTmp;
            }
        }



        private void compareFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lv = _panelCompare.listView_main;
            if (lv.SelectedIndices.Count < 1 || lv.SelectedIndices.Count > 2) return;
            var lvi01 = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];
            var dn01 = (DataNode)lvi01.Tag;


            if (lv.SelectedIndices.Count == 1)
            {
                if (dn01.SelectionType != DataNode.Selection.Middle) return;
                if (dn01.PathLeft.Trim() == string.Empty || dn01.PathRight.Trim() == string.Empty) return;
                _panelCompare.comboBox_main_compare_left.Text = dn01.PathLeft;
                _panelCompare.comboBox_main_compare_right.Text = dn01.PathRight;


                CompareMe();
            }
            else
            {

                var lvi02 = _panelCompare.listView_main.Items[lv.SelectedIndices[1]];
                var dn02 = (DataNode)lvi02.Tag;

                if ((dn01.SelectionType == DataNode.Selection.Left || dn01.SelectionType == DataNode.Selection.Middle)
                    && (dn02.SelectionType == DataNode.Selection.Right || dn02.SelectionType == DataNode.Selection.Middle)
                )
                {
                    if (dn01.PathLeft.Trim() != string.Empty && dn02.PathRight.Trim() != string.Empty)
                    {
                        _panelCompare.comboBox_main_compare_left.Text = dn01.PathLeft;
                        _panelCompare.comboBox_main_compare_right.Text = dn02.PathRight;
                    }
                    else if (dn02.PathLeft.Trim() != string.Empty && dn01.PathRight.Trim() != string.Empty)
                    {
                        _panelCompare.comboBox_main_compare_left.Text = dn02.PathLeft;
                        _panelCompare.comboBox_main_compare_right.Text = dn01.PathRight;
                    }
                }
                if ((dn02.SelectionType == DataNode.Selection.Left || dn02.SelectionType == DataNode.Selection.Middle)
                    && (dn01.SelectionType == DataNode.Selection.Right || dn01.SelectionType == DataNode.Selection.Middle)
                )
                {
                    if (dn02.PathLeft.Trim() != string.Empty && dn01.PathRight.Trim() != string.Empty)
                    {
                        _panelCompare.comboBox_main_compare_left.Text = dn02.PathLeft;
                        _panelCompare.comboBox_main_compare_right.Text = dn01.PathRight;
                    }
                    else if (dn01.PathLeft.Trim() != string.Empty && dn02.PathRight.Trim() != string.Empty)
                    {
                        _panelCompare.comboBox_main_compare_left.Text = dn01.PathLeft;
                        _panelCompare.comboBox_main_compare_right.Text = dn02.PathRight;
                    }
                }
                CompareMe();

            }
        }



        private void toolStripButton_loadLeftSide_Click(object sender, EventArgs e)
        {


            CompareMe();
        }

        private void toolStripButton_loadRightSide_Click(object sender, EventArgs e)
        {
            CompareMe();
        }

        private void toolStripButton_browseFolderLeftSide_Click(object sender, EventArgs e)
        {
            BrowseFolder(true);
        }

        private void toolStripButton_browseFolderRightSide_Click(object sender, EventArgs e)
        {
            BrowseFolder(false);
        }

        private void toolStripButton_upOneLevelLeftSide_Click(object sender, EventArgs e)
        {
            FolderUpOnLevel(true);
        }

        private void toolStripButton_upOneLevelRightSide_Click(object sender, EventArgs e)
        {
            FolderUpOnLevel(false);
        }

        private void toolStripButton_bothFoldersUpOneLevel_Click(object sender, EventArgs e)
        {
            BothFoldersUpOnLevel();
        }



        private void SetBaseFolder(int index)
        {
            try
            {
                var lv = _panelCompare.listView_main;

                var lvi = lv.GetItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);

                var dn = (DataNode)lvi.Tag;

                IsInitializingPanel = true;
                if (index < 4)
                {
                    if (dn.PathLeft.Trim() != string.Empty)
                    {
                        _panelCompare.comboBox_main_compare_left.Text = dn.Type == DataNode.ItemType.File ? Path.GetDirectoryName(dn.PathLeft) : dn.PathLeft;
                    }
                }
                else if (index == 4)
                {
                    if (dn.PathLeft.Trim() != string.Empty)
                    {
                        _panelCompare.comboBox_main_compare_left.Text = dn.Type == DataNode.ItemType.File ? Path.GetDirectoryName(dn.PathLeft) : dn.PathLeft;
                    }

                    if (dn.PathRight.Trim() != string.Empty)
                        _panelCompare.comboBox_main_compare_right.Text = dn.Type == DataNode.ItemType.File ? Path.GetDirectoryName(dn.PathRight) : dn.PathRight;
                }
                else
                {
                    if (dn.PathRight.Trim() != string.Empty)
                        _panelCompare.comboBox_main_compare_right.Text = dn.Type == DataNode.ItemType.File ? Path.GetDirectoryName(dn.PathRight) : dn.PathRight;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializingPanel = false;
            }


            CompareMe();
        }

        private void setAsBasefolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var lv = _panelCompare.listView_main;

                var lvi = lv.GetItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);
                var subItem = lvi.GetSubItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);


                #region  |  get SelectionColumn  |
                var iColumnIndex = -1;
                for (var i = 0; i < lvi.SubItems.Count; i++)
                {
                    if (lvi.SubItems[i] != subItem) continue;
                    iColumnIndex = i;
                    break;
                }

                #endregion


                SetBaseFolder(iColumnIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void toolStripButton_setBaseFolder_rightSide_Click(object sender, EventArgs e)
        {
            SetBaseFolder(5);
        }

        private void toolStripButton_setBaseFolder_leftSide_Click(object sender, EventArgs e)
        {
            SetBaseFolder(0);
        }



        #endregion
        #region  |  file copy/move/delete  |

        private void SetFolderState(DataNode m, bool rightSide)
        {

            var regexPathChar = new Regex(@"\\", RegexOptions.Singleline | RegexOptions.IgnoreCase);



            var parts = regexPathChar.Split(rightSide ? m.PathRight.TrimEnd('\\') : m.PathLeft.TrimEnd('\\')).ToList();


            var levelMinus = 0;
            for (var i = parts.Count; i >= 0; i--)
            {
                if (i < parts.Count)
                {
                    var path = string.Empty;
                    for (var x = 0; x <= i; x++)
                    {
                        path += (path.Trim() != string.Empty ? "\\" : string.Empty) + parts[x];
                    }

                    m = GetDataNodeFolder(m.Level - levelMinus, path, rightSide);
                    levelMinus++;
                }

                if (m == null)
                {
                    break;
                }

                #region  |  set the folder compare state  |

                m.CompareState = DataNode.CompareStates.None;

                if (m.Children.Count == 0)
                {
                    if (m.NameLeft.Trim() != string.Empty && m.NameRight.Trim() != string.Empty)
                        m.CompareState = DataNode.CompareStates.Equal;
                    else if (m.NameLeft.Trim() == string.Empty && m.NameRight.Trim() != string.Empty)
                        m.CompareState = DataNode.CompareStates.OrphansRightside;
                    else if (m.NameLeft.Trim() != string.Empty && m.NameRight.Trim() == string.Empty)
                        m.CompareState = DataNode.CompareStates.OrphansLeftside;
                }
                else
                {
                    foreach (var dn in m.Children)
                    {
                        if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                        {
                            if ((m.CompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch
                                && (m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside
                                && (m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside
                                && (m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside
                                && (m.CompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                            {
                                m.CompareState = m.CompareState | DataNode.CompareStates.Equal;
                            }
                        }
                        {
                            if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                            {
                                if ((m.CompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch)
                                    m.CompareState = m.CompareState | DataNode.CompareStates.Mismatch;

                                if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    m.CompareState &= ~DataNode.CompareStates.Equal;
                            }
                            if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                            {
                                if ((m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside)
                                    m.CompareState = m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;

                                if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    m.CompareState &= ~DataNode.CompareStates.Equal;
                            }
                            if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                            {
                                if ((m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside)
                                    m.CompareState = m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;

                                if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    m.CompareState &= ~DataNode.CompareStates.Equal;
                            }
                            if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                            {
                                if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                                    m.CompareState = m.CompareState | DataNode.CompareStates.OrphansLeftside;

                                if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    m.CompareState &= ~DataNode.CompareStates.Equal;
                            }
                            if ((dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                            {
                                if ((m.CompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                                    m.CompareState = m.CompareState | DataNode.CompareStates.OrphansRightside;

                                if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    m.CompareState &= ~DataNode.CompareStates.Equal;
                            }
                        }
                    }
                }
                #endregion

            }


        }
        //copy
        private void CopySelectedTo(bool fromLeftToRight)
        {
            if (_panelCompare.listView_main.SelectedIndices.Count <= 0) return;
            var f = new CopyFiles { checkBox_overwirte_existing_files = { Checked = false } };




            try
            {
                var iFilesLeft = 0;
                var iFoldersLeft = 0;
                long iSizeLeft = 0;

                var iFilesRight = 0;
                var iFoldersRight = 0;
                long iSizeRight = 0;

                for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]];
                    var dn = (DataNode)lvi.Tag;

                    #region   |  if (dn.Type == DataNode.ItemType.file)  |



                    switch (dn.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iSizeLeft += dn.SizeLeft;
                            }
                            else
                            {
                                iFoldersLeft++;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesRight++;

                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersRight++;
                            }
                            break;
                        case DataNode.Selection.Middle:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iFilesRight++;

                                iSizeLeft += dn.SizeLeft;
                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersLeft++;
                                iFoldersRight++;
                            }
                            break;
                    }




                    #endregion
                }

                f.label_fileCount_left.Text = iFilesLeft.ToString();
                f.label_folderCount_left.Text = iFoldersLeft.ToString();
                f.label_size_left.Text = CalculateFileSize(iSizeLeft);

                f.label_fileCount_right.Text = iFilesRight.ToString();
                f.label_folderCount_right.Text = iFoldersRight.ToString();
                f.label_size_right.Text = CalculateFileSize(iSizeRight);

                f.ShowDialog();
                if (!f.Saved) return;
                {
                    var overwrite = f.checkBox_overwirte_existing_files.Checked;
                    fromLeftToRight = f.radioButton_left_side.Checked;

                    var items = new List<Settings.ComparisonLogEntryItem>();


                    var filesCopied = 0;
                    var regexPathChar = new Regex(@"\\", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    foreach (int i in _panelCompare.listView_main.SelectedIndices)
                    {
                        var mbr = _mMapper[i];

                        if (fromLeftToRight)
                        {
                            if (mbr.Type != DataNode.ItemType.File) continue;
                            if (mbr.PathLeft.Trim() == string.Empty) continue;
                            if (mbr.PathRight.Trim() == string.Empty)
                            {
                                #region  |  get path info  |

                                var parentDirectoryRight = GetParentDirectory(mbr, fromLeftToRight);
                                var parentDirectoryRightMixed = parentDirectoryRight.Replace(_panelCompare.comboBox_main_compare_right.Text.Trim(), string.Empty);

                                var parentDirectoryLeft = GetParentDirectory(mbr, !fromLeftToRight);
                                var parentDirectoryLeftMixed = parentDirectoryLeft.Replace(_panelCompare.comboBox_main_compare_left.Text.Trim(), string.Empty);


                                var parentDirectoryMixed = parentDirectoryRightMixed.Trim() != string.Empty ? parentDirectoryLeftMixed.Replace(parentDirectoryRightMixed, string.Empty) : parentDirectoryLeftMixed;


                                if (parentDirectoryMixed.Trim() != string.Empty)
                                {
                                    if (parentDirectoryMixed.Trim('\\') != string.Empty)
                                    {

                                        parentDirectoryMixed = parentDirectoryMixed.Trim();
                                        parentDirectoryMixed = parentDirectoryMixed.TrimStart('\\');


                                        parentDirectoryRight = parentDirectoryRight.Trim();
                                        parentDirectoryRight = parentDirectoryRight.TrimEnd('\\');

                                        parentDirectoryRight = parentDirectoryRight + "\\" + parentDirectoryMixed;
                                    }
                                }

                                if (parentDirectoryRight.Trim() != string.Empty)
                                {
                                    mbr.PathRight = Path.Combine(parentDirectoryRight, Path.GetFileName(mbr.NameLeft));
                                }
                                #endregion

                                #region  |  update folder path info  |

                                if (parentDirectoryMixed.Trim() != string.Empty)
                                {

                                    var parentDirectoryMixedBaseLeft = parentDirectoryLeft.Replace(parentDirectoryMixed, string.Empty);
                                    parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.Trim();
                                    parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimStart('\\');
                                    parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimEnd('\\');


                                    var parentDirectoryMixedBaseRight = parentDirectoryRight.Replace(parentDirectoryMixed, string.Empty);
                                    parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.Trim();
                                    parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimStart('\\');
                                    parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimEnd('\\');


                                    var paths = regexPathChar.Split(parentDirectoryMixed).ToList();

                                    var levelStart = mbr.Level - paths.Count;
                                    var levelEnd = mbr.Level;
                                    var levelCurrent = 0;
                                    var pathIndex = 0;

                                    var startCheck = false;

                                    foreach (var t in _mMapper)
                                    {
                                        if (t.Level == levelStart)
                                        {
                                            levelCurrent = t.Level;
                                            startCheck = true;
                                        }


                                        if (!startCheck) continue;
                                        if (t.Level == levelEnd)
                                        {
                                            if (t.Type == DataNode.ItemType.Folder)
                                            {
                                                if (string.Compare(t.PathLeft, Path.Combine(parentDirectoryMixedBaseLeft, parentDirectoryMixed), StringComparison.OrdinalIgnoreCase) == 0)
                                                    break;
                                            }
                                        }


                                        if (levelCurrent != t.Level) continue;
                                        var pathTemp = string.Empty;
                                        for (var x = 0; x <= pathIndex; x++)
                                            pathTemp += (pathTemp.Trim() != string.Empty ? "\\" : string.Empty) + paths[x];

                                        var testPathLeft = Path.Combine(parentDirectoryMixedBaseLeft, pathTemp);
                                        var testPathRight = Path.Combine(parentDirectoryMixedBaseRight, pathTemp);

                                        if (string.Compare(t.PathLeft, testPathLeft,
                                                StringComparison.OrdinalIgnoreCase) != 0) continue;
                                        if (t.PathRight.Trim() == string.Empty)
                                        {
                                            t.PathRight = testPathRight;
                                            t.NameRight = new DirectoryInfo(t.PathRight).Name;


                                            var folders = 0;
                                            var files = 0;
                                            CalculateFoldersSize(t, ref folders, ref files);
                                        }

                                        levelCurrent++;
                                        if (pathIndex + 1 < paths.Count)
                                            pathIndex++;
                                    }
                                }


                                #endregion
                            }

                            if (!Directory.Exists(Path.GetDirectoryName(mbr.PathRight)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(mbr.PathRight));
                            }


                            filesCopied++;
                            if (!overwrite && File.Exists(mbr.PathRight))
                            {
                                //message
                            }
                            else
                            {
                                items.Add(new Settings.ComparisonLogEntryItem(File.Exists(mbr.PathRight).ToString(), mbr.PathLeft, mbr.PathRight));

                                File.Copy(mbr.PathLeft, mbr.PathRight, overwrite);
                                mbr.NameRight = mbr.NameLeft;
                                mbr.SizeRight = mbr.SizeLeft;
                                mbr.ModifiedRight = mbr.ModifiedLeft;


                                mbr.CompareState = DataNode.CompareStates.Equal;

                                _panelCompare.listView_main.RedrawItems(i, i, false);

                                var dnFolder = GetDataNodeFolder(mbr.Level - 1, Path.GetDirectoryName(mbr.PathRight), fromLeftToRight);
                                SetFolderState(dnFolder, true);
                            }
                        }
                        else
                        {
                            if (mbr.Type != DataNode.ItemType.File) continue;
                            if (mbr.PathRight.Trim() == string.Empty) continue;
                            if (mbr.PathLeft.Trim() == string.Empty)
                            {
                                #region  |  get path info  |

                                var parentDirectoryLeft = GetParentDirectory(mbr, fromLeftToRight);
                                var parentDirectoryLeftMixed = parentDirectoryLeft.Replace(_panelCompare.comboBox_main_compare_left.Text.Trim(), string.Empty);

                                var parentDirectoryRight = GetParentDirectory(mbr, fromLeftToRight ? false : true);
                                var parentDirectoryRightMixed = parentDirectoryRight.Replace(_panelCompare.comboBox_main_compare_right.Text.Trim(), string.Empty);


                                var parentDirectoryMixed = parentDirectoryLeftMixed.Trim() != string.Empty ? parentDirectoryRightMixed.Replace(parentDirectoryLeftMixed, string.Empty) : parentDirectoryRightMixed;


                                if (parentDirectoryMixed.Trim() != string.Empty)
                                {
                                    if (parentDirectoryMixed.Trim('\\') != string.Empty)
                                    {

                                        parentDirectoryMixed = parentDirectoryMixed.Trim();
                                        parentDirectoryMixed = parentDirectoryMixed.TrimStart('\\');


                                        parentDirectoryLeft = parentDirectoryLeft.Trim();
                                        parentDirectoryLeft = parentDirectoryLeft.TrimEnd('\\');

                                        parentDirectoryLeft = parentDirectoryLeft + "\\" + parentDirectoryMixed;
                                    }
                                }

                                if (parentDirectoryLeft.Trim() != string.Empty)
                                {
                                    mbr.PathLeft = Path.Combine(parentDirectoryLeft, Path.GetFileName(mbr.NameRight));
                                }
                                #endregion

                                #region  |  update folder path info  |

                                if (parentDirectoryMixed.Trim() != string.Empty)
                                {

                                    var parentDirectoryMixedBaseRight = parentDirectoryRight.Replace(parentDirectoryMixed, string.Empty);
                                    parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.Trim();
                                    parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimStart('\\');
                                    parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimEnd('\\');


                                    var parentDirectoryMixedBaseLeft = parentDirectoryLeft.Replace(parentDirectoryMixed, string.Empty);
                                    parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.Trim();
                                    parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimStart('\\');
                                    parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimEnd('\\');


                                    var paths = regexPathChar.Split(parentDirectoryMixed).ToList();

                                    var levelStart = mbr.Level - paths.Count;
                                    var levelEnd = mbr.Level;
                                    var levelCurrent = 0;
                                    var pathIndex = 0;

                                    var startCheck = false;

                                    foreach (var t in _mMapper)
                                    {
                                        if (t.Level == levelStart)
                                        {
                                            levelCurrent = t.Level;
                                            startCheck = true;
                                        }


                                        if (!startCheck) continue;
                                        if (t.Level == levelEnd)
                                        {
                                            if (t.Type == DataNode.ItemType.Folder)
                                            {
                                                if (string.Compare(t.PathRight, Path.Combine(parentDirectoryMixedBaseRight, parentDirectoryMixed), StringComparison.OrdinalIgnoreCase) == 0)
                                                    break;
                                            }
                                        }


                                        if (levelCurrent != t.Level) continue;
                                        var pathTemp = string.Empty;
                                        for (var x = 0; x <= pathIndex; x++)
                                            pathTemp += (pathTemp.Trim() != string.Empty ? "\\" : string.Empty) + paths[x];

                                        var testPathRight = Path.Combine(parentDirectoryMixedBaseRight, pathTemp);
                                        var testPathLeft = Path.Combine(parentDirectoryMixedBaseLeft, pathTemp);

                                        if (string.Compare(t.PathRight, testPathRight,
                                                StringComparison.OrdinalIgnoreCase) != 0) continue;
                                        if (t.PathLeft.Trim() == string.Empty)
                                        {
                                            t.PathLeft = testPathLeft;
                                            t.NameLeft = new DirectoryInfo(t.PathLeft).Name;

                                            var folders = 0;
                                            var files = 0;
                                            CalculateFoldersSize(t, ref folders, ref files);
                                        }

                                        levelCurrent++;
                                        if (pathIndex + 1 < paths.Count)
                                            pathIndex++;
                                    }
                                }


                                #endregion
                            }


                            if (!Directory.Exists(Path.GetDirectoryName(mbr.PathLeft)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(mbr.PathLeft));
                            }

                            filesCopied++;
                            if (!overwrite && File.Exists(mbr.PathLeft))
                            {
                                //message
                            }
                            else
                            {
                                items.Add(new Settings.ComparisonLogEntryItem(File.Exists(mbr.PathLeft).ToString(), mbr.PathRight, mbr.PathLeft));



                                File.Copy(mbr.PathRight, mbr.PathLeft, overwrite);
                                mbr.NameLeft = mbr.NameRight;
                                mbr.SizeLeft = mbr.SizeRight;
                                mbr.ModifiedLeft = mbr.ModifiedRight;

                                mbr.CompareState = DataNode.CompareStates.Equal;

                                _panelCompare.listView_main.RedrawItems(i, i, false);

                                var dnFolder = GetDataNodeFolder(mbr.Level - 1, Path.GetDirectoryName(mbr.PathLeft), fromLeftToRight);
                                SetFolderState(dnFolder, false);


                            }
                        }
                    }

                    if (filesCopied > 0)
                    {



                        #region  |  ComparisonLogEntry  |


                        var cle = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.FilesCopy)
                        {
                            Date = DateTime.Now,
                            Id = Guid.NewGuid().ToString(),
                            ItemName = filesCopied > 1 ? string.Format(Resources.FormMain_CopySelectedTo_Copied_0_files, filesCopied) : string.Format(Resources.FormMain_CopySelectedTo_Copied_0_file, filesCopied),
                            Value01 = string.Empty,
                            Value02 = string.Empty,
                            Items = items
                        };





                        update_comparison_log(cle);

                        #endregion


                        UpdateVisualCompareDirectories();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //move
        private void MoveSelectedTo(bool fromLeftToRight)
        {
            if (_panelCompare.listView_main.SelectedIndices.Count <= 0) return;
            var f = new MoveFiles { checkBox_overwirte_existing_files = { Checked = false } };



            try
            {

                var iFilesLeft = 0;
                var iFoldersLeft = 0;
                long iSizeLeft = 0;

                var iFilesRight = 0;
                var iFoldersRight = 0;
                long iSizeRight = 0;

                for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]];
                    var dn = (DataNode)lvi.Tag;

                    #region   |  if (dn.Type == DataNode.ItemType.file)  |



                    switch (dn.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iSizeLeft += dn.SizeLeft;
                            }
                            else
                            {
                                iFoldersLeft++;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesRight++;

                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersRight++;
                            }
                            break;
                        case DataNode.Selection.Middle:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iFilesRight++;

                                iSizeLeft += dn.SizeLeft;
                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersLeft++;
                                iFoldersRight++;
                            }
                            break;
                    }




                    #endregion
                }

                f.label_fileCount_left.Text = iFilesLeft.ToString();
                f.label_folderCount_left.Text = iFoldersLeft.ToString();
                f.label_size_left.Text = CalculateFileSize(iSizeLeft);

                f.label_fileCount_right.Text = iFilesRight.ToString();
                f.label_folderCount_right.Text = iFoldersRight.ToString();
                f.label_size_right.Text = CalculateFileSize(iSizeRight);

                f.ShowDialog();
                if (!f.Saved) return;

                var overwrite = f.checkBox_overwirte_existing_files.Checked;
                fromLeftToRight = f.radioButton_left_side.Checked;

                var filesCopied = 0;
                var items = new List<Settings.ComparisonLogEntryItem>();


                var regexPathChar = new Regex(@"\\", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (int i in _panelCompare.listView_main.SelectedIndices)
                {
                    var mbr = _mMapper[i];

                    if (fromLeftToRight)
                    {
                        if (mbr.Type != DataNode.ItemType.File) continue;
                        if (mbr.PathLeft.Trim() == string.Empty) continue;
                        if (mbr.PathRight.Trim() == string.Empty)
                        {
                            #region  |  get path info  |

                            var parentDirectoryRight = GetParentDirectory(mbr, fromLeftToRight);
                            var parentDirectoryRightMixed = parentDirectoryRight.Replace(_panelCompare.comboBox_main_compare_right.Text.Trim(), string.Empty);

                            var parentDirectoryLeft = GetParentDirectory(mbr, fromLeftToRight ? false : true);
                            var parentDirectoryLeftMixed = parentDirectoryLeft.Replace(_panelCompare.comboBox_main_compare_left.Text.Trim(), string.Empty);


                            var parentDirectoryMixed = parentDirectoryRightMixed.Trim() != string.Empty ? parentDirectoryLeftMixed.Replace(parentDirectoryRightMixed, string.Empty) : parentDirectoryLeftMixed;


                            if (parentDirectoryMixed.Trim() != string.Empty)
                            {
                                if (parentDirectoryMixed.Trim('\\') != string.Empty)
                                {

                                    parentDirectoryMixed = parentDirectoryMixed.Trim();
                                    parentDirectoryMixed = parentDirectoryMixed.TrimStart('\\');


                                    parentDirectoryRight = parentDirectoryRight.Trim();
                                    parentDirectoryRight = parentDirectoryRight.TrimEnd('\\');

                                    parentDirectoryRight = parentDirectoryRight + "\\" + parentDirectoryMixed;
                                }
                            }

                            if (parentDirectoryRight.Trim() != string.Empty)
                            {
                                mbr.PathRight = Path.Combine(parentDirectoryRight, Path.GetFileName(mbr.NameLeft));
                            }
                            #endregion

                            #region  |  update folder path info  |

                            if (parentDirectoryMixed.Trim() != string.Empty)
                            {

                                var parentDirectoryMixedBaseLeft = parentDirectoryLeft.Replace(parentDirectoryMixed, string.Empty);
                                parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.Trim();
                                parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimStart('\\');
                                parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimEnd('\\');


                                var parentDirectoryMixedBaseRight = parentDirectoryRight.Replace(parentDirectoryMixed, string.Empty);
                                parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.Trim();
                                parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimStart('\\');
                                parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimEnd('\\');


                                var paths = regexPathChar.Split(parentDirectoryMixed).ToList();

                                var levelStart = mbr.Level - paths.Count;
                                var levelEnd = mbr.Level;
                                var levelCurrent = 0;
                                var pathIndex = 0;

                                var startCheck = false;

                                foreach (var t in _mMapper)
                                {
                                    if (t.Level == levelStart)
                                    {
                                        levelCurrent = t.Level;
                                        startCheck = true;
                                    }


                                    if (!startCheck) continue;
                                    if (t.Level == levelEnd)
                                    {
                                        if (t.Type == DataNode.ItemType.Folder)
                                        {
                                            if (string.Compare(t.PathLeft, Path.Combine(parentDirectoryMixedBaseLeft, parentDirectoryMixed), StringComparison.OrdinalIgnoreCase) == 0)
                                                break;
                                        }
                                    }


                                    if (levelCurrent != t.Level) continue;
                                    var pathTemp = string.Empty;
                                    for (var x = 0; x <= pathIndex; x++)
                                        pathTemp += (pathTemp.Trim() != string.Empty ? "\\" : string.Empty) + paths[x];

                                    var testPathLeft = Path.Combine(parentDirectoryMixedBaseLeft, pathTemp);
                                    var testPathRight = Path.Combine(parentDirectoryMixedBaseRight, pathTemp);

                                    if (string.Compare(t.PathLeft, testPathLeft, StringComparison.OrdinalIgnoreCase) != 0) continue;
                                    if (t.PathRight.Trim() == string.Empty)
                                    {
                                        t.PathRight = testPathRight;
                                        t.NameRight = new DirectoryInfo(t.PathRight).Name;

                                        var folders = 0;
                                        var files = 0;
                                        CalculateFoldersSize(t, ref folders, ref files);
                                    }

                                    levelCurrent++;
                                    if (pathIndex + 1 < paths.Count)
                                        pathIndex++;
                                }
                            }


                            #endregion
                        }


                        if (!Directory.Exists(Path.GetDirectoryName(mbr.PathRight)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(mbr.PathRight));
                        }

                        filesCopied++;

                        if (!overwrite && File.Exists(mbr.PathRight))
                        {
                            //message
                        }
                        else
                        {
                            items.Add(new Settings.ComparisonLogEntryItem(File.Exists(mbr.PathRight).ToString(), mbr.PathLeft, mbr.PathRight));

                            if (File.Exists(mbr.PathRight))
                                File.Delete(mbr.PathRight);

                            File.Move(mbr.PathLeft, mbr.PathRight);

                            mbr.NameRight = mbr.NameLeft;
                            mbr.SizeRight = mbr.SizeLeft;
                            mbr.ModifiedRight = mbr.ModifiedLeft;

                            mbr.NameLeft = string.Empty;
                            mbr.SizeLeft = -1;
                            mbr.PathLeft = string.Empty;

                            mbr.CompareState = DataNode.CompareStates.OrphansRightside;

                            _panelCompare.listView_main.RedrawItems(i, i, false);

                            var dnFolder = GetDataNodeFolder(mbr.Level - 1, Path.GetDirectoryName(mbr.PathRight), fromLeftToRight);
                            SetFolderState(dnFolder, true);
                        }
                    }
                    else
                    {
                        if (mbr.Type != DataNode.ItemType.File) continue;
                        if (mbr.PathRight.Trim() == string.Empty) continue;
                        if (mbr.PathLeft.Trim() == string.Empty)
                        {
                            #region  |  get path info  |

                            var parentDirectoryLeft = GetParentDirectory(mbr, fromLeftToRight);
                            var parentDirectoryLeftMixed = parentDirectoryLeft.Replace(_panelCompare.comboBox_main_compare_left.Text.Trim(), string.Empty);

                            var parentDirectoryRight = GetParentDirectory(mbr, fromLeftToRight ? false : true);
                            var parentDirectoryRightMixed = parentDirectoryRight.Replace(_panelCompare.comboBox_main_compare_right.Text.Trim(), string.Empty);


                            var parentDirectoryMixed = parentDirectoryLeftMixed.Trim() != string.Empty ? parentDirectoryRightMixed.Replace(parentDirectoryLeftMixed, string.Empty) : parentDirectoryRightMixed;


                            if (parentDirectoryMixed.Trim() != string.Empty)
                            {
                                if (parentDirectoryMixed.Trim('\\') != string.Empty)
                                {

                                    parentDirectoryMixed = parentDirectoryMixed.Trim();
                                    parentDirectoryMixed = parentDirectoryMixed.TrimStart('\\');


                                    parentDirectoryLeft = parentDirectoryLeft.Trim();
                                    parentDirectoryLeft = parentDirectoryLeft.TrimEnd('\\');

                                    parentDirectoryLeft = parentDirectoryLeft + "\\" + parentDirectoryMixed;
                                }
                            }

                            if (parentDirectoryLeft.Trim() != string.Empty)
                            {
                                mbr.PathLeft = Path.Combine(parentDirectoryLeft, Path.GetFileName(mbr.NameRight));
                            }
                            #endregion

                            #region  |  update folder path info  |

                            if (parentDirectoryMixed.Trim() != string.Empty)
                            {

                                var parentDirectoryMixedBaseRight = parentDirectoryRight.Replace(parentDirectoryMixed, string.Empty);
                                parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.Trim();
                                parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimStart('\\');
                                parentDirectoryMixedBaseRight = parentDirectoryMixedBaseRight.TrimEnd('\\');


                                var parentDirectoryMixedBaseLeft = parentDirectoryLeft.Replace(parentDirectoryMixed, string.Empty);
                                parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.Trim();
                                parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimStart('\\');
                                parentDirectoryMixedBaseLeft = parentDirectoryMixedBaseLeft.TrimEnd('\\');


                                var paths = regexPathChar.Split(parentDirectoryMixed).ToList();

                                var levelStart = mbr.Level - paths.Count;
                                var levelEnd = mbr.Level;
                                var levelCurrent = 0;
                                var pathIndex = 0;

                                var startCheck = false;

                                foreach (var t in _mMapper)
                                {
                                    if (t.Level == levelStart)
                                    {
                                        levelCurrent = t.Level;
                                        startCheck = true;
                                    }


                                    if (startCheck)
                                    {
                                        if (t.Level == levelEnd)
                                        {
                                            if (t.Type == DataNode.ItemType.Folder)
                                            {
                                                if (string.Compare(t.PathRight, Path.Combine(parentDirectoryMixedBaseRight, parentDirectoryMixed), StringComparison.OrdinalIgnoreCase) == 0)
                                                    break;
                                            }
                                        }


                                        if (levelCurrent != t.Level) continue;
                                        var pathTemp = string.Empty;
                                        for (var x = 0; x <= pathIndex; x++)
                                            pathTemp += (pathTemp.Trim() != string.Empty ? "\\" : string.Empty) + paths[x];

                                        var testPathRight = Path.Combine(parentDirectoryMixedBaseRight, pathTemp);
                                        var testPathLeft = Path.Combine(parentDirectoryMixedBaseLeft, pathTemp);

                                        if (string.Compare(t.PathRight, testPathRight, StringComparison.OrdinalIgnoreCase) != 0) continue;
                                        if (t.PathLeft.Trim() == string.Empty)
                                        {
                                            t.PathLeft = testPathLeft;
                                            t.NameLeft = new DirectoryInfo(t.PathLeft).Name;


                                            var folders = 0;
                                            var files = 0;
                                            CalculateFoldersSize(t, ref folders, ref files);
                                        }

                                        levelCurrent++;
                                        if (pathIndex + 1 < paths.Count)
                                            pathIndex++;
                                    }
                                }
                            }


                            #endregion
                        }


                        if (!Directory.Exists(Path.GetDirectoryName(mbr.PathLeft)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(mbr.PathLeft));
                        }

                        filesCopied++;

                        if (!overwrite && File.Exists(mbr.PathLeft))
                        {
                            //message
                        }
                        else
                        {
                            items.Add(new Settings.ComparisonLogEntryItem(File.Exists(mbr.PathLeft).ToString(), mbr.PathRight, mbr.PathLeft));


                            if (File.Exists(mbr.PathLeft))
                                File.Delete(mbr.PathLeft);
                            File.Move(mbr.PathRight, mbr.PathLeft);

                            mbr.NameLeft = mbr.NameRight;
                            mbr.SizeLeft = mbr.SizeRight;
                            mbr.ModifiedLeft = mbr.ModifiedRight;


                            mbr.NameRight = string.Empty;
                            mbr.SizeRight = -1;
                            mbr.PathRight = string.Empty;


                            mbr.CompareState = DataNode.CompareStates.OrphansLeftside;

                            _panelCompare.listView_main.RedrawItems(i, i, false);

                            var dnFolder = GetDataNodeFolder(mbr.Level - 1, Path.GetDirectoryName(mbr.PathLeft), fromLeftToRight);
                            SetFolderState(dnFolder, false);
                        }
                    }
                }


                if (filesCopied <= 0) return;

                #region  |  ComparisonLogEntry  |


                var cle = new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.FilesMove)
                {
                    Date = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    ItemName = "Moved " + filesCopied + (filesCopied > 1 ? " files" : " file"),
                    Value01 = string.Empty,
                    Value02 = string.Empty,
                    Items = items
                };





                update_comparison_log(cle);

                #endregion


                UpdateVisualCompareDirectories();
            }

            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //delete
        private void DeleteSelected(bool fromLeftToRight)
        {
            if (_panelCompare.listView_main.SelectedIndices.Count <= 0) return;
            var f = new DeleteFiles();


            try
            {
                var iFilesLeft = 0;
                var iFoldersLeft = 0;
                long iSizeLeft = 0;

                var iFilesRight = 0;
                var iFoldersRight = 0;
                long iSizeRight = 0;

                for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]];
                    var dn = (DataNode)lvi.Tag;

                    #region   |  if (dn.Type == DataNode.ItemType.file)  |



                    switch (dn.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iSizeLeft += dn.SizeLeft;
                            }
                            else
                            {
                                iFoldersLeft++;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesRight++;

                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersRight++;
                            }
                            break;
                        case DataNode.Selection.Middle:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iFilesRight++;

                                iSizeLeft += dn.SizeLeft;
                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersLeft++;
                                iFoldersRight++;
                            }
                            break;
                    }




                    #endregion
                }

                f.label_fileCount_left.Text = iFilesLeft.ToString();
                f.label_folderCount_left.Text = iFoldersLeft.ToString();
                f.label_size_left.Text = CalculateFileSize(iSizeLeft);

                f.label_fileCount_right.Text = iFilesRight.ToString();
                f.label_folderCount_right.Text = iFoldersRight.ToString();
                f.label_size_right.Text = CalculateFileSize(iSizeRight);

                f.ShowDialog();
                if (!f.Saved) return;

                var dr = MessageBox.Show(this, Resources.FormMain_DeleteSelected, System.Windows.Forms.Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;
                var filesDeleted = 0;
                var items = new List<Settings.ComparisonLogEntryItem>();

                foreach (int i in _panelCompare.listView_main.SelectedIndices)
                {
                    var mbr = _mMapper[i];

                    if (mbr.Type != DataNode.ItemType.File) continue;
                    if (mbr.PathLeft.Trim() != string.Empty && f.checkBox_selected_left_side.Checked)
                    {
                        items.Add(new Settings.ComparisonLogEntryItem("True", mbr.PathLeft, string.Empty));

                        var dnFolder = GetDataNodeFolder(mbr.Level - 1, Path.GetDirectoryName(mbr.PathLeft), false);

                        filesDeleted++;
                        File.Delete(mbr.PathLeft);
                        mbr.PathLeft = string.Empty;
                        mbr.NameLeft = string.Empty;
                        mbr.SizeLeft = -1;


                        mbr.CompareState = DataNode.CompareStates.None;


                        if (mbr.NameRight.Trim() != string.Empty)
                        {
                            mbr.CompareState = DataNode.CompareStates.OrphansRightside;
                        }
                        _panelCompare.listView_main.RedrawItems(i, i, false);

                        SetFolderState(dnFolder, false);
                    }

                    if (mbr.PathRight.Trim() != string.Empty && f.checkBox_selected_right_side.Checked)
                    {

                        items.Add(new Settings.ComparisonLogEntryItem("True", mbr.PathRight, string.Empty));


                        var dnFolder = GetDataNodeFolder(mbr.Level - 1, Path.GetDirectoryName(mbr.PathRight), true);


                        filesDeleted++;
                        File.Delete(mbr.PathRight);

                        mbr.PathRight = string.Empty;
                        mbr.NameRight = string.Empty;
                        mbr.SizeRight = -1;

                        mbr.CompareState = DataNode.CompareStates.None;


                        if (mbr.NameLeft.Trim() != string.Empty)
                        {
                            mbr.CompareState = DataNode.CompareStates.OrphansLeftside;
                        }

                        _panelCompare.listView_main.RedrawItems(i, i, false);


                        SetFolderState(dnFolder, true);
                    }
                }

                if (filesDeleted <= 0) return;

                #region  |  ComparisonLogEntry  |


                var cle =
                    new Settings.ComparisonLogEntry(Settings.ComparisonLogEntry.EntryType.FilesDelete)
                    {
                        Date = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        ItemName = filesDeleted > 1 ? string.Format(Resources.FormMain_DeleteSelected_Deleted_0_files, filesDeleted) : Resources.FormMain_DeleteSelected_Deleted_1_file,
                        Value01 = string.Empty,
                        Value02 = string.Empty,
                        Items = items
                    };





                update_comparison_log(cle);

                #endregion


                UpdateVisualCompareDirectories();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //copy to folder
        private void CopyToFolder(bool fromLeftToRight)
        {
            if (_panelCompare.listView_main.SelectedIndices.Count <= 0) return;
            var f = new CopyFilesToFolder { checkBox_overwirte_existing_files = { Checked = false } };



            try
            {
                var iFilesLeft = 0;
                var iFoldersLeft = 0;
                long iSizeLeft = 0;

                var iFilesRight = 0;
                var iFoldersRight = 0;
                long iSizeRight = 0;

                for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]];
                    var dn = (DataNode)lvi.Tag;

                    #region   |  if (dn.Type == DataNode.ItemType.file)  |



                    switch (dn.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iSizeLeft += dn.SizeLeft;
                            }
                            else
                            {
                                iFoldersLeft++;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesRight++;

                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersRight++;
                            }
                            break;
                        case DataNode.Selection.Middle:
                            if (dn.Type == DataNode.ItemType.File)
                            {
                                iFilesLeft++;
                                iFilesRight++;

                                iSizeLeft += dn.SizeLeft;
                                iSizeRight += dn.SizeRight;
                            }
                            else
                            {
                                iFoldersLeft++;
                                iFoldersRight++;
                            }
                            break;
                    }




                    #endregion
                }

                f.label_fileCount_left.Text = iFilesLeft.ToString();
                f.label_folderCount_left.Text = iFoldersLeft.ToString();
                f.label_size_left.Text = CalculateFileSize(iSizeLeft);

                f.label_fileCount_right.Text = iFilesRight.ToString();
                f.label_folderCount_right.Text = iFoldersRight.ToString();
                f.label_size_right.Text = CalculateFileSize(iSizeRight);
                f.ShowDialog();
                if (!f.Saved) return;
                var overwrite = f.checkBox_overwirte_existing_files.Checked;

                MessageBox.Show(this, Resources.FormMain_CopyToFolder_Functionality_not_implemented_yet, System.Windows.Forms.Application.ProductName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void copyToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedTo(MostlyLeftSelection);
        }


        private void copyToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToFolder(MostlyLeftSelection);
        }


        private void moveToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSelectedTo(MostlyLeftSelection);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected(MostlyLeftSelection);
        }

        private static void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private static void attributesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private static void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion
        #region  |  resize  |

        private bool Maximized { get; set; }
        private int PreviousWindowHeight { get; set; }
        private int PreviousWindowWidth { get; set; }
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Maximized:
                    Maximized = true;
                    FormMain_ResizeEnd(null, null);
                    break;
                case FormWindowState.Normal:
                    if (Maximized)
                    {
                        FormMain_ResizeEnd(null, null);
                        Maximized = false;
                    }
                    break;
            }
        }


        private void panel_CompareProject_Resize(object sender, EventArgs e)
        {

            FormMain_ResizeEnd(sender, e);
        }


        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {


            _panelCompare.listView_main.BeginUpdate();

            var usableWidth = _panelCompare.listView_main.Width - 50;

            var widthType = 0;
            var widthSize = 0;
            var widthDate = 120;

            if (Application.Settings.ViewListViewColumnType)
            {
                widthType = 110;
            }
            else
            {
                widthType = 0;
            }

            if (Application.Settings.ViewListViewColumnSize)
            {
                widthSize = 60;
            }
            else
            {
                widthSize = 0;
            }

            var staticColumnWidth = widthType * 2 + widthSize * 2 + widthDate * 2;

            usableWidth = usableWidth - staticColumnWidth;

            var widthName = usableWidth / 2;



            _panelCompare.listView_main.Columns[0].Width = widthName - 1; //name
            _panelCompare.listView_main.Columns[1].Width = widthType; //type
            _panelCompare.listView_main.Columns[2].Width = widthSize; //size
            _panelCompare.listView_main.Columns[3].Width = widthDate; //date

            _panelCompare.listView_main.Columns[5].Width = widthName - 1; //name
            _panelCompare.listView_main.Columns[6].Width = widthType; //type
            _panelCompare.listView_main.Columns[7].Width = widthSize; //size
            _panelCompare.listView_main.Columns[8].Width = widthDate; //date



            _panelCompare.listView_main.EndUpdate();



            _panelCompare.panel_overListView_left.Width = Convert.ToInt32(widthName + widthType + widthSize + widthDate) + 34;
            _panelCompare.comboBox_main_compare_left.Select(0, 0);
            _panelCompare.comboBox_main_compare_right.Select(0, 0);





            _panelCompare.panel_listViewMessage.Left = _panelCompare.panel_main_diff.Width / 2 - 150;


            PreviousWindowHeight = Height;
            PreviousWindowWidth = Width;




        }


        #endregion
        #region  |  compare  |



        //compare directories
        private void CompareDirectories(bool writeLogEntry)
        {


            if (_panelCompare == null || _panelCompare.DockPanel == null)
            {
                ShowPanelCompare(true);
            }

            SetFilterList(toolStripButton_Activate_Filters.Checked);


            try
            {

                UpdateFolderViewerStatusLabel();


                if (_panelCompare != null)
                {
                    _panelCompare.toolStripButton_loadLeftSide.Enabled = false;
                    _panelCompare.toolStripButton_loadRightSide.Enabled = false;

                    _panelCompare.comboBox_main_compare_left.BackColor = Directory.Exists(_panelCompare.comboBox_main_compare_left.Text) ? Color.White : Color.Pink;
                    _panelCompare.comboBox_main_compare_right.BackColor = Directory.Exists(_panelCompare.comboBox_main_compare_right.Text) ? Color.White : Color.Pink;


                    IsInitializingPanel = true;
                    AddComparisonEntriesToComboboxes(_panelCompare.comboBox_main_compare_left.Text, _panelCompare.comboBox_main_compare_right.Text);
                    IsInitializingPanel = false;





                    _panelCompare.listView_main.BeginUpdate();

                    _mModel.DataPool.Clear();
                    _panelCompare.listView_main.Items.Clear();



                    _panelCompare.comboBox_main_compare_left.Select(0, 0);
                    _panelCompare.comboBox_main_compare_right.Select(0, 0);


                    var cancel = false;
                    WaitingWindow.WaitingDialog = new WaitingDialog();
                    try
                    {


                        WaitingWindow.WaitingDialogWorker = new BackgroundWorker();

                        WaitingWindow.WaitingDialogWorker.WorkerReportsProgress = true;


                        var arguments = new List<object> { _panelCompare.comboBox_main_compare_left.Text, _panelCompare.comboBox_main_compare_right.Text };

                        WaitingWindow.WaitingDialogWorker.DoWork += (sender, e) => arguments = worker_getFolders_DoWork(null, new DoWorkEventArgs(arguments));



                        WaitingWindow.WaitingDialogWorker.RunWorkerCompleted += WaitingWindowHandlers.waitingWorker_RunWorkerCompleted;
                        WaitingWindow.WaitingDialogWorker.ProgressChanged += WaitingWindowHandlers.waitingWorker_ProgressChanged;

                        WaitingWindow.WaitingDialogWorker.RunWorkerAsync();
                        WaitingWindow.WaitingDialog.DialogResult = DialogResult.OK;
                        WaitingWindow.WaitingDialog.ShowDialog();
                        if (WaitingWindow.WaitingDialog.hitCancel)
                        {
                            cancel = true;
                        }



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        WaitingWindow.WaitingDialog.Dispose();
                    }


                    if (cancel)
                    {
                        _panelCompare.listView_main.Items.Clear();
                        _mModel.DataPool.Clear();
                        //m_Mapper.Clear();
                        System.Windows.Forms.Application.DoEvents();
                    }

                    try
                    {
                        _panelCompare.panel_listViewMessage.Visible = true;
                        System.Windows.Forms.Application.DoEvents();

                        InitVirtualListViewNodes();
                        _panelCompare.listView_main.Refresh();


                        if (Application.Settings.AutomaciallyExpandComparisonFolders)
                        {
                            ExpandAll();
                        }
                    }
                    finally
                    {
                        _panelCompare.panel_listViewMessage.Visible = false;
                    }
                }

                toolStripStatusLabel_message_1.Text = string.Format(Resources.FormMain_CompareDirectories_Left_Side_Directories_0_Files_1_Right_Side_Directories_2_Files_3, _mModel.TotalFoldersLeft, _mModel.TotalFilesLeft, _mModel.TotalFoldersRight, _mModel.TotalFilesRight);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_panelCompare != null)
                {
                    _panelCompare.listView_main.EndUpdate();

                    _panelCompare.comboBox_main_compare_left.Select(0, 0);
                    _panelCompare.comboBox_main_compare_right.Select(0, 0);

                    listView_comparison_projects_ItemSelectionChanged(null, null);

                    CheckEnabledButtonToolbarMain();

                    if (writeLogEntry)
                    {

                        #region  |  ComparisonLogEntry  |


                        var comparisonLogEntry = new Settings.ComparisonLogEntry(
                                Settings.ComparisonLogEntry.EntryType.ComparisonFoldersCompare)
                            {
                                Date = DateTime.Now,
                                Id = Guid.NewGuid().ToString(),
                                ItemName = "",
                                Value01 = _panelCompare.comboBox_main_compare_left.Text,
                                Value02 = _panelCompare.comboBox_main_compare_right.Text,
                                Items = null
                            };




                        update_comparison_log(comparisonLogEntry);
                        #endregion
                    }
                }

                Cursor = Cursors.Default;

            }


        }


        private void UpdateFolderViewerStatusLabel()
        {
            var projectIsCurrent = false;
            var comparisonProjectCurrent = new Settings.ComparisonProject();

            if (_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag != null)
            {
                comparisonProjectCurrent =
                           (Settings.ComparisonProject)_panelCompare.toolStripDropDownButton_comparison_project_left_side_move.Tag;


                if (string.Compare(comparisonProjectCurrent.PathLeft.Trim(), _panelCompare.comboBox_main_compare_left.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0
                    && string.Compare(comparisonProjectCurrent.PathRight.Trim(), _panelCompare.comboBox_main_compare_right.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    projectIsCurrent = true;
                }
                else
                {
                    comparisonProjectCurrent = new Settings.ComparisonProject
                    {
                        Name = string.Empty,
                        PathLeft = _panelCompare.comboBox_main_compare_left.Text,
                        PathRight = _panelCompare.comboBox_main_compare_right.Text
                    };
                }
            }
            else
            {
                comparisonProjectCurrent.Name = string.Empty;
                comparisonProjectCurrent.PathLeft = _panelCompare.comboBox_main_compare_left.Text;
                comparisonProjectCurrent.PathRight = _panelCompare.comboBox_main_compare_right.Text;
            }




            if (projectIsCurrent)
            {
                _panelCompare.pictureBox_statusBar_comparison_project_image.Tag = comparisonProjectCurrent;
                _panelCompare.pictureBox_statusBar_comparison_project_image.Image = _panelCompare.imageList_status.Images[0];
            }
            else
            {
                _panelCompare.pictureBox_statusBar_comparison_project_image.Tag = null;
                _panelCompare.pictureBox_statusBar_comparison_project_image.Image = _panelCompare.imageList_status.Images[1];
            }

            _panelCompare.richTextBox_statusbar_comparelist.Clear();
            _panelCompare.richTextBox_statusbar_comparelist.SelectionCharOffset = -5;

            _panelCompare.richTextBox_statusbar_comparelist.SelectionColor = Color.Black;
            _panelCompare.richTextBox_statusbar_comparelist.SelectedText = " ";

            _panelCompare.richTextBox_statusbar_comparelist.SelectionFont = new Font(_panelCompare.richTextBox_statusbar_comparelist.SelectionFont.FontFamily.Name, _panelCompare.richTextBox_statusbar_comparelist.SelectionFont.Size, FontStyle.Regular);
            _panelCompare.richTextBox_statusbar_comparelist.SelectionColor = Color.Black;
            _panelCompare.richTextBox_statusbar_comparelist.SelectedText = comparisonProjectCurrent.Name;


            _panelCompare.richTextBox_statusbar_comparelist.SelectionFont = new Font(_panelCompare.richTextBox_statusbar_comparelist.SelectionFont.FontFamily.Name, _panelCompare.richTextBox_statusbar_comparelist.SelectionFont.Size, FontStyle.Regular);
            _panelCompare.richTextBox_statusbar_comparelist.SelectionColor = Color.Black;
            _panelCompare.richTextBox_statusbar_comparelist.SelectedText = projectIsCurrent ? " - " : " ";


            _panelCompare.richTextBox_statusbar_comparelist.SelectionFont = new Font(_panelCompare.richTextBox_statusbar_comparelist.SelectionFont.FontFamily.Name, _panelCompare.richTextBox_statusbar_comparelist.SelectionFont.Size, FontStyle.Italic);
            _panelCompare.richTextBox_statusbar_comparelist.SelectionColor = Color.Gray;
            _panelCompare.richTextBox_statusbar_comparelist.SelectedText = comparisonProjectCurrent.PathLeft;

            _panelCompare.richTextBox_statusbar_comparelist.SelectionFont = new Font(_panelCompare.richTextBox_statusbar_comparelist.SelectionFont.FontFamily.Name, _panelCompare.richTextBox_statusbar_comparelist.SelectionFont.Size, FontStyle.Regular | FontStyle.Bold);
            _panelCompare.richTextBox_statusbar_comparelist.SelectionColor = Color.Black;
            _panelCompare.richTextBox_statusbar_comparelist.SelectedText = " <> ";

            _panelCompare.richTextBox_statusbar_comparelist.SelectionFont = new Font(_panelCompare.richTextBox_statusbar_comparelist.SelectionFont.FontFamily.Name, _panelCompare.richTextBox_statusbar_comparelist.SelectionFont.Size, FontStyle.Italic);
            _panelCompare.richTextBox_statusbar_comparelist.SelectionColor = Color.Gray;
            _panelCompare.richTextBox_statusbar_comparelist.SelectedText = comparisonProjectCurrent.PathRight;
        }

        private List<object> worker_getFolders_DoWork(object sender, DoWorkEventArgs e)
        {
            var objects = (List<object>)e.Argument;
            var folderLeft = objects[0].ToString();
            var folderRight = objects[1].ToString();

            if (!Directory.Exists(folderLeft) || !Directory.Exists(folderRight))
                return objects;

            _mModel.FileAlignments = new List<Settings.FileAlignment>();
            if (_panelCompare.pictureBox_statusBar_comparison_project_image.Tag != null)
            {
                var comparisonProject = (Settings.ComparisonProject)_panelCompare.pictureBox_statusBar_comparison_project_image.Tag;
                if (comparisonProject.FileAlignment != null)
                    _mModel.FileAlignments = comparisonProject.FileAlignment;
            }


            _mModel.ImportDataModel(folderLeft, folderRight);

            return objects;
        }

        private void UpdateVisualCompareDirectories()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _panelCompare.listView_main.Items.Clear();

                InitVirtualListViewNodes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }




        #endregion
        #region  |  report  |

        //create report
        private void CreateReport()
        {
            var lv = _panelCompare.listView_main;

            RateGroup = new Settings.PriceGroup();

            ProgressPercentage = 0;
            ProgressStatus = true;
            _timer.Interval = 250;

            _timer.Elapsed += TimerElapsed;

            try
            {
                _timer.Start();


                var comparer = new Processor();

                #region  |  initialze Processor settings  |
                Processor.Settings.comparisonType = Application.Settings.ReportViewerSettings.comparisonType;
                Processor.Settings.ComparisonIncludeTags = Application.Settings.ReportViewerSettings.ComparisonIncludeTags;
                Processor.Settings.IncludeIndividualFileInformation = true;


                Processor.Settings.UseCustomStyleSheet = false;
                Processor.Settings.FilePathCustomStyleSheet = string.Empty;

                Processor.Settings.StyleNewText = Application.Settings.ReportViewerSettings.StyleNewText;
                Processor.Settings.StyleRemovedText = Application.Settings.ReportViewerSettings.StyleRemovedText;
                Processor.Settings.StyleNewTag = Application.Settings.ReportViewerSettings.StyleNewTag;
                Processor.Settings.StyleRemovedTag = Application.Settings.ReportViewerSettings.StyleRemovedTag;


                Processor.Settings.SearchSubFolders = false;


                Processor.Settings.ReportDirectory = Application.Settings.ApplicationSettingsPath;
                Processor.Settings.ReportFileName = Path.Combine(Application.Settings.ApplicationSettingsPath, "PostEdit.Compare.Report" + ".xml");
                Processor.Settings.ViewReportWhenFinishedProcessing = false;


                Processor.Settings.reportFormat = Settings.ReportFormat.Html;

                Processor.Settings.ReportFilterSegmentsWithNoChanges = Application.Settings.ReportViewerSettings.ReportFilterSegmentsWithNoChanges;
                Processor.Settings.ReportFilterChangedTargetContent = Application.Settings.ReportViewerSettings.ReportFilterChangedTargetContent;
                Processor.Settings.ReportFilterSegmentStatusChanged = Application.Settings.ReportViewerSettings.ReportFilterSegmentStatusChanged;
                Processor.Settings.ReportFilterSegmentsContainingComments = Application.Settings.ReportViewerSettings.ReportFilterSegmentsContainingComments;
                Processor.Settings.ReportFilterLockedSegments = Application.Settings.ReportViewerSettings.ReportFilterLockedSegments;

                Processor.Settings.ReportFilterFilesWithNoRecordsFiltered = Application.Settings.ReportViewerSettings.ReportFilterFilesWithNoRecordsFiltered;

                Processor.Settings.ShowOriginalSourceSegment = Application.Settings.ReportViewerSettings.ShowOriginalSourceSegment;
                Processor.Settings.ShowOriginalTargetSegment = Application.Settings.ReportViewerSettings.ShowOriginalTargetSegment;
                Processor.Settings.ShowOriginalRevisionMarkerTargetSegment = Application.Settings.ReportViewerSettings.ShowOriginalRevisionMarkerTargetSegment;
                Processor.Settings.ShowUpdatedTargetSegment = Application.Settings.ReportViewerSettings.ShowUpdatedTargetSegment;
                Processor.Settings.ShowUpdatedRevisionMarkerTargetSegment = Application.Settings.ReportViewerSettings.ShowUpdatedRevisionMarkerTargetSegment;
                Processor.Settings.ShowTargetComparison = Application.Settings.ReportViewerSettings.ShowTargetComparison;
                Processor.Settings.ShowSegmentComments = Application.Settings.ReportViewerSettings.ShowSegmentComments;
                Processor.Settings.ShowSegmentLocked = Application.Settings.ReportViewerSettings.ShowSegmentLocked;

                #endregion


                var f = new ReportWizard();


                f.comboBox_priceGroup.Items.Clear();
                foreach (var p in Application.Settings.PriceGroups)
                    f.comboBox_priceGroup.Items.Add(p.Name);

                if (f.comboBox_priceGroup.Items.Count > 0)
                    f.comboBox_priceGroup.SelectedIndex = 0;

                if (lv.SelectedIndices.Count > 0)
                    f.radioButton_compareSelectedFiles.Checked = true;
                else
                    f.radioButton_compareAllFiles.Checked = true;



                f.checkBox_viewFilesWithNoTranslationDifferences.Checked = Application.Settings.ReportViewerSettings.ReportFilterFilesWithNoRecordsFiltered;
                f.checkBox_showGoogleChartsInReport.Checked = Application.Settings.ReportViewerSettings.ShowGoogleChartsInReport;
                f.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Checked = Application.Settings.ReportViewerSettings.CalculateSummaryAnalysisBasedOnFilteredRows;

                f.checkBox_viewSegmentsWithNoChanges.Checked = Application.Settings.ReportViewerSettings.ReportFilterSegmentsWithNoChanges;
                f.checkBox_viewSegmentsWithTranslationChanges.Checked = Application.Settings.ReportViewerSettings.ReportFilterChangedTargetContent;
                f.checkBox_viewSegmentsWithStatusChanges.Checked = Application.Settings.ReportViewerSettings.ReportFilterSegmentStatusChanged;
                f.checkBox_viewSegmentsWithComments.Checked = Application.Settings.ReportViewerSettings.ReportFilterSegmentsContainingComments;
                f.checkBox_viewLockedSegments.Checked = Application.Settings.ReportViewerSettings.ReportFilterLockedSegments;

                f.checkBox_includeAllSubfolders.Checked = true;

                f.checkBox_showOriginalSourceSegment.Checked = Application.Settings.ReportViewerSettings.ShowOriginalSourceSegment;
                f.checkBox_showOriginalTargetSegment.Checked = Application.Settings.ReportViewerSettings.ShowOriginalTargetSegment;
                f.checkBox_showOriginalRevisionMarkerTargetSegment.Checked = Application.Settings.ReportViewerSettings.ShowOriginalRevisionMarkerTargetSegment;
                f.checkBox_showUpdatedTargetSegment.Checked = Application.Settings.ReportViewerSettings.ShowUpdatedTargetSegment;
                f.checkBox_showUpdatedRevisionMarkerTargetSegment.Checked = Application.Settings.ReportViewerSettings.ShowUpdatedRevisionMarkerTargetSegment;
                f.checkBox_showTargetSegmentComparison.Checked = Application.Settings.ReportViewerSettings.ShowTargetComparison;
                f.checkBox_showSegmentComments.Checked = Application.Settings.ReportViewerSettings.ShowSegmentComments;
                f.checkBox_showLockedSegments.Checked = Application.Settings.ReportViewerSettings.ShowSegmentLocked;

                f.checkBox_showSegmentStatus.Checked = Application.Settings.ReportViewerSettings.ShowSegmentStatus;
                f.checkBox_showSegmentMatch.Checked = Application.Settings.ReportViewerSettings.ShowSegmentMatch;
                f.checkBox_showSegmentTERPAnalysis.Checked = Application.Settings.ReportViewerSettings.ShowSegmentTerp;
                f.textBox_javaExecutablePath.Text = Application.Settings.ReportViewerSettings.JavaExecutablePath;
                f.checkBox_showSegmentPEM.Checked = Application.Settings.ReportViewerSettings.ShowSegmentPem;


                f.comboBox_segments_match_value_original.SelectedItem = Application.Settings.ReportViewerSettings.ReportFilterTranslationMatchValuesOriginal;
                f.comboBox_segments_match_value_updated.SelectedItem = Application.Settings.ReportViewerSettings.ReportFilterTranslationMatchValuesUpdated;
                f.tagVisualizationComboBox.SelectedItem = Application.Settings.ReportViewerSettings.TagVisualStyle.ToString();

                var iFiles = 0;
                if (_panelCompare.listView_main.SelectedIndices.Count > 0)
                {
                    for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                    {
                        var mbr = (DataNode)_panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]].Tag;


                        if (mbr.Type == DataNode.ItemType.File)
                            iFiles++;
                    }
                }


                var signlePairSelectionPossible = false;
                if (iFiles > 0 && lv.SelectedIndices.Count == 2)
                {
                    var singleLeftPath = string.Empty;
                    var singleRightPath = string.Empty;

                    var lvi01 = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];
                    var lvi02 = _panelCompare.listView_main.Items[lv.SelectedIndices[1]];

                    var dn01 = (DataNode)lvi01.Tag;
                    var dn02 = (DataNode)lvi02.Tag;

                    switch (dn01.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (File.Exists(dn01.PathLeft))
                            {
                                singleLeftPath = dn01.PathLeft;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (File.Exists(dn01.PathRight))
                            {
                                singleRightPath = dn01.PathRight;
                            }
                            break;
                    }

                    switch (dn02.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (File.Exists(dn02.PathLeft))
                            {
                                singleLeftPath = dn02.PathLeft;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (File.Exists(dn02.PathRight))
                            {
                                singleRightPath = dn02.PathRight;
                            }
                            break;
                    }

                    if (singleLeftPath.Trim() != string.Empty && singleRightPath.Trim() != string.Empty)
                    {
                        signlePairSelectionPossible = true;
                    }
                }

                if (iFiles > 0)
                {
                    if (signlePairSelectionPossible)
                    {
                        f.checkBox_extendedSelection.Enabled = true;
                        f.checkBox_extendedSelection.Checked = false;
                    }
                    else
                    {
                        f.checkBox_extendedSelection.Enabled = false;
                        f.checkBox_extendedSelection.Checked = true;
                    }
                }
                else
                {
                    f.radioButton_compareAllFiles.Checked = true;
                    f.radioButton_compareSelectedFiles.Enabled = false;
                    f.checkBox_extendedSelection.Enabled = false;
                    f.checkBox_extendedSelection.Checked = false;
                }



                f.ShowDialog();

                if (!f.Saved) return;
                {
                    if (f.comboBox_priceGroup.Items.Count > 0)
                    {
                        foreach (var p in Application.Settings.PriceGroups)
                        {
                            if (string.Compare(p.Name, f.comboBox_priceGroup.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
                                continue;
                            RateGroup = p;
                            break;
                        }
                    }

                    Processor.Settings.ReportFilterFilesWithNoRecordsFiltered = f.checkBox_viewFilesWithNoTranslationDifferences.Checked;
                    Processor.Settings.ShowGoogleChartsInReport = f.checkBox_showGoogleChartsInReport.Checked;
                    Processor.Settings.CalculateSummaryAnalysisBasedOnFilteredRows = f.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Checked;

                    Processor.Settings.ReportFilterSegmentsWithNoChanges = f.checkBox_viewSegmentsWithNoChanges.Checked;
                    Processor.Settings.ReportFilterChangedTargetContent = f.checkBox_viewSegmentsWithTranslationChanges.Checked;
                    Processor.Settings.ReportFilterSegmentStatusChanged = f.checkBox_viewSegmentsWithStatusChanges.Checked;
                    Processor.Settings.ReportFilterSegmentsContainingComments = f.checkBox_viewSegmentsWithComments.Checked;
                    Processor.Settings.ReportFilterLockedSegments = f.checkBox_viewLockedSegments.Checked;

                    Processor.Settings.ReportFilterTranslationMatchValuesOriginal = f.comboBox_segments_match_value_original.SelectedItem.ToString();
                    Processor.Settings.ReportFilterTranslationMatchValuesUpdated = f.comboBox_segments_match_value_updated.SelectedItem.ToString();
                    Processor.Settings.TagVisualStyle = (Settings.TagVisual)Enum.Parse(typeof(Settings.TagVisual), f.tagVisualizationComboBox.SelectedItem.ToString(), true);

                    Processor.Settings.ShowOriginalSourceSegment = f.checkBox_showOriginalSourceSegment.Checked;
                    Processor.Settings.ShowOriginalTargetSegment = f.checkBox_showOriginalTargetSegment.Checked;
                    Processor.Settings.ShowOriginalRevisionMarkerTargetSegment = f.checkBox_showOriginalRevisionMarkerTargetSegment.Checked;
                    Processor.Settings.ShowUpdatedTargetSegment = f.checkBox_showUpdatedTargetSegment.Checked;
                    Processor.Settings.ShowUpdatedRevisionMarkerTargetSegment = f.checkBox_showUpdatedRevisionMarkerTargetSegment.Checked;
                    Processor.Settings.ShowTargetComparison = f.checkBox_showTargetSegmentComparison.Checked;
                    Processor.Settings.ShowSegmentComments = f.checkBox_showSegmentComments.Checked;
                    Processor.Settings.ShowSegmentLocked = f.checkBox_showLockedSegments.Checked;

                    Processor.Settings.ShowSegmentStatus = f.checkBox_showSegmentStatus.Checked;
                    Processor.Settings.ShowSegmentMatch = f.checkBox_showSegmentMatch.Checked;
                    Processor.Settings.ShowSegmentTerp = f.checkBox_showSegmentTERPAnalysis.Checked;
                    Processor.Settings.JavaExecutablePath = f.textBox_javaExecutablePath.Text;
                    Processor.Settings.ShowSegmentPem = f.checkBox_showSegmentPEM.Checked;



                    var pairedFiles = GetPairedFiles(f, lv);



                    if (pairedFiles.Count == 0)
                    {
                        MessageBox.Show(this, Resources.FormMain_CreateReport_Empty_file_comparison_list, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var hitCancel = false;

                        ParseContentFromFiles(comparer, pairedFiles, ref hitCancel);
                        CreateComparisonReport(hitCancel, comparer);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                _timer.Stop();
            }


        }

        private void ParseContentFromFiles(Processor comparer, List<PairedFiles.PairedFile> pairedFiles, ref bool hitCancel)
        {
            try
            {
                comparer.ProgressComparer += ComparerProgress;
                comparer.ProgressParser += ParserProgress;
                comparer.ProgressFiles += FilesProgress;


                ProgressWindow.ProgressDialog = new ProgressDialog();
                try
                {
                    ProgressWindow.ProgressDialogWorker = new BackgroundWorker();

                    ProgressWindow.ProgressDialogWorker.WorkerReportsProgress = true;

                    Exception exParsing = null;
                    var arguments = new List<object> { comparer, pairedFiles, exParsing };

                    ProgressWindow.ProgressDialogWorker.DoWork +=
                        (sender, e) => arguments = worker_CreateReport_DoWork(null, new DoWorkEventArgs(arguments));
                    ProgressWindow.ProgressDialogWorker.RunWorkerCompleted +=
                        ProgressWindowHandlers.progressWorker_RunWorkerCompleted;
                    ProgressWindow.ProgressDialogWorker.ProgressChanged += ProgressWindowHandlers.progressWorker_ProgressChanged;

                    ProgressWindow.ProgressDialogWorker.RunWorkerAsync();
                    ProgressWindow.ProgressDialog.DialogResult = DialogResult.OK;
                    ProgressWindow.ProgressDialog.ShowDialog();
                    if (ProgressWindow.ProgressDialog.HitCancel)
                    {
                        hitCancel = true;

                        ProgressWindow.ProgressDialogWorker.ProgressChanged -=
                            ProgressWindowHandlers.progressWorker_ProgressChanged;
                        ProgressWindow.ProgressDialogWorker.RunWorkerCompleted -=
                            ProgressWindowHandlers.progressWorker_RunWorkerCompleted;
                        ProgressWindow.ProgressDialogWorker.DoWork -=
                            (sender, e) => arguments = worker_CreateReport_DoWork(null, new DoWorkEventArgs(arguments));
                    }
                    exParsing = (Exception)arguments[2];
                    if (exParsing != null)
                        throw exParsing;
                }
                finally
                {
                    ProgressWindow.ProgressDialog.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                comparer.ProgressComparer -= ComparerProgress;
                comparer.ProgressParser -= ParserProgress;
                comparer.ProgressFiles -= FilesProgress;
            }
        }

        private void CreateComparisonReport(bool hitCancel, Processor comparer)
        {
            var reportFileName = Processor.Settings.ReportFileName + "_" + Guid.NewGuid() + "";

            if (!reportFileName.ToLower().EndsWith(".xml"))
                reportFileName += ".xml";

            ReportDialog.ReportFileFullPath = reportFileName;

            if (!hitCancel)
            {
                List<TERp.DocumentResult> terpResults = null;

                Cursor = Cursors.WaitCursor;
                if (FileComparisonParagraphUnits != null)
                {
                    #region  |  create report  |

                    var reportFilePath = reportFileName;

                    const bool includeHeaderTitle = false;

                    comparer.ProgressReport += ComparerProgressReport;

                    var cancel = false;

                    ProgressWindow.ProgressDialog = new ProgressDialog();
                    try
                    {
                        ProgressWindow.ProgressDialogWorker = new BackgroundWorker
                        {
                            WorkerReportsProgress = true
                        };


                        Exception exParsing = null;
                        var arguments = new List<object>
                        {
                            comparer,
                            reportFilePath,
                            FileComparisonParagraphUnits,
                            includeHeaderTitle,
                            RateGroup,
                            terpResults,
                            exParsing
                        };

                        ProgressWindow.ProgressDialogWorker.DoWork +=
                            (sender, e) => arguments = worker_CreateReport2_DoWork(null, new DoWorkEventArgs(arguments));
                        ProgressWindow.ProgressDialogWorker.RunWorkerCompleted +=
                            ProgressWindowHandlers.progressWorker_RunWorkerCompleted;
                        ProgressWindow.ProgressDialogWorker.ProgressChanged +=
                            ProgressWindowHandlers.progressWorker_ProgressChanged;

                        ProgressWindow.ProgressDialogWorker.RunWorkerAsync();
                        ProgressWindow.ProgressDialog.DialogResult = DialogResult.OK;
                        ProgressWindow.ProgressDialog.ShowDialog();
                        if (ProgressWindow.ProgressDialog.HitCancel)
                        {
                            cancel = true;

                            ProgressWindow.ProgressDialogWorker.ProgressChanged -=
                                ProgressWindowHandlers.progressWorker_ProgressChanged;
                            ProgressWindow.ProgressDialogWorker.RunWorkerCompleted -=
                                ProgressWindowHandlers.progressWorker_RunWorkerCompleted;
                            ProgressWindow.ProgressDialogWorker.DoWork -=
                                (sender, e) => arguments = worker_CreateReport_DoWork(null, new DoWorkEventArgs(arguments));
                        }
                        terpResults = (List<TERp.DocumentResult>)arguments[5];
                        exParsing = (Exception)arguments[6];
                        if (exParsing != null)
                            throw exParsing;
                    }
                    finally
                    {
                        ProgressWindow.ProgressDialog.Dispose();

                        comparer.ProgressReport -= ComparerProgressReport;
                    }

                    if (!cancel)
                    {
                        ReportDialog.PanelReportViewer.webBrowserReport.Navigate(
                            new Uri(Path.Combine("file://", reportFileName + ".html")));


                        try
                        {
                            ReportDialog.ViewSegmentsWithNoChanges = Processor.Settings.ReportFilterSegmentsWithNoChanges;
                            ReportDialog.ViewSegmentsWithTranslationChanges = Processor.Settings.ReportFilterChangedTargetContent;
                            ReportDialog.ViewSegmentsWithStatusChanges = Processor.Settings.ReportFilterSegmentStatusChanged;
                            ReportDialog.ViewSegmentsWithComments = Processor.Settings.ReportFilterSegmentsContainingComments;
                            ReportDialog.ViewFilesWithNoDifferences = Processor.Settings.ReportFilterFilesWithNoRecordsFiltered;

                            ReportDialog.toolStripButton_viewSegmentsWithNoChanges.Click += toolStripButton_viewSegmentsWithNoChanges_Click;
                            ReportDialog.toolStripButton_viewSegmentsWithTranslationChanges.Click += toolStripButton_viewSegmentsWithTranslationChanges_Click;
                            ReportDialog.toolStripButton_viewSegmentsWithStatusChanges.Click += toolStripButton_viewSegmentsWithStatusChanges_Click;
                            ReportDialog.toolStripButton_viewSegmentsWithComments.Click += toolStripButton_viewSegmentsWithComments_Click;
                            ReportDialog.toolStripButton_viewFilesWithNoDifferences.Click += toolStripButton_viewFilesWithNoDifferences_Click;


                            ReportDialog.PanelReportViewerNavigation.treeView_navigation.Nodes.Clear();

                            var tnReportHeader = ReportDialog.PanelReportViewerNavigation.treeView_navigation.Nodes.Add("filesId_report_header", Resources.FormMain_CreateComparisonReport_Report_Header);
                            tnReportHeader.Tag = "filesId_report_header";

                            var tnReportTotal = tnReportHeader.Nodes.Add("filesId_total", Resources.FormMain_CreateComparisonReport_Report_Summary);
                            tnReportTotal.Tag = "filesId_total";

                            var tnFiles = ReportDialog.PanelReportViewerNavigation.treeView_navigation.Nodes.Add("filesId_report_files", Resources.FormMain_CreateComparisonReport_Report_Files);
                            tnFiles.Tag = "filesId_report_files";
                            //comparer
                            var iFileIndex = -1;
                            foreach (var fileComparisonFileParagraphUnit in FileComparisonParagraphUnits)
                            {
                                var iFileInnderIndex = 0;
                                iFileIndex++;
                                var fileId = "fileId_" + iFileIndex + "_" + iFileInnderIndex++;


                                var fileUnitProperties = fileComparisonFileParagraphUnit.Key;
                                if (fileUnitProperties.FilePathOriginal.Trim() == string.Empty ||
                                    fileUnitProperties.FilePathUpdated.Trim() == string.Empty)
                                    continue;

                                var tn = tnFiles.Nodes.Add(fileId, Path.GetFileName(fileUnitProperties.FilePathOriginal));
                                tn.Tag = fileId;


                                if (fileComparisonFileParagraphUnit.Value == null)
                                    continue;

                                foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
                                {
                                    var fileInnerId = "fileId_" + iFileIndex + "_" + iFileInnderIndex++;
                                    var comparisonParagraphUnits = fileComparisonParagraphUnit.Value;

                                    #region  |  innerFile  |

                                    var innerFileFilteredParagraphCount = 0;


                                    foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
                                    {
                                        var filteredAsegment = false;
                                        foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                                        {
                                            if ((comparisonSegmentUnit.SegmentTextUpdated || comparisonSegmentUnit.SegmentSegmentStatusUpdated || comparisonSegmentUnit.SegmentHasComments || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                                && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                                && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                                && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments))
                                                continue;
                                            filteredAsegment = true;
                                        }

                                        if (filteredAsegment)
                                            innerFileFilteredParagraphCount++;
                                    }

                                    if (innerFileFilteredParagraphCount <= 0)
                                        continue;

                                    var name = Path.GetFileName(fileComparisonParagraphUnit.Key);
                                    var _tn = tn.Nodes.Add(fileInnerId, name);
                                    _tn.Tag = fileInnerId;

                                    #endregion
                                }
                            }


                            ReportDialog.ShowDialog();
                        }
                        catch
                        {
                            Cursor = Cursors.Default;
                            throw;
                        }
                        finally
                        {
                            Cursor = Cursors.Default;

                            ReportDialog.toolStripButton_viewSegmentsWithNoChanges.Click += toolStripButton_viewSegmentsWithNoChanges_Click;
                            ReportDialog.toolStripButton_viewSegmentsWithTranslationChanges.Click += toolStripButton_viewSegmentsWithTranslationChanges_Click;
                            ReportDialog.toolStripButton_viewSegmentsWithStatusChanges.Click += toolStripButton_viewSegmentsWithStatusChanges_Click;
                            ReportDialog.toolStripButton_viewSegmentsWithComments.Click += toolStripButton_viewSegmentsWithComments_Click;
                            ReportDialog.toolStripButton_viewFilesWithNoDifferences.Click += toolStripButton_viewFilesWithNoDifferences_Click;
                        }
                    }

                    #endregion
                }

                if (Application.Settings.ReportsAutoSave)
                {
                    if (Application.Settings.ReportsAutoSaveFullPath.Trim() != string.Empty && Directory.Exists(Application.Settings.ReportsAutoSaveFullPath))
                    {
                        try
                        {
                            var reportPathAutoSave = Application.Settings.ReportsAutoSaveFullPath;
                            var reportNameAutoSave = Path.GetFileName(Processor.Settings.ReportFileName);
                            if (reportNameAutoSave != null)
                                reportNameAutoSave = reportNameAutoSave.Substring(0, reportNameAutoSave.Length - 4);

                            reportNameAutoSave += "." + DateTime.Now.Year
                                                  + "" + DateTime.Now.Month.ToString().PadLeft(2, '0')
                                                  + "" + DateTime.Now.Day.ToString().PadLeft(2, '0')
                                                  + "T" + DateTime.Now.Hour.ToString().PadLeft(2, '0')
                                                  + "" + DateTime.Now.Minute.ToString().PadLeft(2, '0')
                                                  + "" + DateTime.Now.Second.ToString().PadLeft(2, '0');

                            if (Application.Settings.ReportsCreateMonthlySubFolders)
                            {
                                reportPathAutoSave = Path.Combine(reportPathAutoSave,
                                    DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0'));
                                if (!Directory.Exists(reportPathAutoSave))
                                    Directory.CreateDirectory(reportPathAutoSave);
                            }

                            var reportFullPathAutoSave = Path.Combine(reportPathAutoSave, reportNameAutoSave);

                            File.Copy(reportFileName + ".html", reportFullPathAutoSave + ".html", true);
                            File.Copy(reportFileName, reportFullPathAutoSave + ".xml", true);
                            File.Delete(reportFileName);

                            if (terpResults != null)
                            {
                                foreach (var documentResult in terpResults)
                                {
                                    if (!File.Exists(documentResult.HtmlPath))
                                        continue;

                                    var terpFileName = Path.GetFileName(documentResult.HtmlPath);

                                    File.Move(documentResult.HtmlPath, reportFullPathAutoSave + "." + terpFileName);
                                }
                            }

                            #region  |  ComparisonLogEntry  |

                            var comparisonLogEntry = new Settings.ComparisonLogEntry(
                                Settings.ComparisonLogEntry.EntryType.ReportCreate)
                            {
                                Date = DateTime.Now,
                                Id = Guid.NewGuid().ToString(),
                                ItemName = reportNameAutoSave,
                                Value01 = reportPathAutoSave,
                                Value02 = string.Empty,
                                Items = null
                            };

                            update_comparison_log(comparisonLogEntry);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Cursor = Cursors.Default;
                            MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        finally
                        {
                            Cursor = Cursors.Default;
                        }
                    }
                }
            }

            try
            {
                if (File.Exists(reportFileName + ".html"))
                    File.Delete(reportFileName + ".html");
            }
            catch
            {
                // ignored
            }
        }

        private void toolStripButton_viewSegmentsWithNoChanges_Click(object sender, EventArgs e)
        {
            ReportDialog.ViewSegmentsWithNoChanges = !ReportDialog.ViewSegmentsWithNoChanges;
            ReportDialog.toolStripButton_viewSegmentsWithNoChanges.CheckState = ReportDialog.ViewSegmentsWithNoChanges ? CheckState.Checked : CheckState.Unchecked;
            Processor.Settings.ReportFilterSegmentsWithNoChanges = ReportDialog.ViewSegmentsWithNoChanges;

            ReQueryReport();

        }

        private void toolStripButton_viewSegmentsWithTranslationChanges_Click(object sender, EventArgs e)
        {
            ReportDialog.ViewSegmentsWithTranslationChanges = !ReportDialog.ViewSegmentsWithTranslationChanges;
            ReportDialog.toolStripButton_viewSegmentsWithTranslationChanges.CheckState = ReportDialog.ViewSegmentsWithTranslationChanges ? CheckState.Checked : CheckState.Unchecked;
            Processor.Settings.ReportFilterChangedTargetContent = ReportDialog.ViewSegmentsWithTranslationChanges;

            ReQueryReport();

        }

        private void toolStripButton_viewSegmentsWithStatusChanges_Click(object sender, EventArgs e)
        {
            ReportDialog.ViewSegmentsWithStatusChanges = !ReportDialog.ViewSegmentsWithStatusChanges;
            ReportDialog.toolStripButton_viewSegmentsWithStatusChanges.CheckState = ReportDialog.ViewSegmentsWithStatusChanges ? CheckState.Checked : CheckState.Unchecked;
            Processor.Settings.ReportFilterSegmentStatusChanged = ReportDialog.ViewSegmentsWithStatusChanges;

            ReQueryReport();

        }

        private void toolStripButton_viewSegmentsWithComments_Click(object sender, EventArgs e)
        {
            ReportDialog.ViewSegmentsWithComments = !ReportDialog.ViewSegmentsWithComments;
            ReportDialog.toolStripButton_viewSegmentsWithComments.CheckState = ReportDialog.ViewSegmentsWithComments ? CheckState.Checked : CheckState.Unchecked;
            Processor.Settings.ReportFilterSegmentsContainingComments = ReportDialog.ViewSegmentsWithComments;

            ReQueryReport();
        }

        private void toolStripButton_viewFilesWithNoDifferences_Click(object sender, EventArgs e)
        {
            ReportDialog.ViewFilesWithNoDifferences = !ReportDialog.ViewFilesWithNoDifferences;
            ReportDialog.toolStripButton_viewFilesWithNoDifferences.CheckState = ReportDialog.ViewFilesWithNoDifferences ? CheckState.Checked : CheckState.Unchecked;
            Processor.Settings.ReportFilterFilesWithNoRecordsFiltered = ReportDialog.ViewFilesWithNoDifferences;

            ReQueryReport();

        }

        private List<PairedFiles.PairedFile> GetPairedFiles(ReportWizard f, ListView lv)
        {
            var pairedFiles = new List<PairedFiles.PairedFile>();
            if (f.radioButton_compareSelectedFiles.Checked && lv.SelectedIndices.Count > 0)
            {
                #region  |  selected files  |

                var singleLeftPath = string.Empty;
                var singleRightPath = string.Empty;

                if (lv.SelectedIndices.Count == 2 && f.checkBox_extendedSelection.Checked == false)
                {
                    var lvi01 = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];
                    var lvi02 = _panelCompare.listView_main.Items[lv.SelectedIndices[1]];

                    var dn01 = (DataNode)lvi01.Tag;
                    var dn02 = (DataNode)lvi02.Tag;

                    switch (dn01.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (File.Exists(dn01.PathLeft))
                            {
                                singleLeftPath = dn01.PathLeft;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (File.Exists(dn01.PathRight))
                            {
                                singleRightPath = dn01.PathRight;
                            }
                            break;
                    }

                    switch (dn02.SelectionType)
                    {
                        case DataNode.Selection.Left:
                            if (File.Exists(dn02.PathLeft))
                            {
                                singleLeftPath = dn02.PathLeft;
                            }
                            break;
                        case DataNode.Selection.Right:
                            if (File.Exists(dn02.PathRight))
                            {
                                singleRightPath = dn02.PathRight;
                            }
                            break;
                    }

                    if (singleLeftPath.Trim() == string.Empty || singleRightPath.Trim() == string.Empty)
                        return pairedFiles;
                    var pf = new PairedFiles.PairedFile
                    {
                        IsError = false,
                        Message = string.Empty,
                        OriginalFilePath = new FileInfo(singleLeftPath),
                        UpdatedFilePath = new FileInfo(singleRightPath)
                    };


                    pairedFiles.Add(pf);
                }
                else
                {
                    for (var i = 0; i < lv.SelectedIndices.Count; i++)
                    {
                        var lvi = _panelCompare.listView_main.Items[lv.SelectedIndices[i]];
                        var dn = (DataNode)lvi.Tag;

                        #region   |  if (dn.Type == DataNode.ItemType.file)  |

                        if (dn.Type != DataNode.ItemType.File) continue;

                        var pf = new PairedFiles.PairedFile
                        {
                            IsError = false,
                            Message = string.Empty
                        };


                        if (!dn.PathLeft.Trim().ToLower().EndsWith(".sdlxliff") &&
                            !dn.PathRight.Trim().ToLower().EndsWith(".sdlxliff")) continue;
                        if (File.Exists(dn.PathLeft))
                        {
                            pf.OriginalFilePath = new FileInfo(dn.PathLeft);
                        }
                        else
                        {
                            pf.OriginalFilePath = null;
                            pf.IsError = true;
                            pf.Message = Resources.FormMain_GetPairedFiles_Unable_to_locate_left_file;
                        }

                        if (File.Exists(dn.PathRight))
                        {
                            pf.UpdatedFilePath = new FileInfo(dn.PathRight);
                        }
                        else
                        {
                            pf.UpdatedFilePath = null;
                            pf.IsError = true;
                            pf.Message = Resources.FormMain_GetPairedFiles_Unable_to_locate_right_file;
                        }
                        pairedFiles.Add(pf);

                        #endregion
                    }
                }

                #endregion
            }
            else
            {
                if (f.checkBox_includeAllSubfolders.Checked)
                {
                    #region  |  include all subfolders   |

                    _mMapperReport.Clear();
                    ObtainAllNodesPlusSubFolders(_mMapper);

                    if (_panelCompare.listView_main.Items.Count <= 0) return pairedFiles;
                    foreach (var dn in _mMapperReport)
                    {
                        #region   |  if (dn.Type == DataNode.ItemType.file)  |

                        if (dn.Type != DataNode.ItemType.File) continue;
                        var pf = new PairedFiles.PairedFile
                        {
                            IsError = false,
                            Message = string.Empty
                        };


                        if (!dn.PathLeft.Trim().ToLower().EndsWith(".sdlxliff") &&
                            !dn.PathRight.Trim().ToLower().EndsWith(".sdlxliff")) continue;
                        if (File.Exists(dn.PathLeft))
                        {
                            pf.OriginalFilePath = new FileInfo(dn.PathLeft);
                        }
                        else
                        {
                            pf.OriginalFilePath = null;
                            pf.IsError = true;
                            pf.Message = Resources.FormMain_GetPairedFiles_Unable_to_locate_left_file;
                        }

                        if (File.Exists(dn.PathRight))
                        {
                            pf.UpdatedFilePath = new FileInfo(dn.PathRight);
                        }
                        else
                        {
                            pf.UpdatedFilePath = null;
                            pf.IsError = true;
                            pf.Message = Resources.FormMain_GetPairedFiles_Unable_to_locate_right_file;
                        }


                        pairedFiles.Add(pf);

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    if (_panelCompare.listView_main.Items.Count <= 0) return pairedFiles;
                    for (var i = 0; i < _panelCompare.listView_main.Items.Count; i++)
                    {
                        var lvi = _panelCompare.listView_main.Items[i];
                        var dn = (DataNode)lvi.Tag;

                        #region   |  if (dn.Type == DataNode.ItemType.file)  |

                        if (dn.Type != DataNode.ItemType.File) continue;

                        var pf = new PairedFiles.PairedFile
                        {
                            IsError = false,
                            Message = string.Empty
                        };


                        if (!dn.PathLeft.Trim().ToLower().EndsWith(".sdlxliff") &&
                            !dn.PathRight.Trim().ToLower().EndsWith(".sdlxliff")) continue;
                        if (File.Exists(dn.PathLeft))
                        {
                            pf.OriginalFilePath = new FileInfo(dn.PathLeft);
                        }
                        else
                        {
                            pf.OriginalFilePath = null;
                            pf.IsError = true;
                            pf.Message = Resources.FormMain_GetPairedFiles_Unable_to_locate_left_file;
                        }

                        if (File.Exists(dn.PathRight))
                        {
                            pf.UpdatedFilePath = new FileInfo(dn.PathRight);
                        }
                        else
                        {
                            pf.UpdatedFilePath = null;
                            pf.IsError = true;
                            pf.Message = Resources.FormMain_GetPairedFiles_Unable_to_locate_right_file;
                        }


                        pairedFiles.Add(pf);

                        #endregion
                    }
                }
            }

            return pairedFiles;
        }

        private static void WebBrowserReportDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (ReportDialog.PanelReportViewer.webBrowserReport.ReadyState == WebBrowserReadyState.Complete)
            {
                //ReportDialog.PanelReportViewer.Invalidate();
            }
        }


        private void ComparerProgressReport(int filesMax, int filesCurrent, string fileNameCurrent, int fileMaximum, int fileCurrent, int filePercent, string message)
        {
            #region  |  progressObject  |

            if (!ProgressStatus) return;
            ProgressStatus = false;

            ProgressObject = new ProgressObject
            {
                ProgessTitle = Resources.FormMain_ComparerProgressReport_Creating_Report,
                CurrentProcessingMessage = fileNameCurrent
            };



            var fileProgressValue = Convert.ToDecimal(fileMaximum) > 0 ? Convert.ToInt32(Convert.ToDecimal(fileCurrent) / Convert.ToDecimal(fileMaximum) * Convert.ToDecimal(100)) : 0;
            ProgressObject.CurrentProgressValue = fileProgressValue;
            ProgressObject.CurrentProgressValueMessage = message;



            var totalProgressValueMessage = string.Format(Resources.FormMain_ComparerProgressReport_Processing_file_0_of_1_files, filesCurrent, filesMax);
            var totalProgressValue = Convert.ToDecimal(filesMax) > 0 ? Convert.ToInt32(Convert.ToDecimal(filesCurrent) / Convert.ToDecimal(filesMax) * Convert.ToDecimal(100)) : 0;
            ProgressObject.TotalProgressValue = totalProgressValue;

            ProgressObject.TotalProgressValueMessage = totalProgressValueMessage;



            ProgressWindow.ProgressDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);

            #endregion
        }


        private static void LoadFileViewer()
        {
            var f = new FileViewer();
            f.ShowDialog();
        }

        private List<object> worker_CreateReport_DoWork(object sender, DoWorkEventArgs e)
        {
            var objects = (List<object>)e.Argument;
            var comparer = (Processor)objects[0];
            var pairedFiles = (List<PairedFiles.PairedFile>)objects[1];

            try
            {
                FileComparisonParagraphUnits = comparer.ProcessFiles(pairedFiles);
            }
            catch (Exception ex)
            {
                var exParsing = ex;
                objects[2] = exParsing;
            }

            return objects;
        }
        private static List<object> worker_CreateReport2_DoWork(object sender, DoWorkEventArgs e)
        {
            var objects = (List<object>)e.Argument;
            var comparer = (Processor)objects[0];
            var reportFilePath = objects[1].ToString();
            var fileComparisonParagraphUnits = (Dictionary<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>>)objects[2];
            var includeHeaderTitle = (bool)objects[3];
            var priceGroup = (Settings.PriceGroup)objects[4];
            var terpResults = (List<TERp.DocumentResult>)objects[5];
            var exParsing = (Exception)objects[6];

            try
            {
                comparer.CreateReport(reportFilePath, fileComparisonParagraphUnits, priceGroup, out terpResults);
                objects[5] = terpResults;
            }
            catch (Exception ex)
            {
                exParsing = ex;
                objects[6] = exParsing;
            }

            return objects;
        }



        private void ReQueryReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;


                var comparer = new Processor();

                var fileComparisonParagraphUnits = new Dictionary<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>>();
                foreach (var fileComparisonFileParagraphUnit in FileComparisonParagraphUnits)
                {
                    var fileProperties = fileComparisonFileParagraphUnit.Key;
                    if (fileComparisonFileParagraphUnit.Value != null)
                        fileProperties = new Comparer.FileUnitProperties(fileComparisonFileParagraphUnit.Key, fileComparisonFileParagraphUnit.Value);

                    fileComparisonParagraphUnits.Add(fileProperties, fileComparisonFileParagraphUnit.Value);
                }

                List<TERp.DocumentResult> terpResults;
                comparer.CreateReport(ReportDialog.ReportFileFullPath, fileComparisonParagraphUnits, RateGroup, out terpResults);

                ReportDialog.PanelReportViewer.webBrowserReport.Url = new Uri(Path.Combine("file://", ReportDialog.ReportFileFullPath + ".html"));
                System.Windows.Forms.Application.DoEvents();
                ReportDialog.PanelReportViewer.webBrowserReport.Focus();
                ReportDialog.PanelReportViewer.webBrowserReport.Refresh();

                if (terpResults == null)
                    return;
                foreach (var documentResult in terpResults)
                {
                    if (!File.Exists(documentResult.HtmlPath))
                        continue;

                    File.Delete(documentResult.HtmlPath);
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }


        private void toolStripButton_createReport_Click(object sender, EventArgs e)
        {
            CreateReport();
        }

        private void toolStripButton_compareInViewer_Click(object sender, EventArgs e)
        {
            LoadFileViewer();
        }

        private void createComparisonReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateReport();
        }


        #endregion
        #region  |  events log  |


        private void update_comparison_log(Settings.ComparisonLogEntry comparisonLogEntry)
        {


            Application.Settings.ComparisonLogEntries.Insert(0, comparisonLogEntry);

            if (Application.Settings.ComparisonLogEntries.Count > Application.Settings.ComparisonLogMaxEntries)
            {
                Application.Settings.ComparisonLogEntries.RemoveRange(Application.Settings.ComparisonLogMaxEntries,
                    Application.Settings.ComparisonLogEntries.Count - Application.Settings.ComparisonLogMaxEntries);
            }


            CreateEntriesLogReport();

        }

        private void CreateEntriesLogReport()
        {
            if (_panelEventsLog == null || _panelEventsLog.DockPanel == null) return;
            var assmebly = Assembly.GetExecutingAssembly();
            var files = assmebly.GetManifestResourceNames();
            var xmlFileFullPath = Path.Combine(Application.Settings.ApplicationSettingsPath, "PostEdit.Compare.EventLog.xml");



            var xmlTxtWriter = new XmlTextWriter(xmlFileFullPath, Encoding.UTF8)
            {
                Formatting = Formatting.None,
                Indentation = 3,
                Namespaces = false
            };
            xmlTxtWriter.WriteStartDocument(true);



            xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + "PostEdit.Compare.EventLog.xslt" + "'");
            xmlTxtWriter.WriteComment("SDLXLIFF Compare by Patrick Hartnett, 2011");

            xmlTxtWriter.WriteStartElement("log_entries");

            foreach (var cle in Application.Settings.ComparisonLogEntries)
            {
                #region  |  add to list  |




                var addToList = !(!Application.Settings.EventsLogTrackCompare
                                  && (cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonFoldersCompare
                                      || cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonFoldersLeftChange
                                      || cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonFoldersRightChange
                                      || cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonProjectCompare));
                if (!Application.Settings.EventsLogTrackReports
                    &&
                    (
                        cle.Type == Settings.ComparisonLogEntry.EntryType.ReportCreate
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.ReportSave))
                {
                    addToList = false;

                }
                if (!Application.Settings.EventsLogTrackProjects
                    && (cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonProjectEdit
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonProjectNew
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonProjectRemove
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.ComparisonProjectSave))
                {
                    addToList = false;
                }
                if (!Application.Settings.EventsLogTrackFiles
                    && (cle.Type == Settings.ComparisonLogEntry.EntryType.FilesCopy
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.FilesDelete
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.FilesMove))
                {
                    addToList = false;
                }
                if (!Application.Settings.EventsLogTrackFilters
                    && (cle.Type == Settings.ComparisonLogEntry.EntryType.FilterEdit
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.FiltersAdd
                        || cle.Type == Settings.ComparisonLogEntry.EntryType.FiltersDelete))
                {
                    addToList = false;
                }

                #endregion

                switch (cle.Type)
                {
                    case Settings.ComparisonLogEntry.EntryType.ReportCreate:
                        cle.Action = Resources.FormMain_CreateEntriesLogReport_Saved_Report__Auto;
                        break;
                    case Settings.ComparisonLogEntry.EntryType.ReportSave:
                        addToList = false;
                        break;
                }


                if (!addToList) continue;

                xmlTxtWriter.WriteStartElement("log_entry");

                xmlTxtWriter.WriteAttributeString("id", cle.Id);
                xmlTxtWriter.WriteAttributeString("date", cle.Date.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("type", cle.Type.ToString());
                xmlTxtWriter.WriteAttributeString("itemName", cle.ItemName);
                xmlTxtWriter.WriteAttributeString("action", cle.Action);
                xmlTxtWriter.WriteAttributeString("value01", cle.Value01);
                xmlTxtWriter.WriteAttributeString("value02", cle.Value02);


                if (cle.Items != null && cle.Items.Count > 0)
                {
                    xmlTxtWriter.WriteStartElement("items");


                    xmlTxtWriter.WriteAttributeString("count", cle.Items.Count.ToString());

                    foreach (var item in cle.Items)
                    {
                        xmlTxtWriter.WriteStartElement("item");

                        xmlTxtWriter.WriteAttributeString("option", item.Option);
                        xmlTxtWriter.WriteAttributeString("from", item.From);
                        xmlTxtWriter.WriteAttributeString("to", item.To);
                        xmlTxtWriter.WriteEndElement();//item
                    }



                    xmlTxtWriter.WriteEndElement();//items

                }



                xmlTxtWriter.WriteEndElement();//log_entry
            }



            xmlTxtWriter.WriteEndElement();//log_entries



            xmlTxtWriter.WriteEndDocument();
            xmlTxtWriter.Flush();
            xmlTxtWriter.Close();



            WriteReportResourcesToDirectory(Application.Settings.ApplicationSettingsPath);


            TransformComparisonLogXmlReport(xmlFileFullPath);




            _panelEventsLog.webBrowser1.Url = new Uri(Path.Combine("file://", Path.Combine(Application.Settings.ApplicationSettingsPath, "PostEdit.Compare.EventLog.xml" + ".html")));
            _panelEventsLog.webBrowser1.Refresh();


            System.Windows.Forms.Application.DoEvents();
        }

        private static void TransformComparisonLogXmlReport(string reportFilePath)
        {


            var filePathXslt = Path.Combine(Path.GetDirectoryName(reportFilePath), "PostEdit.Compare.EventLog.xslt");



            var xsltSetting = new XsltSettings();
            xsltSetting.EnableDocumentFunction = true;
            xsltSetting.EnableScript = true;


            var myXPathDoc = new XPathDocument(reportFilePath);


            var myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(filePathXslt, xsltSetting, null);



            var myWriter = new XmlTextWriter(reportFilePath + ".html", Encoding.UTF8);
            myXslTrans.Transform(myXPathDoc, null, myWriter);


            myWriter.Flush();
            myWriter.Close();



        }
        private static void WriteReportResourcesToDirectory(string reportDirectory)
        {
            var filePathXslt = Path.Combine(reportDirectory, "PostEdit.Compare.EventLog.xslt");
            //var asb = Assembly.GetEntryAssembly();
            var asb = Assembly.GetExecutingAssembly();
            const string templateXsltName = "Sdl.Community.PostEdit.Compare.Reports.EventLog.StyleSheet.01.xslt";
            var resources = asb.GetManifestResourceNames();
            using (var inputStream = asb.GetManifestResourceStream(templateXsltName))
            {

                Stream outputStream = File.Open(filePathXslt, FileMode.Create);

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



        #endregion





        public static string CalculateFileSize(long numBytes)
        {
            string fileSize;

            if (numBytes > 1073741824)
                fileSize = string.Format("{0:0.00} Gb", (double)numBytes / 1073741824);
            else if (numBytes > 1048576)
                fileSize = string.Format("{0:0.00} Mb", (double)numBytes / 1048576);
            else
                fileSize = string.Format("{0:0} Kb", (double)numBytes / 1024);


            if (fileSize != "0 Kb") return fileSize;
            fileSize = numBytes > 0 ? "1 Kb" : "0 kb";
            return fileSize;
        }



        private Settings.PriceGroup RateGroup { get; set; }


        private ProgressObject ProgressObject { get; set; }
        private int ProgressPercentage { get; set; }
        private bool ProgressStatus { get; set; }

        private readonly Timer _timer = new Timer();
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (ProgressPercentage != 80)
                ProgressPercentage++;
            else
                ProgressPercentage = 1;

            ProgressStatus = true;
        }
        private int ProgressTotalProgressValue { get; set; }
        private string ProgressTotalProgressValueMessage { get; set; }

        private void ComparerProgress(int maximum, int current, int percent, string message)
        {

            if (!ProgressStatus) return;
            ProgressStatus = false;

            ProgressObject = new ProgressObject
            {
                ProgessTitle = Resources.FormMain_ComparerProgress_Processing_Files,
                CurrentProcessingMessage = message,
                CurrentProgressValue = percent,
                CurrentProgressValueMessage = Resources.FormMain_ComparerProgress_Comparing_segments,
                TotalProgressValue = ProgressTotalProgressValue,
                TotalProgressValueMessage = ProgressTotalProgressValueMessage
            };

            ProgressWindow.ProgressDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);


        }
        private void ParserProgress(int maximum, int current, int percent, string message)
        {

            if (!ProgressStatus) return;
            ProgressStatus = false;

            ProgressObject = new ProgressObject
            {
                ProgessTitle = Resources.FormMain_ComparerProgress_Processing_Files,
                CurrentProcessingMessage = message,
                CurrentProgressValue = percent,
                CurrentProgressValueMessage = Resources.FormMain_ParserProgress_Parsing_segments,
                TotalProgressValue = ProgressTotalProgressValue,
                TotalProgressValueMessage = ProgressTotalProgressValueMessage
            };


            ProgressWindow.ProgressDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);

        }
        private void FilesProgress(int maximum, int current, int percent, string message)
        {


            ProgressStatus = false;

            ProgressObject = new ProgressObject
            {
                ProgessTitle = Resources.FormMain_ComparerProgress_Processing_Files,
                CurrentProcessingMessage = message,
                CurrentProgressValue = 0,
                CurrentProgressValueMessage = Resources.FormMain_FilesProgress_Parsing_comparing_segments,
                TotalProgressValue = percent,
                TotalProgressValueMessage = string.Format(Resources.FormMain_FilesProgress_Processing_0_of_1_files, current, maximum)
            };



            ProgressTotalProgressValue = percent;
            ProgressTotalProgressValueMessage = ProgressObject.TotalProgressValueMessage;


            ProgressWindow.ProgressDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);



        }


        private bool CompareToActive { get; set; }
        private bool CompareFilePairActive { get; set; }
        private void compareToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CompareToActive = true;
                Cursor = Cursors.Hand;


            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void compareFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareFilePairActive = true;
            CreateReport();

        }



        private void CalculateFoldersSize(DataNode dn, ref int folders, ref int files)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                foreach (var _dn in _mModel.DataPool)
                {
                    if (_dn.Type == DataNode.ItemType.Folder)
                    {
                        long sizeLeft = 0;
                        long sizeRight = 0;

                        CalculateFolderChilderen(_dn, ref sizeLeft, ref sizeRight, ref folders, ref files);

                        _dn.SizeLeft = sizeLeft;
                        _dn.SizeRight = sizeRight;
                    }
                    else
                    {
                        if (AddToFilteredList(dn))
                        {
                            files++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void CalculateFolderChilderen(DataNode dn, ref long sizeLeft, ref long sizeRight, ref int folders, ref int files)
        {
            if (dn.Type == DataNode.ItemType.Folder)
            {
                if (dn.Children.Count > 0)
                {
                    foreach (var _dn in dn.Children)
                    {
                        long sizeLeftB = 0;// sizeLeft;
                        long sizeRightB = 0;// sizeRight;

                        folders++;


                        CalculateFolderChilderen(_dn, ref sizeLeftB, ref sizeRightB, ref folders, ref files);

                        sizeLeft += sizeLeftB;
                        sizeRight += sizeRightB;


                    }
                    dn.SizeLeft = sizeLeft;
                    dn.SizeRight = sizeRight;
                }
            }
            else
            {
                if (!AddToFilteredList(dn)) return;
                sizeLeft += dn.SizeLeft;
                sizeRight += dn.SizeRight;


                files++;
            }
        }


        private void ExpandAll()
        {
            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();


                var lv = _panelCompare.listView_main;


                if (lv.Items.Count <= 0) return;
                for (var i = 0; i < lv.Items.Count; i++)
                {

                    var lvi = _panelCompare.listView_main.Items[i];

                    var dn = (DataNode)lvi.Tag;



                    var mbr = _mMapper[lvi.Index];


                    dn.Expanded = true;
                    mbr.Expanded = true;
                    _mMapper[lvi.Index].Expanded = true;
                    lvi.Checked = true;

                    ExpandDataNode(mbr);



                    PrepareNodes(lvi.Index, _mMapper[lvi.Index].Expanded);

                }

                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }


        }
        private static void ExpandDataNode(DataNode dn)
        {
            if (dn.Type != DataNode.ItemType.Folder) return;
            dn.Expanded = true;

            if (dn.Children.Count <= 0) return;
            foreach (var _dn in dn.Children)
            {
                ExpandDataNode(_dn);
            }
        }

        private void CollapseAll()
        {

            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();




                var lv = _panelCompare.listView_main;


                if (lv.Items.Count <= 0) return;
                for (var i = 0; i < lv.Items.Count; i++)
                {

                    var lvi = _panelCompare.listView_main.Items[i];

                    var dn = (DataNode)lvi.Tag;



                    var mbr = _mMapper[lvi.Index];



                    dn.Expanded = false;
                    mbr.Expanded = false;
                    _mMapper[lvi.Index].Expanded = false;
                    lvi.Checked = false;


                    CollapseDataNode(mbr);



                    PrepareNodes(lvi.Index, _mMapper[lvi.Index].Expanded);

                    lv.RedrawItems(lvi.Index, lvi.Index, false);


                }

                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }


        }
        private static void CollapseDataNode(DataNode dn)
        {
            if (dn.Type != DataNode.ItemType.Folder) return;
            dn.Expanded = false;
            if (dn.Children.Count <= 0) return;
            foreach (var _dn in dn.Children)
            {
                CollapseDataNode(_dn);
            }
        }


        private void ExpanceAllCurrentNode(object sender, EventArgs e)
        {
            var lv = _panelCompare.listView_main;


            if (lv.SelectedIndices.Count <= 0) return;
            var lvi = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];

            var dn = (DataNode)lvi.Tag;



            var mbr = _mMapper[lvi.Index];


            dn.Expanded = true;
            mbr.Expanded = true;
            _mMapper[lvi.Index].Expanded = true;
            lvi.Checked = true;

            lv.RedrawItems(lvi.Index, lvi.Index, false);

            ExpandDataNode(mbr);



            PrepareNodes(lvi.Index, _mMapper[lvi.Index].Expanded);

            _panelCompare.listView_main.Invalidate(false);
        }
        private void CollapseAllCurrentNode(object sender, EventArgs e)
        {
            var lv = _panelCompare.listView_main;


            if (lv.SelectedIndices.Count <= 0) return;
            var lvi = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];

            var dn = (DataNode)lvi.Tag;



            var mbr = _mMapper[lvi.Index];


            dn.Expanded = true;
            mbr.Expanded = true;
            _mMapper[lvi.Index].Expanded = true;
            lvi.Checked = true;

            lv.RedrawItems(lvi.Index, lvi.Index, false);

            CollapseDataNode(mbr);



            PrepareNodes(lvi.Index, _mMapper[lvi.Index].Expanded);


            _panelCompare.listView_main.Invalidate(false);
        }



        private LinearGradientBrush CreateBrush()
        {
            var br = new LinearGradientBrush(_mRect, _mColorFrom, _mColorTo, 90f) { Blend = _mBlend };
            return br;
        }


        private int _iStartSelectionColumn = 4;
        private int _iEndSelectionColumn = 4;
        private int _iStartSelectionIndex = -1;
        private int _iEndSelectionIndex = -1;

        private int _iRightMouseClickLocationX = -1;
        private int _iRightMouseClickLocationY = -1;

        private void CheckEnabledButtonToolbarMain()
        {


            if (_panelCompare.listView_main.SelectedIndices.Count > 0)
            {


                var folderLeftInSelection = false;
                var folderRightInSelection = false;


                foreach (int index in _panelCompare.listView_main.SelectedIndices)
                {
                    var lvi01 = _panelCompare.listView_main.Items[index];

                    var dn01 = (DataNode)lvi01.Tag;

                    if (dn01.Type == DataNode.ItemType.Folder)
                    {
                        switch (dn01.SelectionType)
                        {
                            case DataNode.Selection.Left:
                                folderLeftInSelection = true;
                                break;
                            case DataNode.Selection.Right:
                                folderRightInSelection = true;
                                break;
                            case DataNode.Selection.Middle:
                                folderLeftInSelection = true;
                                folderRightInSelection = true;
                                break;
                        }
                    }
                    else
                    {
                        switch (dn01.SelectionType)
                        {
                            case DataNode.Selection.Left:
                                break;
                            case DataNode.Selection.Right:
                                break;
                            case DataNode.Selection.Middle:
                                break;
                        }
                    }
                }






                if (folderLeftInSelection)
                {
                    _panelCompare.toolStripButton_setBaseFolder_leftSide.Enabled = true;
                    setBaseFolderleftSideToolStripMenuItem.Enabled = true;
                }
                else
                {
                    _panelCompare.toolStripButton_setBaseFolder_leftSide.Enabled = false;
                    setBaseFolderleftSideToolStripMenuItem.Enabled = false;
                }
                if (folderRightInSelection)
                {
                    _panelCompare.toolStripButton_setBaseFolder_rightSide.Enabled = true;
                    setBaseFolderrightSideToolStripMenuItem.Enabled = true;
                }
                else
                {
                    _panelCompare.toolStripButton_setBaseFolder_rightSide.Enabled = false;
                    setBaseFolderrightSideToolStripMenuItem.Enabled = false;
                }

                if (folderLeftInSelection || folderRightInSelection)
                {
                    toolStripButton_expandAllCurrentFolder.Enabled = true;
                    toolStripButton_collapseCurrentFolder.Enabled = true;
                }
                else
                {
                    toolStripButton_expandAllCurrentFolder.Enabled = false;
                    toolStripButton_collapseCurrentFolder.Enabled = false;
                }

            }
            else
            {


                _panelCompare.toolStripButton_setBaseFolder_leftSide.Enabled = false;
                _panelCompare.toolStripButton_setBaseFolder_rightSide.Enabled = false;

                setBaseFolderleftSideToolStripMenuItem.Enabled = false;
                setBaseFolderrightSideToolStripMenuItem.Enabled = false;

                toolStripButton_expandAllCurrentFolder.Enabled = false;
                toolStripButton_collapseCurrentFolder.Enabled = false;
            }

            if (_panelCompare.Pane != null && _panelCompare.listView_main.Items.Count > 0)
            {
                toolStripButton_expandAll.Enabled = true;
                toolStripButton_collapseAll.Enabled = true;
            }
            else
            {
                toolStripButton_expandAll.Enabled = false;
                toolStripButton_collapseAll.Enabled = false;
            }
        }



        private void ExpandCollapseNodes(object sender, MouseEventArgs e)
        {


            var compareToActivated = CompareToActive;



            CompareToActive = false;
            CompareFilePairActive = false;

            try
            {
                Cursor = Cursors.WaitCursor;

                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {

                    if (e.Button == MouseButtons.Left)
                    {
                        _iRightMouseClickLocationX = e.X;
                        _iRightMouseClickLocationY = e.Y;
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        _iRightMouseClickLocationX = e.X;
                        _iRightMouseClickLocationY = e.Y;
                    }

                    var lv = (ListView)sender;


                    var lvi = lv.GetItemAt(e.X, e.Y);
                    var subItem = lvi.GetSubItemAt(e.X, e.Y);

                    var dn = (DataNode)lvi.Tag;


                    #region  |  get SelectionColumn  |
                    var iColumnIndex = -1;
                    for (var i = 0; i < lvi.SubItems.Count; i++)
                    {
                        if (lvi.SubItems[i] == subItem)
                        {
                            iColumnIndex = i;
                            break;
                        }
                    }
                    _iEndSelectionColumn = iColumnIndex;

                    if ((ModifierKeys & Keys.Control) == Keys.Control
                        || compareToActivated)
                    {
                        _iStartSelectionColumn = iColumnIndex;
                        _iStartSelectionIndex = lvi.Index;
                        _iEndSelectionIndex = -1;
                    }
                    else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        if (_iStartSelectionIndex == -1)
                        {
                            _iStartSelectionColumn = iColumnIndex;
                            _iStartSelectionIndex = lvi.Index;
                        }
                        else
                            _iEndSelectionIndex = lvi.Index;
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        var multiRight = true;

                        if (lv.SelectedIndices.Count > 0)
                        {
                            var isLeft = false;
                            var isMiddle = false;
                            var isRight = false;
                            if (iColumnIndex < 4)
                                isLeft = true;
                            else if (iColumnIndex == 4)
                                isMiddle = true;
                            else
                                isRight = true;

                            if (dn.SelectionType == DataNode.Selection.None
                                || (dn.SelectionType == DataNode.Selection.Left && !isLeft)
                                || (dn.SelectionType == DataNode.Selection.Right && !isRight)
                                || (dn.SelectionType == DataNode.Selection.Middle && !isMiddle)
                                )
                            {
                                multiRight = false;

                                _iStartSelectionColumn = iColumnIndex;
                                _iStartSelectionIndex = lvi.Index;
                                _iEndSelectionIndex = -1;

                                //int index=0;
                                foreach (var _dn in _mMapper)
                                {
                                    _dn.SelectionType = DataNode.Selection.None;

                                }
                            }
                        }



                        if (multiRight)
                        {
                            if (_iStartSelectionIndex == -1)
                            {
                                _iStartSelectionColumn = iColumnIndex;
                                _iStartSelectionIndex = lvi.Index;
                            }
                            else
                                _iEndSelectionIndex = lvi.Index;
                        }

                    }

                    else
                    {
                        _iStartSelectionColumn = iColumnIndex;
                        _iStartSelectionIndex = lvi.Index;
                        _iEndSelectionIndex = -1;

                        foreach (var _dn in _mMapper)
                        {
                            _dn.SelectionType = DataNode.Selection.None;
                        }
                    }
                    #endregion

                    #region  |  single selection  |

                    if (_iEndSelectionColumn < 4)
                    {
                        if (dn.SelectionType == DataNode.Selection.Right
                            &&
                            (
                                (ModifierKeys & Keys.Shift) == Keys.Shift
                                || (ModifierKeys & Keys.Control) == Keys.Control
                            )
                        )
                        {
                            lvi.Selected = true;
                            dn.SelectionType = DataNode.Selection.Middle;
                        }
                        else
                        {
                            dn.SelectionType = DataNode.Selection.Left;
                        }

                    }
                    else if (_iEndSelectionColumn == 4)
                    {
                        dn.SelectionType = DataNode.Selection.Middle;
                    }
                    else
                    {
                        if (dn.SelectionType == DataNode.Selection.Left
                            && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control))
                        {
                            lvi.Selected = true;
                            dn.SelectionType = DataNode.Selection.Middle;
                        }
                        else
                        {
                            dn.SelectionType = DataNode.Selection.Right;
                        }
                    }
                    lvi.Tag = dn;
                    #endregion

                    if (_iEndSelectionIndex > -1)
                    {
                        #region  |  multiple selection  |
                        if (_iStartSelectionIndex < _iEndSelectionIndex)
                        {
                            #region  |  selection down  |
                            for (var i = _iStartSelectionIndex; i <= _iEndSelectionIndex; i++)
                            {

                                var _lvi = lv.Items[i];
                                var _dn = (DataNode)_lvi.Tag;
                                if (_iEndSelectionColumn < 4)
                                {
                                    if ((_dn.SelectionType == DataNode.Selection.Right || _iStartSelectionColumn > 4)
                                        &&
                                        (
                                            (ModifierKeys & Keys.Shift) == Keys.Shift
                                            || (ModifierKeys & Keys.Control) == Keys.Control
                                        )
                                    )
                                    {
                                        _dn.SelectionType = DataNode.Selection.Middle;
                                    }
                                    else
                                    {
                                        _dn.SelectionType = DataNode.Selection.Left;
                                    }

                                }
                                else if (_iEndSelectionColumn == 4)
                                {
                                    _dn.SelectionType = DataNode.Selection.Middle;
                                }
                                else
                                {
                                    if ((_dn.SelectionType == DataNode.Selection.Left || _iStartSelectionColumn < 4)
                                        &&
                                        (
                                            (ModifierKeys & Keys.Shift) == Keys.Shift
                                            || (ModifierKeys & Keys.Control) == Keys.Control
                                        )
                                    )
                                    {
                                        _dn.SelectionType = DataNode.Selection.Middle;
                                    }
                                    else
                                    {
                                        _dn.SelectionType = DataNode.Selection.Right;
                                    }
                                }

                                lv.RedrawItems(_lvi.Index, _lvi.Index, false);
                            }
                            #endregion
                        }
                        else
                        {
                            #region  |  selection up  |
                            for (var i = _iEndSelectionIndex; i <= _iStartSelectionIndex; i++)
                            {
                                var _lvi = lv.Items[i];
                                var _dn = (DataNode)_lvi.Tag;
                                if (_iEndSelectionColumn < 4)
                                {
                                    if ((_dn.SelectionType == DataNode.Selection.Right || _iStartSelectionColumn > 4)
                                        && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control)
                                    )
                                    {
                                        _dn.SelectionType = DataNode.Selection.Middle;
                                    }
                                    else
                                    {
                                        _dn.SelectionType = DataNode.Selection.Left;
                                    }

                                }
                                else if (_iEndSelectionColumn == 4)
                                {
                                    _dn.SelectionType = DataNode.Selection.Middle;
                                }
                                else
                                {
                                    if ((_dn.SelectionType == DataNode.Selection.Left || _iStartSelectionColumn < 4)
                                        && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control)
                                    )
                                    {
                                        _dn.SelectionType = DataNode.Selection.Middle;
                                    }
                                    else
                                    {
                                        _dn.SelectionType = DataNode.Selection.Right;
                                    }
                                }

                                lv.RedrawItems(_lvi.Index, _lvi.Index, false);
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else if (dn.Type == DataNode.ItemType.File)
                    {
                        lv.RedrawItems(lvi.Index, lvi.Index, false);
                    }

                    if (dn.Type == DataNode.ItemType.Folder)
                    {

                        // hack to draw first column correctly
                        lv.RedrawItems(lvi.Index, lvi.Index, false);

                        var mbr = _mMapper[lvi.Index];



                        var xfrom = lvi.IndentCount * 20;
                        var xto = xfrom + 20;

                        if (iColumnIndex == 5)
                        {
                            xfrom += subItem.Bounds.X;
                            xto += subItem.Bounds.X;
                        }

                        if ((e.X >= xfrom && e.X <= xto) || e.Clicks > 1)
                        {

                            if (dn.Type == DataNode.ItemType.Folder)
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                    mbr.Expanded = !mbr.Expanded;
                                    lvi.Checked = !lvi.Checked;
                                    PrepareNodes(lvi.Index, mbr.Expanded);
                                }
                            }

                        }
                    }

                    _panelCompare.listView_main.Invalidate(false);
                }
                CheckEnabledButtonToolbarMain();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (compareToActivated)
                CreateReport();
        }
        private void ExpandCollapseNodes(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                #region  |  get SelectionColumn  |
                if ((ModifierKeys & Keys.Control) == Keys.Control)
                {
                    _iEndSelectionColumn = _iStartSelectionColumn;
                }
                else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:

                            if (_iStartSelectionColumn > 4)
                                _iStartSelectionColumn = 4;
                            else if (_iStartSelectionColumn == 4)
                                _iStartSelectionColumn = 0;
                            break;
                        case Keys.Right:
                            if (_iStartSelectionColumn < 4)
                                _iStartSelectionColumn = 4;
                            else if (_iStartSelectionColumn == 4)
                                _iStartSelectionColumn = 5;
                            break;
                    }
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:

                            if (_iStartSelectionColumn > 4)
                                _iStartSelectionColumn = 4;
                            else if (_iStartSelectionColumn == 4)
                                _iStartSelectionColumn = 0;
                            break;
                        case Keys.Right:
                            if (_iStartSelectionColumn < 4)
                                _iStartSelectionColumn = 4;
                            else if (_iStartSelectionColumn == 4)
                                _iStartSelectionColumn = 5;
                            break;
                    }
                    _iEndSelectionColumn = _iStartSelectionColumn;
                    foreach (var _dn in _mMapper)
                    {
                        _dn.SelectionType = DataNode.Selection.None;
                    }
                }
                #endregion


                var lv = (ListView)sender;
                var lvi = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];
                var dn = (DataNode)lvi.Tag;

                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    if (lv.SelectedIndices.Count > 0)
                    {


                        foreach (Int32 i in lv.SelectedIndices)
                        {
                            var _dn = (DataNode)lv.Items[i].Tag;
                            if (_iEndSelectionColumn < 4)
                            {
                                if ((_dn.SelectionType == DataNode.Selection.Right || _iStartSelectionColumn > 4)
                                    && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control))
                                {
                                    _dn.SelectionType = DataNode.Selection.Middle;
                                }
                                else
                                {
                                    _dn.SelectionType = DataNode.Selection.Left;
                                }

                            }
                            else if (_iEndSelectionColumn == 4)
                            {
                                _dn.SelectionType = DataNode.Selection.Middle;
                            }
                            else
                            {
                                if ((_dn.SelectionType == DataNode.Selection.Left || _iStartSelectionColumn < 4)
                                    && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control))
                                {
                                    _dn.SelectionType = DataNode.Selection.Middle;
                                }
                                else
                                {
                                    _dn.SelectionType = DataNode.Selection.Right;
                                }
                            }

                            lv.RedrawItems(lv.Items[i].Index, lv.Items[i].Index, false);
                        }
                    }
                }

                #region  |  DataNode.ItemType.folder  |

                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {

                    if (lv.SelectedIndices.Count > 0)
                    {

                        if (_iEndSelectionColumn < 4)
                        {
                            if ((dn.SelectionType == DataNode.Selection.Right || _iStartSelectionColumn > 4)
                                && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control))
                            {
                                dn.SelectionType = DataNode.Selection.Middle;
                            }
                            else
                            {
                                dn.SelectionType = DataNode.Selection.Left;
                            }

                        }
                        else if (_iEndSelectionColumn == 4)
                        {
                            dn.SelectionType = DataNode.Selection.Middle;
                        }
                        else
                        {
                            if ((dn.SelectionType == DataNode.Selection.Left || _iStartSelectionColumn < 4)
                                && ((ModifierKeys & Keys.Shift) == Keys.Shift || (ModifierKeys & Keys.Control) == Keys.Control))
                            {
                                dn.SelectionType = DataNode.Selection.Middle;
                            }
                            else
                            {
                                dn.SelectionType = DataNode.Selection.Right;
                            }
                        }

                        // hack to draw first column correctly
                        lv.RedrawItems(lvi.Index, lvi.Index, false);



                        var xfrom = lvi.IndentCount * 20;
                        var xto = xfrom + 20;

                        if (dn.Type == DataNode.ItemType.Folder || dn.Type == DataNode.ItemType.File)
                        {
                            var mbr = _mMapper[lvi.Index];
                            if (e.KeyCode == Keys.Left)
                            {
                                if (mbr.Expanded)
                                {
                                    mbr.Expanded = false;
                                    lvi.Checked = false;
                                    PrepareNodes(lvi.Index, mbr.Expanded);
                                }
                            }
                            else
                            {
                                if (!mbr.Expanded && e.KeyCode == Keys.Right)
                                {
                                    mbr.Expanded = true;
                                    lvi.Checked = true;
                                    PrepareNodes(lvi.Index, mbr.Expanded);
                                }
                            }
                        }
                    }
                }

                #endregion


                if (_panelCompare.listView_main.SelectedIndices.Count == 0)
                {
                    foreach (var mbr in _mMapper)
                    {
                        mbr.SelectionType = DataNode.Selection.None;
                    }
                    for (var i = 0; i < _panelCompare.listView_main.Items.Count; i++)
                    {
                        lvi = _panelCompare.listView_main.Items[i];

                        var _dn = (DataNode)lvi.Tag;
                        _dn.SelectionType = DataNode.Selection.None;

                    }

                    _panelCompare.listView_main.Invalidate(false);
                }
                else
                {
                    if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        //ignore
                    }
                    else if (_panelCompare.listView_main.SelectedIndices.Count > 0)
                    {
                        _panelCompare.listView_main.Invalidate(false);
                    }
                }


                CheckEnabledButtonToolbarMain();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }





        private void PrepareNodes(int pos, bool add)
        {
            var mbr = _mMapper[pos];
            //pos++;

            if (add)
            {

                PopulateDescendantMembers(ref pos, mbr);

            }
            else
            {
                var kids = ObtainExpandedChildrenCount(pos);
                _mMapper.RemoveRange(pos + 1, kids);
            }

            _panelCompare.listView_main.VirtualListSize = _mMapper.Count;
        }

        private bool ExistsInMapper(int pos, DataNode m)
        {
            var exists = false;
            for (var i = pos; i < _mMapper.Count; i++)
            {
                if (string.Compare(m.PathLeft, _mMapper[i].PathLeft, StringComparison.OrdinalIgnoreCase) != 0 ||
                    string.Compare(m.PathRight, _mMapper[i].PathRight, StringComparison.OrdinalIgnoreCase) != 0) continue;
                exists = true;
                break;
            }
            return exists;
        }
        private void PopulateDescendantMembers(ref int pos, DataNode mbr)
        {

            foreach (var m in mbr.Children)
            {
                if (!AddToFilteredList(m)) continue;
                var continueAdd = true;
                if (m.SizeLeft == 0 && m.SizeRight == 0)
                {
                    if (!Application.Settings.ShowEmptyFolders)
                        continueAdd = false;
                }

                if (!continueAdd) continue;
                if (ExistsInMapper(pos, m)) continue;
                _mMapper.Insert(++pos, m);
                if (m.Expanded)
                {
                    PopulateDescendantMembers(ref pos, m);
                }
            }
        }
        private int ObtainExpandedChildrenCount(int pos)
        {
            var kids = 0;
            var mi = _mMapper[pos];
            var level = mi.Level;

            for (var i = pos + 1; i < _mMapper.Count; i++, kids++)
            {
                var mix = _mMapper[i];
                var lvlx = mix.Level;
                if (lvlx <= level) break;
            }

            return kids;
        }

        private static ListViewItem MakeListViewItem(DataNode dn)
        {

            var lvi = new ListViewItem { Text = dn.NameLeft };

            lvi.SubItems.Add(dn.FileType);
            lvi.SubItems.Add(CalculateFileSize(dn.SizeLeft));
            lvi.SubItems.Add(dn.ModifiedLeft.ToString(CultureInfo.InvariantCulture));
            lvi.SubItems.Add(dn.CompareState.ToString());
            lvi.SubItems.Add(dn.NameRight);
            lvi.SubItems.Add(dn.FileType);
            lvi.SubItems.Add(CalculateFileSize(dn.SizeRight));
            lvi.SubItems.Add(dn.ModifiedRight.ToString(CultureInfo.InvariantCulture));
            lvi.IndentCount = dn.Level;
            lvi.Tag = dn;

            if (dn.Expanded)
                lvi.StateImageIndex = 1;
            else if (dn.CountChildren > 0 || dn.Type == DataNode.ItemType.Folder)
                lvi.StateImageIndex = 0;



            return lvi;
        }
        private void InitVirtualListViewNodes()
        {
            _mMapper.Clear();

            ObtainAllNodes(_mModel.DataPool); // obtain top level nodes and expanded child nodes

            _panelCompare.listView_main.VirtualListSize = _mMapper.Count;
            _panelCompare.listView_main.VirtualMode = true;
            _panelCompare.listView_main.Invalidate();

        }
        private void ObtainAllNodes(IEnumerable<DataNode> nds)
        {




            foreach (var dn in nds)
            {

                if (AddToFilteredList(dn))
                {
                    var continueAdd = true;
                    if (dn.SizeLeft == 0 && dn.SizeRight == 0)
                    {
                        if (!Application.Settings.ShowEmptyFolders)
                            continueAdd = false;
                    }
                    if (continueAdd)
                    {
                        _mMapper.Add(dn);
                        if (dn.Expanded)
                        {
                            ObtainAllNodes(dn.Children);
                        }
                    }
                }

                if (dn.Type == DataNode.ItemType.Folder)
                {
                    var folders = 0;
                    var files = 0;
                    CalculateFoldersSize(dn, ref folders, ref files);
                }
            }

        }



        private void ObtainAllNodesPlusSubFolders(IEnumerable<DataNode> nds)
        {




            foreach (var dn in nds)
            {

                if (AddToFilteredList(dn))
                {
                    var continueAdd = true;
                    if (dn.SizeLeft == 0 && dn.SizeRight == 0)
                    {
                        if (!Application.Settings.ShowEmptyFolders)
                            continueAdd = false;
                    }
                    if (continueAdd)
                    {
                        if (dn.Type == DataNode.ItemType.File)
                        {
                            _mMapperReport.Add(dn);
                        }
                        else
                        {
                            ObtainAllNodesPlusSubFolders(dn.Children);
                        }

                    }
                }

                if (dn.Type == DataNode.ItemType.Folder)
                {
                    //ObtainAllNodesPlusSubFolders(dn.Children);
                }
            }

        }

        private void listView_main_MouseClick(object sender, MouseEventArgs e)
        {

            ExpandCollapseNodes(sender, e);
        }

        private void listView_main_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            ExpandCollapseNodes(sender, e);
        }

        private void listView_main_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {

            if (_mMapper != null && _mMapper.Count > 0 && _mMapper.Count >= e.ItemIndex)
                e.Item = MakeListViewItem(_mMapper[e.ItemIndex]);

        }





        private string GetParentDirectory(DataNode mbrCurrent, bool fromLeftToRight)
        {
            var parentDirectory = string.Empty;



            if (fromLeftToRight)
            {

                if (mbrCurrent.PathRight.Trim() != string.Empty)
                {
                    parentDirectory = Path.GetDirectoryName(mbrCurrent.PathRight);
                }

            }
            else
            {

                if (mbrCurrent.PathLeft.Trim() != string.Empty)
                {
                    parentDirectory = Path.GetDirectoryName(mbrCurrent.PathLeft);

                }

            }


            if (parentDirectory != null && parentDirectory.Trim() == string.Empty)
            {
                parentDirectory = fromLeftToRight ? _panelCompare.comboBox_main_compare_right.Text : _panelCompare.comboBox_main_compare_left.Text;
            }


            return parentDirectory;
        }

        private DataNode getDataNodeFolderChild(DataNode dnParent, Int32 level, string path, bool rightSide)
        {
            DataNode node = null;

            foreach (var dn in dnParent.Children)
            {
                if (dn.Type != DataNode.ItemType.Folder) continue;
                if (dn.Children.Count > 0)
                {
                    node = getDataNodeFolderChild(dn, level, path, rightSide);

                    if (node != null)
                        break;
                }
                else if (dn.Level == level &&
                         string.Compare(path.TrimEnd('\\'), rightSide ? dn.PathRight.TrimEnd('\\') : dn.PathLeft.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    node = dn;
                    break;
                }
            }


            return node;
        }
        private DataNode GetDataNodeFolder(int level, string path, bool rightSide)
        {
            DataNode node = null;
            if (level <= -1)
                return null;

            foreach (var dn in _mMapper)
            {
                if (dn.Type != DataNode.ItemType.Folder) continue;
                if (dn.Level == level &&
                    string.Compare(path.TrimEnd('\\'), rightSide ? dn.PathRight.TrimEnd('\\') : dn.PathLeft.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    node = dn;
                    break;
                }
                if (dn.Children.Count <= 0) continue;
                node = getDataNodeFolderChild(dn, level, path, rightSide);

                if (node != null)
                    break;
            }

            return node;
        }



        private static void listView_main_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private static void listView_main_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //not used

        }

        private readonly SolidBrush _myBrush = new SolidBrush(Color.FromArgb(195, 243, 200));
        private void listView_main_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            try
            {

                var dn = (DataNode)e.Item.Tag;

                var bEqual = (dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal;
                var bSimilar = (dn.CompareState & DataNode.CompareStates.Similar) == DataNode.CompareStates.Similar;
                var bMismatch = (dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch;
                var bNewerRightside = (dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside;
                var bNewerLeftside = (dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside;
                var bOrphansLeftside = (dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside;
                var bOrphansRightside = (dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside;



                switch (e.ColumnIndex)
                {
                    case 0:
                        {
                            #region  |  e.ColumnIndex == 0  |
                            // calculate x offset from ident-level
                            var xOffset = e.Item.IndentCount * 20;

                            // calculate x position
                            var xPos = e.Bounds.X + xOffset + 20;

                            var r = e.Bounds;



                            // drawing of first column, icon as well as text

                            r = e.Bounds;
                            r.Y += 1; r.Height -= 1; r.Width -= 1;
                            e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, r);

                            // set rectangle bounds for drawing of state-icon
                            _mRect.Height = 18;
                            _mRect.Width = 18;
                            _mRect.X = e.Bounds.X + xOffset;
                            _mRect.Y = e.Bounds.Y;

                            var itemForeColor = e.Item.ForeColor;


                            if (dn.Type == DataNode.ItemType.Folder)
                            {
                                if (e.Item.Checked)
                                {
                                    #region  |  folder expanded  |

                                    if (bEqual)
                                    {
                                        #region  |  equal  |

                                        //yellow-open
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["yellow-open"], _mRect);
                                        #endregion
                                    }
                                    else if (dn.NameLeft != string.Empty)
                                    {
                                        if (bMismatch || bNewerLeftside || bNewerRightside)
                                        {
                                            if (bOrphansLeftside || bOrphansRightside)
                                            {
                                                #region  |  with orphans  |
                                                //newer left=true;
                                                //newer right=true;
                                                if (bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside  && b_orphans_rightside  |
                                                    //orphan left = true
                                                    //orphan right = true
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        //red-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (!bOrphansLeftside)
                                                {
                                                    #region  |  !b_orphans_leftside && b_orphans_rightside  |
                                                    //orphan left = false
                                                    //orphan right = true                                       
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);

                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);

                                                    }
                                                    else if (bNewerLeftside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region  |  b_orphans_leftside && !b_orphans_rightside |
                                                    //orphan left = true
                                                    //orphan right = false
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    #endregion
                                                }

                                                #endregion
                                            }
                                            else
                                            {
                                                #region  |  without orphans  |

                                                if (bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);

                                                }
                                                else if (!bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //grey
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);

                                                }
                                                else if (bNewerLeftside && !bNewerRightside)
                                                {
                                                    e.Graphics.DrawImage(
                                                        e.ColumnIndex == 0
                                                            ? _panelCompare.imageList_foldersAndFiles.Images["red-open"]
                                                            : _panelCompare.imageList_foldersAndFiles.Images["grey-open"],
                                                        _mRect);
                                                }
                                                else if (bMismatch)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                }
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);

                                                #endregion
                                            }
                                        }

                                        else
                                        {
                                            #region  |  else  |

                                            if (bNewerRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                            }
                                            else if (bNewerLeftside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                            }
                                            else if (bMismatch)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                            }
                                            else if (bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                }
                                            }
                                            else if (bOrphansLeftside && !bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    if (dn.NameRight.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                }
                                            }
                                            else if (!bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                }
                                                else
                                                {
                                                    if (dn.NameLeft.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                }
                                            }
                                            else if (bSimilar)
                                            {
                                                //purple
                                                e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                                else if (0 == e.Item.StateImageIndex)
                                {
                                    #region  |  folder closed  |

                                    if (bEqual)
                                    {
                                        #region  |  equal  |

                                        //yellow-open
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["yellow-closed"], _mRect);
                                        #endregion
                                    }
                                    else if (dn.NameLeft != string.Empty)
                                    {
                                        if (bMismatch || bNewerLeftside || bNewerRightside)
                                        {
                                            if (bOrphansLeftside || bOrphansRightside)
                                            {
                                                #region  |  with orphans  |
                                                //newer left=true;
                                                //newer right=true;
                                                if (bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside  && b_orphans_rightside  |
                                                    //orphan left = true
                                                    //orphan right = true
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        //red-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (!bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  !b_orphans_leftside && b_orphans_rightside  |
                                                    //orphan left = false
                                                    //orphan right = true                                       
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);

                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (bOrphansLeftside && !bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside && !b_orphans_rightside |
                                                    //orphan left = true
                                                    //orphan right = false
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        e.Graphics.DrawImage(
                                                            e.ColumnIndex == 0
                                                                ? _panelCompare.imageList_foldersAndFiles.Images[
                                                                    "grey-blue-closed"]
                                                                : _panelCompare.imageList_foldersAndFiles.Images["red-closed"],
                                                            _mRect);
                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        e.Graphics.DrawImage(
                                                            e.ColumnIndex == 0
                                                                ? _panelCompare.imageList_foldersAndFiles.Images[
                                                                    "red-blue-closed"]
                                                                : _panelCompare.imageList_foldersAndFiles.Images["grey-closed"],
                                                            _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region  |  without orphans  |

                                                if (bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);

                                                }
                                                else if (!bNewerLeftside && bNewerRightside)
                                                {
                                                    e.Graphics.DrawImage(
                                                        e.ColumnIndex == 0
                                                            ? _panelCompare.imageList_foldersAndFiles.Images["grey-closed"]
                                                            : _panelCompare.imageList_foldersAndFiles.Images["red-closed"],
                                                        _mRect);
                                                }
                                                else if (bNewerLeftside && !bNewerRightside)
                                                {
                                                    e.Graphics.DrawImage(
                                                        e.ColumnIndex == 0
                                                            ? _panelCompare.imageList_foldersAndFiles.Images["red-closed"]
                                                            : _panelCompare.imageList_foldersAndFiles.Images["grey-closed"],
                                                        _mRect);
                                                }
                                                else if (bMismatch)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                }
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);

                                                #endregion
                                            }
                                        }

                                        else
                                        {
                                            #region  |  else  |

                                            if (bNewerRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                            }
                                            else if (bNewerLeftside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                            }
                                            else if (bMismatch)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                            }
                                            else if (bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                }
                                            }
                                            else if (bOrphansLeftside && !bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    if (dn.NameRight.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                }
                                            }
                                            else if (!bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                }
                                                else
                                                {
                                                    if (dn.NameLeft.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                }
                                            }
                                            else if (bSimilar)
                                            {
                                                //purple
                                                e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {

                                #region  |  file level  |
                                //file
                                if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                {
                                    itemForeColor = Color.Black;
                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Similar) == DataNode.CompareStates.Similar)
                                {
                                    if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                    {
                                        if (e.ColumnIndex == 0)
                                        {
                                            itemForeColor = Color.Gray;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                        }
                                        else
                                        {
                                            itemForeColor = Color.Purple;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Purple"], _mRect);
                                        }
                                    }
                                    else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                    {
                                        if (e.ColumnIndex == 0)
                                        {
                                            itemForeColor = Color.Purple;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Purple"], _mRect);
                                        }
                                        else
                                        {
                                            itemForeColor = Color.Gray;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                        }
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                {

                                    if (e.ColumnIndex == 0)
                                    {
                                        itemForeColor = Color.Gray;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                    }
                                    else
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }

                                }
                                else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                {
                                    if (e.ColumnIndex == 0)
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }
                                    else
                                    {
                                        itemForeColor = Color.Gray;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                                {
                                    if (e.ColumnIndex == 0)
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }
                                    else
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }

                                }
                                else if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                {
                                    itemForeColor = Color.Blue;
                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Blue"], _mRect);
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                {
                                    //nothing
                                }
                                else if (dn.NameLeft.Trim() != string.Empty)
                                {
                                    itemForeColor = Color.Black;
                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                }
                                #endregion
                            }


                            // set rectangle bounds for drawing of item/subitem text
                            _mRect.Height = e.Bounds.Height;
                            _mRect.Width = e.Bounds.Width + e.Bounds.X - xPos;
                            _mRect.X = xPos;
                            _mRect.Y = e.Bounds.Y;

                            // draw item/subitem text
                            if ((e.ItemState & ListViewItemStates.Selected) != 0
                                && (dn.SelectionType == DataNode.Selection.Left || dn.SelectionType == DataNode.Selection.Middle)
                            )
                            {
                                //LinearGradientBrush br = CreateBrush();

                                e.Graphics.FillRectangle(_myBrush, _mRect);

                                e.DrawFocusRectangle(_mRect);
                                // draw selected item's text
                                TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, _mRect, itemForeColor, SmTff);
                            }
                            else if ((e.ItemState & ListViewItemStates.Selected) != 0)
                            {
                                e.DrawFocusRectangle(_mRect);
                                TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, _mRect, itemForeColor, SmTff);
                            }
                            else
                            {
                                TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, _mRect, itemForeColor, SmTff);
                            }

                            #endregion
                        }
                        break;
                    case 4:
                        {
                            #region  |  e.ColumnIndex == 4  |

                            // calculate x offset from ident-level
                            var xOffset = 6;

                            // calculate x position
                            var xPos = e.Bounds.X + xOffset;



                            var r = e.Bounds;

                            // drawing of first column, icon as well as text

                            r = e.Bounds;
                            r.Y += 1;
                            r.Height -= 1;
                            r.Width -= 1;

                            e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, r);



                            var selectionOn = false || e.ColumnIndex == 4 && ((dn.SelectionType == DataNode.Selection.Left && dn.SelectionType == DataNode.Selection.Right) || dn.SelectionType == DataNode.Selection.Middle);

                            // drawing of all other columns, e. g. drawing of subitems
                            if ((e.ItemState & ListViewItemStates.Selected) != 0
                                && selectionOn
                            )
                            {
                                //SolidBrush myBrush = new SolidBrush(Color.BlanchedAlmond);
                                e.Graphics.FillRectangle(_myBrush, e.Bounds);
                                e.DrawFocusRectangle(e.Bounds);
                            }
                            else if ((e.ItemState & ListViewItemStates.Selected) != 0)
                            {
                                e.DrawFocusRectangle(e.Bounds);
                            }







                            var myPen = new Pen(Color.Gray) { Width = 1 };

                            e.Graphics.DrawLine(myPen, r.X + 2, r.Y, r.X + 2, r.Y + r.Height);
                            e.Graphics.DrawLine(myPen, r.X + r.Width - 2, r.Y, r.X + r.Width - 2, r.Y + r.Height);


                            // set rectangle bounds for drawing of state-icon
                            _mRect.Height = 10;
                            _mRect.Width = 10;
                            _mRect.X = e.Bounds.X + xOffset + 4;
                            _mRect.Y = e.Bounds.Y + 4;



                            if (dn.Type == DataNode.ItemType.File) //file
                            {

                                if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                {
                                    e.Graphics.DrawImage(_panelCompare.imageList_listview.Images["EqualToBlack"], _mRect);
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Similar) == DataNode.CompareStates.Similar)
                                {
                                    e.Graphics.DrawImage(_panelCompare.imageList_listview.Images["NotEqualToPurple"], _mRect);
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch
                                         || (dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside
                                         || (dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                {
                                    e.Graphics.DrawImage(_panelCompare.imageList_listview.Images["NotEqualToRed"], _mRect);
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside
                                         || (dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                {
                                    //e.Graphics.DrawImage(_panel_Compare.imageList_listview.Images["NotEqualToBlue"], m_Rect);
                                }

                            }








                            #endregion
                        }
                        break;
                    case 5:
                        {
                            #region  |  e.ColumnIndex == 5  |
                            // calculate x offset from ident-level
                            var xOffset = e.Item.IndentCount * 20;

                            // calculate x position
                            var xPos = e.Bounds.X + xOffset + 20;



                            var r = e.Bounds;

                            // drawing of first column, icon as well as text

                            r = e.Bounds;
                            r.Y += 1; r.Height -= 1; r.Width -= 1;
                            e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, r);

                            // set rectangle bounds for drawing of state-icon
                            _mRect.Height = 18;
                            _mRect.Width = 18;
                            _mRect.X = e.Bounds.X + xOffset;
                            _mRect.Y = e.Bounds.Y;

                            var itemForeColor = e.Item.SubItems[4].ForeColor;

                            if (dn.Type == DataNode.ItemType.Folder)
                            {

                                if (e.Item.Checked)
                                {
                                    #region  |  folder expanded  |

                                    if (bEqual)
                                    {
                                        #region  |  equal  |

                                        //yellow-open
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["yellow-open"], _mRect);
                                        #endregion
                                    }
                                    else if (dn.NameRight != string.Empty)
                                    {
                                        if (bMismatch || bNewerLeftside || bNewerRightside)
                                        {
                                            if (bOrphansLeftside || bOrphansRightside)
                                            {
                                                #region  |  with orphans  |
                                                //newer left=true;
                                                //newer right=true;
                                                if (bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside  && b_orphans_rightside  |
                                                    //orphan left = true
                                                    //orphan right = true
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        //red-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (!bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  !b_orphans_leftside && b_orphans_rightside  |
                                                    //orphan left = false
                                                    //orphan right = true                                       
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);

                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (bOrphansLeftside && !bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside && !b_orphans_rightside |
                                                    //orphan left = true
                                                    //orphan right = false
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-open"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-open"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region  |  without orphans  |

                                                if (bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);

                                                }
                                                else if (!bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //grey
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);

                                                }
                                                else if (bNewerLeftside && !bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    else
                                                        //grey
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                }
                                                else if (bMismatch)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                }
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);

                                                #endregion
                                            }
                                        }

                                        else
                                        {
                                            #region  |  else  |

                                            if (bNewerRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                            }
                                            else if (bNewerLeftside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                            }
                                            else if (bMismatch)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                            }
                                            else if (bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                }
                                            }
                                            else if (bOrphansLeftside && !bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    if (dn.NameRight.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                }
                                            }
                                            else if (!bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-open"], _mRect);
                                                }
                                                else
                                                {
                                                    if (dn.NameLeft.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-open"], _mRect);
                                                    }
                                                }
                                            }
                                            else if (bSimilar)
                                            {
                                                //purple
                                                e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-open"], _mRect);
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion

                                }
                                else if (0 == e.Item.StateImageIndex)
                                {
                                    #region  |  folder closed  |

                                    if (bEqual)
                                    {
                                        #region  |  equal  |

                                        //yellow-open
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["yellow-closed"], _mRect);
                                        #endregion
                                    }
                                    else if (dn.NameRight != string.Empty)
                                    {
                                        if (bMismatch || bNewerLeftside || bNewerRightside)
                                        {
                                            if (bOrphansLeftside || bOrphansRightside)
                                            {
                                                #region  |  with orphans  |
                                                //newer left=true;
                                                //newer right=true;
                                                if (bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside  && b_orphans_rightside  |
                                                    //orphan left = true
                                                    //orphan right = true
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        //red-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (!bOrphansLeftside && bOrphansRightside)
                                                {
                                                    #region  |  !b_orphans_leftside && b_orphans_rightside  |
                                                    //orphan left = false
                                                    //orphan right = true                                       
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);

                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                else if (bOrphansLeftside && !bOrphansRightside)
                                                {
                                                    #region  |  b_orphans_leftside && !b_orphans_rightside |
                                                    //orphan left = true
                                                    //orphan right = false
                                                    if (bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                        else
                                                            //red-blue-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    }
                                                    else if (!bNewerLeftside && bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);

                                                    }
                                                    else if (bNewerLeftside && !bNewerRightside)
                                                    {
                                                        if (e.ColumnIndex == 0)
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-blue-closed"], _mRect);
                                                        else
                                                            //grey-open
                                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-blue-closed"], _mRect);
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region  |  without orphans  |

                                                if (bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);

                                                }
                                                else if (!bNewerLeftside && bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //grey
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);

                                                }
                                                else if (bNewerLeftside && !bNewerRightside)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    else
                                                        //grey
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                }
                                                else if (bMismatch)
                                                {
                                                    if (e.ColumnIndex == 0)
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                    else
                                                        //red
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                }
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);

                                                #endregion
                                            }
                                        }

                                        else
                                        {
                                            #region  |  else  |

                                            if (bNewerRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                            }
                                            else if (bNewerLeftside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                else
                                                    //grey
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                            }
                                            else if (bMismatch)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                                else
                                                    //red
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                            }
                                            else if (bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                }
                                            }
                                            else if (bOrphansLeftside && !bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                    if (dn.NameRight.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                else
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                }
                                            }
                                            else if (!bOrphansLeftside && bOrphansRightside)
                                            {
                                                if (e.ColumnIndex == 0)
                                                {
                                                    //grey-blue-open
                                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["grey-closed"], _mRect);
                                                }
                                                else
                                                {
                                                    if (dn.NameLeft.Trim() != string.Empty)
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                    else
                                                    {
                                                        //grey-blue-open                                 
                                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["blue-closed"], _mRect);
                                                    }
                                                }
                                            }
                                            else if (bSimilar)
                                            {
                                                //purple
                                                e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["red-closed"], _mRect);
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                #region  |  file level  |
                                //file
                                if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                {
                                    itemForeColor = Color.Black;
                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Similar) == DataNode.CompareStates.Similar)
                                {
                                    if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                    {
                                        if (e.ColumnIndex == 0)
                                        {
                                            itemForeColor = Color.Gray;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                        }
                                        else
                                        {
                                            itemForeColor = Color.Purple;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Purple"], _mRect);
                                        }
                                    }
                                    else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                    {
                                        if (e.ColumnIndex == 0)
                                        {
                                            itemForeColor = Color.Purple;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Purple"], _mRect);
                                        }
                                        else
                                        {
                                            itemForeColor = Color.Gray;
                                            e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                        }
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                {

                                    if (e.ColumnIndex == 0)
                                    {
                                        itemForeColor = Color.Gray;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                    }
                                    else
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }

                                }
                                else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                {
                                    if (e.ColumnIndex == 0)
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }
                                    else
                                    {
                                        itemForeColor = Color.Gray;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                                {
                                    if (e.ColumnIndex == 0)
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }
                                    else
                                    {
                                        itemForeColor = Color.Red;
                                        e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Red"], _mRect);
                                    }

                                }
                                else if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                {
                                    //nothing
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                {
                                    itemForeColor = Color.Blue;
                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Blue"], _mRect);
                                }
                                else if (dn.NameRight.Trim() != string.Empty)
                                {
                                    itemForeColor = Color.Black;
                                    e.Graphics.DrawImage(_panelCompare.imageList_foldersAndFiles.Images["Bullet-Black"], _mRect);
                                }
                                #endregion
                            }

                            // set rectangle bounds for drawing of item/subitem text
                            _mRect.Height = e.Bounds.Height;
                            _mRect.Width = e.Bounds.Width + e.Bounds.X - xPos;
                            _mRect.X = xPos;
                            _mRect.Y = e.Bounds.Y;


                            // draw item/subitem text
                            if ((e.ItemState & ListViewItemStates.Selected) != 0
                                && (dn.SelectionType == DataNode.Selection.Right || dn.SelectionType == DataNode.Selection.Middle)
                            )
                            {
                                //LinearGradientBrush br = CreateBrush();
                                //e.Graphics.FillRectangle(br, m_Rect);

                                // SolidBrush myBrush = new SolidBrush(Color.BlanchedAlmond);

                                e.Graphics.FillRectangle(_myBrush, _mRect);

                                e.DrawFocusRectangle(_mRect);
                                // draw selected item's text
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[5].Text, e.Item.SubItems[5].Font, _mRect, itemForeColor, SmTff);
                            }
                            else if ((e.ItemState & ListViewItemStates.Selected) != 0)
                            {


                                //Pen blackPen = new Pen(Color.Gray);
                                //blackPen.Width = 1;

                                //Point p1 = new Point(m_Rect.X, (m_Rect.Y + 1));
                                //Point p2 = new Point(m_Rect.X + m_Rect.Width, (m_Rect.Y + 1));
                                //e.Graphics.DrawLine(blackPen, p1, p2);

                                //Point p3 = new Point(m_Rect.X, m_Rect.Y + (m_Rect.Height - 1));
                                //Point p4 = new Point(m_Rect.X + m_Rect.Width, m_Rect.Y + (m_Rect.Height - 1));
                                //e.Graphics.DrawLine(blackPen, p3, p4);

                                e.DrawFocusRectangle(_mRect);
                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[5].Text, e.Item.SubItems[5].Font, _mRect, itemForeColor, SmTff);

                            }
                            else
                            {

                                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[5].Text, e.Item.SubItems[5].Font, _mRect, itemForeColor, SmTff);
                            }

                            #endregion
                        }
                        break;
                    default:
                        {
                            #region  |  else  |
                            // calculate x offset from ident-level
                            var xOffset = e.Item.IndentCount * 17;

                            // calculate x position
                            var xPos = e.Bounds.X + xOffset + 17;

                            var r = e.Bounds;

                            r.Y += 1; r.Height -= 1; r.Width -= 1;
                            e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, r);

                            var subItemText = string.Empty;

                            if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                            {
                                if (dn.NameLeft.Trim() != string.Empty)
                                    subItemText = e.SubItem.Text;
                            }
                            else if (e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                            {
                                if (dn.NameRight.Trim() != string.Empty)
                                    subItemText = e.SubItem.Text;
                            }




                            var itemForeColor = e.Item.ForeColor;

                            if (dn.Type == 0)
                            {
                                itemForeColor = Color.Gray;
                            }
                            else
                            {

                                if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                {
                                    itemForeColor = Color.Black;
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Similar) == DataNode.CompareStates.Similar)//0=equal; 1=different; 2=exist only on left; 3=exists only on right; 4=similar
                                {
                                    if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                    {
                                        if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                                        {
                                            itemForeColor = Color.Gray;
                                        }
                                        else if (e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                                        {
                                            itemForeColor = Color.Purple;
                                        }
                                    }
                                    else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                    {
                                        if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                                        {
                                            itemForeColor = Color.Purple;
                                        }
                                        else if (e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                                        {
                                            itemForeColor = Color.Gray;
                                        }
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                {
                                    if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                                    {
                                        itemForeColor = Color.Gray;
                                    }
                                    else if (e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                                    {
                                        itemForeColor = Color.Red;
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                {
                                    if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                                    {
                                        itemForeColor = Color.Red;
                                    }
                                    else if (e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                                    {
                                        itemForeColor = Color.Gray;
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                                {
                                    if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                                    {
                                        itemForeColor = Color.Red;
                                    }
                                    else if (e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                                    {
                                        itemForeColor = Color.Red;
                                    }
                                }
                                else if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside
                                         || (dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                {
                                    itemForeColor = Color.Blue;
                                }



                            }

                            var selectionOn = false;
                            if (e.ColumnIndex == 1 && (dn.SelectionType == DataNode.Selection.Left || dn.SelectionType == DataNode.Selection.Middle))
                                selectionOn = true;
                            else if (e.ColumnIndex == 2 && (dn.SelectionType == DataNode.Selection.Left || dn.SelectionType == DataNode.Selection.Middle))
                                selectionOn = true;
                            else if (e.ColumnIndex == 3 && (dn.SelectionType == DataNode.Selection.Left || dn.SelectionType == DataNode.Selection.Middle))
                                selectionOn = true;
                            else if (e.ColumnIndex == 6 && (dn.SelectionType == DataNode.Selection.Right || dn.SelectionType == DataNode.Selection.Middle))
                                selectionOn = true;
                            else if (e.ColumnIndex == 7 && (dn.SelectionType == DataNode.Selection.Right || dn.SelectionType == DataNode.Selection.Middle))
                                selectionOn = true;
                            else if (e.ColumnIndex == 8 && (dn.SelectionType == DataNode.Selection.Right || dn.SelectionType == DataNode.Selection.Middle))
                                selectionOn = true;

                            // drawing of all other columns, e. g. drawing of subitems
                            if ((e.ItemState & ListViewItemStates.Selected) != 0
                                && selectionOn
                            )
                            {
                                e.Graphics.FillRectangle(_myBrush, e.Bounds);
                                e.DrawFocusRectangle(e.Bounds);
                                TextRenderer.DrawText(e.Graphics, subItemText, e.Item.Font, e.Bounds, itemForeColor, SmTff);
                            }
                            else if ((e.ItemState & ListViewItemStates.Selected) != 0)
                            {

                                e.DrawFocusRectangle(e.Bounds);
                                TextRenderer.DrawText(e.Graphics, subItemText, e.Item.Font, e.Bounds, itemForeColor, SmTff);
                            }
                            else
                            {

                                TextRenderer.DrawText(e.Graphics, subItemText, e.Item.Font, e.Bounds, itemForeColor, SmTff);
                            }
                            #endregion
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }










        private void toolStripButton_expandAll_Click(object sender, EventArgs e)
        {
            ExpandAll();
        }

        private void toolStripButton_collapseAll_Click(object sender, EventArgs e)
        {
            CollapseAll();
        }

        private void expandAllSubfoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpanceAllCurrentNode(sender, e);
        }

        private void toolStripButton_collapseCurrentFolder_Click(object sender, EventArgs e)
        {
            CollapseAllCurrentNode(sender, e);
        }

        private void collapseAllSubfoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollapseAllCurrentNode(sender, e);
        }






        private void CompareMe()
        {
            #region  |  compare  |
            var cp = new Settings.ComparisonProject();
            foreach (var comparisonProjectCurrent in Application.Settings.ComparisonProjects)
            {
                if (string.Compare(comparisonProjectCurrent.PathLeft.Trim(), _panelCompare.comboBox_main_compare_left.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0 ||
                    string.Compare(comparisonProjectCurrent.PathRight.Trim(), _panelCompare.comboBox_main_compare_right.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0) continue;
                cp = comparisonProjectCurrent;
                break;
            }
            if (cp.PathLeft.Trim() != string.Empty)
            {
                SelectComparisonProject(cp, true);
            }
            else
            {
                CompareDirectories(true);
            }
            #endregion
        }




        private void toolStripButton_refresh_Click(object sender, EventArgs e)
        {
            CompareMe();
        }

        private void listView_main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                ExpandCollapseNodes(sender, e);
            }
            else if (e.KeyCode == Keys.Return)
            {
                CreateReport();
            }
            else if ((ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.A)
            {
                if (_panelCompare.listView_main.Items.Count <= 0) return;
                foreach (var mbr in _mMapper)
                {
                    mbr.SelectionType = DataNode.Selection.Middle;
                }
                for (var i = 0; i < _panelCompare.listView_main.Items.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[i];

                    var _dn = (DataNode)lvi.Tag;
                    _dn.SelectionType = DataNode.Selection.Middle;

                }

                _panelCompare.listView_main.Invalidate(false);
            }


        }




        private void toolStripButton_expandAllCurrentFolder_Click(object sender, EventArgs e)
        {
            ExpanceAllCurrentNode(sender, e);
        }

        private void toolStripButton_ignoreEqualFiles_Click(object sender, EventArgs e)
        {

            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();

                toolStripButton_ignoreEqualFiles.Image = toolStripButton_ignoreEqualFiles.Checked ? imageList1.Images["Filter_Flag_Black_Off"] : imageList1.Images["Filter_Flag_Black_On"];

                Application.Settings.ShowEqualFiles = !Application.Settings.ShowEqualFiles;
                toolStripButton_ignoreEqualFiles.CheckState = Application.Settings.ShowEqualFiles ? CheckState.Checked : CheckState.Unchecked;
                UpdateVisualCompareDirectories();

                UpdateVisualCompareDirectories();


                CheckToolTips();

                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }



        }

        private void toolStripButton_ignoreOrphanLeftSide_Click(object sender, EventArgs e)
        {


            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();



                toolStripButton_ignoreOrphansLeftSide.Image = toolStripButton_ignoreOrphansLeftSide.Checked ? imageList1.Images["Filter_Flag_Blue_Left_Off"] : imageList1.Images["Filter_Flag_Blue_Left_On"];

                Application.Settings.ShowOrphanFilesLeft = !Application.Settings.ShowOrphanFilesLeft;
                toolStripButton_ignoreOrphansLeftSide.CheckState = Application.Settings.ShowOrphanFilesLeft ? CheckState.Checked : CheckState.Unchecked;
                UpdateVisualCompareDirectories();

                UpdateVisualCompareDirectories();


                CheckToolTips();

                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }



        }

        private void toolStripButton_ignoreOrpansRightSide_Click(object sender, EventArgs e)
        {

            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();



                toolStripButton_ignoreOrphansRightSide.Image = toolStripButton_ignoreOrphansRightSide.Checked ? imageList1.Images["Filter_Flag_Blue_Right_Off"] : imageList1.Images["Filter_Flag_Blue_Right_On"];


                Application.Settings.ShowOrphanFilesRight = !Application.Settings.ShowOrphanFilesRight;
                toolStripButton_ignoreOrphansRightSide.CheckState = Application.Settings.ShowOrphanFilesRight ? CheckState.Checked : CheckState.Unchecked;
                UpdateVisualCompareDirectories();
                UpdateVisualCompareDirectories();

                CheckToolTips();
                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }



        }

        private void toolStripButton_ignoreDifferencesLeftSide_Click(object sender, EventArgs e)
        {

            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();



                toolStripButton_ignoreDifferencesLeftSide.Image = toolStripButton_ignoreDifferencesLeftSide.Checked ? imageList1.Images["Filter_Flag_Red_Left_Off"] : imageList1.Images["Filter_Flag_Red_Left_On"];

                Application.Settings.ShowDifferencesFilesLeft = !Application.Settings.ShowDifferencesFilesLeft;
                toolStripButton_ignoreDifferencesLeftSide.CheckState = Application.Settings.ShowDifferencesFilesLeft ? CheckState.Checked : CheckState.Unchecked;
                UpdateVisualCompareDirectories();

                UpdateVisualCompareDirectories();

                CheckToolTips();
                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }



        }

        private void toolStripButton_ignoreDifferencesRightSide_Click(object sender, EventArgs e)
        {
            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();



                toolStripButton_ignoreDifferencesRightSide.Image = toolStripButton_ignoreDifferencesRightSide.Checked ? imageList1.Images["Filter_Flag_Red_Right_Off"] : imageList1.Images["Filter_Flag_Red_Right_On"];


                Application.Settings.ShowDifferencesFilesRight = !Application.Settings.ShowDifferencesFilesRight;
                toolStripButton_ignoreDifferencesRightSide.CheckState = Application.Settings.ShowDifferencesFilesRight ? CheckState.Checked : CheckState.Unchecked;


                UpdateVisualCompareDirectories();


                UpdateVisualCompareDirectories();

                CheckToolTips();
                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }


        }

        private void toolStripButton_showEmptyFolders_Click(object sender, EventArgs e)
        {

            try
            {
                _panelCompare.panel_listViewMessage.Visible = true;
                System.Windows.Forms.Application.DoEvents();



                toolStripButton_showEmptyFolders.Image = toolStripButton_showEmptyFolders.Checked ? imageList1.Images["Filter_Flag_Folder_Off"] : imageList1.Images["Filter_Flag_Folder_On"];



                Application.Settings.ShowEmptyFolders = !Application.Settings.ShowEmptyFolders;
                toolStripButton_showEmptyFolders.CheckState = Application.Settings.ShowEmptyFolders ? CheckState.Checked : CheckState.Unchecked;
                UpdateVisualCompareDirectories();

                UpdateVisualCompareDirectories();

                CheckToolTips();
                _panelCompare.listView_main.Invalidate(false);
            }
            finally
            {
                _panelCompare.panel_listViewMessage.Visible = false;
            }



        }



        private void listView_main_DoubleClick(object sender, EventArgs e)
        {

            var lv = (ListView)sender;
            var lvi = _panelCompare.listView_main.Items[lv.SelectedIndices[0]];
            var dn = (DataNode)lvi.Tag;
            if (dn.Type == DataNode.ItemType.File)
                CreateReport();
        }

        private void expandFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lv = _panelCompare.listView_main;
            var lvi = lv.GetItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);
            var subItem = lvi.GetSubItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);

            var dn = (DataNode)lvi.Tag;
            if (dn.Type == DataNode.ItemType.Folder)
            {
                if (!dn.Expanded)
                {
                    dn.Expanded = true;
                    lvi.Checked = true;
                    PrepareNodes(lvi.Index, dn.Expanded);
                }
            }
        }





        private bool MostlyLeftSelection { get; set; }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                MostlyLeftSelection = false;

                var iFiles = 0;
                var iFolders = 0;
                if (_panelCompare.listView_main.SelectedIndices.Count > 0)
                {
                    for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                    {
                        var mbr = (DataNode)_panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]].Tag;

                        if (mbr.SelectionType == DataNode.Selection.Left)
                            MostlyLeftSelection = true;

                        if (mbr.Type == DataNode.ItemType.File)
                            iFiles++;
                        else
                            iFolders++;
                    }

                }
                else
                {
                    e.Cancel = true;
                }




                var lv = _panelCompare.listView_main;
                var lvi = lv.GetItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);
                var subItem = lvi.GetSubItemAt(_iRightMouseClickLocationX, _iRightMouseClickLocationY);

                var iColumnIndex = -1;
                for (var i = 0; i < lvi.SubItems.Count; i++)
                {
                    if (lvi.SubItems[i] != subItem) continue;
                    iColumnIndex = i;
                    break;
                }

                var dn = (DataNode)lvi.Tag;


                var expandFolderToolStripMenuItemVisible = false;
                var setAsBasefolderToolStripMenuItemVisible = false;
                var expandAllSubfoldersToolStripMenuItemVisible = false;
                var collapseAllSubfoldersToolStripMenuItemVisible = false;
                var compareFoldersToolStripMenuItemVisible = false;
                var compareFilesToolStripMenuItemVisible = false;


                if (lv.SelectedIndices.Count >= 1 && lv.SelectedIndices.Count <= 2)
                {
                    switch (_panelCompare.listView_main.SelectedIndices.Count)
                    {
                        case 1:
                            if (dn.Type == DataNode.ItemType.Folder)
                            {
                                expandFolderToolStripMenuItemVisible = true;
                                setAsBasefolderToolStripMenuItemVisible = true;
                                expandAllSubfoldersToolStripMenuItemVisible = true;
                                collapseAllSubfoldersToolStripMenuItemVisible = true;
                            }
                            else
                            {
                                expandFolderToolStripMenuItemVisible = false;
                                setAsBasefolderToolStripMenuItemVisible = false;
                                expandAllSubfoldersToolStripMenuItemVisible = false;
                                collapseAllSubfoldersToolStripMenuItemVisible = false;
                            }

                            if (dn.Type == DataNode.ItemType.Folder
                                && (dn.SelectionType == DataNode.Selection.Middle
                                    || (dn.SelectionType == DataNode.Selection.Left && dn.SelectionType == DataNode.Selection.Right)))
                            {
                                compareFoldersToolStripMenuItemVisible = true;
                            }
                            else
                            {
                                compareFoldersToolStripMenuItemVisible = false;
                            }
                            break;
                        case 2:
                            var lvi01 = lv.Items[lv.SelectedIndices[0]];
                            var lvi02 = lv.Items[lv.SelectedIndices[1]];

                            var dn01 = (DataNode)lvi01.Tag;
                            var dn02 = (DataNode)lvi02.Tag;

                            if (dn01.Type == DataNode.ItemType.Folder
                                && dn02.Type == DataNode.ItemType.Folder
                            )
                            {
                                compareFoldersToolStripMenuItemVisible = true;
                            }
                            else
                            {
                                compareFoldersToolStripMenuItemVisible = false;
                            }

                            if (dn01.Type == DataNode.ItemType.File
                                && dn02.Type == DataNode.ItemType.File
                            )
                            {
                                compareFilesToolStripMenuItemVisible = true;
                            }
                            else
                            {
                                compareFilesToolStripMenuItemVisible = false;
                            }
                            break;
                    }
                }
                _panelCompare.expandFolderToolStripMenuItem.Visible = expandFolderToolStripMenuItemVisible;
                _panelCompare.setAsBasefolderToolStripMenuItem.Visible = setAsBasefolderToolStripMenuItemVisible;
                _panelCompare.expandAllSubfoldersToolStripMenuItem.Visible = expandAllSubfoldersToolStripMenuItemVisible;
                _panelCompare.collapseAllSubfoldersToolStripMenuItem.Visible = collapseAllSubfoldersToolStripMenuItemVisible;
                _panelCompare.compareFoldersToolStripMenuItem.Visible = compareFoldersToolStripMenuItemVisible;
                _panelCompare.compareFilesToolStripMenuItem.Visible = compareFilesToolStripMenuItemVisible;

                if (!expandFolderToolStripMenuItemVisible
                    && !compareFoldersToolStripMenuItemVisible)
                {
                    _panelCompare.toolStripSeparator_folders.Visible = false;
                }
                else
                {
                    _panelCompare.toolStripSeparator_folders.Visible = true;
                }


                if (iColumnIndex < 4)
                {
                    _panelCompare.setAsBasefolderToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Set_base_folder__left;
                    _panelCompare.setAsBasefolderToolStripMenuItem.Tag = Resources.FormMain_BrowseFolder_left;
                }
                else if (iColumnIndex == 4)
                {
                    _panelCompare.setAsBasefolderToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Set_base_folders_left_right;
                    _panelCompare.setAsBasefolderToolStripMenuItem.Tag = "both";
                }
                else
                {
                    _panelCompare.setAsBasefolderToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Set_base_folder_right;
                    _panelCompare.setAsBasefolderToolStripMenuItem.Tag = Resources.FormMain_BrowseFolder_right;
                }



                _panelCompare.copyToFolderToolStripMenuItem.Enabled = false;

                if (MostlyLeftSelection)
                {

                    _panelCompare.copyToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Copy_to_the_right;
                    _panelCompare.copyToFolderToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Copy_to_folder;
                    _panelCompare.moveToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Move_to_the_right;

                }
                else
                {
                    _panelCompare.copyToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Copy_to_the_left;
                    _panelCompare.copyToFolderToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Copy_to_folder;
                    _panelCompare.moveToToolStripMenuItem.Text = Resources.FormMain_contextMenuStrip1_Opening_Move_to_the_left;

                }

                if (iFiles <= 0)
                {
                    _panelCompare.copyToToolStripMenuItem.Enabled = false;

                    _panelCompare.moveToToolStripMenuItem.Enabled = false;
                    _panelCompare.deleteToolStripMenuItem.Enabled = false;
                }
                else
                {
                    _panelCompare.copyToToolStripMenuItem.Enabled = true;

                    _panelCompare.moveToToolStripMenuItem.Enabled = true;
                    _panelCompare.deleteToolStripMenuItem.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                Console.WriteLine(ex.Message);
            }

        }


        private bool MouseLeftIsDownOutsideBounds { get; set; }
        private bool MouseLeftIsDown { get; set; }
        private void listView_main_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLeftIsDown = true;
            if (e.Button != MouseButtons.Left) return;
            var lvi = _panelCompare.listView_main.GetItemAt(e.X, e.Y);
            MouseLeftIsDownOutsideBounds = lvi == null;
        }

        private void listView_main_MouseUp(object sender, MouseEventArgs e)
        {
            MouseLeftIsDown = false;


            if (sender.GetType() != typeof(ListView)) return;
            if (_panelCompare.listView_main.SelectedIndices.Count == 0)
            {
                foreach (var mbr in _mMapper)
                {
                    mbr.SelectionType = DataNode.Selection.None;
                }
                for (var i = 0; i < _panelCompare.listView_main.Items.Count; i++)
                {
                    var lvi = _panelCompare.listView_main.Items[i];

                    var _dn = (DataNode)lvi.Tag;
                    _dn.SelectionType = DataNode.Selection.None;

                }

                _panelCompare.listView_main.Invalidate(false);
            }
            else
            {
                if (!MouseLeftIsDownOutsideBounds) return;
                if ((ModifierKeys & Keys.Control) == Keys.Control
                    || (ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    //ignore
                }
                else if (_panelCompare.listView_main.SelectedIndices.Count > 0)
                {
                    if (_panelCompare.listView_main.SelectedIndices.Count > 1)
                    {
                        for (var i = 0; i < _panelCompare.listView_main.SelectedIndices.Count; i++)
                        {
                            var lvi = _panelCompare.listView_main.Items[_panelCompare.listView_main.SelectedIndices[i]];

                            var _dn = (DataNode)lvi.Tag;

                            _dn.SelectionType = DataNode.Selection.Middle;

                        }
                    }
                    _panelCompare.listView_main.Invalidate(false);
                }
            }
        }





        #endregion



        private void CloseAllDocumentsComparePanel()
        {
            if (dockPanel_manager.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (var form in MdiChildren)
                    form.Close();
            }
            else
            {
                for (var index = dockPanel_manager.Contents.Count - 1; index >= 0; index--)
                {
                    if (dockPanel_manager.Contents[index] == null) continue;
                    var content = dockPanel_manager.Contents[index];
                    content.DockHandler.Close();
                }
            }
        }

        private IDockContent FindContentComparePanel(string text)
        {
            return dockPanel_manager.DocumentStyle == DocumentStyle.SystemMdi ? (from form in MdiChildren where form.Text == text select form as IDockContent).FirstOrDefault() : dockPanel_manager.Documents.FirstOrDefault(content => content.DockHandler.TabText == text);
        }
        private IDockContent GetContentFromPersistStringComparePanel(string persistString)
        {
            if (persistString == typeof(PanelCompare).ToString())
            {
                return _panelCompare;
            }
            if (persistString == typeof(PanelComparisonProjects).ToString())
            {
                return _panelComparisonProjects;
            }
            if (persistString == typeof(PanelEventsLog).ToString())
            {
                return _panelEventsLog;
            }
            var parsedStrings = persistString.Split(',');
            if (parsedStrings.Length != 3)
                return null;


            return null;
        }

    }
}
