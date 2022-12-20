using System;
using System.Globalization;
using System.Windows;
using Sdl.Desktop.Platform.Controls.Converters;

namespace Sdl.Community.ApplyTMTemplate.Converters
{
	public class NullableBooleanToVisibilityConverter : BooleanToVisibilityConverter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? False : True;
		}
	}
}