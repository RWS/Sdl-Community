using System.Windows;
using System.Windows.Controls;

namespace GoogleCloudTranslationProvider.Helpers
{
	public class PasswordHelper
	{
		public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
			"Password",
			typeof(string),
			typeof(PasswordHelper),
			new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

		public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
			"Attach",
			typeof(bool),
			typeof(PasswordHelper),
			new PropertyMetadata(false, Attach));

		private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached(
			"IsUpdating",
			typeof(bool),
			typeof(PasswordHelper));

		public static bool GetAttach(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(AttachProperty);
		}

		public static void SetAttach(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(AttachProperty, value);
		}

		public static string GetPassword(DependencyObject dependencyObject)
		{
			return (string)dependencyObject.GetValue(PasswordProperty);
		}

		public static void SetPassword(DependencyObject dependencyObject, string value)
		{
			dependencyObject.SetValue(PasswordProperty, value);
		}

		private static bool GetIsUpdating(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(IsUpdatingProperty);
		}

		private static void SetIsUpdating(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(IsUpdatingProperty, value);
		}

		private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			passwordBox.PasswordChanged -= PasswordChanged;
			if (!GetIsUpdating(passwordBox))
			{
				passwordBox.Password = (string)e.NewValue;
			}

			passwordBox.PasswordChanged += PasswordChanged;
		}

		private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PasswordBox passwordBox)
			{
				return;
			}

			if ((bool)e.OldValue)
			{
				passwordBox.PasswordChanged -= PasswordChanged;
			}

			if ((bool)e.NewValue)
			{
				passwordBox.PasswordChanged += PasswordChanged;
			}
		}

		private static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			SetIsUpdating(passwordBox, true);
			SetPassword(passwordBox, passwordBox?.Password);
			SetIsUpdating(passwordBox, false);
		}
	}
}