using System;
using System.ComponentModel;

namespace ProjectWizardExample.Extensions
{
	public static class EventsExtensions
	{
		/// <summary>
		/// Extension method used to invoke the event with an empty argument.
		/// </summary>
		/// <param name="e">The event to be invoked.</param>
		/// <param name="sender">The event caller.</param>
		public static void Raise(this EventHandler e, object sender)
		{
			e?.Invoke(sender, System.EventArgs.Empty);
		}

		/// <summary>
		/// Extension method used to invoke the event with a custom event argument.
		/// </summary>
		/// <typeparam name="T">Type of the event argument</typeparam>
		/// <param name="e">The event to be invoked.</param>
		/// <param name="sender">The event caller.</param>
		/// <param name="eventArgs">The argument of the event.</param>
		public static void Raise<T>(this EventHandler<T> e, object sender, T eventArgs)
			where T : System.EventArgs
		{
			e?.Invoke(sender, eventArgs);
		}

		/// <summary>
		/// Extension method used to invoke a cancelable event with a custom event argument.
		/// </summary>
		/// <typeparam name="T">Type of the event argument</typeparam>
		/// <param name="e">The event to be invoked.</param>
		/// <param name="sender">The event caller.</param>
		/// <param name="eventArgs">The argument of the event.</param>
		/// <returns>True if cancelling is not requested.</returns>
		public static bool RaiseCancelable<T>(this EventHandler<T> e, object sender, T eventArgs)
			where T : CancelEventArgs
		{
			e?.Invoke(sender, eventArgs);

			return !eventArgs.Cancel;
		}

		/// <summary>
		/// Extension method used to invoke a cancelable event with a custom event argument with the initial cancel value defined.
		/// </summary>
		/// <typeparam name="T">Type of the event argument</typeparam>
		/// <param name="e">The event to be invoked.</param>
		/// <param name="sender">The event caller.</param>
		/// <param name="eventArgs">The argument of the event.</param>
		/// <param name="cancel">The argument of the event.</param>
		/// <returns>True if cancelling is not requested.</returns>
		public static bool RaiseCancelable<T>(this EventHandler<T> e, object sender, T eventArgs, bool cancel)
			where T : CancelEventArgs
		{
			if (e != null)
			{
				eventArgs.Cancel = cancel;
				e.Invoke(sender, eventArgs);
			}

			return !eventArgs.Cancel;
		}

		/// <summary>
		/// Extension method used to invoke a cancelable event with a default cancel event argument.
		/// </summary>
		/// <param name="e">The event to be invoked.</param>
		/// <param name="sender">The event caller.</param>
		/// <param name="cancel">An optional cancel initializer for the cancel event argument.</param>
		/// <returns>True if cancelling is not requested.</returns>
		public static bool RaiseCancelable(this EventHandler<CancelEventArgs> e, object sender, bool cancel = false)
		{
			var cancelArgs = new CancelEventArgs(cancel);
			return e.RaiseCancelable(sender, cancelArgs);
		}


	}
}
