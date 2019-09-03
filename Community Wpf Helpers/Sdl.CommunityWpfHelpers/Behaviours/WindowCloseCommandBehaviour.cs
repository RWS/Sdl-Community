using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Sdl.CommunityWpfHelpers.Behaviours
{
	public static class WindowCloseCommandBehaviour
	{
		public static readonly DependencyProperty CancelClosingProperty = DependencyProperty.RegisterAttached(
			"CancelClosing", typeof(ICommand), typeof(WindowCloseCommandBehaviour));

		public static readonly DependencyProperty ClosingProperty = DependencyProperty.RegisterAttached(
			"Closing", typeof(ICommand), typeof(WindowCloseCommandBehaviour),
			new UIPropertyMetadata(ClosingChanged));

		public static ICommand GetClosing(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(ClosingProperty);
		}

		public static void SetClosing(DependencyObject obj, ICommand value)
		{
			obj.SetValue(ClosingProperty, value);
		}

		private static void ClosingChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			var window = target as Window;

			if (window == null) return;
			if (e.NewValue != null)
			{
				window.Closing += Window_Closing;
			}
			else
			{
				window.Closing -= Window_Closing;
			}
		}

		public static ICommand GetCancelClosing(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CancelClosingProperty);
		}

		public static void SetCancelClosing(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CancelClosingProperty, value);
		}

		static void Window_Closing(object sender, CancelEventArgs e)
		{
			var closing = GetClosing(sender as Window);
			if (closing != null)
			{
				if (closing.CanExecute(null))
				{
					closing.Execute(null);
				}
				else
				{
					var cancelClosing = GetCancelClosing(sender as Window);
					cancelClosing?.Execute(null);

					e.Cancel = true;
				}
			}
		}
	}
}
