using System.Windows;
using System.Windows.Controls;

namespace InterpretBank.Controls
{
    /// <summary>
    /// Interaction logic for GlossariesOrTagsControl.xaml
    /// </summary>
    public partial class GlossariesOrTagsControl : UserControl
    {
        public GlossariesOrTagsControl()
        {
            InitializeComponent();
        }

        private void UseGlossariesButton_OnGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as RadioButton).IsChecked = true;
        }
    }
}