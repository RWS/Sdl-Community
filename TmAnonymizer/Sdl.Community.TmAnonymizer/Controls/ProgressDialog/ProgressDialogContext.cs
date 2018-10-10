using System;
using System.ComponentModel;

namespace Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog
{
	public class ProgressDialogContext
	{
		public BackgroundWorker Worker { get; }
		public DoWorkEventArgs Arguments { get; }

		public ProgressDialogContext(BackgroundWorker worker, DoWorkEventArgs arguments)
		{
			Worker = worker ?? throw new ArgumentNullException("worker");
			Arguments = arguments ?? throw new ArgumentNullException("arguments");
		}

		public bool CheckCancellationPending()
		{
			if (Worker.WorkerSupportsCancellation && Worker.CancellationPending)
			{
				Arguments.Cancel = true;
			}

			return Arguments.Cancel;
		}

		public void ThrowIfCancellationPending()
		{
			if (CheckCancellationPending())
			{
				throw new ProgressDialogCancellationExcpetion();
			}
		}

		public void Report(string message)
		{
			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(0, message);
			}
		}

		public void Report(string format, params object[] arg)
		{
			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(0, string.Format(format, arg));
			}
		}

		public void Report(int percentProgress, string message)
		{
			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(percentProgress, message);
			}
		}

		public void Report(int percentProgress, string format, params object[] arg)
		{
			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(percentProgress, string.Format(format, arg));
			}
		}

		public void ReportWithCancellationCheck(string message)
		{
			ThrowIfCancellationPending();

			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(0, message);
			}
		}

		public void ReportWithCancellationCheck(string format, params object[] arg)
		{
			ThrowIfCancellationPending();

			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(0, string.Format(format, arg));
			}
		}

		public void ReportWithCancellationCheck(int percentProgress, string message)
		{
			ThrowIfCancellationPending();

			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(percentProgress, message);
			}
		}

		public void ReportWithCancellationCheck(int percentProgress, string format, params object[] arg)
		{
			ThrowIfCancellationPending();

			if (Worker.WorkerReportsProgress)
			{
				Worker.ReportProgress(percentProgress, string.Format(format, arg));
			}
		}
	}
}
