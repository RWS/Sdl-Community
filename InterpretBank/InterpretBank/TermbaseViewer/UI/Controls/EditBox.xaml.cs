using InterpretBank.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InterpretBank.TermbaseViewer.UI.Controls
{
    /// <summary>
    /// Interaction logic for EditBox.xaml
    /// </summary>
    public partial class EditBox : UserControl
    {
        public static readonly DependencyProperty EditBoxTextProperty = DependencyProperty.Register(nameof(EditBoxText), typeof(string), typeof(EditBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(EditBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(EditBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SaveEditCommandParameterProperty = DependencyProperty.Register(nameof(SaveEditCommandParameter), typeof(object), typeof(EditBox), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty SaveEditCommandProperty = DependencyProperty.Register(nameof(SaveEditCommand), typeof(ICommand), typeof(EditBox), new PropertyMetadata(default(ICommand)));

        public EditBox()
        {
            //EditBoxHelper.GotFocus += EditBoxHelper_SomeControlGotFocus;
            InitializeComponent();
        }

        public string EditBoxText
        {
            get => (string)GetValue(EditBoxTextProperty);
            set => SetValue(EditBoxTextProperty, value);
        }

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set
            {
                SetValue(IsEditingProperty, value);
                if (value) PreviousText = EditBoxText;
            }
        }

        public ICommand KeyboardKeypressCommand => new RelayCommand(KeyboardKeypress);

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public ICommand SaveEditCommand
        {
            get => (ICommand)GetValue(SaveEditCommandProperty);
            set => SetValue(SaveEditCommandProperty, value);
        }

        public object SaveEditCommandParameter
        {
            get => (object)GetValue(SaveEditCommandParameterProperty);
            set => SetValue(SaveEditCommandParameterProperty, value);
        }

        private string PreviousText { get; set; }

        private void ConfirmEditButton_Click(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            SaveEditCommand?.Execute(SaveEditCommandParameter);
        }

        //private void EditBoxHelper_SomeControlGotFocus(DependencyObject obj)
        //{
        //    if (TextBox.IsFocused) return;
        //    SaveEditCommand?.Execute(SaveEditCommandParameter);
        //    IsEditing = false;
        //}

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
            TextBox.Focus();
        }

        private void KeyboardKeypress(object obj)
        {
            var parameter = obj.ToString();
            if (parameter == "ConfirmEdit")
            {
                SaveEditCommand?.Execute(null);
                var control =
                    (((Parent as FrameworkElement).TemplatedParent as FrameworkElement).TemplatedParent as
                        FrameworkElement);

                //control?.Focus();
                Keyboard.Focus(control);
                IsEditing = false;
            }

            if (EditBoxGrid.IsFocused && parameter == "EnterEdit")
            {
                IsEditing = true;
                TextBox.Focus();
            }

            if (parameter == "RejectEdit")
            {
                RejectEditing();
            }
        }

        private void RejectEditButton_OnClick(object sender, RoutedEventArgs e)
        {
            RejectEditing();
        }

        private void RejectEditing()
        {
            EditBoxText = PreviousText;
            IsEditing = false;
        }

        private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            EditBoxHelper.RaiseGotFocus(null);
            IsEditing = true;
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            SaveEditCommand?.Execute(SaveEditCommandParameter);
        }
    }
}