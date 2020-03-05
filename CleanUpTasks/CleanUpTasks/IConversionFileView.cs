using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(IConversionFileViewContract))]
    public interface IConversionFileView : IDisposable
    {
        BindingSource BindingSource { get; }
        CheckBox CaseSensitive { get; }
        Button ClearFilter { get; }
        GroupBox ColumnFilter { get; }
        TextBox Description { get; }
        DialogResult DialogResult { get; set; }
        ErrorProvider ErrorProvider { get; }
        TextBox Filter { get; }
        Form Form { get; }
        DataGridView Grid { get; }
        CheckBox Placeholder { get; }
        CheckBox Regex { get; }
        TextBox Replace { get; }
        Button SaveAsButton { get; }
        Button SaveButton { get; }
        string SavedFilePath { get; set; }
        TextBox Search { get; }
        CheckBox EmbeddedTags { get; }
        CheckBox StrConv { get; }
        CheckBox TagPair { get; }
        CheckBox ToLower { get; }
        CheckBox ToUpper { get; }
        Sdl.Community.CheckBoxComboBox.CheckBoxComboBox VbStrConv { get; }
        CheckBox WholeWord { get; }

        void InitializeUI();

        void SetPresenter(IConversionFileViewPresenter presenter);

        DialogResult ShowDialog();
    }
}