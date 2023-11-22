﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MicrosoftTranslatorProvider.Converters
{
	public class EmptyToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Collapsed
														   : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}