using System;
using System.Globalization;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	public class PluginVersionToImagePathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not PluginVersion pluginVersion)
			{
				throw new ArgumentException("Invalid argument type");
			}

			return pluginVersion switch
			{
				PluginVersion.LanguageWeaverEdge => "../Resources/lwHeader_Cloud.png",
				PluginVersion.LanguageWeaverCloud => "../Resources/lwHeader_Edge.png",
				PluginVersion.None => "../Resources/lwHeader_Main.png",
				_ => throw new ArgumentException("Unsupported PluginVersion value"),
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}