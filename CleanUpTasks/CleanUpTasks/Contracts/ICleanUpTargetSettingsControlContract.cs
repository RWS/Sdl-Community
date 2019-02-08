using System.Windows.Forms;

namespace Sdl.Community.CleanUpTasks.Contracts
{
    internal abstract class ICleanUpTargetSettingsControlContract : ICleanUpTargetSettingsControl
    {
        public Button BackupButton => default(Button);

	    public TextBox BackupFolder => default(TextBox);

	    public CheckBox GenerateTarget => default(CheckBox);

	    public CheckBox MakeBackups => default(CheckBox);

	    public CheckBox OverwriteSdlXliff => default(CheckBox);

	    public Button SaveButton => default(Button);

	    public TextBox SaveFolder => default(TextBox);

	    public CleanUpTargetSettings Settings { get; set; }

	}
}