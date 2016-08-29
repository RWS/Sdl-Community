using System;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.TMOptimizer.Control
{
    /// <summary>
    /// Radio button that supports binding to an enum.
    /// </summary>
    public class EnumRadioButton : RadioButton
    {
        public EnumRadioButton()
        {
            Loaded += _Loaded;
            Checked += _Checked;
        }

        private void _Loaded(object sender, RoutedEventArgs event_arguments)
        {
            _SetChecked();
        }

        private void _Checked(object sender, RoutedEventArgs event_arguments)
        {
            if (IsChecked == true)
            {
                object binding = EnumBinding;

                if ((binding is Enum) && (EnumValue != null))
                {
                    try
                    {
                        EnumBinding = Enum.Parse(binding.GetType(), EnumValue);
                    }

                    catch (ArgumentException exception)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            string.Format(
                                "EnumRadioButton [{0}]: " +
                                "EnumBinding = {1}, " +
                                "EnumValue = {2}, " +
                                "ArgumentException {3}",
                                Name,
                                EnumBinding,
                                EnumValue,
                                exception));

                        throw;
                    }
                }
            }
        }

        private void _SetChecked()
        {
            object binding = EnumBinding;

            if ((binding is Enum) && (EnumValue != null))
            {
                try
                {
                    object value = Enum.Parse(binding.GetType(), EnumValue);

                    IsChecked = ((Enum)binding).CompareTo(value) == 0;
                }

                catch (ArgumentException exception)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format(
                            "EnumRadioButton [{0}]: " +
                            "EnumBinding = {1}, " +
                            "EnumValue = {2}, " +
                            "ArgumentException {3}",
                            Name,
                            EnumBinding,
                            EnumValue,
                            exception));

                    throw;
                }
            }
        }

        static EnumRadioButton()
        {
            FrameworkPropertyMetadata enum_binding_metadata = new FrameworkPropertyMetadata();

            enum_binding_metadata.BindsTwoWayByDefault = true;
            enum_binding_metadata.PropertyChangedCallback = OnEnumBindingChanged;

            EnumBindingProperty = DependencyProperty.Register("EnumBinding",
                                                               typeof(object),
                                                               typeof(EnumRadioButton),
                                                               enum_binding_metadata);

            EnumValueProperty = DependencyProperty.Register("EnumValue",
                                                               typeof(string),
                                                               typeof(EnumRadioButton));
        }

        public static readonly DependencyProperty EnumBindingProperty;

        private static void OnEnumBindingChanged(DependencyObject dependency_object,
                                                 DependencyPropertyChangedEventArgs event_arguments)
        {
            if (dependency_object is EnumRadioButton)
            {
                ((EnumRadioButton)dependency_object)._SetChecked();
            }
        }

        public object EnumBinding
        {
            set
            {
                SetValue(EnumBindingProperty, value);
            }

            get
            {
                return (object)GetValue(EnumBindingProperty);
            }
        }

        public static readonly DependencyProperty EnumValueProperty;

        public string EnumValue
        {
            set
            {
                SetValue(EnumValueProperty, value);
            }

            get
            {
                return (string)GetValue(EnumValueProperty);
            }
        }
    }
}
