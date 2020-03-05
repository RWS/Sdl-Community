using System;
using System.Windows.Forms;
using SDLCommunityCleanUpTasks;

namespace Sdl.Community.CleanUpTasks
{
	public partial class ConversionFileView : Form, IConversionFileView
    {
        private readonly ICleanUpConversionSettings settings = null;
        private IConversionFileViewPresenter presenter = null;

        public ConversionFileView(ICleanUpConversionSettings settings)
        {
            this.settings = settings;

            InitializeComponent();

            saveAsButton.Click += SaveAsButton_Click;
            saveButton.Click += SaveButton_Click;
            dataGridView.RowsAdded += DataGridView_RowsAdded;
            dataGridView.RowsRemoved += DataGridView_RowsRemoved;
            searchTextBox.KeyUp += SearchTextBox_KeyUp;
            replaceTextBox.KeyUp += ReplaceTextBox_KeyUp;
            descriptionTextBox.KeyUp += DescriptionTextBox_KeyUp;
        }

        private void DescriptionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bindingSource.ResetBindings(false);
        }

        public BindingSource BindingSource { get { return bindingSource; } }

        public CheckBox CaseSensitive { get { return caseSensitiveCheckBox; } }

        public Button ClearFilter { get { return clearButton; } }

        public GroupBox ColumnFilter { get { return groupBox; } }

        public TextBox Description { get { return descriptionTextBox; } }

        public ErrorProvider ErrorProvider { get { return errorProvider; } }

        public TextBox Filter { get { return filterTextBox; } }

        public Form Form { get { return this; } }

        public DataGridView Grid { get { return dataGridView; } }

        public CheckBox Placeholder { get { return placeHolderCheckBox; } }

        public CheckBox Regex { get { return regexCheckBox; } }

        public TextBox Replace { get { return replaceTextBox; } }

        public Button SaveAsButton { get { return saveAsButton; } }

        public Button SaveButton { get { return saveButton; } }

        public string SavedFilePath { get; set; }

        public TextBox Search { get { return searchTextBox; } }

        public CheckBox EmbeddedTags { get { return embeddedTagsCheckBox; } }

        public CheckBox StrConv { get { return strConvCheckBox; } }

        public CheckBox TagPair { get { return tagPairCheckBox; } }

        public CheckBox ToLower { get { return toLowerCheckBox; } }

        public CheckBox ToUpper { get { return toUpperCheckBox; } }

        public CheckBoxComboBox.CheckBoxComboBox VbStrConv { get { return checkBoxComboBox; } }

        public CheckBox WholeWord { get { return wholeWordCheckBox; } }

        public void InitializeUI()
        {
            presenter.Initialize();
        }

        public void SetPresenter(IConversionFileViewPresenter presenter)
        {
            this.presenter = presenter;
        }

        private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            presenter.CheckSaveButton();
        }

        private void DataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            presenter.CheckSaveButton();
        }

        private void ReplaceTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bindingSource.ResetBindings(false);
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            presenter.SaveFile(settings.LastFileDirectory, true);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            presenter.SaveFile(settings.LastFileDirectory, false);
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bindingSource.ResetBindings(false);
        }
    }
}