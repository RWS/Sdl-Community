using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.Service
{
	public sealed class AsyncTaskWatcherService<TResult>:INotifyPropertyChanged
	{
		public AsyncTaskWatcherService(Task<TResult> task)
		{
			AsyncTask = task;
			if (!task.IsCompleted)
			{
				var _ = WatchTaskAsync(task);
			}
		}
		private async Task WatchTaskAsync(Task<TResult> task)
		{
			try
			{
				await task;
			}
			catch
			{
			}
			var propertyChanged = PropertyChanged;
			if (propertyChanged == null)
				return;
			propertyChanged(this, new PropertyChangedEventArgs("Status"));
			propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
			propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
			if (task.IsCanceled)
			{
				propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
			}
			else if (task.IsFaulted)
			{
				propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
				propertyChanged(this, new PropertyChangedEventArgs("Exception"));
				propertyChanged(this,
				  new PropertyChangedEventArgs("InnerException"));
				propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
			}
			else
			{
				propertyChanged(this,
				  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
				propertyChanged(this, new PropertyChangedEventArgs("Result"));
			}
		}

		public void StartTask(Task<TResult> task)
		{
			AsyncTask = task;
			var _ = WatchTaskAsync(task);
		}

		public Task<TResult> AsyncTask { get; private set; }
		public TResult Result
		{
			get
			{
				return (AsyncTask.Status == TaskStatus.RanToCompletion) ? AsyncTask.Result : default(TResult);
			}
		}

		public TaskStatus Status { get { return AsyncTask.Status; } }
		public bool IsCompleted { get { return AsyncTask.IsCompleted; } }
		public bool IsNotCompleted { get { return !AsyncTask.IsCompleted; } }
		public bool IsSuccessfullyCompleted
		{
			get
			{
				return AsyncTask.Status == TaskStatus.RanToCompletion;
			}
		}
		public bool IsCanceled { get { return AsyncTask.IsCanceled; } }
		public bool IsFaulted { get { return AsyncTask.IsFaulted; } }
		public AggregateException Exception { get { return AsyncTask.Exception; } }
		public Exception InnerException
		{
			get
			{
				return Exception?.InnerException;
			}
		}
		public string ErrorMessage
		{
			get
			{
				return InnerException?.Message;
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
	


