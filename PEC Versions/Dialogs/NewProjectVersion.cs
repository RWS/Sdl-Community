using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Versions.Structures;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    public partial class NewProjectVersion : Form
    {

        public bool Saved { get; set; }
        public ProjectsController ProjectsController { get; set; }
        public string SelectedProjectId { get; set; }




        public ProjectVersion ProjectVersion { get; set; }
        public Project CurrentProject { get; set; }

        private ProjectInfo CurrentProjectInfo { get; set; }
        public Settings Settings { get; set; }

        private bool IsLoading { get; set; }

        private int IndexCurrentPanel { get; set; }
        private int TotalFiles { get; set; }

        public NewProjectVersion()
        {
            InitializeComponent();
            IsLoading = true;
            panel_details.Dock = DockStyle.Fill;
            panel_progress.Dock = DockStyle.Fill;
        }

        private void SaveVersion_Load(object sender, EventArgs e)
        {

            comboBox_projects.Items.Clear();

            var projects = ProjectsController.GetProjects().ToList();
            foreach (var proj in projects)
            {
                var projectInfo = proj.GetProjectInfo();
                var comboboxItem = new ComboboxItem
                {
                    Text = projectInfo.Name,
                    Value = projectInfo
                };
                comboBox_projects.Items.Add(comboboxItem);
            }

            #region  |  set the default project selection  |
            for (var i = 0; i < comboBox_projects.Items.Count; i++)
            {
                var comboboxItem = (ComboboxItem)comboBox_projects.Items[i];

                if (string.Compare(((ProjectInfo)comboboxItem.Value).Id.ToString(), SelectedProjectId, StringComparison.OrdinalIgnoreCase) != 0) continue;
                comboBox_projects.SelectedIndex = i;
                break;
            }
            #endregion


            IsLoading = false;

            ChangeProjectDetails();

            IndexCurrentPanel = 0;
            switch_Panel(IndexCurrentPanel == 0 ? panel_details : panel_progress);
            CheckEnableContinue();

        }
        private void switch_Panel(Control panel)
        {
            switch (panel.Name)
            {
                case "panel_details":
                    {
                        label_titleBar_title.Text = PluginResources.Project_Version_Step_1_of_2;
                        label_titlebar_description.Text =
                            PluginResources.Select_the_SDL_Trados_Studio_Project_and_then_;
                        label_titlebar_note.Text =
                            PluginResources
                                .Note_Click_on_the_button_Continue_to_proceed_with_the_project_version_creation;

                        panel_details.BringToFront();

                        button_wizard_help.Enabled = false;

                        button_wizard_continue.Text = PluginResources.Continue;
                        button_wizard_continue.Enabled = true;
                        button_wizard_cancel.Enabled = true;
                        break;
                    }
                case "panel_progress":
                    {
                        label_titleBar_title.Text = PluginResources.Project_Version_Step_2_of_2;
                        label_titlebar_description.Text = PluginResources.Progress_Creating_new_project_version;
                        label_titlebar_note.Text = "";

                        panel_progress.BringToFront();

                        button_wizard_help.Enabled = false;

                        button_wizard_continue.Text = PluginResources.Close;
                        button_wizard_continue.Enabled = true;
                        button_wizard_cancel.Enabled = false;


                        label_progress_process.Text = @"...";
                        label_progress_percentage.Text = @"0%";
                        label_progress_message.Text = @"...";

                        CreateProjectVersion();
                        break;
                    }
            }
        }


        private void CreateProjectVersion()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                button_wizard_continue.Enabled = false;
                button_wizard_cancel.Enabled = false;
                button_wizard_help.Enabled = false;



                ProjectVersion.name = textBox_name.Text;
                ProjectVersion.description = textBox_description.Text;
                ProjectVersion.shallowCopy = checkBox_createShallowCopy.Checked;



                ProjectVersion.location = Path.Combine(textBox_location.Text, ProjectVersion.createdAt);
                if (!Directory.Exists(ProjectVersion.location))
                    Directory.CreateDirectory(ProjectVersion.location);


                textBox_progress_deatails.Text = string.Empty;
                textBox_progress_deatails.Text += string.Format(PluginResources.Start_Processing_0, DateTime.Now) + "\r\n\r\n";
                textBox_progress_deatails.Text += PluginResources.CreateProjectVersion_ + "\r\n\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Name_0, CurrentProject.name) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Location_0, CurrentProject.location) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Source_Language_0, CurrentProject.sourceLanguage.name) + "\r\n";
                var targetLanguages = CurrentProject.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Target_Languages_0, targetLanguages) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Source_Files_Translatable_Reference_Localizable_Unknown, CurrentProject.translatableCount, CurrentProject.referenceCount, CurrentProject.localizableCount, CurrentProject.unKnownCount) + "\r\n\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Name, ProjectVersion.name) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Description_0, ProjectVersion.description) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Location_0, ProjectVersion.location) + "\r\n";


                label_progress_process.Text = PluginResources.Initializing_;
                label_progress_percentage.Text = @"0%";
                label_progress_message.Text = @"...";

                Application.DoEvents();



                TotalFiles = 0;
                var hasError = false;
                var errorMessage = string.Empty;
                try
                {
                    if (Directory.Exists(CurrentProject.location))
                    {
                        #region  |  standard project  |
                        CountAll(new DirectoryInfo(CurrentProject.location), ProjectVersion.shallowCopy ? "*.sdlxliff" : "*.*");

                        progress_progressBar.Value = 0;
                        progress_progressBar.Maximum = TotalFiles;

                        #region  |  copy .sdlproj file always  |
                        if (ProjectVersion.shallowCopy)
                        {
                            var files = new DirectoryInfo(CurrentProject.location).GetFiles("*.sdlproj").ToList();
                            if (files.Count > 0)
                            {
                                var fiToCopy = files[0];

                                if (files.Count == 1)
                                {
                                    File.Copy(fiToCopy.FullName, Path.Combine(ProjectVersion.location, fiToCopy.Name), true);
                                }
                                else
                                {
                                    foreach (var fi in files)
                                    {
                                        if (fi.Name.ToLower().IndexOf(CurrentProject.name.ToLower().Trim(), StringComparison.Ordinal) <= -1)
                                            continue;
                                        fiToCopy = fi;
                                        break;
                                    }
                                    File.Copy(fiToCopy.FullName, Path.Combine(ProjectVersion.location, fiToCopy.Name), true);
                                }

                                TotalFiles++;
                                progress_progressBar.Maximum++;
                                progress_progressBar.Value++;
                            }

                        }
                        #endregion


                        ProjectVersion.filesCopiedCount = TotalFiles;

                        CopyAll(new DirectoryInfo(CurrentProject.location), new DirectoryInfo(ProjectVersion.location), ProjectVersion.shallowCopy ? "*.sdlxliff" : "*.*");


                        #endregion
                    }
                    else
                    {
                        #region  |  single file project  |
                        if (CurrentProject.translatableCount == 1 && CurrentProject.location.EndsWith(".ProjectFiles"))
                        {
                            var locationSdlproj = CurrentProject.location.Substring(0, CurrentProject.location.LastIndexOf(".ProjectFiles", StringComparison.Ordinal)) + ".sdlproj";
                            var locationSource = CurrentProject.location.Substring(0, CurrentProject.location.LastIndexOf(".ProjectFiles", StringComparison.Ordinal));
                            var locationSdlxliff = CurrentProject.location.Substring(0, CurrentProject.location.LastIndexOf(".ProjectFiles", StringComparison.Ordinal)) + ".sdlxliff";


                            progress_progressBar.Maximum = 0;
                            progress_progressBar.Value = 0;


                            if (File.Exists(locationSdlproj))
                            {
                                File.Copy(locationSdlproj, Path.Combine(ProjectVersion.location, Path.GetFileName(locationSdlproj)), true);

                                TotalFiles++;
                                progress_progressBar.Maximum++;
                                progress_progressBar.Value++;


                            }

                            if (File.Exists(locationSdlxliff))
                            {
                                File.Copy(locationSdlxliff, Path.Combine(ProjectVersion.location, Path.GetFileName(locationSdlxliff)), true);
                                TotalFiles++;
                                progress_progressBar.Maximum++;
                                progress_progressBar.Value++;
                            }

                            try
                            {
                                if (!ProjectVersion.shallowCopy)
                                {
                                    if (File.Exists(locationSource))
                                    {
                                        File.Copy(locationSource, Path.Combine(ProjectVersion.location, Path.GetFileName(locationSource)), true);
                                        TotalFiles++;
                                        progress_progressBar.Maximum++;
                                        progress_progressBar.Value++;
                                    }
                                }

                            }
                            catch
                            {
                                // ignored
                            }


                            ProjectVersion.filesCopiedCount = TotalFiles;

                        }

                        #endregion
                    }

                 
                    textBox_progress_deatails.Text += string.Format(PluginResources.Total_Files_0, ProjectVersion.filesCopiedCount) + "\r\n\r\n";


                    if (ProjectVersion.filesCopiedCount == 0)
                    {
                        label_titlebar_note.Text = PluginResources.Warning_no_local_files_where_copied_during_the_project_version_creation_process;
                    }


                    label_progress_percentage.Text = @"100%";
                    label_progress_message.Text = string.Format(PluginResources.Copied_files_0_of_1, progress_progressBar.Value, progress_progressBar.Maximum);

                    Saved = true;


                }
                catch (Exception ex)
                {
                    hasError = true;
                    errorMessage = ex.Message;
                }

                var message = PluginResources.Finished_Processing_without_errors;
                if (hasError)
                    message = PluginResources.Finished_Processing_with_errors;

                textBox_progress_deatails.Text += message + "\r\n\r\n";
                if (hasError)
                {
                    textBox_progress_deatails.Text += PluginResources.Exception_Message__ + errorMessage + "\r\n\r\n";
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                Saved = false;
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                textBox_progress_deatails.Text += string.Format(PluginResources.End_Processing_0_, DateTime.Now);
                label_progress_process.Text = PluginResources.Finished_Processing;
                label_titlebar_description.Text = PluginResources.Progress_Finished_Processing;

                Cursor = Cursors.Default;
                button_wizard_continue.Enabled = true;
                button_wizard_cancel.Enabled = false;
                button_wizard_help.Enabled = false;
            }
        }

        public string GetStringFromDateTime(DateTime dateTime)
        {
            return dateTime.Year
                + "-" + dateTime.Month.ToString().PadLeft(2, '0')
                + "-" + dateTime.Day.ToString().PadLeft(2, '0')
                + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
                + "." + dateTime.Minute.ToString().PadLeft(2, '0')
                + "." + dateTime.Second.ToString().PadLeft(2, '0');
        }

        public void CountAll(DirectoryInfo source, string searchPattern)
        {

            TotalFiles += source.GetFiles(searchPattern).Length;


            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                CountAll(diSourceSubDir, searchPattern);
            }
        }
        public void CopyAll(DirectoryInfo source, DirectoryInfo target, string searchPattern)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (var fi in source.GetFiles(searchPattern))
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);

                if (progress_progressBar.Value + 1 > progress_progressBar.Maximum) continue;
                label_progress_process.Text = fi.Name;

                #region  |  progress  |

                progress_progressBar.Invoke((MethodInvoker)delegate
                {
                    var i = progress_progressBar.Value;

                    if (progress_progressBar.Value != 0)
                        progress_progressBar.Value = progress_progressBar.Value - 1;
                    progress_progressBar.Value = i + 1;
                });


                var perc = Convert.ToDouble(progress_progressBar.Value) / Convert.ToDouble(progress_progressBar.Maximum);
                label_progress_percentage.Text = Convert.ToString(Math.Round(perc * 100, 0), CultureInfo.InvariantCulture) + "%";

                label_progress_message.Text = string.Format(PluginResources.Copied_files_0_of_1, progress_progressBar.Maximum, progress_progressBar.Value);
                if (progress_progressBar.Value % 5 == 0)
                {
                    Application.DoEvents();
                    //Cursor = Cursors.WaitCursor;
                }
                #endregion
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, searchPattern);
            }
        }

        private void ChangeProjectDetails()
        {
            if (IsLoading) return;
            FileBasedProject currentSelectedProject = null;

            var selectedItem = (ComboboxItem)comboBox_projects.SelectedItem;
            var projectInfo = (ProjectInfo)selectedItem.Value;

            var projects = ProjectsController.GetProjects().ToList();
            foreach (var proj in projects)
            {
                if (
                    string.Compare(projectInfo.Id.ToString(), proj.GetProjectInfo().Id.ToString(),
                        StringComparison.OrdinalIgnoreCase) != 0) 
                    continue;

                CurrentProjectInfo = proj.GetProjectInfo();

                currentSelectedProject = proj;
                break;
            }

            SelectedProjectId = CurrentProjectInfo.Id.ToString();

            #region  |  get settings project reference  |

            var settingsProject = Settings.projects.FirstOrDefault(project => string.Compare(project.id, CurrentProjectInfo.Id.ToString(), StringComparison.OrdinalIgnoreCase) == 0);

            #endregion

            if (settingsProject == null)
            {

                #region  |  check create new project entry  |

                CurrentProject = new Project
                {
                    id = CurrentProjectInfo.Id.ToString(),
                    name = CurrentProjectInfo.Name,
                    description = CurrentProjectInfo.Description,
                    createdAt = Helper.GetStringFromDateTime(CurrentProjectInfo.CreatedAt.Date),
                    createdBy = CurrentProjectInfo.CreatedBy,
                    location = CurrentProjectInfo.LocalProjectFolder,
                    projectFileName = CurrentProjectInfo.Name + ".sdlproj",
                    sourceLanguage = new Structures.LanguageProperty
                    {
                        id = CurrentProjectInfo.SourceLanguage.CultureInfo.Name,
                        name = CurrentProjectInfo.SourceLanguage.DisplayName
                    },
                    targetLanguages = new List<Structures.LanguageProperty>()
                };


                #region  |  get source language  |

                #endregion

                #region  |  get target langauges  |

                foreach (var language in CurrentProjectInfo.TargetLanguages)
                {
                    var targetLanguage = new Structures.LanguageProperty
                    {
                        id = language.CultureInfo.Name,
                        name = language.DisplayName
                    };

                    CurrentProject.targetLanguages.Add(targetLanguage);
                }
                #endregion

                #region  |  get files  |

                CurrentProject.translatableCount = 0;
                CurrentProject.referenceCount = 0;
                CurrentProject.localizableCount = 0;
                CurrentProject.unKnownCount = 0;
                CurrentProject.files = new List<FileProperty>();


                #region  |  get source files  |


                ProjectFile[] sourceFiles = null;

                try
                {
                    if (currentSelectedProject != null) sourceFiles = currentSelectedProject.GetSourceLanguageFiles();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    try
                    {
                        if (currentSelectedProject != null)
                            sourceFiles = currentSelectedProject.GetTargetLanguageFiles();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
                if (sourceFiles != null)
                {
                    foreach (var sfile in sourceFiles)
                    {
                        var fileProperty = new FileProperty
                        {
                            name = sfile.Name,
                            path = sfile.LocalFilePath
                        };
                        switch (sfile.Role)
                        {
                            case FileRole.Localizable: fileProperty.fileType = FileProperty.FileType.Localizable; CurrentProject.localizableCount++; break;
                            case FileRole.Reference: fileProperty.fileType = FileProperty.FileType.Reference; CurrentProject.referenceCount++; break;
                            case FileRole.Translatable: fileProperty.fileType = FileProperty.FileType.Translatable; CurrentProject.translatableCount++; break;
                            case FileRole.Unknown: fileProperty.fileType = FileProperty.FileType.Unknown; CurrentProject.unKnownCount++; break;

                        }
                        fileProperty.sourceId = CurrentProjectInfo.SourceLanguage.CultureInfo.Name;
                        fileProperty.targetId = CurrentProjectInfo.SourceLanguage.CultureInfo.Name;
                        fileProperty.isSource = true;
                    }
                }

                #endregion


                #endregion

                #endregion


            }
            else
            {
                CurrentProject = (Project)settingsProject.Clone();
            }




            #region  |  new project version  |

            ProjectVersion = new ProjectVersion
            {
                id = Guid.NewGuid().ToString(),
                parentId = CurrentProjectInfo.Id.ToString()
            };

            var existingNames = CurrentProject.projectVersions.Select(pv => pv.name).ToList();

            ProjectVersion.name = Helper.GetUniqueName(CurrentProject.name, existingNames);
            ProjectVersion.description = CurrentProject.description;
            ProjectVersion.createdAt = Helper.GetStringFromDateTime(DateTime.Now);
            ProjectVersion.createdBy = CurrentProject.createdBy;

            ProjectVersion.shallowCopy = Settings.create_shallow_copy;
            ProjectVersion.location = Settings.versions_folder_path;
            ProjectVersion.projectFileName = CurrentProject.projectFileName;



            #region  |  get source language  |

            ProjectVersion.sourceLanguage = new Structures.LanguageProperty
            {
                id = CurrentProjectInfo.SourceLanguage.CultureInfo.Name,
                name = CurrentProjectInfo.SourceLanguage.DisplayName
            };

            #endregion

            #region  |  get target langauges  |
            ProjectVersion.targetLanguages = new List<Structures.LanguageProperty>();
            foreach (var language in CurrentProjectInfo.TargetLanguages)
            {
                var targetLanguage = new Structures.LanguageProperty
                {
                    id = language.CultureInfo.Name,
                    name = language.DisplayName
                };

                ProjectVersion.targetLanguages.Add(targetLanguage);
            }
            #endregion

            #region  |  get files  |
            ProjectVersion.translatableCount = 0;
            ProjectVersion.referenceCount = 0;
            ProjectVersion.localizableCount = 0;
            ProjectVersion.unKnownCount = 0;
            ProjectVersion.files = new List<FileProperty>();
            #region  |  get source files  |

            ProjectVersion.localizableCount = CurrentProject.localizableCount;
            ProjectVersion.referenceCount = CurrentProject.referenceCount;
            ProjectVersion.translatableCount = CurrentProject.translatableCount;
            ProjectVersion.unKnownCount = CurrentProject.unKnownCount;

            #endregion

            #endregion

            #endregion

            textBox_name.Text = ProjectVersion.name;
            textBox_location.Text = ProjectVersion.location;
            textBox_description.Text = ProjectVersion.description;
            textBox_createdAt.Text = ProjectVersion.createdAt;
            checkBox_createShallowCopy.Checked = ProjectVersion.shallowCopy;
            checkBox_createSubFolderProject.Checked = true;
        }

        private void CheckEnableContinue()
        {
            if (textBox_name.Text.Trim() != string.Empty
                && textBox_location.Text.Trim() != string.Empty
                && Directory.Exists(textBox_location.Text))
            {
                button_wizard_continue.Enabled = true;
            }
            else
            {
                button_wizard_continue.Enabled = false;
            }
        }

        private void button_wizard_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, PluginResources.No_help_file_found, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_wizard_continue_Click(object sender, EventArgs e)
        {
            if (IndexCurrentPanel == 0)
            {
                IndexCurrentPanel++;
                switch_Panel(IndexCurrentPanel == 0 ? panel_details : panel_progress);
            }
            else
            {
                Saved = true;
                Close();
            }


        }

        private void button_wizard_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_browseProjectVersionsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var sPath = textBox_location.Text;

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
                    Title = PluginResources.Select_Project_Versions_Folder,
                    InitialDirectory = sPath
                };
                if (!fsd.ShowDialog(IntPtr.Zero)) return;
                if (fsd.FileName.Trim() == string.Empty) return;
                sPath = fsd.FileName;


                textBox_location.Text = sPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                CheckEnableContinue();
            }
        }

        private void linkLabel_viewFoldersInWindowsExplorer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var sPath = textBox_location.Text;

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
                if (Directory.Exists(sPath))
                {
                    Process.Start(sPath);
                }
                else
                {
                    MessageBox.Show(this, PluginResources.Invalid_directory, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            CheckEnableContinue();
        }

        private void textBox_location_TextChanged(object sender, EventArgs e)
        {
            CheckEnableContinue();
        }

        private void comboBox_projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeProjectDetails();
        }
    }
}
