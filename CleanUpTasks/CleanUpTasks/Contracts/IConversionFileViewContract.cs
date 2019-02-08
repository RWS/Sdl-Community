using System.Windows.Forms;

namespace Sdl.Community.CleanUpTasks.Contracts
{
    internal abstract class IConversionFileViewContract : IConversionFileView
    {
        public BindingSource BindingSource => default(BindingSource);

	    public CheckBox CaseSensitive => default(CheckBox);

	    public Button ClearFilter => default(Button);

	    public GroupBox ColumnFilter => default(GroupBox);

	    public TextBox Description => default(TextBox);

	    public DialogResult DialogResult { get; set; }

        public CheckBox EmbeddedTags => default(CheckBox);

	    public ErrorProvider ErrorProvider => default(ErrorProvider);

	    public TextBox Filter => default(TextBox);

	    public Form Form => default(Form);

	    public DataGridView Grid => default(DataGridView);

	    public CheckBox IgnoreTags => default(CheckBox);

	    public CheckBox Placeholder => default(CheckBox);

	    public CheckBox Regex => default(CheckBox);

	    public TextBox Replace => default(TextBox);

	    public Button SaveAsButton => default(Button);

	    public Button SaveButton => default(Button);

	    public string SavedFilePath { get; set; }
	
        public TextBox Search => default(TextBox);

	    public CheckBox StrConv => default(CheckBox);
	    public CheckBox TagPair => default(CheckBox);

	    public CheckBox ToLower => default(CheckBox);

	    public CheckBox ToUpper => default(CheckBox);

	    public CheckBoxComboBox.CheckBoxComboBox VbStrConv => default(CheckBoxComboBox.CheckBoxComboBox);

	    public CheckBox WholeWord => default(CheckBox);

	    public void Dispose()
        {
        }

        public void InitializeUI()
        {
        }

        public void SetPresenter(IConversionFileViewPresenter presenter)
        {
        }

        public DialogResult ShowDialog()
        {
            return default(DialogResult);
        }
    }
}