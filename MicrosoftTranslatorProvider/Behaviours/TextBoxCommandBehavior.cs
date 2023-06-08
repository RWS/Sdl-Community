using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MicrosoftTranslatorProvider.Behaviours
{
    public static class TextBoxCommandBehavior
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(TextBoxCommandBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(TextBoxCommandBehavior),
                new PropertyMetadata(null));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox)
            {
                return;
            }

            textBox.GotFocus -= TextBox_GotFocus;
            if (e.NewValue is ICommand command)
            {
                textBox.GotFocus += TextBox_GotFocus;
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox textBox)
            {
                return;
            }

            var command = GetCommand(textBox);
            var parameter = GetCommandParameter(textBox);
            if ((bool)command?.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}