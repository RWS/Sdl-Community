using System.Windows;
using System.Windows.Controls;

namespace InterpretBank.TermbaseViewer.UI.Controls
{
    /// <summary>
    /// Interaction logic for EditBox.xaml
    /// </summary>
    public partial class EditBox : UserControl
    {
        public static readonly DependencyProperty EditBoxTextProperty = DependencyProperty.Register(nameof(EditBoxText), typeof(string), typeof(EditBox), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(EditBox), new PropertyMetadata(default(string)));

        public EditBox()
        {
            InitializeComponent();
        }

        public string EditBoxText
        {
            get => (string)GetValue(EditBoxTextProperty);
            set => SetValue(EditBoxTextProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
    }
}