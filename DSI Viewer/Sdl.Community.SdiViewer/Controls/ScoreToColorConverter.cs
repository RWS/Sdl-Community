namespace Sdl.Community.DsiViewer.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ScoreToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string score)
                return Brushes.Gray;

            if (int.TryParse(score, out var numericScore))
            {
                return numericScore switch
                {
                    >= 67 => Brushes.Green,
                    >= 34 => new BrushConverter().ConvertFromString("#FFBF00"), //amber
                    _ => Brushes.Red
                };
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}