using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InterpretBank.SettingsService.ViewModel.SettingsService;
using System.Windows.Data;
using System.Windows;

namespace InterpretBank.SettingsService.Converters
{
	//public class EnumToVisibilityConverter : IValueConverter
	//{
	//	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	//	{
	//		if (value is VisibleViewSwitch enumValue && Enum.TryParse(parameter?.ToString(), out VisibleViewSwitch parameterValue))
	//		{
	//			return enumValue == parameterValue ? Visibility.Visible : Visibility.Collapsed;
	//		}

	//		return Visibility.Collapsed;
	//	}

	//	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	//	{
	//		throw new NotSupportedException();
	//	}
	//}
}
