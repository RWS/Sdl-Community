using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Versions.Structures;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    public partial class RestoreProjectVersion : Form
    {

        public bool Saved { get; set; }
        public ProjectsController ProjectsController { get; set; }

        public ProjectVersion ProjectVersionNew { get; set; }
        public ProjectVersion ProjectVersion { get; set; }
        public Project CurrentProject { get; set; }

        public ProjectInfo CurrentProjectInfo { get; set; }
        public Settings Settings { get; set; }

        private bool IsLoading { get; set; }

        private int IndexCurrentPanel { get; set; }
        private int TotalFiles { get; set; }


        public RestoreProjectVersion()
        {
            InitializeComponent();
            IsLoading = true;
            panel_details.Dock = DockStyle.Fill;
            panel_progress.Dock = DockStyle.Fill;
        }

        private void SaveVersion_Load(object sender, EventArgs e)
        {


            IsLoading = false;

            textBox_project_name.Text = CurrentProject.name;
            textBox_project_location.Text = CurrentProject.location;


            textBox_project_version_name.Text = ProjectVersion.name;
            textBox_project_version_location.Text = ProjectVersion.location;
            textBox_project_version_description.Text = ProjectVersion.description;
            textBox_project_version_createdAt.Text = ProjectVersion.createdAt;
            textBox_project_version_total_files.Text = ProjectVersion.filesCopiedCount.ToString();

            if (ProjectVersion.shallowCopy)
            {
                checkBox_restore_shallow_copy.Enabled = false;
                checkBox_restore_shallow_copy.Checked = true;
            }
            else
            {
                checkBox_restore_shallow_copy.Enabled = true;
                checkBox_restore_shallow_copy.Checked = true;
            }

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
                        label_titleBar_title.Text = PluginResources.Restore_Project_Version_Step_1_of_2;
                        label_titlebar_description.Text = PluginResources.Review_the_information_and_select_the_appropriate_options_;
                        label_titlebar_note.Text = PluginResources.Note_Click_on_the_button_Continue_to_proceed_with_the_restoring_the_project_;

                        panel_details.BringToFront();

                        button_wizard_help.Enabled = false;

                        button_wizard_continue.Text = PluginResources.Continue;
                        button_wizard_continue.Enabled = true;
                        button_wizard_cancel.Enabled = true;
                        break;
                    }
                case "panel_progress":
                    {
                        label_titleBar_title.Text = PluginResources.Restore_Project_Version_Step_2_of_2;
                        label_titlebar_description.Text = PluginResources.Progress_Restoring_the_project_from_project_version_;
                        label_titlebar_note.Text = "";

                        panel_progress.BringToFront();

                        button_wizard_help.Enabled = false;

                        button_wizard_continue.Text = PluginResources.Close;
                        button_wizard_continue.Enabled = true;
                        button_wizard_cancel.Enabled = false;


                        label_progress_process.Text = "...";
                        label_progress_percentage.Text = "0%";
                        label_progress_message.Text = "...";


                        break;
                    }
            }
        }





        private void RestoreProjectFromProjectVersion()
        {
            try
            {
                try
                {
                    CreateProjectVersion();
                }
                catch (Exception ex)
                {
                    throw new Exception(PluginResources.Error_during_restore_point_creation + ex.Message);
                }

                try
                {

                    Cursor = Cursors.WaitCursor;

                    button_wizard_continue.Enabled = false;
                    button_wizard_cancel.Enabled = false;
                    button_wizard_help.Enabled = false;




                    textBox_progress_deatails.Text += "\r\n\r\n";
                    textBox_progress_deatails.Text += PluginResources.Restoring_Project_from_Project_Version_ + "\r\n\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Name_0, CurrentProject.name) + "\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Location_0, CurrentProject.location) + "\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Source_Language_0, CurrentProject.sourceLanguage.name) + "\r\n";
                    var targetLanguages = CurrentProject.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Target_Languages_0, targetLanguages) + "\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Source_Files_Translatable_Reference_Localizable_Unknown, CurrentProject.translatableCount, CurrentProject.referenceCount, CurrentProject.localizableCount, CurrentProject.unKnownCount) + "\r\n\r\n";

                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Name, ProjectVersion.name) + "\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Location_0, ProjectVersion.location) + "\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Soruce_Language_0, ProjectVersion.sourceLanguage.name) + "\r\n";
                    var targetLanguagesVersion = ProjectVersion.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Target_Languages_0, targetLanguagesVersion) + "\r\n";
                    textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Soruce_Files_Translatable_Reference_Localizable_Unknown, ProjectVersion.translatableCount, ProjectVersion.referenceCount, ProjectVersion.localizableCount, ProjectVersion.unKnownCount) + "\r\n\r\n";


                    label_progress_process.Text = PluginResources.Initializing_restore_project_from_project_version_procedure_;
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
                            CountAll(new DirectoryInfo(ProjectVersion.location), checkBox_restore_shallow_copy.Checked ? "*.sdlxliff" : "*.*");

                            progress_progressBar.Value = 0;
                            progress_progressBar.Maximum = TotalFiles;

                            #region  |  copy .sdlproj file always  |
                            if (checkBox_restore_shallow_copy.Checked)
                            {
                                var files = new DirectoryInfo(ProjectVersion.location).GetFiles("*.sdlproj").ToList();

                                if (files.Count > 0)
                                {
                                    var fiToCopy = files[0];

                                    if (files.Count == 1)
                                    {
                                        File.Copy(fiToCopy.FullName, Path.Combine(CurrentProject.location, fiToCopy.Name), true);
                                    }
                                    else
                                    {
                                        foreach (var fi in files)
                                        {
                                            if (fi.Name.ToLower().IndexOf(ProjectVersion.name.ToLower().Trim(), StringComparison.Ordinal) <= -1)
                                                continue;
                                            fiToCopy = fi;
                                            break;
                                        }
                                        File.Copy(fiToCopy.FullName, Path.Combine(CurrentProject.location, fiToCopy.Name), true);
                                    }

                                    TotalFiles++;
                                    progress_progressBar.Maximum++;
                                    progress_progressBar.Value++;
                                }

                            }
                            #endregion


                            ProjectVersion.filesCopiedCount = TotalFiles;

                            CopyAll(new DirectoryInfo(ProjectVersion.location), new DirectoryInfo(CurrentProject.location), checkBox_restore_shallow_copy.Checked ? "*.sdlxliff" : "*.*");


                            #endregion
                        }
                        else
                        {
                            #region  |  single file project  |
                            if (ProjectVersion.translatableCount == 1 && ProjectVersion.location.EndsWith(".ProjectFiles"))
                            {
                                var locationSdlproj = ProjectVersion.location.Substring(0, ProjectVersion.location.LastIndexOf(".ProjectFiles", StringComparison.Ordinal)) + ".sdlproj";
                                var locationSource = ProjectVersion.location.Substring(0, ProjectVersion.location.LastIndexOf(".ProjectFiles", StringComparison.Ordinal));
                                var locationSdlxliff = ProjectVersion.location.Substring(0, ProjectVersion.location.LastIndexOf(".ProjectFiles", StringComparison.Ordinal)) + ".sdlxliff";


                                progress_progressBar.Maximum = 0;
                                progress_progressBar.Value = 0;


                                if (File.Exists(locationSdlproj))
                                {
                                    File.Copy(locationSdlproj, Path.Combine(CurrentProject.location, Path.GetFileName(locationSdlproj)), true);

                                    TotalFiles++;
                                    progress_progressBar.Maximum++;
                                    progress_progressBar.Value++;


                                }

                                if (File.Exists(locationSdlxliff))
                                {
                                    File.Copy(locationSdlxliff, Path.Combine(CurrentProject.location, Path.GetFileName(locationSdlxliff)), true);
                                    TotalFiles++;
                                    progress_progressBar.Maximum++;
                                    progress_progressBar.Value++;
                                }

                                try
                                {
                                    if (!ProjectVersionNew.shallowCopy)
                                    {
                                        if (File.Exists(locationSource))
                                        {
                                            File.Copy(locationSource, Path.Combine(CurrentProject.location, Path.GetFileName(locationSource)), true);
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

                        textBox_progress_deatails.Text += string.Format(PluginResources.Total_Files_Retored_from_Project_Version_0, ProjectVersion.filesCopiedCount) + "\r\n\r\n";


                        if (ProjectVersion.filesCopiedCount == 0)
                        {
                            label_titlebar_note.Text = PluginResources.Warning_no_local_files_where_copied_during_the_project_project_restore_process;
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
                catch
                {
                    Cursor = Cursors.Default;
                    Saved = false;
                    throw;
                }
                finally
                {

                    Cursor = Cursors.Default;
                    button_wizard_continue.Enabled = true;
                    button_wizard_cancel.Enabled = false;
                    button_wizard_help.Enabled = false;
                }
            }
            catch (Exception ex)
            {
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



        private void CreateProjectVersion()
        {
            try
            {

                #region  |  new project version  |

                ProjectVersionNew = new ProjectVersion
                {
                    id = Guid.NewGuid().ToString(),
                    parentId = CurrentProjectInfo.Id.ToString()
                };

                var existingNames = CurrentProject.projectVersions.Select(pv => pv.name).ToList();

                ProjectVersionNew.name = Helper.GetUniqueName(CurrentProject.name, existingNames);
                ProjectVersionNew.description = string.Format(PluginResources.RESTORE_POINT_before_restoring_the_Project_from_the_Project_Version, "\r\n", ProjectVersion.id, "\r\n", ProjectVersion.name);
                ProjectVersionNew.createdAt = Helper.GetStringFromDateTime(DateTime.Now);
                ProjectVersionNew.createdBy = CurrentProject.createdBy;

                ProjectVersionNew.shallowCopy = false;
                ProjectVersionNew.location = Path.Combine(Settings.versions_folder_path, ProjectVersionNew.createdAt);
                if (!Directory.Exists(ProjectVersionNew.location))
                    Directory.CreateDirectory(ProjectVersionNew.location);
                ProjectVersionNew.projectFileName = CurrentProject.projectFileName;



                #region  |  get source language  |

                ProjectVersionNew.sourceLanguage = new Structures.LanguageProperty
                {
                    id = CurrentProjectInfo.SourceLanguage.CultureInfo.Name,
                    name = CurrentProjectInfo.SourceLanguage.DisplayName
                };

                #endregion

                #region  |  get target langauges  |
                ProjectVersionNew.targetLanguages = new List<Structures.LanguageProperty>();
                foreach (var language in CurrentProjectInfo.TargetLanguages)
                {
                    var targetLanguage = new Structures.LanguageProperty
                    {
                        id = language.CultureInfo.Name,
                        name = language.DisplayName
                    };

                    ProjectVersionNew.targetLanguages.Add(targetLanguage);
                }
                #endregion

                #region  |  get files  |
                ProjectVersionNew.translatableCount = 0;
                ProjectVersionNew.referenceCount = 0;
                ProjectVersionNew.localizableCount = 0;
                ProjectVersionNew.unKnownCount = 0;
                ProjectVersionNew.files = new List<FileProperty>();
                #region  |  get source files  |


                ProjectVersionNew.localizableCount = CurrentProject.localizableCount;
                ProjectVersionNew.referenceCount = CurrentProject.referenceCount;
                ProjectVersionNew.translatableCount = CurrentProject.translatableCount;
                ProjectVersionNew.unKnownCount = CurrentProject.unKnownCount;

                #endregion

                #endregion

                #endregion


                Cursor = Cursors.WaitCursor;

                button_wizard_continue.Enabled = false;
                button_wizard_cancel.Enabled = false;
                button_wizard_help.Enabled = false;



                textBox_progress_deatails.Text = string.Empty;
                textBox_progress_deatails.Text += string.Format(PluginResources.Start_Processing_0, DateTime.Now) + "\r\n\r\n";
                textBox_progress_deatails.Text += PluginResources.Creating_new_restore_point_from_the_project__ + "\r\n\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Name_0, CurrentProject.name) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Location_0, CurrentProject.location) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Source_Language_0, CurrentProject.sourceLanguage.name) + "\r\n";
                var targetLanguages = CurrentProject.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Target_Languages_0, targetLanguages) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Source_Files_Translatable_Reference_Localizable_Unknown, CurrentProject.translatableCount, CurrentProject.referenceCount, CurrentProject.localizableCount, CurrentProject.unKnownCount) + "\r\n\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Name, ProjectVersionNew.name) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Description_0, ProjectVersionNew.description) + "\r\n";
                textBox_progress_deatails.Text += string.Format(PluginResources.Project_Version_Location_0, ProjectVersionNew.location) + "\r\n";


                label_progress_process.Text = PluginResources.Initializing_restore_point_from_project_procedure__;
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
                        CountAll(new DirectoryInfo(CurrentProject.location), ProjectVersionNew.shallowCopy ? "*.sdlxliff" : "*.*");

                        progress_progressBar.Value = 0;
                        progress_progressBar.Maximum = TotalFiles;

                        #region  |  copy .sdlproj file always  |
                        if (ProjectVersionNew.shallowCopy)
                        {
                            var files = new DirectoryInfo(CurrentProject.location).GetFiles("*.sdlproj").ToList();
                            if (files.Count > 0)
                            {
                                var fiToCopy = files[0];

                                if (files.Count == 1)
                                {
                                    File.Copy(fiToCopy.FullName, Path.Combine(ProjectVersionNew.location, fiToCopy.Name), true);
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
                                    File.Copy(fiToCopy.FullName, Path.Combine(ProjectVersionNew.location, fiToCopy.Name), true);
                                }

                                TotalFiles++;
                                progress_progressBar.Maximum++;
                                progress_progressBar.Value++;
                            }

                        }
                        #endregion


                        ProjectVersionNew.filesCopiedCount = TotalFiles;

                        CopyAll(new DirectoryInfo(CurrentProject.location), new DirectoryInfo(ProjectVersionNew.location), ProjectVersionNew.shallowCopy ? "*.sdlxliff" : "*.*");


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
                                File.Copy(locationSdlproj, Path.Combine(ProjectVersionNew.location, Path.GetFileName(locationSdlproj)), true);

                                TotalFiles++;
                                progress_progressBar.Maximum++;
                                progress_progressBar.Value++;


                            }

                            if (File.Exists(locationSdlxliff))
                            {
                                File.Copy(locationSdlxliff, Path.Combine(ProjectVersionNew.location, Path.GetFileName(locationSdlxliff)), true);
                                TotalFiles++;
                                progress_progressBar.Maximum++;
                                progress_progressBar.Value++;
                            }

                            try
                            {
                                if (!ProjectVersionNew.shallowCopy)
                                {
                                    if (File.Exists(locationSource))
                                    {
                                        File.Copy(locationSource, Path.Combine(ProjectVersionNew.location, Path.GetFileName(locationSource)), true);
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


                            ProjectVersionNew.filesCopiedCount = TotalFiles;

                        }

                        #endregion
                    }

                    textBox_progress_deatails.Text += string.Format(PluginResources.Total_Files_included_in_the_Project_restore_point_0, ProjectVersionNew.filesCopiedCount) + "\r\n\r\n";


                    if (ProjectVersionNew.filesCopiedCount == 0)
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

                var message = PluginResources.Finished_creating_project_restore_point_without_errors;
                if (hasError)
                    message = PluginResources.Finished_creating_project_restore_point_with_errors;

                textBox_progress_deatails.Text += message + "\r\n\r\n";
                if (hasError)
                {
                    textBox_progress_deatails.Text += PluginResources.Exception_Message__ + errorMessage + "\r\n\r\n";
                }
            }
            catch
            {
                Cursor = Cursors.Default;
                Saved = false;
                throw;
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

                if (progress_progressBar.Value + 1 > progress_progressBar.Maximum) 
                    continue;

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

                label_progress_message.Text = string.Format(PluginResources.Copied_files_0_of_1, progress_progressBar.Value, progress_progressBar.Maximum);
                if (progress_progressBar.Value % 5 == 0)
                {
                    Application.DoEvents();                  
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


        private void CheckEnableContinue()
        {

            button_wizard_continue.Enabled = true;

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

                RestoreProjectFromProjectVersion();
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



        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            CheckEnableContinue();
        }

        private void textBox_location_TextChanged(object sender, EventArgs e)
        {
            CheckEnableContinue();
        }

        private void linkLabel_open_folder_project_location_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var sPath = textBox_project_location.Text;

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

        private void linkLabel_open_folder_project_version_location_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var sPath = textBox_project_version_location.Text;

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


    }
}
