using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.DQF;
using Sdl.Community.DQF.Core;
using Sdl.Community.Qualitivity.Dialogs.DQF;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.DQF;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.TM.Database;
using Process = System.Diagnostics.Process;

namespace Sdl.Community.Qualitivity.Panels.DQF
{
    public partial class QualitivityViewDqfControl : UserControl
    {
        public event ProjectSelectionHandler ProjectSelectionChanged;
        public delegate void ProjectSelectionHandler();

        private QualitivityViewController _controller { get; set; }
        public QualitivityViewController Controller
        {
            get
            {
                return _controller;
            }
            set
            {

                _controller = value;
            }
        }

        public QualitivityViewDqfControl()
        {
            InitializeComponent();
            CheckEnabledObjects();
        }

        public static Project Project { get; set; }
        public static Activity Activity { get; set; }

        private void CheckEnabledObjects()
        {
            if (treeView_dqf_projects.SelectedNode != null)
            {
                var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;

                toolStripButton_dqf_project_new.Enabled = true;
                toolStripButton_dqf_import_project_settings.Enabled = true;
                toolStripButton_dqf_export_project_settings.Enabled = true;

                if (dqfProject != null)
                    toolStripButton_dqf_remove_project.Enabled = dqfProject.DqfTasks.Count == 0;
                linkLabel_view_project.Enabled = true;
            }
            else if (Project != null)
            {
                label_DQF_PROJECT_NAME.Text = string.Empty;
                toolStripButton_dqf_project_new.Enabled = true;
                toolStripButton_dqf_import_project_settings.Enabled = true;
                toolStripButton_dqf_export_project_settings.Enabled = false;
                toolStripButton_dqf_remove_project.Enabled = false;
                linkLabel_view_project.Enabled = false;
            }
            else
            {
                label_DQF_PROJECT_NAME.Text = string.Empty;
                toolStripButton_dqf_project_new.Enabled = false;
                toolStripButton_dqf_import_project_settings.Enabled = false;
                toolStripButton_dqf_export_project_settings.Enabled = false;
                toolStripButton_dqf_remove_project.Enabled = false;
                linkLabel_view_project.Enabled = false;
            }

            if (listView1.SelectedItems.Count > 0)
                viewProjectTaskInfoToolStripMenuItem.Enabled = true;
            else
                viewProjectTaskInfoToolStripMenuItem.Enabled = false;



            newDQFProjectToolStripMenuItem.Enabled = toolStripButton_dqf_project_new.Enabled;
            importDQFProjectFromFileToolStripMenuItem.Enabled = toolStripButton_dqf_import_project_settings.Enabled;
            saveDQFProjectSettingsToFileToolStripMenuItem.Enabled = toolStripButton_dqf_export_project_settings.Enabled;
            viewDQFProjectInfoToolStripMenuItem.Enabled = toolStripButton_dqf_export_project_settings.Enabled;
            viewDQFProjectReportsToolStripMenuItem.Enabled = linkLabel_view_project.Enabled;
            removeDQFProjectToolStripMenuItem.Enabled = toolStripButton_dqf_remove_project.Enabled;

        }

        public void initialize_dqfProjects()
        {
            try
            {
                var continueInitialize = true;

                if (treeView_dqf_projects.SelectedNode != null && Project != null)
                {
                    var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;
                    if (dqfProject != null && dqfProject.ProjectId == Project.Id)
                        continueInitialize = false;
                }

                if (continueInitialize)
                {
                    listView1.Items.Clear();
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

                    if (treeView_dqf_projects.Nodes.Count > 0)
                        treeView_dqf_projects.SelectedNode = treeView_dqf_projects.Nodes[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                treeView_dqf_projects.EndUpdate();
                CheckEnabledObjects();
            }
        }

        public void ClearDqfProjectList()
        {
            listView1.Items.Clear();
            treeView_dqf_projects.Nodes.Clear();


            label_DQF_PROJECT_NAME.Text = string.Empty;
            toolStripButton_dqf_project_new.Enabled = false;
            toolStripButton_dqf_import_project_settings.Enabled = false;
            toolStripButton_dqf_export_project_settings.Enabled = false;
            toolStripButton_dqf_remove_project.Enabled = false;
            viewProjectTaskInfoToolStripMenuItem.Enabled = false;
            linkLabel_view_project.Enabled = false;

            newDQFProjectToolStripMenuItem.Enabled = toolStripButton_dqf_project_new.Enabled;
            importDQFProjectFromFileToolStripMenuItem.Enabled = toolStripButton_dqf_import_project_settings.Enabled;
            saveDQFProjectSettingsToFileToolStripMenuItem.Enabled = toolStripButton_dqf_export_project_settings.Enabled;
            viewDQFProjectInfoToolStripMenuItem.Enabled = toolStripButton_dqf_export_project_settings.Enabled;
            viewDQFProjectReportsToolStripMenuItem.Enabled = linkLabel_view_project.Enabled;
            removeDQFProjectToolStripMenuItem.Enabled = toolStripButton_dqf_remove_project.Enabled;



        }
        public void NewDqfProject()
        {
            if (Tracked.Settings.DqfSettings.UserKey.Trim() == string.Empty)
            {
                MessageBox.Show(PluginResources.The_DQF_API_key_cannot_be_null + "\r\n\r\n"
                + PluginResources.To_create_a_TAUS_DQF_Project__you_must_first_save_your_DQF_API_key_
                    , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            var f = new DqfProjectCreate { DqfProject = new DqfProject { Name = Project.Name } };
            f.ShowDialog();
            if (!f.Saved) return;
            if (Project != null && Project.DqfProjects.Exists(a => a.Name.ToLower().Trim() == f.DqfProject.Name.ToLower().Trim()))
            {
                MessageBox.Show(PluginResources.The_DQF_project_name_already_exists, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    var dqfProject = f.DqfProject;
                    dqfProject.ProjectId = Project.Id;
                    dqfProject.ProjectIdStudio = Project.StudioProjectId;
                    dqfProject.SourceLanguage = Project.SourceLanguage;
                    dqfProject.Created = DateTime.Now;
                    dqfProject.DqfPmanagerKey = Tracked.Settings.DqfSettings.UserKey;
                    dqfProject.DqfProjectId = -1;
                    dqfProject.DqfProjectKey = string.Empty;
                    dqfProject.Imported = false;

                    var processor = new Processor();
                    var productivityProject = new ProductivityProject
                    {
                        Name = dqfProject.Name,
                        QualityLevel = dqfProject.QualityLevel,
                        SourceLanguage = dqfProject.SourceLanguage,
                        Process = dqfProject.Process,
                        ContentType = dqfProject.ContentType,
                        Industry = dqfProject.Industry
                    };
                    productivityProject = processor.PostDqfProject(Tracked.Settings.DqfSettings.UserKey, productivityProject);
                    dqfProject.DqfProjectId = productivityProject.ProjectId;
                    dqfProject.DqfProjectKey = productivityProject.ProjectKey;

                    var query = new Query();
                    dqfProject.Id = query.CreateDqfProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, dqfProject);

                    var tn = treeView_dqf_projects.Nodes.Add(dqfProject.Name);
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = tn.ImageIndex;
                    tn.Tag = dqfProject;

                    Project.DqfProjects.Add(dqfProject);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void ImportDqfProjectSettings()
        {
            var f = new OpenFileDialog
            {
                Filter = PluginResources.DQF_Project_Import_File + @" (*.dqfpi)|*.dqfpi",
                Title = PluginResources.Open_TAUS_DQF_Project_Import_File,
                RestoreDirectory = true
            };
            f.ShowDialog();
            if (f.FileName.Trim() == string.Empty) return;
            var dqfProject = new DqfProject();

            var xmlTextReader = new XmlTextReader(f.FileName);
            var xmlReaderSettings = new XmlReaderSettings { ValidationType = ValidationType.None };

            using (var xmlReader = XmlReader.Create(xmlTextReader, xmlReaderSettings))
            {
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                        {
                            if (string.Compare(xmlReader.Name, "dqf_project_import_file", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                var xmlReaderInner = xmlReader.ReadSubtree();
                                dqfProject = read_DQFProjectSettings(xmlReaderInner);
                                xmlReaderInner.Close();
                            }
                        } break;
                    }
                }
            }


            if (dqfProject.DqfProjectId <= -1) return;
            var dqfProjectFind = Project.DqfProjects.Find(a => a.DqfProjectId == dqfProject.DqfProjectId);
            if (dqfProjectFind != null)
            {
                MessageBox.Show(string.Format(PluginResources.The_DQF_Project_0_already_exists_in_the_list, dqfProject.Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dqfProject.ProjectId = Project.Id;
                dqfProject.ProjectIdStudio = Project.StudioProjectId;
                dqfProject.DqfPmanagerKey = Tracked.Settings.DqfSettings.UserKey;
                dqfProject.Imported = true;

                var query = new Query();
                dqfProject.Id = query.CreateDqfProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, dqfProject);

                var tn = treeView_dqf_projects.Nodes.Add(dqfProject.Name);
                tn.ImageIndex = dqfProject.Imported ? 1 : 0;
                tn.SelectedImageIndex = tn.ImageIndex;
                tn.Tag = dqfProject;

                Project.DqfProjects.Add(dqfProject);
            }
        }
        public void SaveDqfProjectSettings()
        {
            if (treeView_dqf_projects.SelectedNode == null) return;
            var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;

            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = PluginResources.DQF_Project_Import_File + @" (*.dqfpi)|*.dqfpi",
                Title = PluginResources.Save_TAUS_DQF_Project_Import_File,
                AddExtension = true,
                DefaultExt = ".dqfpi"
            };

            if (dqfProject == null) return;
            saveFileDialog1.FileName = dqfProject.Name + "_" + Helper.GetStringFromDateTime(DateTime.Now).Replace(":", ".").Replace("T", " ") + ".dqfpi";

            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName == string.Empty) return;
            var xmlTxtWriter = new XmlTextWriter(saveFileDialog1.FileName, Encoding.UTF8)
            {
                Formatting = Formatting.None,
                Indentation = 3,
                Namespaces = false
            };

            xmlTxtWriter.WriteStartDocument(true);
            xmlTxtWriter.WriteStartElement("dqf_project_import_file");
            xmlTxtWriter.WriteAttributeString("xml:space", "preserve");

            xmlTxtWriter.WriteStartElement(@"settings");
            xmlTxtWriter.WriteAttributeString(@"project_id", dqfProject.DqfProjectId.ToString());
            xmlTxtWriter.WriteAttributeString(@"project_key", dqfProject.DqfProjectKey);

            xmlTxtWriter.WriteAttributeString(@"name", dqfProject.Name);
            if (dqfProject.Created != null)
                xmlTxtWriter.WriteAttributeString(@"created", Helper.GetStringFromDateTime(dqfProject.Created.Value));
            xmlTxtWriter.WriteAttributeString(@"source_language", dqfProject.SourceLanguage);

            xmlTxtWriter.WriteAttributeString(@"process", dqfProject.Process.ToString());
            xmlTxtWriter.WriteAttributeString(@"content_type", dqfProject.ContentType.ToString());
            xmlTxtWriter.WriteAttributeString(@"industry", dqfProject.Industry.ToString());
            xmlTxtWriter.WriteAttributeString(@"quality_level", dqfProject.QualityLevel.ToString());

            xmlTxtWriter.WriteEndElement();//settings

            xmlTxtWriter.WriteEndElement();//dqf_project_import_file

            xmlTxtWriter.WriteEndDocument();
            xmlTxtWriter.Flush();
            xmlTxtWriter.Close();
        }


        private void toolStripButton_dqf_project_new_Click(object sender, EventArgs e)
        {
            NewDqfProject();
        }

        private void toolStripButton_dqf_import_project_settings_Click(object sender, EventArgs e)
        {
            ImportDqfProjectSettings();


        }
        private static DqfProject read_DQFProjectSettings(XmlReader xmlReader)
        {
            var dqfProject = new DqfProject();


            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "settings", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            while (xmlReader.MoveToNextAttribute())
                            {
                                if (string.Compare(xmlReader.Name, "project_id", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.DqfProjectId = Convert.ToInt32(xmlReader.Value);
                                }
                                else if (string.Compare(xmlReader.Name, "project_key", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.DqfProjectKey = xmlReader.Value;
                                }
                                else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.Name = xmlReader.Value;
                                }
                                else if (string.Compare(xmlReader.Name, "created", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.Created = Helper.GetDateTimeFromString(xmlReader.Value);
                                }
                                else if (string.Compare(xmlReader.Name, "source_language", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.SourceLanguage = xmlReader.Value;
                                }
                                else if (string.Compare(xmlReader.Name, "process", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.Process = Convert.ToInt32(xmlReader.Value);
                                }
                                else if (string.Compare(xmlReader.Name, "content_type", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.ContentType = Convert.ToInt32(xmlReader.Value);
                                }
                                else if (string.Compare(xmlReader.Name, "industry", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.Industry = Convert.ToInt32(xmlReader.Value);
                                }
                                else if (string.Compare(xmlReader.Name, "quality_level", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    dqfProject.QualityLevel = Convert.ToInt32(xmlReader.Value);
                                }
                            }
                        }
                        break;
                }
            }

            return dqfProject;
        }

        private void toolStripButton_dqf_export_project_settings_Click(object sender, EventArgs e)
        {
            SaveDqfProjectSettings();
        }

        private void toolStripButton_dqf_remove_project_Click(object sender, EventArgs e)
        {
            if (treeView_dqf_projects.SelectedNode == null) return;
            var dr = MessageBox.Show(PluginResources.Are_you_sure_that_you_want_to_remove_the_selected_DQF_Project + "\r\n\r\n"
                                     + PluginResources.Note_you_will_not_be_able_to_recover_this_data_if_you_continue + "\r\n\r\n"
                                     + PluginResources.Click_Click_Yes_to_continue + "\r\n"
                                     + PluginResources.Click_Click_No_to_cancel
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;
            var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;


            if (dqfProject != null && dqfProject.DqfTasks.Count == 0)
            {
                var query = new Query();
                query.DeleteDqfProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, dqfProject.ProjectId, dqfProject.Id);


                Project.DqfProjects.RemoveAll(a => a.Id == dqfProject.Id);
                treeView_dqf_projects.Nodes.Remove(treeView_dqf_projects.SelectedNode);

                listView1.Items.Clear();


                if (treeView_dqf_projects.Nodes.Count > 0)
                    treeView_dqf_projects.SelectedNode = treeView_dqf_projects.Nodes[0];
            }
            CheckEnabledObjects();
        }

        public void treeView_dqf_projects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();
            if (treeView_dqf_projects.SelectedNode != null)
            {
                var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;

                if (dqfProject != null)
                {
                    label_DQF_PROJECT_NAME.Text = dqfProject.Name + @" - { " + dqfProject.DqfTasks.Count + @" tasks }";


                    foreach (var task in dqfProject.DqfTasks)
                    {
                        var pa = Project.Activities.Find(a => a.Id == task.ProjectActivityId);
                        var activityName = pa != null ? pa.Name : "<none>";

                        var item = listView1.Items.Add(activityName, 0);
                        item.SubItems.Add(task.DqfTaskId.ToString());
                        item.SubItems.Add(task.DocumentName);
                        item.SubItems.Add(dqfProject.SourceLanguage);
                        item.SubItems.Add(task.TargetLanguage);
                        if (task.Uploaded != null)
                            item.SubItems.Add(Helper.GetStringFromDateTime(task.Uploaded.Value).Replace("T", " "));
                        item.SubItems.Add(task.TotalSegments.ToString());
                        item.Tag = task;
                    }
                }
                if (listView1.Items.Count > 0)
                    listView1.Items[0].Selected = true;


            }
            else
            {

                label_DQF_PROJECT_NAME.Text = string.Empty;
            }
            CheckEnabledObjects();

            if (ProjectSelectionChanged != null)
                ProjectSelectionChanged();

        }

        private void treeView_dqf_projects_DoubleClick(object sender, EventArgs e)
        {
            if (treeView_dqf_projects.SelectedNode == null) return;
            var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;

            var f = new DqfProjectInfo {DqfProject = dqfProject};
            f.ShowDialog();
        }

        private void linkLabel_view_project_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (treeView_dqf_projects.SelectedNode == null) return;
            var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;

            if (dqfProject != null) Process.Start("https://dqf.taus.net/reports/" + dqfProject.DqfProjectId);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView_dqf_projects.SelectedNode == null) return;
            var dqfProject = treeView_dqf_projects.SelectedNode.Tag as DqfProject;
            if (listView1.SelectedItems.Count <= 0) return;
            var task = (DqfProjectTask)listView1.SelectedItems[0].Tag;

            var f = new DqfProjectTaskInfo
            {
                DqfProject = dqfProject,
                DqfProjectTask = task
            };
            f.ShowDialog();
        }

        private void viewProjectTaskInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1_DoubleClick(null, null);
        }

        private void newDQFProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_dqf_project_new_Click(null, null);
        }

        private void importDQFProjectFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_dqf_import_project_settings_Click(null, null);
        }

        private void saveDQFProjectSettingsToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_dqf_export_project_settings_Click(null, null);
        }

        private void viewDQFProjectInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView_dqf_projects_DoubleClick(null, null);
        }

        private void viewDQFProjectReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            linkLabel_view_project_LinkClicked(null, null);
        }

        private void removeDQFProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_dqf_remove_project_Click(null, null);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            viewProjectTaskInfoToolStripMenuItem.Enabled = listView1.SelectedItems.Count > 0;
        }





    }
}
