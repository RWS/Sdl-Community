using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog
{
	public partial class ProgressDialog : Window
	{
		public static ProgressDialogContext Current { get; set; }

		private volatile bool _isBusy;
		private BackgroundWorker _worker;
		private readonly WindowInteropHelper _interopHelper;
		private readonly ProgressDialogSettings _settings;

		public string Label
		{
			get => TextLabel.Text;
			set => TextLabel.Text = value;
		}

		public string SubLabel
		{
			get => SubTextLabel.Text;
			set => SubTextLabel.Text = value;
		}

		internal ProgressDialogResult Result { get; private set; }

		public ProgressDialog(ProgressDialogSettings settings)
		{
			InitializeComponent();

			_settings = settings;

			if (_settings.ShowSubLabel)
			{
				Height = 140;
				MinHeight = 140;
				SubTextLabel.Visibility = Visibility.Visible;
			}
			else
			{
				Height = 110;
				MinHeight = 110;
				SubTextLabel.Visibility = Visibility.Collapsed;
			}

			CancelButton.Visibility = _settings.ShowCancelButton ? Visibility.Visible : Visibility.Collapsed;

			ProgressBar.IsIndeterminate = _settings.ShowProgressBarIndeterminate;

			Closing += OnClosing;

			if (settings.Owner != null)
			{
				Owner = settings.Owner;
				Owner.IsEnabled = false;
			}
			else if (settings.FormOwner != null)
			{
				_interopHelper = new WindowInteropHelper(this)
				{
					Owner = settings.FormOwner.Handle
				};

				_settings.FormOwner.Enabled = false;
			}
		}

		internal ProgressDialogResult Execute(object operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}

			ProgressDialogResult result = null;

			_isBusy = true;

			_worker = new BackgroundWorker
			{
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			_worker.DoWork +=
				(s, e) =>
				{
					try
					{
						Current = new ProgressDialogContext(ProgressBar, s as BackgroundWorker, e as DoWorkEventArgs);

						if (operation is Action action)
						{
							action();
						}
						else if (operation is Func<object>)
						{
							e.Result = ((Func<object>)operation)();
						}
						else
						{
							throw new InvalidOperationException(StringResources.Operation_type_is_not_supported);
						}

						Current?.CheckCancellationPending();
					}
					catch (ProgressDialogCancellationExcpetion) { }
					catch (Exception ex)
					{						
						if (!Current.CheckCancellationPending())
						{
							if (ex is LanguagePlatform.Core.LanguagePlatformException exception)
							{
								var errorText = exception.Description?.Data?.ToString();
								if (errorText != null && errorText.Contains("OutOfMemoryException"))
								{
									throw new Exception(StringResources.System_OutOfMemoryException_was_thrown);
								}
								else
								{
									throw exception;
								}
							}						
							else
							{
								throw;
							}
						}
					}
					finally
					{
						Current = null;
					}
				};

			_worker.RunWorkerCompleted +=
				(s, e) =>
				{

					result = new ProgressDialogResult(e);

					Dispatcher.BeginInvoke(DispatcherPriority.Send, (SendOrPostCallback)delegate
					{
						_isBusy = false;
						Close();
					}, null);

				};

			_worker.ProgressChanged +=
				(s, e) =>
				{

					if (!_worker.CancellationPending)
					{
						SubLabel = (e.UserState as string) ?? string.Empty;
						ProgressBar.Value = e.ProgressPercentage;
					}
				};

			_worker.RunWorkerAsync();

			ShowDialog();

			return result;
		}

		private void OnCancelButtonClick(object sender, RoutedEventArgs e)
		{
			if (_worker != null && _worker.WorkerSupportsCancellation)
			{
				SubLabel = StringResources.Please_wait_while_process_will_be_cancelled;
				CancelButton.IsEnabled = false;
				_worker.CancelAsync();
			}
			else
			{
				Close();
			}
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			if (Owner != null)
			{
				Owner.IsEnabled = true;
			}
			else if (_interopHelper?.Owner != null)
			{
				_settings.FormOwner.Enabled = true;
			}

			e.Cancel = _isBusy;
		}

		internal static ProgressDialogResult Execute(string label, Action operation)
		{
			return ExecuteInternal(label, operation, null);
		}

		internal static ProgressDialogResult Execute(string label, Action operation, ProgressDialogSettings settings)
		{
			return ExecuteInternal(label, operation, settings);
		}

		internal static ProgressDialogResult Execute(string label, Func<object> operationWithResult)
		{
			return ExecuteInternal(label, operationWithResult, null);
		}

		internal static ProgressDialogResult Execute(string label, Func<object> operationWithResult, ProgressDialogSettings settings)
		{
			return ExecuteInternal(label, operationWithResult, settings);
		}

		internal static void Execute(string label, Action operation, Action<ProgressDialogResult> successOperation, Action<ProgressDialogResult> failureOperation = null, Action<ProgressDialogResult> cancelledOperation = null)
		{
			var result = ExecuteInternal(label, operation, null);

			if (result.Cancelled && cancelledOperation != null)
			{
				cancelledOperation(result);
			}
			else if (result.OperationFailed && failureOperation != null)
			{
				failureOperation(result);
			}
			else
			{
				successOperation?.Invoke(result);
			}
		}

		internal static ProgressDialogResult ExecuteInternal(string label, object operation, ProgressDialogSettings settings)
		{

			var dialog = new ProgressDialog(settings);

			if (!string.IsNullOrEmpty(label))
				dialog.Label = label;

			return dialog.Execute(operation);

		}
	}
}
