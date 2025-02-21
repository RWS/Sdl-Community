//using System;
//using System.Globalization;
//using System.Windows;
//using System.Windows.Data;

//namespace InterpretBank.TermbaseViewer.UI.Converters
//{
//    public class BoolToVisibilityConverter : IMultiValueConverter
//    {
//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
//            ((bool)values[0]) || ((bool)values[1]) ? Visibility.Visible : Visibility.Collapsed;

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}