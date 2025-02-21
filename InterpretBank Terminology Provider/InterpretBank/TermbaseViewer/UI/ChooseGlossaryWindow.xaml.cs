using System.Collections.Generic;
using System.Windows;

namespace InterpretBank.TermbaseViewer.UI
{
    /// <summary>
    /// Interaction logic for ChooseGlossaryWindow.xaml
    /// </summary>
    public partial class ChooseGlossaryWindow : Window
    {
        public static readonly DependencyProperty GlossariesProperty = DependencyProperty.Register(nameof(Glossaries), typeof(List<string>), typeof(ChooseGlossaryWindow), new PropertyMetadata(default(List<string>)));
        public static readonly DependencyProperty SelectedGlossaryProperty = DependencyProperty.Register(nameof(SelectedGlossary), typeof(string), typeof(ChooseGlossaryWindow), new PropertyMetadata(default(string)));

        public ChooseGlossaryWindow(List<string> glossaries)
        {
            Glossaries = glossaries;
            InitializeComponent();
            SelectedGlossary = Glossaries[0];
        }

        public List<string> Glossaries
        {
            get => (List<string>)GetValue(GlossariesProperty);
            set => SetValue(GlossariesProperty, value);
        }

        public string SelectedGlossary
        {
            get => (string)GetValue(SelectedGlossaryProperty);
            set => SetValue(SelectedGlossaryProperty, value);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}