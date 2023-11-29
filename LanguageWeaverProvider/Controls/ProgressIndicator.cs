using System.Windows;
using System.Windows.Controls;

namespace LanguageWeaverProvider.Controls
{
	public class ProgressIndicator : Control
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ProgressIndicator));

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}
	}
}