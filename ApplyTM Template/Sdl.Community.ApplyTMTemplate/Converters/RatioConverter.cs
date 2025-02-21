﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sdl.Community.ApplyTMTemplate.Converters
{
	public class RatioConverter : MarkupExtension, IValueConverter
	{
		private static RatioConverter _instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
			return size.ToString("G0", CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance ?? (_instance = new RatioConverter());
		}
	}
}