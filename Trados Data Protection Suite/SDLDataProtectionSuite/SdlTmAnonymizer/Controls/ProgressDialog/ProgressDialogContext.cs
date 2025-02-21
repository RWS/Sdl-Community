using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls.ProgressDialog
{
	public class ProgressDialogContext
	{
		private bool _showProgressBarIndeterminate;
		private readonly ProgressBar _progressBar;

		public BackgroundWorker Worker { get; }
		public DoWorkEventArgs Arguments { get; }
		public bool ProgressBarIsIndeterminate
		{
			get { return _showProgressBarIndeterminate; }
			set
			{
				_showProgressBarIndeterminate = value;

				_progressBar.Dispatcher.Invoke(delegate { _progressBar.IsIndeterminate = _showProgressBarIndeterminate; });
			}
		}
		
		public ProgressDialogContext(ProgressBar progressBar, BackgroundWorker worker, DoWorkEventArgs arguments)
		{
			_progressBar = progressBar;
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
