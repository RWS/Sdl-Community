using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using SDLCommunityCleanUpTasks;
using SDLCommunityCleanUpTasks.Utilities;

namespace Sdl.Community.CleanUpTasks
{
	public partial class ConversionsSettingsControl : UserControl, IConversionsSettingsControl
    {
        private IConversionsSettingsPresenter _presenter;
        private BatchTaskMode _taskMode;

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

        public Button Add => addButton;

	    public Button Down => downButton;

	    public Button Edit => editButton;

	    public CheckedListBox FileList => checkedListBox;

	    public Button New => generateButton;

	    public Button Remove => removeButton;

	    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICleanUpConversionSettings Settings { get; set; } = new CleanUpSourceSettings();

        public Button Up => upButton;

	    public CheckBox ApplyToNonTranslatables => useOnTranslatableCheckBox;

	    public void SaveSettings()
        {
            _presenter.SaveSettings();
        }

        public void SetPresenter(IConversionsSettingsPresenter presenter)
        {
            _presenter = presenter;
        }

        public void SetSettings(ICleanUpConversionSettings settings, BatchTaskMode taskMode)
        {
            SettingsBinder.DataBindSetting<bool>(convCheckGroupBox, "Checked", settings.Settings,
                                                 nameof(settings.UseConversionSettings));

            SettingsBinder.DataBindSetting<bool>(useOnTranslatableCheckBox, "Checked", settings.Settings,
                                                 nameof(settings.ApplyToNonTranslatables));

            Settings = settings;
            _taskMode = taskMode;
        }

        public void InitializeUI()
        {
            _presenter.Initialize();
        }

        private void AddButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                _presenter.AddFile();
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
            _presenter.DownClick();
        }

        private void EditButton_Click(object sender, System.EventArgs e)
        {
            using (var view = ViewFactory.Create<IConversionFileView>(Settings, ConversionFileViewMode.Existing, _taskMode))
            {
                _presenter.EditFile(view);
            }
        }

        private void GenerateButton_Click(object sender, System.EventArgs e)
        {
            using (var view = ViewFactory.Create<IConversionFileView>(Settings, ConversionFileViewMode.New, _taskMode))
            {
                _presenter.GenerateFile(view);
            }
        }

        private void RemoveButton_Click(object sender, System.EventArgs e)
        {
            _presenter.RemoveFile();
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            _presenter.UpClick();
        }
    }
}