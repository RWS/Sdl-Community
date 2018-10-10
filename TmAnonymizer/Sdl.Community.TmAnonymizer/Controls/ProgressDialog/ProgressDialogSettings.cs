using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog
{
	public class ProgressDialogSettings
	{
		//public static ProgressDialogSettings WithLabelOnly = new ProgressDialogSettings(null, false, false, true);
		//public static ProgressDialogSettings WithSubLabel = new ProgressDialogSettings(null, true, false, true);
		//public static ProgressDialogSettings WithSubLabelAndCancel = new ProgressDialogSettings(null, true, true, true);

		public bool ShowSubLabel { get; set; }
		public bool ShowCancelButton { get; set; }
		public bool ShowProgressBarIndeterminate { get; set; }
		public Window Owner { get; set; }
		public Form FormOwner { get; set; }
		
		public ProgressDialogSettings()
		{
			ShowSubLabel = false;
			ShowCancelButton = false;
			ShowProgressBarIndeterminate = true;
		}

		public ProgressDialogSettings(Window owner, bool showSubLabel, bool showCancelButton, bool showProgressBarIndeterminate)
		{
			Owner = owner;
			ShowSubLabel = showSubLabel;
			ShowCancelButton = showCancelButton;
			ShowProgressBarIndeterminate = showProgressBarIndeterminate;
		}

		public ProgressDialogSettings(Form owner, bool showSubLabel, bool showCancelButton, bool showProgressBarIndeterminate)
		{						
			FormOwner = owner;
			ShowSubLabel = showSubLabel;
			ShowCancelButton = showCancelButton;
			ShowProgressBarIndeterminate = showProgressBarIndeterminate;			
		}
	}
}
