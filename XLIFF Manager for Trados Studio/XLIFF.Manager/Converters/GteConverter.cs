using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.XLIFF.Manager.Converters
{
	public class RelationalValueConverter : IMultiValueConverter
	{
		public enum RelationsEnum
		{
			Gt, Lt, Gte, Lte, Eq, Neq
		}

		public RelationsEnum Relations { get; protected set; }

		public RelationalValueConverter(RelationsEnum relations)
		{
			Relations = relations;
		}

		public RelationalValueConverter()
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length != 2)
				throw new ArgumentException(@"Must have two parameters", "values");

			var v0 = values[0] as IComparable;
			var v1 = values[1] as IComparable;

			if (v0 == null || v1 == null)
				throw new ArgumentException(@"Must arguments must be IComparible", "values");

			var r = v0.CompareTo(v1);

			switch (Relations)
			{
				case RelationsEnum.Gt:
					return r > 0;
					break;
				case RelationsEnum.Lt:
					return r < 0;
					break;
				case RelationsEnum.Gte:
					return r >= 0;
					break;
				case RelationsEnum.Lte:
					return r <= 0;
					break;
				case RelationsEnum.Eq:
					return r == 0;
					break;
				case RelationsEnum.Neq:
					return r != 0;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
