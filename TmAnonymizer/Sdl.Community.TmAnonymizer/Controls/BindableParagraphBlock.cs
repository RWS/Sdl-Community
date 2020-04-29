using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public class BindableParagraphBlock : Paragraph
	{
		public BindableParagraphBlock()
		{
			Helpers.FixupDataContext(this);
		}

		public Inline BoundInline
		{
			get => (Inline)GetValue(BoundInlineProperty);
			set => SetValue(BoundInlineProperty, value);
		}

		public static readonly DependencyProperty BoundInlineProperty = DependencyProperty.Register("BoundInline"
			, typeof(Inline)
			, typeof(BindableParagraphBlock)
			, new PropertyMetadata(OnBoundSpanChanged));

		private static void OnBoundSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BindableParagraphBlock)d).Inlines.Clear();
			if (e.NewValue != null)
			{
				((BindableParagraphBlock)d).Inlines.Add(e.NewValue as Inline);
			}
		}
	}

	internal static class Helpers
	{
		public static void FixupDataContext(FrameworkContentElement element)
		{
			var binding = new Binding(FrameworkContentElement.DataContextProperty.Name)
			{
				RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1)
			};

			element.SetBinding(FrameworkContentElement.DataContextProperty, binding);
		}
	}
}
