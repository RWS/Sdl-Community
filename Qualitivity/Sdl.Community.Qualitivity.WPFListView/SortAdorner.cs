using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Sdl.Community.WPFListView
{
    public class SortAdorner : Adorner
    {
        private static readonly Geometry AscGeometry =
            Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private static readonly Geometry DescGeometry =
            Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
            : base(element)
        { Direction = dir; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            drawingContext.PushTransform(
                new TranslateTransform(
                  AdornedElement.RenderSize.Width - 15,
                  (AdornedElement.RenderSize.Height - 5) / 2));

            drawingContext.DrawGeometry(Brushes.Black, null,
                Direction == ListSortDirection.Ascending ?
                  AscGeometry : DescGeometry);

            drawingContext.Pop();
        }
    }
}
