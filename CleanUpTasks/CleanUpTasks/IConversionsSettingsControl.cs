using System.Windows.Forms;

namespace Sdl.Community.CleanUpTasks
{
	//[ContractClass(typeof(IConversionsSettingsControlContract))]
	public interface IConversionsSettingsControl
    {
        Button Add { get; }
        Button Down { get; }
        Button Edit { get; }
        CheckedListBox FileList { get; }
        Button New { get; }
        Button Remove { get; }
        ICleanUpConversionSettings Settings { get; set; }
        Button Up { get; }
        CheckBox ApplyToNonTranslatables { get; }

        void InitializeUI();

        void SaveSettings();

        void SetPresenter(IConversionsSettingsPresenter presenter);

        void SetSettings(ICleanUpConversionSettings settings, BatchTaskMode taskMode);
    }
}