using System.Windows;
using System.Windows.Controls;

namespace CustomViewExample.Controls
{
	public partial class LabelTextBlock : UserControl
	{
		public LabelTextBlock()
		{
			InitializeComponent();
		}

		public string Label
		{
			get => (string)GetValue(LabelProperty);
			set => SetValue(LabelProperty, value);
		}

		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register(nameof(Label), typeof(string), 
				typeof(LabelTextBlock), 
				new PropertyMetadata("", (s, e) 
					=> ((LabelTextBlock)s).LabelBlock.Text = (string)e.NewValue));

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(nameof(Text), typeof(string), 
				typeof(LabelTextBlock), 
				new PropertyMetadata("", (s, e) 
					=> ((LabelTextBlock)s).TextBlock.Text = (string)e.NewValue));
    }
}
