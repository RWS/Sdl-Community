using System;
using System.ComponentModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls.ProgressDialog
{
	internal class ProgressDialogResult
	{
		public object Result { get; }
		public bool Cancelled { get; }
		public Exception Error { get; }

		public bool OperationFailed => Error != null;

		public ProgressDialogResult(RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
				Cancelled = true;
			else if (e.Error != null)
				Error = e.Error;
			else
				Result = e.Result;
		}
	}
}
