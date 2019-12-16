using System.Windows.Forms;
using UIToolbox;

namespace Sdl.Community.CleanUpTasks.Contracts
{
	internal abstract class ISegmentLockerControlContract : ISegmentLockerControl
    {
        public DataGridView ContentGrid => default(DataGridView);

        public ICleanUpSourceSettings Settings { get; set; }

        public CheckGroupBox StructureGroupBox => default(CheckGroupBox);

        public CheckedListBox StructureList => default(CheckedListBox);

        public void InitializeUI()
        {
        }

        public void SaveSettings()
        {
        }

        public void SetPresenter(ISegmentLockerPresenter segmentLockerPresenter)
        {
        }

        public void SetSettings(ICleanUpSourceSettings settings)
        {
        }
    }
}