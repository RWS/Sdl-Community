using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Studio.SpotCheck.SdlXliff;
using Sdl.Studio.SpotCheck.Helpers;

namespace Sdl.Studio.SpotCheck
{
    public partial class SpotCheckViewPartControl : UserControl
    {
        #region variables

        private SpotCheckProcessor _processor;
        private FilesController _filesController;
        private EditorController _editorController;
        private ProjectsController _projectsController;
        private ApplicationSettings _settings;
        private bool _updatingUi = false;
        private static Random _rnd = new Random();

        #endregion

        #region init

        public SpotCheckViewPartControl()
        {
            InitializeComponent();
            _settings = new ApplicationSettings();

            this.Load += SpotCheckViewPartControl_Load;

            _processor = new SpotCheckProcessor();
            _filesController = SdlTradosStudio.Application.GetController<FilesController>();
            _editorController = SdlTradosStudio.Application.GetController<EditorController>();
            _projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            }

        private void SpotCheckViewPartControl_Load(object sender, EventArgs e)
        {
            string[] args = new string[2];
            _settings.Init(new string[0]); // to get the default settings path - which is Studio, so change it:
            args[0] = "-f";
            args[1] = ChangeName(_settings.SettingsFile, "Studio.Plugins.Spotcheck");
            if (File.Exists(args[1]))
                _settings.Init(args);
            UpdateUiFromSettings();
            // was a requirement, no longer
            // chkSkipLocked.CheckState = CheckState.Indeterminate;
}

        #endregion

        #region event handlers

        private void cmdAddMarkers_Click(object sender, EventArgs e)
        {
            /* was a requirement, no longer
            if (chkSkipLocked.CheckState == CheckState.Indeterminate)
            {
                MessageBox.Show("Please select how to handle locked segments in the options first.", "Undefined option", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            */
            ClearEditor();
            Note("Processing...");
            UpdateSettingsFromUi();

            List<ProjectFile> translatableFiles = _filesController.SelectedFiles.Where(f => f.Role == FileRole.Translatable).ToList();
            if (translatableFiles.Count == 0)
                Note("No translatable files are selected");
            Shuffle(translatableFiles);

            List<ProjectFile> validFiles = new List<ProjectFile>();
            List<string> alreadyMarkedFiles = new List<string>();
            bool addedMarkers = false;

            int totalSelectedWords = 0;
            int totalSegments = 0;

            int totalWords = 0;
            double fraction = 1;


            if (_settings.LimitByWords)
            {
                foreach (ProjectFile file in translatableFiles)
                    // When a file is first opened, need to trigger word count
                    if (file.AnalysisStatistics.Total.Words == 0)
                        _projectsController.CurrentProject.RunAutomaticTask(new Guid[] { file.Id }, AutomaticTaskTemplateIds.WordCount);
                // for some weird reason, the word count statistics are not updated unless
                // we jump through the hoop of reselecting the files
                translatableFiles = _filesController.SelectedFiles.Where(f => f.Role == FileRole.Translatable).ToList();
                foreach (ProjectFile file in translatableFiles)
                    totalWords += file.AnalysisStatistics.Total.Words;
                fraction = (double)totalWords / _settings.TotalWords;
            }

            int requestedWords = _settings.TotalWords;
            foreach (ProjectFile file in translatableFiles)
            {
                _processor.Open(file.LocalFilePath);
                if (CommentHandler.ContainsComments(file.LocalFilePath))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);
                    alreadyMarkedFiles.Add(fileName);
                }
                else
                {
                    if (_settings.LimitByWords)
                        _settings.TotalWords = (int)(file.AnalysisStatistics.Total.Words / fraction);
                    bool addedThisFile = _processor.AddMarkers(_settings);
                    totalSegments += _processor.Segments;
                    addedMarkers = addedMarkers || addedThisFile;
                    if (addedThisFile)
                        validFiles.Add(file);
                    if (_settings.LimitByWords)
                    {
                        totalSelectedWords += _processor.Words;
                        if (totalSelectedWords > requestedWords)
                        {
                            _processor.Close();
                            break;
                        }
                    }
                }
                _processor.Close();
            }
            if (_settings.LimitByWords)
            {
                Note($"Selected {totalSelectedWords} words in {totalSegments} segments.");
            }
            else
            {
                Note($"Selected {totalSegments} segments.");
            }

            if (alreadyMarkedFiles.Count > 0)
            {
                if (alreadyMarkedFiles.Count == 1)
                    Note("This file does already contain markers: " + alreadyMarkedFiles[0]);
                else
                    Note("These files do already contain markers: " + string.Join(", ", alreadyMarkedFiles.ToArray()));
            }

            if (validFiles.Count > 0)
            {
                if (!addedMarkers)
                {
                    Note("No segment could be found that matches the settings.");
                }
                else
                {
                    // was a requirement, no longer
                    // chkSkipLocked.CheckState = CheckState.Indeterminate;
                    _editorController.Open(validFiles, EditingMode.Review);
                }
            }
            else
            {
                ClearNotes();
            }
            // was a requirement, no longer
            // chkSkipLocked.CheckState = CheckState.Indeterminate;
        }

        private void cmdRemoveMarkers_Click(object sender, EventArgs e)
        {
            ClearEditor();
            Note("Processing...");

            List<ProjectFile> translatableFiles = _filesController.SelectedFiles.Where(f => f.Role == FileRole.Translatable).ToList();
            if (translatableFiles.Count == 0)
                Note("No translatable files are selected");

            List<ProjectFile> validFiles = new List<ProjectFile>();
            List<string> invalidFiles = new List<string>();
            foreach (ProjectFile file in translatableFiles)
            {
                _processor.Open(file.LocalFilePath);
                if (!CommentHandler.ContainsComments(file.LocalFilePath))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);
                    invalidFiles.Add(fileName);
                }
                else
                {
                    _processor.RemoveMarkers(_settings);
                    validFiles.Add(file);
                }
                _processor.Close();
            }
            if (invalidFiles.Count > 0)
            {
                if (invalidFiles.Count == 1)
                    Note("This file does not contain markers: " + invalidFiles[0]);
                else
                    Note("These files do not contain markers: " + string.Join(", ", invalidFiles.ToArray()));
            }
            if (validFiles.Count > 0)
            {
                Note("");
                _editorController.Open(validFiles, EditingMode.Review);
            }
            else
            {
                ClearNotes();
            }
        }


        #endregion

        #region save settings on change

        private void numPercentage_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void numMinWords_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void numMaxWords_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void chkSkipLocked_CheckStateChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void chkSkip100_CheckStateChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void chkSkipCm_CheckStateChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void chkLimitByWords_CheckedChanged(object sender, EventArgs e)
        {
            lblWordLimit.Enabled = numTotalWords.Enabled = chkLimitByWords.Checked;
            lblSegmentLimit.Enabled = numPercentage.Enabled = !chkLimitByWords.Checked;
            SaveSettings();
        }
        private void chkSkipRepetition_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void numTotalWords_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            UpdateSettingsFromUi();
            _settings.Save(ChangeName(_settings.SettingsFile, "Studio.Plugins.Spotcheck"));
        }

        #endregion

        #region helpers

        private void ClearEditor()
        {
            // need to close documents to ensure the modified ones are reloaded
            List<Document> allDocs = new List<Document>(_editorController.GetDocuments());
            foreach (Document doc in allDocs)
            {
                _editorController.Save(doc);
                _editorController.Close(doc);
            }
        }

        private void UpdateSettingsFromUi()
        {
            if (_settings == null || _updatingUi) return; //during initialisation

            _settings.MinWords = (int)numMinWords.Value;
            _settings.MaxWords = (int)numMaxWords.Value;
            _settings.Percentage = (int)numPercentage.Value;
            _settings.SkipLocked = chkSkipLocked.Checked;
            _settings.SkipCm = chkSkipCm.Checked;
            _settings.SkipRepetition = chkSkipRepetition.Checked;
            _settings.Skip100 = chkSkip100.Checked;
            _settings.LimitByWords = chkLimitByWords.Checked;
            _settings.TotalWords = (int)numTotalWords.Value;
        }

        private void UpdateUiFromSettings()
        {
            if (_settings == null) return; //during initialisation

            _updatingUi = true;

            numMinWords.Value = _settings.MinWords;
            numMaxWords.Value = _settings.MaxWords;
            numPercentage.Value = _settings.Percentage;
            numTotalWords.Value = _settings.TotalWords;

            chkSkipCm.Checked = _settings.SkipCm;
            chkSkipRepetition.Checked = _settings.SkipRepetition;
            chkSkip100.Checked = _settings.Skip100;
            chkSkipLocked.Checked = _settings.SkipLocked;
            chkLimitByWords.Checked = _settings.LimitByWords;

            _updatingUi = false;
        }

        private string ChangeName(string path, string newName)
        {
            string folder = Path.GetDirectoryName(path);
            string extension = Path.GetExtension(path);
            string newPath = Path.Combine(folder, newName + extension);
            return newPath;
        }

        private void Note(string text)
        {
            lblNotes.Text = text;
            lblNotes.Refresh();
        }

        private void ClearNotes()
        {
            TimerCallback tc = timerCallback;
            System.Threading.Timer t = new System.Threading.Timer(tc, lblNotes, 5000, System.Threading.Timeout.Infinite);
        }

        void timerCallback(object info)
        {
            Note("");
        }

        public static void Shuffle(List<ProjectFile> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rnd.Next(n + 1);
                ProjectFile value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion

    }
}
