using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class ValidationDetailsWindow : Window
    {
        public ValidationDetailsWindow(string issues, string notes)
        {
            InitializeComponent();
            BuildContent(issues, notes);
        }

        private void BuildContent(string issues, string notes)
        {
            var grid = (Grid)Content;

            MessageTextBox.Visibility = Visibility.Collapsed;

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            Grid.SetRow(scrollViewer, 0);

            var stackPanel = new StackPanel();
            scrollViewer.Content = stackPanel;

            if (!string.IsNullOrEmpty(issues))
                stackPanel.Children.Add(CreateSection("⚠️ Issues", issues, "#C0392B", "#FDEDEC", "#FFFAF9", bottomMargin: !string.IsNullOrEmpty(notes)));

            if (!string.IsNullOrEmpty(notes))
                stackPanel.Children.Add(CreateSection("ℹ️ Notes", notes, "#1E7EC2", "#EBF5FB", "#F7FBFF", bottomMargin: false));

            grid.Children.Add(scrollViewer);
        }

        private static Border CreateSection(string title, string content, string accentHex, string headerBgHex, string contentBgHex, bool bottomMargin)
        {
            var accent = BrushFromHex(accentHex);
            var headerBgBrush = BrushFromHex(headerBgHex);
            var contentBgBrush = BrushFromHex(contentBgHex);

            var outerBorder = new Border
            {
                BorderBrush = accent,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, bottomMargin ? 10 : 0)
            };

            var headerBorder = new Border
            {
                Background = headerBgBrush,
                BorderBrush = accent,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(8, 6, 8, 6),
                Child = new TextBlock
                {
                    Text = title,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12,
                    Foreground = accent
                }
            };

            var contentTextBox = new TextBox
            {
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12,
                Background = contentBgBrush,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(8, 6, 8, 6),
                Text = content
            };

            var stack = new StackPanel();
            stack.Children.Add(headerBorder);
            stack.Children.Add(contentTextBox);
            outerBorder.Child = stack;

            return outerBorder;
        }

        private static Brush BrushFromHex(string hex) =>
            (Brush)new BrushConverter().ConvertFrom(hex);

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
