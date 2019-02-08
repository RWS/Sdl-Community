using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.CleanUpTasks
{
	public partial class ConversionsSettingsControl : UserControl, IConversionsSettingsControl
    {
        private IConversionsSettingsPresenter presenter = null;
        private BatchTaskMode taskMode;

        public ConversionsSettingsControl()
        {
            InitializeComponent();

            addButton.Click += AddButton_Click;
            removeButton.Click += RemoveButton_Click;
            generateButton.Click += GenerateButton_Click;
            editButton.Click += EditButton_Click;
            upButton.Click += UpButton_Click;
            downButton.Click += DownButton_Click;
        }

        public Button Add { get { return addButton; } }

        public Button Down { get { return downButton; } }

        public Button Edit { get { return editButton; } }

        public CheckedListBox FileList { get { return checkedListBox; } }

        public Button New { get { return generateButton; } }

        public Button Remove { get { return removeButton; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICleanUpConversionSettings Settings { get; set; } = new CleanUpSourceSettings();

        public Button Up { get { return upButton; } }

        public CheckBox ApplyToNonTranslatables { get { return useOnTranslatableCheckBox; } }

        public void SaveSettings()
        {
            presenter.SaveSettings();
        }

        public void SetPresenter(IConversionsSettingsPresenter presenter)
        {
            this.presenter = presenter;
        }

        public void SetSettings(ICleanUpConversionSettings settings, BatchTaskMode taskMode)
        {
            SettingsBinder.DataBindSetting<bool>(convCheckGroupBox, "Checked", settings.Settings,
                                                 nameof(settings.UseConversionSettings));

            SettingsBinder.DataBindSetting<bool>(useOnTranslatableCheckBox, "Checked", settings.Settings,
                                                 nameof(settings.ApplyToNonTranslatables));

            Settings = settings;
            this.taskMode = taskMode;
        }

        public void InitializeUI()
        {
            presenter.Initialize();
        }

        private void AddButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                presenter.AddFile();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($@"An error occurred.
                                Are you sure this file is valid?
                                Error: {ex.Message}");
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            presenter.DownClick();
        }

        private void EditButton_Click(object sender, System.EventArgs e)
        {
            using (var view = ViewFactory.Create<IConversionFileView>(Settings, ConversionFileViewMode.Existing, taskMode))
            {
                presenter.EditFile(view);
            }
        }

        private void GenerateButton_Click(object sender, System.EventArgs e)
        {
            using (var view = ViewFactory.Create<IConversionFileView>(Settings, ConversionFileViewMode.New, taskMode))
            {
                presenter.GenerateFile(view);
            }
        }

        private void RemoveButton_Click(object sender, System.EventArgs e)
        {
            presenter.RemoveFile();
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            presenter.UpClick();
        }
    }
}