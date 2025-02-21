using Sdl.Community.ApplyTMTemplate.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.ApplyTMTemplate.Converters
{
    [ValueConversion(typeof(ApplyTMMethod), typeof(string))]
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // Ensure value is of enum type
            if (value.GetType().IsEnum)
            {
                return Enum.GetName(value.GetType(), value);
            }

            return value.ToString(); // Default conversion for non-enum values
        }

        // Converts string back to enum value (e.g., "Overwrite" -> ApplyTMMethod.Overwrite)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return null;

            if (targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, value.ToString());
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}
