using System.Diagnostics.Contracts;
using System.Windows.Forms;
using SDLCommunityCleanUpTasks.Contracts;
using UIToolbox;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(ISegmentLockerControlContract))]
    public interface ISegmentLockerControl
    {
        DataGridView ContentGrid { get; }

        ICleanUpSourceSettings Settings { get; set; }
        CheckGroupBox StructureGroupBox { get; }

        CheckedListBox StructureList { get; }

        void InitializeUI();

        void SaveSettings();

        void SetPresenter(ISegmentLockerPresenter segmentLockerPresenter);

        void SetSettings(ICleanUpSourceSettings settings);
    }
}