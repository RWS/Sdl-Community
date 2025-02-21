using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters
{
    public class EnumToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type { IsEnum: true } enumType)
                return Enum.GetValues(enumType).Cast<object>();
            return null;
        }

        public static T ConvertStringsToFlagEnum<T>(IList values) where T : Enum
        {
            var enumType = typeof(T);

            if (!enumType.IsDefined(typeof(FlagsAttribute), false))
                throw new ArgumentException("The provided type must be a Flags enum.");

            ulong result = 0;
            foreach (var value in values)
                if (Enum.IsDefined(enumType, value))
                    result |= System.Convert.ToUInt64(Enum.Parse(enumType, value.ToString()));
                else
                    throw new ArgumentException($"Invalid value '{value}' for enum '{enumType.Name}'.");

            return (T)Enum.ToObject(enumType, result);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}