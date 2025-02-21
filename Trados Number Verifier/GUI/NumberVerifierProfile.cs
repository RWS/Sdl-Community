using Sdl.Community.NumberVerifier.DTOs;
using Sdl.Community.NumberVerifier.Extensions;
using Sdl.Community.NumberVerifier.Services;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.NumberVerifier.GUI
{
    public partial class NumberVerifierProfile : UserControl
    {
        private readonly FilePathDialogService _filePathDialogService;
        private readonly NumberVerifierProfileManager _profileManager;
        private NumberVerifierSettings _settings;
        private bool _profileMatch;
        private string _profilePath;

        public NumberVerifierProfile(
            FilePathDialogService filePathDialogService,
            NumberVerifierSettings settings, 
            NumberVerifierProfileManager manager)
        {
            _settings = settings;
            _profilePath = _settings.ProfilePath ?? string.Empty;

            InitializeComponent();
            _filePathDialogService = filePathDialogService;
            _profileManager = manager;
            _profileMatch = !_profileManager.ProfileHasChanged(_settings);
            SetCurrentProfileLabel();
        }

        private void button1_ImportSettings_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(
                PluginResources.NumberVerifierProfile_Import_ConfirmationMessage,
                PluginResources.NumberVerifierProfile_QuestionTitle, 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question) == DialogResult.No)
                    return;

            var initialDirectory = GetInitialDirectory();
            var files = _filePathDialogService.GetFilePathInputFromUser(
                PluginResources.NumberVerifierProfile_ExportDialogTitle, 
                initialDirectory, 
                "Settings XML File | *.xml");

            if (files is null || !files.Any()) return;
            var filePath = files.First();

            try
            {
                var settingsDto = _profileManager.Importer.ImportSettings(filePath);
                if (settingsDto == null)
                {
                    MessageBox.Show(PluginResources.NumberVerifierProfile_ImportOnInvalid, PluginResources.NumberVerifierProfile_OperationErrorTitle);
                    return;
                }

                settingsDto.OverwriteNumberVerifierSettings(_settings);
                _settings.ProfilePath = filePath;
                _profilePath = filePath;
                _profileMatch = true;
                SetCurrentProfileLabel();

                MessageBox.Show(PluginResources.NumberVerifierProfile_ImportOnSuccess, PluginResources.NumberVerifierProfile_OperationSuccessTitle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, PluginResources.NumberVerifierProfile_OperationErrorTitle);
            }
        }

        private void button2_ExportSettings_Click(object sender, System.EventArgs e)
        {
            var fileName = "My Settings";
            if (_settings.ProfilePath != null && File.Exists(_settings.ProfilePath))
                fileName = Path.GetFileName(_settings.ProfilePath);

            var saveLocation = GetSaveLocation(
                PluginResources.NumberVerifierProfile_ExportDialogTitle, 
                "Settings XML File | *.xml", 
                fileName);

            if (string.IsNullOrEmpty(saveLocation)) return;
            if (_settings is null) return;

            var settings = _settings.ToSettingsDTO();

            try
            {
                _profileManager.Exporter.ExportSettings(saveLocation, settings);
                MessageBox.Show(PluginResources.NumberVerifierProfile_Export_OnSuccess, PluginResources.NumberVerifierProfile_OperationSuccessTitle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, PluginResources.NumberVerifierProfile_OperationErrorTitle);
            }
        }

        private string GetSaveLocation(string title, string extension, string fileName)
        {
            var saveLocation = "";
            try
            {
                _filePathDialogService.GetSaveLocationInputFromUser(out saveLocation,
                    title, extension, fileName);
            }
            catch (Exception e)
            {
            }

            return saveLocation;
        }

        private void SetCurrentProfileLabel()
        {
            if (string.IsNullOrEmpty(_settings.ProfilePath)) return;

            if (_profileMatch)
            {
                labelProfilePath.Text = _settings.ProfilePath;
                labelProfilePath.Location = new System.Drawing.Point(
                    labelCurrentProfile.Location.X + labelCurrentProfile.Width + 3,
                    labelCurrentProfile.Location.Y
                );
                labelProfilePath.Visible = true;
                labelCurrentProfile.Visible = true;

            }
            else
            {
                labelProfileMismatchMessage.Visible = true;
            }
        }

        private string GetInitialDirectory()
        {
            var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
            return projectController.CurrentProject?.FilePath ?? string.Empty;
        }
    }
}
