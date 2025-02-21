using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Multilingual.Excel.FileType.Providers.StudioComment;

namespace Multilingual.Excel.FileType.Converters
{
	public class CommentNameToNameAndDescriptionConverter : IValueConverter
	{
		private readonly StudioCommentPropertyProvider _studioCommentPropertyProvider;

		public CommentNameToNameAndDescriptionConverter()
		{
			_studioCommentPropertyProvider = new StudioCommentPropertyProvider();
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var itemValue = value?.ToString();
			if (string.IsNullOrEmpty(itemValue))
			{
				return null;
			}

			var item = _studioCommentPropertyProvider.DefaultCommentProperties.FirstOrDefault(a =>
				a.Name == value.ToString());
			if (item != null)
			{
				return item.Name + " - " + item.Description;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
