using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace Trados.Transcreate.Controls
{
	public class BindableParagraphBlock : Paragraph
	{
		public BindableParagraphBlock()
		{
			FixupDataContext(this);
		}

		public static readonly DependencyProperty BoundInlineProperty = DependencyProperty.Register("BoundInline"
			, typeof(Inline)
			, typeof(BindableParagraphBlock)
			, new PropertyMetadata(OnBoundSpanChanged));

		public Inline BoundInline
		{
			get => (Inline)GetValue(BoundInlineProperty);
			set => SetValue(BoundInlineProperty, value);
		}

		private static void OnBoundSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BindableParagraphBlock control)
			{
				control.Inlines.Clear();

				if (e.NewValue != null)
				{
					control.Inlines.Add(e.NewValue as Inline);
				}
			}
		}

		private static void FixupDataContext(FrameworkContentElement element)
		{
			var b = new Binding(DataContextProperty.Name)
			{
				RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1)
			};

			element.SetBinding(DataContextProperty, b);
		}
	}
}
