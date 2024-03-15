using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LanguageWeaverProvider.Converters
{
	[ValueConversion(typeof(string), typeof(Color))]
	public class LanguageCodeToColorConverter : IValueConverter
	{
		private readonly Color BlackColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
		private readonly Color GrayColor = Color.FromArgb(0xFF, 0x80, 0x80, 0x80);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var invalidColor = parameter switch
			{
				string colorString when ColorConverter.ConvertFromString(colorString) is Color color => color,
				_ => GrayColor,
			};

			return value switch
			{
				string languageCode when languageCode.Equals("n/a") => new SolidColorBrush(invalidColor),
				_ => new SolidColorBrush(BlackColor)
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}