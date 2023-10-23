using System;
using System.Windows;

namespace LanguageWeaverProvider.Converters
{
	public static class VisibilityConveter
	{
		public static Visibility ToVisibility(this object parameter, bool isVisible)
		{
			if (parameter is null)
			{
				return isVisible ? Visibility.Visible : Visibility.Collapsed;
			}

			var direction = (Direction)Enum.Parse(typeof(Direction), parameter as string);
			return direction switch
			{
				Direction.Inverted => isVisible ? Visibility.Collapsed : Visibility.Visible,
				_ => isVisible ? Visibility.Visible : Visibility.Collapsed,
			};
		}
	}
}